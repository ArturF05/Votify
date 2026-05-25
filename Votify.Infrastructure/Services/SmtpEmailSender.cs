using System.Net;
using System.Net.Mail;
using Votify.Core.Interfaces;
using Votify.Core.Observer;

namespace Votify.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguracionSmtpRepository _configRepo;

        public SmtpEmailSender(IConfiguracionSmtpRepository configRepo)
        {
            _configRepo = configRepo;
        }

        public void Send(NotificationEvent notificationEvent)
        {
            SendAsync(notificationEvent).GetAwaiter().GetResult();
        }

        public async Task SendAsync(NotificationEvent notificationEvent)
        {
            var config = await _configRepo.ObtenerAsync();

            if (config == null || string.IsNullOrWhiteSpace(config.UserName) || string.IsNullOrWhiteSpace(config.Password))
                return;

            using var client = new SmtpClient(config.Host, config.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(config.UserName, config.Password)
            };

            var message = new MailMessage(
                new MailAddress(config.UserName, config.FromName),
                new MailAddress(notificationEvent.RecipientEmail))
            {
                Subject = notificationEvent.Subject,
                Body = notificationEvent.Message
            };

            await client.SendMailAsync(message);
        }
    }
}
