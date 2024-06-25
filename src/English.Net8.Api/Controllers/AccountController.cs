using English.Net8.Api.Configuration;
using English.Net8.Api.Dtos;
using English.Net8.Api.Extensions;
using English.Net8.Api.Services.Mailing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        private readonly ILogger<AccountController> _logger;
        private readonly AuthSettings _authSettings;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<MongoUser> userManager,
                                 SignInManager<MongoUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IOptions<AuthSettings> authSettings,
                                 IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _authSettings = authSettings.Value;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        [ProducesDefaultResponseType(typeof(ResponseDto))]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                var claims = await GetUserClaimsAsync(user);
                var token = GenerateJwt(claims);
                SetCookiesInResponse(token);
                return CustomResponse();
            }

            if (result.IsLockedOut)
            {
                NotifierError("The user has been temporarily blocked due to invalid attempts");
                return CustomResponse();
            }

            NotifierError("Username or password is invalid");
            return CustomResponse();
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        [ProducesDefaultResponseType(typeof(ResponseDto))]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new MongoUser { UserName = registerDto.Email, Email = registerDto.Email, EmailConfirmed = false };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                NotifierError(result.Errors.Select(e => e.Description));
                return CustomResponse();
            }

            _logger.LogInformation("User created a new account with password.");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return CustomResponse();
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetUserAccoutAsync()
        {
            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var token))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                var account = await _userManager.FindByIdAsync(jwt.Subject);

                /*TO DO: GET THE USER INSTEAD OF THE ACCOUNT*/
                return CustomResponse(account);
            }

            NotifierError("Unable to get authentication cookie");
            return CustomResponse();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                NotifierError("Userid or code was not provided");
                return CustomResponse();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return CustomResponse();
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Append(_authSettings.AuthCookieName, "", new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(-1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
            });
            HttpContext.Response.Cookies.Append(_authSettings.ExpiresCookieName, Guid.NewGuid().ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(-1),
                HttpOnly = false,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
            });
            return Ok();
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
            });
            HttpContext.Response.Cookies.Append(_authSettings.ExpiresCookieName, Guid.NewGuid().ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(_authSettings.ExpiresMinutes),
                HttpOnly = false,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
            });
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }
}
