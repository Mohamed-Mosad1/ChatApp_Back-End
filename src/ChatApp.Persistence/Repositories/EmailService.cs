using ChatApp.Application.Persistence.Contracts;
using ChatApp.Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace ChatApp.Persistence.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();
            var fromName = _configuration["EmailSettings:FromName"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = emailModel.Body
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_configuration["EmailSettings:MailSmtpServer"], 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_configuration["EmailSettings:MailServerUserName"], _configuration["EmailSettings:MailServerPassword"]);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
    }
}
