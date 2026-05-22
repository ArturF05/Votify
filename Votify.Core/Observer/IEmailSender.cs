namespace Votify.Core.Observer
{
    public interface IEmailSender
    {
        void Send(NotificationEvent notificationEvent);
    }
}
