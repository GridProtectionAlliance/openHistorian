//******************************************************************************************************
//  INotifier.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  05/26/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;
using TVA.Adapters;

namespace openHistorian.Notifiers
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates the type of notification being sent using a <see cref="INotifier">Notifier</see>.
    /// </summary>
    [Flags()]
    public enum NotificationTypes
    {
        /// <summary>
        /// Notification is of unknown type.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Notification is informational in nature.
        /// </summary>
        Information = 1,
        /// <summary>
        /// Notification is being sent to report a warning.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Notification is being sent to report an alarm.
        /// </summary>
        Alarm = 4,
        /// <summary>
        /// Notification is being sent to report activity.
        /// </summary>
        Heartbeat = 8
    }

    #endregion

    /// <summary>
    /// Defines a notifier that can process notification messages.
    /// </summary>
    /// <seealso cref="NotificationTypes"/>
    public interface INotifier : IAdapter
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when a notification is being sent.
        /// </summary>
        event EventHandler NotificationSendStart;

        /// <summary>
        /// Occurs when a notification has been sent.
        /// </summary>
        event EventHandler NotificationSendComplete;

        /// <summary>
        /// Occurs when a timeout is encountered while sending a notification.
        /// </summary>
        event EventHandler NotificationSendTimeout;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered while sending a notification.
        /// </summary>
        event EventHandler<EventArgs<Exception>> NotificationSendException;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the number of seconds to wait for <see cref="Notify"/> to complete.
        /// </summary>
        int NotifyTimeout { get; set; }

        /// <summary>
        /// Gets or set <see cref="NotificationTypes"/> that can be processed by the notifier.
        /// </summary>
        NotificationTypes NotifyOptions { get; set;}

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Process a notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        /// <param name="notificationType">One of the <see cref="NotificationTypes"/> values.</param>
        /// <returns>true if notification is processed successfully; otherwise false.</returns>
        bool Notify(string subject, string message, string details, NotificationTypes notificationType);

        #endregion
    }
}