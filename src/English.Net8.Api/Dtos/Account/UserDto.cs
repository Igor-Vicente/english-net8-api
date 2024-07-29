using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Account
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(14)]
        public string? Phone { get; set; }

        [MaxLength(2000)]
        public string? Bio { get; set; }

        [MaxLength(100)]
        public string? ContactMeOn { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? Hobbies { get; set; }
    }

    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public string? ContactMeOn { get; set; }
        public string Hobbies { get; set; }
        public bool IsPremium { get; set; }
        public bool IsAdmin { get; set; }
        public Location? Location { get; set; }
    }

    public class UserResponseWithDistanceDto : UserResponseDto
    {
        public double Distance { get; set; }
    }
}
