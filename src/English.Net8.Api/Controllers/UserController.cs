using English.Net8.Api.Dtos;
using English.Net8.Api.Dtos.Account;
using English.Net8.Api.Models;
using English.Net8.Api.Repository.Interfaces;
using English.Net8.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;

namespace English.Net8.Api.Controllers
{
    [Authorize]
    [Route("api/v1/user")]
    public class UserController : MainController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("authenticated")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> GetUserAsync()
        {
            if (!ObjectId.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return ErrorResponse("Invalid userId");

            var user = await _userRepository.FindByIdAsync(userId);

            return SuccessResponse(UserConverter.ToResponseUser(user));
        }

        [HttpPut("details")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> UpdateUserAsync(UpdateUserDto userDto)
        {
            if (!ModelState.IsValid) return ErrorResponse(ModelState);

            if (userDto.BirthDate != null && userDto.BirthDate > DateTime.Now)
                return ErrorResponse("Birthday date cannot be in the future");

            if (!ObjectId.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return ErrorResponse("Invalid userId");

            var user = UserConverter.ToUser(userDto);
            user.Id = userId;
            user.Email = User.FindFirst(ClaimTypes.Email)!.Value;

            if (string.IsNullOrEmpty(user.Email))
                throw new ApplicationException($"Unable to find the user email '{userId}' 'User.FindFirst(ClaimTypes.Email)!.Value'.");

            await _userRepository.UpdateAsync(user);

            return SuccessResponse(UserConverter.ToResponseUser(user));
        }

        [HttpPut("location")]
        [ProducesResponseType(typeof(SuccessResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<UserResponseDto>>> UpdateUserLocationAsync(Location location)
        {
            if (!LocationValidator.Validate(location))
                return ErrorResponse("The location provided is not valid.");

            if (!ObjectId.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return ErrorResponse("Invalid userId");

            var user = await _userRepository.FindByIdAsync(userId);

            if (user == null)
                return ErrorResponse("User was not found");

            await _userRepository.UpdateUserLocationAsync(user, location);
            user.Location = location;
            return SuccessResponse(UserConverter.ToResponseUser(user));
        }

        [HttpGet("filtered-users")]
        [ProducesResponseType(typeof(SuccessResponseDto<IEnumerable<UserResponseWithDistanceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessResponseDto<IEnumerable<UserResponseWithDistanceDto>>>> GetAllUsersWithDistanceAsync([FromQuery] Location location, bool closest, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ErrorResponse("The page params is not valid.");

            if (!LocationValidator.Validate(location))
                return ErrorResponse("The location provided is not valid.");

            if (!ObjectId.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                return ErrorResponse("Invalid userId");

            IEnumerable<UserWithDistance> users;

            if (closest)
                users = await _userRepository.GetClosestUsersAsync(location, userId, pageNumber, pageSize);
            else
                users = await _userRepository.GetMostDistantUsersAsync(location, userId, pageNumber, pageSize);

            return SuccessResponse(UserConverter.ToResponseUser(users));
        }
    }
}

