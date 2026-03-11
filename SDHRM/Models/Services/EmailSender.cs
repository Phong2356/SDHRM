using Microsoft.Extensions.Options;
using SDHRM.Models.Process;
using System.Net.Mail;
using System.Net;

namespace SDHRM.Models.Services
{
    public class EmailSender
    {
        private readonly MailSettings _settings;

        public EmailSender(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_settings.Host, _settings.Port))
            {
                client.Credentials = new NetworkCredential(_settings.Mail, _settings.Password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_settings.Mail, _settings.DisplayName);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}