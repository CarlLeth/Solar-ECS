using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Messaging
{
    /// <summary>
    /// Defines parameters necessary for composing and sending automated messages from the app.
    /// </summary>
    public interface IAppMessagingSettings
    {
        /// <summary>
        /// How the application should refer to itself in messages.
        /// </summary>
        string AppTitle { get; }

        /// <summary>
        /// The email address to use in the "from" header of emails.
        /// </summary>
        string FromEmail { get; }

        /// <summary>
        /// The root URL of the application or where the recipient can go for more information.
        /// </summary>
        string Url { get; }

    }
}
