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
        [ProducesDefaultResponseType(typeof(ResponseDto))]
        public async Task<IActionResult> GetUserAsync()
        {
            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var user = await _userRepository.FindByIdAsync(id);
                var userDto = UserConverter.ToResponseUser(user);

                return CustomResponse(userDto);
            }

            NotifierError("Unable to get authentication cookie");
            return CustomResponse();
        }

        [HttpPut("information")]
        [ProducesDefaultResponseType(typeof(ResponseDto))]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserDto userDto)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var user = UserConverter.ToUser(userDto);
                user.Id = id;
                user.Email = User.FindFirst(ClaimTypes.Email)?.Value;

                await _userRepository.ReplaceAsync(user);
                return CustomResponse();
            }

            NotifierError("Unable to get authentication cookie");
            return CustomResponse();
        }

        [HttpPut("location-closest")]
        [ProducesDefaultResponseType(typeof(ResponseDto))]
        public async Task<IActionResult> UpdateUserLocationAsyc(Location location)
        {
            if (!LocationValidator.Validate(location))
            {
                NotifierError("The location provided is not valid.");
                return CustomResponse();
            }

            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var users = await _userRepository.GetClosestUsersAsync(location);
                users = users.Where(u => u.Id != id);
                var usersDto = UserConverter.ToResponseUser(users);

                await _userRepository.UpdateUserLocationAsync(id, location);
                return CustomResponse(usersDto);
            }

            NotifierError("Unable to get authentication cookie");
            return CustomResponse();
        }

        /* 
         * 
        [HttpGet("closest")]
        [ProducesDefaultResponseType(typeof(ResponseDto))]
        public async Task<IActionResult> GetClosestUsers([FromQuery] Location location)
        {
            if (!LocationValidator.Validate(location))
            {
                NotifierError("The location provided is not valid.");
                return CustomResponse();
            }
            if (Request.Cookies.TryGetValue(_authSettings.AuthCookieName, out var cookieToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadToken(cookieToken) as JwtSecurityToken;
                if (!ObjectId.TryParse(jwt.Subject, out var id))
                    throw new ArgumentNullException(nameof(jwt));

                var users = await _userRepository.GetClosestUsersAsync(location);
                users = users.Where(u => u.Id != id);

                var usersDto = UserConverter.ToResponseUser(users);
                return CustomResponse(usersDto);
            }

            NotifierError("Unable to get authentication cookie");
            return CustomResponse();
        } */
    }
}

