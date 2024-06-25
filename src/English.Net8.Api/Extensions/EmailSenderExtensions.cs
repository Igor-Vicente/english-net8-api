using English.Net8.Api.Services.Mailing;
using System.Text.Encodings.Web;

namespace English.Net8.Api.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"<p> Please confirm your account by clicking this <a href='{HtmlEncoder.Default.Encode(link)}'>link</a> </p>");
        }
    }
}
