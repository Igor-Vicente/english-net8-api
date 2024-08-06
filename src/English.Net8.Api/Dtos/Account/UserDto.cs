using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Account
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [Range(0, 3)]
        public EnglishLevel EnglishLevel { get; set; }

        [MaxLength(14)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Bio { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CurrentCity { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CurrentCountry { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Hobbies { get; set; } = string.Empty;

        [MaxLength(100)]
        public string PersonalSiteLink { get; set; } = string.Empty;

        [MaxLength(100)]
        public string InstagramLink { get; set; } = string.Empty;

        [MaxLength(100)]
        public string GithubLink { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FacebookLink { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TwitterLink { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LinkedinLink { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }
        public EnglishLevel EnglishLevel { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string CurrentCity { get; set; } = string.Empty;
        public string CurrentCountry { get; set; } = string.Empty;
        public string PersonalSiteLink { get; set; } = string.Empty;
        public string InstagramLink { get; set; } = string.Empty;
        public string GithubLink { get; set; } = string.Empty;
        public string FacebookLink { get; set; } = string.Empty;
        public string TwitterLink { get; set; } = string.Empty;
        public string LinkedinLink { get; set; } = string.Empty;
        public string Hobbies { get; set; } = string.Empty;
        public Location? Location { get; set; }
    }

    public class UserResponseWithDistanceDto : UserResponseDto
    {
        public double Distance { get; set; }
    }
}
