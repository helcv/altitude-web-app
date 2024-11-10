using backend.DTOs;
using backend.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmail(EmailMessageDto emailMessageDto)
        {
            var client = new SendGridClient(_config["MailSettings:ApiKey"]);
            var from = new EmailAddress(_config["MailSettings:From"]);
            var subject = $"{emailMessageDto.Subject}";
            var to = new EmailAddress(emailMessageDto.Email);
            var text = "";
            var htmlContent = $"<strong>{emailMessageDto.Callback}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, text, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
