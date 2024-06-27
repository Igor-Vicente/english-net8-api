using English.Net8.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(14)]
        public string? Phone { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(1000)]
        public string? AvatarUrl { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }
    }

    public class ResponseUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? City { get; set; }
        public Location? Location { get; set; }
    }
}
