using Microsoft.Extensions.Logging;

namespace Votify.Core.Observer
{
    public class LogEmailSender : IEmailSender
    {
        private readonly ILogger<LogEmailSender> _logger;

        public LogEmailSender(ILogger<LogEmailSender> logger)
        {
            _logger = logger;
        }

        public void Send(NotificationEvent notificationEvent)
        {
            _logger.LogInformation(
                "[EMAIL] Para: {Email} | Asunto: {Subject} | Mensaje: {Message}",
                notificationEvent.RecipientEmail,
                notificationEvent.Subject,
                notificationEvent.Message);
        }
    }
}
