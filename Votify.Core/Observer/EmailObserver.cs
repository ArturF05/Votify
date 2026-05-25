namespace Votify.Core.Observer
{
    public class EmailObserver : Observer
    {
        private readonly string _recipientEmail;
        private readonly IEmailSender _emailSender;

        public EmailObserver(string recipientEmail, IEmailSender emailSender)
        {
            _recipientEmail = recipientEmail;
            _emailSender = emailSender;
        }

        public void Update(NotificationEvent notificationEvent)
        {
            var evt = new NotificationEvent(_recipientEmail, notificationEvent.Subject, notificationEvent.Message);
            _emailSender.Send(evt);
        }

        public async Task UpdateAsync(NotificationEvent notificationEvent)
        {
            var evt = new NotificationEvent(_recipientEmail, notificationEvent.Subject, notificationEvent.Message);
            await _emailSender.SendAsync(evt);
        }
    }
}
