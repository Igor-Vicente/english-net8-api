using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
