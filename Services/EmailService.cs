using System.Net;
using System.Net.Mail;
using HeThongDatLichVaKhamBenh.Models;
using Microsoft.Extensions.Options;

namespace HeThongDatLichVaKhamBenh.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrEmpty(_emailSettings.Mail) || _emailSettings.Mail == "your_email@gmail.com")
                {
                    _logger.LogWarning("Email sending skipped because SMTP is not configured in appsettings.");
                    return;
                }

                using var message = new MailMessage();
                message.From = new MailAddress(_emailSettings.Mail, _emailSettings.DisplayName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = htmlBody;
                message.IsBodyHtml = true;

                using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
                _logger.LogInformation($"Email sent to {toEmail} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
            }
        }
    }
}
