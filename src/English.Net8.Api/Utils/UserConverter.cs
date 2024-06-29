using English.Net8.Api.Dtos;
using English.Net8.Api.Models;

namespace English.Net8.Api.Utils
{
    public static class UserConverter
    {
        public static UserResponseDto ToResponseUser(User user)
        {
            return new UserResponseDto
            {
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                BirthDate = user.BirthDate,
                City = user.City,
                Email = user.Email,
                Id = user.Id.ToString(),
                Location = user.Location,
                Name = user.Name,
                Phone = user.Phone,
            };
        }

        public static IEnumerable<UserResponseDto> ToResponseUser(IEnumerable<User> users)
        {
            var usersDto = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var userDto = ToResponseUser(user);
                usersDto.Add(userDto);
            }

            return usersDto;
        }

        public static User ToUser(UpdateUserDto updateUserDto)
        {
            return new User
            {
                AvatarUrl = updateUserDto.AvatarUrl,
                Bio = updateUserDto.Bio,
                BirthDate = updateUserDto.BirthDate,
                City = updateUserDto.City,
                Name = updateUserDto.Name,
                Phone = updateUserDto.Phone,
            };
        }
    }
}
