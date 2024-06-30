using English.Net8.Api.Configuration;
using English.Net8.Api.Dtos;
using English.Net8.Api.Models;
using English.Net8.Api.Repository.Interfaces;
using English.Net8.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace English.Net8.Api.Controllers
{
    [Authorize]
    [Route("api/v1/user")]
    public class UserController : MainController
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthSettings _authSettings;

        public UserController(IUserRepository userRepository, IOptions<AuthSettings> authSettings)
        {
            _userRepository = userRepository;
            _authSettings = authSettings.Value;
        }

        [HttpGet("authenticated")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> GetUserAsync()
        {
            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var user = await _userRepository.FindByIdAsync(id);

                return SuccessResponse(UserConverter.ToResponseUser(user));
            }

            return ErrorResponse("Unable to get authentication cookie");
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(SuccessResponseDto<IEnumerable<UserResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<IEnumerable<UserResponseDto>>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 8)
        {
            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var users = await _userRepository.GetAllAsync(pageNumber, pageSize);

                return SuccessResponse(UserConverter.ToResponseUser(users));
            }

            return ErrorResponse("Unable to get authentication cookie");
        }

        [HttpPut("information")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> UpdateUserAsync(UpdateUserDto userDto)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var user = UserConverter.ToUser(userDto);
                user.Id = id;
                user.Email = User.FindFirst(ClaimTypes.Email)?.Value;

                await _userRepository.ReplaceAsync(user);

                return SuccessResponse(UserConverter.ToResponseUser(user));
            }

            return ErrorResponse("Unable to get authentication cookie");
        }

        [HttpPut("location-closest")]
        [ProducesResponseType(typeof(SuccessResponseDto<IEnumerable<UserResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<IEnumerable<UserResponseDto>>>> UpdateUserLocationAsync(Location location, int pageNumber = 1, int pageSize = 8)
        {
            if (!LocationValidator.Validate(location))
                return ErrorResponse("The location provided is not valid.");

            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var users = await _userRepository.GetClosestUsersAsync(location, id, pageNumber, pageSize);

                await _userRepository.UpdateUserLocationAsync(id, location);
                return SuccessResponse(UserConverter.ToResponseUser(users));
            }

            return ErrorResponse("Unable to get authentication cookie");
        }
    }
}

