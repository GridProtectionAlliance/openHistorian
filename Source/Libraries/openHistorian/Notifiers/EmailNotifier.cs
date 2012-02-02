//******************************************************************************************************
//  EmailNotifier.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  05/28/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using TVA.Adapters;
using TVA.Configuration;
using TVA.Net.Smtp;

namespace openHistorian.Notifiers
{
    /// <summary>
    /// Represents a notifier that can send notifications in email messages.
    /// </summary>
    public class EmailNotifier : NotifierBase
    {
        #region [ Members ]

        // Fields
        private string m_emailServer;
        private string m_emailSender;
        private string m_emailRecipients;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotifier"/> class.
        /// </summary>
        public EmailNotifier()
            : base(NotificationTypes.Information | NotificationTypes.Warning | NotificationTypes.Alarm)
        {
            m_emailServer = Mail.DefaultSmtpServer;
            m_emailSender = string.Format("{0}@{1}.local", Environment.UserName, Environment.UserDomainName);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the SMTP server to use for sending the email messages.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string EmailServer
        {
            get
            {
                return m_emailServer;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_emailServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the email address to be used for sending the email messages.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is a null or empty string.</exception>
        public string EmailSender
        {
            get
            {
                return m_emailSender;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                m_emailSender = value;
            }
        }

        /// <summary>
        /// Gets or sets the email addresses (comma or semicolon delimited) where the email messages are to be sent.
        /// </summary>
        /// <remarks>
        /// Email address can be provided in the &lt;Email Address&gt;:sms format (Example: 123456789@provider.com:sms), 
        /// to indicate that the reciepient is a mobile device and a very brief email message is to be sent.
        /// </remarks>
        public string EmailRecipients
        {
            get
            {
                return m_emailRecipients;
            }
            set
            {
                m_emailRecipients = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Saves <see cref="EmailNotifier"/> settings to the config file if the <see cref="Adapter.PersistSettings"/> property is set to true.
        /// </summary>        
        public override void SaveSettings()
        {
            base.SaveSettings();
            if (PersistSettings)
            {
                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings["EmailServer", true].Update(m_emailServer);
                settings["EmailSender", true].Update(m_emailSender);
                settings["EmailRecipients", true].Update(m_emailRecipients);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved <see cref="EmailNotifier"/> settings from the config file if the <see cref="Adapter.PersistSettings"/> property is set to true.
        /// </summary>        
        public override void LoadSettings()
        {
            base.LoadSettings();
            if (PersistSettings)
            {
                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("EmailServer", m_emailServer, "SMTP server to use for sending the email notifications.");
                settings.Add("EmailSender", m_emailSender, "Email address to be used for sending the email notifications.");
                settings.Add("EmailRecipients", m_emailRecipients, "Email addresses (comma or semicolon delimited) where the email notifications are to be sent.");
                EmailServer = settings["EmailServer"].ValueAs(m_emailServer);
                EmailSender = settings["EmailSender"].ValueAs(m_emailSender);
                EmailRecipients = settings["EmailRecipients"].ValueAs(m_emailRecipients);
            }
        }

        /// <summary>
        /// Processes a <see cref="NotificationTypes.Alarm"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected override void NotifyAlarm(string subject, string message, string details)
        {
            SendEmail("ALRM: " + subject, message, details);
        }

        /// <summary>
        /// Processes a <see cref="NotificationTypes.Warning"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected override void NotifyWarning(string subject, string message, string details)
        {
            SendEmail("WARN: " + subject, message, details);
        }

        /// <summary>
        /// Processes a <see cref="NotificationTypes.Information"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected override void NotifyInformation(string subject, string message, string details)
        {
            SendEmail("INFO: " + subject, message, details);
        }

        /// <summary>
        /// Processes a <see cref="NotificationTypes.Heartbeat"/> notification.
        /// </summary>
        /// <param name="subject">Subject matter for the notification.</param>
        /// <param name="message">Brief message for the notification.</param>
        /// <param name="details">Detailed message for the notification.</param>
        protected override void NotifyHeartbeat(string subject, string message, string details)
        {
            throw new NotSupportedException();
        }

        private void SendEmail(string subject, string message, string details)
        {
            if (string.IsNullOrEmpty(m_emailRecipients))
                throw new ArgumentNullException("EmailRecipients");

            Mail briefMessage = new Mail(m_emailSender, m_emailSender, m_emailServer);
            Mail detailedMessage = new Mail(m_emailSender, m_emailSender, m_emailServer);

            briefMessage.Subject = subject;
            detailedMessage.Subject = subject;
            detailedMessage.Body = message + "\r\n\r\n" + details;
            foreach (string recipient in m_emailRecipients.Replace(" ", "").Split(';', ','))
            {
                string[] addressParts = recipient.Split(':');
                if (addressParts.Length > 1)
                {
                    if (string.Compare(addressParts[1], "sms", true) == 0)
                    {
                        // A brief message is to be sent.
                        briefMessage.ToRecipients = addressParts[0];
                        briefMessage.Send();
                    }
                }
                else
                {
                    // A detailed message is to be sent.
                    detailedMessage.ToRecipients = recipient;
                    detailedMessage.Send();
                }
            }
        }

        #endregion
    }
}