using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IPushNotificationManager
    {
        bool QueueMessage(string to, string title, string message, string urlNotificationClick);
    }
}
