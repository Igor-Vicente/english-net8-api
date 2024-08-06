using English.Net8.Api.Dtos.Account;
using English.Net8.Api.Models;

namespace English.Net8.Api.Utils
{
    public static class UserConverter
    {
        public static UserResponseDto ToResponseUser(User user)
        {
            return new UserResponseDto
            {
                Bio = user.Bio,
                BirthDate = user.BirthDate,
                CurrentCity = user.CurrentCity,
                CurrentCountry = user.CurrentCountry,
                Email = user.Email,
                EnglishLevel = user.EnglishLevel,
                Id = user.Id.ToString(),
                Location = user.Location,
                Name = user.Name,
                Phone = user.Phone,
                Hobbies = user.Hobbies,
                PersonalSiteLink = user.PersonalSiteLink,
                FacebookLink = user.FacebookLink,
                GithubLink = user.GithubLink,
                InstagramLink = user.InstagramLink,
                LinkedinLink = user.LinkedinLink,
                TwitterLink = user.TwitterLink
            };
        }
        public static UserResponseWithDistanceDto ToResponseUser(UserWithDistance userWithDistance)
        {
            return new UserResponseWithDistanceDto
            {
                Bio = userWithDistance.Bio,
                BirthDate = userWithDistance.BirthDate,
                CurrentCity = userWithDistance.CurrentCity,
                CurrentCountry = userWithDistance.CurrentCountry,
                Email = userWithDistance.Email,
                EnglishLevel = userWithDistance.EnglishLevel,
                Id = userWithDistance.Id.ToString(),
                Location = userWithDistance.Location,
                Name = userWithDistance.Name,
                Phone = userWithDistance.Phone,
                Hobbies = userWithDistance.Hobbies,
                PersonalSiteLink = userWithDistance.PersonalSiteLink,
                FacebookLink = userWithDistance.FacebookLink,
                GithubLink = userWithDistance.GithubLink,
                InstagramLink = userWithDistance.InstagramLink,
                LinkedinLink = userWithDistance.LinkedinLink,
                TwitterLink = userWithDistance.TwitterLink,
                Distance = Math.Ceiling(userWithDistance.Distance),
            };
        }

        public static IEnumerable<UserResponseWithDistanceDto> ToResponseUser(IEnumerable<UserWithDistance> users)
        {
            var usersDto = new List<UserResponseWithDistanceDto>();
            foreach (var user in users)
            {
                var userDto = ToResponseUser(user);
                usersDto.Add(userDto);
            }

            return usersDto;
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
                Bio = updateUserDto.Bio,
                BirthDate = updateUserDto.BirthDate,
                CurrentCity = updateUserDto.CurrentCity,
                Name = updateUserDto.Name,
                Phone = updateUserDto.Phone,
                Hobbies = updateUserDto.Hobbies,
                EnglishLevel = updateUserDto.EnglishLevel,
                PersonalSiteLink = updateUserDto.PersonalSiteLink,
                FacebookLink = updateUserDto.FacebookLink,
                GithubLink = updateUserDto.GithubLink,
                InstagramLink = updateUserDto.InstagramLink,
                LinkedinLink = updateUserDto.LinkedinLink,
                TwitterLink = updateUserDto.TwitterLink,
                CurrentCountry = updateUserDto.CurrentCountry,
            };
        }

    }
}
