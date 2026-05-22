using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Observer
{
    public class NotificationEvent
    {
        public string RecipientEmail { get; }
        public string Subject { get; }
        public string Message { get; }

        public NotificationEvent(string recipientEmail, string subject, string message)
        {
            RecipientEmail = recipientEmail;
            Subject = subject;
            Message = message;
        }
    }
}
