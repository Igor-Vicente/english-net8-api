using English.Net8.Api.Configuration;
using English.Net8.Api.Dtos;
using English.Net8.Api.Dtos.Account;
using English.Net8.Api.Extensions;
using English.Net8.Api.Models;
using English.Net8.Api.Repository.Interfaces;
using English.Net8.Api.Services.Mailing;
using English.Net8.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using Store.MongoDb.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace English.Net8.Api.Controllers
{

    [Authorize]
    [Route("api/v1/account")]
    public class AccountController : MainController
    {
        private readonly UserManager<MongoUser> _userManager;
        private readonly SignInManager<MongoUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly AuthSettings _authSettings;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<MongoUser> userManager,
                                 SignInManager<MongoUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IOptions<AuthSettings> authSettings,
                                 IEmailSender emailSender,
                                 IUserRepository userRepository,
                                 IQuestionRepository questionRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _authSettings = authSettings.Value;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _questionRepository = questionRepository;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> Login(SigninDto loginDto)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, true);
            if (result.Succeeded)
            {
                var account = await _userManager.FindByEmailAsync(loginDto.Email);
                var user = await _userRepository.FindByIdAsync(account.Id);

                var claims = await GetUserClaimsAsync(account);
                var token = GenerateJwt(claims);
                SetCookiesInResponse(token);

                return SuccessResponse(UserConverter.ToResponseUser(user));
            }

            if (result.IsLockedOut)
                return ErrorResponse("The user has been temporarily blocked due to invalid attempts");

            if (result.IsNotAllowed)
                return ErrorResponse("Your account has not been confirmed, you cannot proceed");

            return ErrorResponse("Username or password is invalid");
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> Register(SignupDto registerDto)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            var user = new User { Name = registerDto.Name, Email = registerDto.Email, IsAdmin = false, IsPremium = false };
            var account = new MongoUser { Id = user.Id, UserName = registerDto.Email, Email = registerDto.Email, EmailConfirmed = false };

            var result = await _userManager.CreateAsync(account, registerDto.Password);

            if (!result.Succeeded)
                return ErrorResponse(result.Errors.Select(e => e.Description));

            await _userRepository.InsertAsync(user);
            _logger.LogInformation($"User {user.Id} created a new account with password.");
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(account);
            var callbackUrl = Url.EmailConfirmationLink(account.Id.ToString(), code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(account.Email, callbackUrl);

            var claims = await GetUserClaimsAsync(account);
            var token = GenerateJwt(claims);
            SetCookiesInResponse(token);
            return SuccessResponse(UserConverter.ToResponseUser(user));
        }

        [AllowAnonymous]
        [HttpGet("email-confirmation")]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto>> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return ErrorResponse("The link is invalid");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new ApplicationException($"Unable to find the user with ID '{userId}'.");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return SuccessResponse();
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(SuccessResponseDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<string>>> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return SuccessResponse("Please check your email to reset the password.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordLink(_authSettings.AcceptedDomainsCors, user.Id.ToString(), code);
            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
               $"Hello 👋\r\n, <br /><br />You can click <a href='{callbackUrl}'>here</a> to reset your password. <br /><br />" +
               $"If you didn’t ask to reset your password, you can ignore this message. <br /> Thanks, good studies 👋\r\n");

            return SuccessResponse("Please check your email to reset the password.");
        }


        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(SuccessResponseDto<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<string>>> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            if (!ObjectId.TryParse(model.UserId, out var userId))
                return ErrorResponse("The link used is not valid");

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return ErrorResponse("The link used is not valid");

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (!result.Succeeded)
                return ErrorResponse(result.Errors.Select(e => e.Description));

            return SuccessResponse("Your password has been reset.");
        }


        [HttpPost("logout")]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SuccessResponseDto>> Logout()
        {
            SetLogoutCookiesResponse();
            return SuccessResponse();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto>> Delete()
        {
            var account = await _userManager.GetUserAsync(User);

            if (account == null) return ErrorResponse("The user was not found");

            var deleteAccount = await _userManager.DeleteAsync(account);
            if (!deleteAccount.Succeeded)
                return ErrorResponse(deleteAccount.Errors.Select(e => e.Description));

            await _questionRepository.DeleteUserAnswersByUserIdAsync(account.Id);
            await _userRepository.DeleteUserByIdAsync(account.Id);

            SetLogoutCookiesResponse();
            return SuccessResponse();
        }

        private string GenerateJwt(IEnumerable<Claim> claims)
        {
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = _authSettings.Audience,
                Issuer = _authSettings.Issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_authSettings.ExpiresMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });
            return tokenHandler.WriteToken(token);
        }
        private async Task<IEnumerable<Claim>> GetUserClaimsAsync(MongoUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            return claims;
        }
        private void SetCookiesInResponse(string token)
        {
            HttpContext.Response.Cookies.Append(_authSettings.AuthCookieName, token, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(_authSettings.ExpiresMinutes),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Domain = Request.Host.Host
            });
            HttpContext.Response.Cookies.Append(_authSettings.ExpiresCookieName, Guid.NewGuid().ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(_authSettings.ExpiresMinutes),
                HttpOnly = false,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Domain = Request.Host.Host
            });
        }
        private void SetLogoutCookiesResponse()
        {
            HttpContext.Response.Cookies.Append(_authSettings.AuthCookieName, "", new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(-1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Domain = Request.Host.Host
            });
            HttpContext.Response.Cookies.Append(_authSettings.ExpiresCookieName, Guid.NewGuid().ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(-1),
                HttpOnly = false,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Domain = Request.Host.Host
            });
        }
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }
}
