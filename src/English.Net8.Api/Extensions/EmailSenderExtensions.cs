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

        public static Task SendEmailResetPasswordAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Reset Password",
               $"Hi, <br /><br />You can click <a href='{link}'>here</a> to reset your password. <br /><br />" +
               $"If you didn’t ask to reset your password, you can ignore this message. <br /> Thanks, good studies 👋\r\n");
        }
    }
}
