using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Observer
{
    public interface Observer
    {
        void Update(NotificationEvent notificationEvent);
    }
}
