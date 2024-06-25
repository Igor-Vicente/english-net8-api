using English.Net8.Api.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace English.Net8.Api.Services.Mailing
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailProviderSettings _emailProviderSettings;

        public EmailSender(IOptions<EmailProviderSettings> emailSettings)
        {
            _emailProviderSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_emailProviderSettings.ApiKey);
            var from = new EmailAddress(_emailProviderSettings.DomainEmail);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Send message via SendGrid failed. Status Code: {response.StatusCode}. Message: {response.Body.ReadAsStringAsync().Result}");
        }
    }
}
