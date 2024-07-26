using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Dtos.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
