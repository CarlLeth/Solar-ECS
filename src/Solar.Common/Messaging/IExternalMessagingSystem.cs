using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Messaging
{
    /// <summary>
    /// Allows the sending of messages to users via a channel external to the main application, e.g. for security alerts or two-factor authentication.
    /// </summary>
    public interface IExternalMessagingSystem
    {
        /// <summary>
        /// Send a message to the given user via a channel external to the main application.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task SendMessage(Guid to, string subject, string body);
    }
}
