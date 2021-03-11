//******************************************************************************************************
//  ScheduledEmail.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/28/2020 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using System.Text;
using GSF.Configuration;
using GSF.Data;
using GSF.Diagnostics;
using GSF.TimeSeries.Adapters;
using MeasurementRecord = openHistorian.Model.Measurement;
using SignalTypeRecord = openHistorian.Model.SignalType;
using System.IO;
using GSF.IO;
using GSF;
using GSF.Net.Smtp;
using System.Timers;
using System.Xml.Linq;
using GSF.Xml;
using System.Threading;

// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
namespace openHistorian.Adapters
{
    /// <summary>
    /// The data Source used to get the XML File for creating the email Content
    /// </summary>
    public enum EmailDataSource
    {
        /// <summary>
        /// Pull Data from the Database using a SQL statement
        /// </summary>
        DataBase,
        /// <summary>
        /// Pull Data from an existing XML File
        /// </summary>
        File

    }

    /// <summary>
    /// Defines an Adapter that calculates Unbalance Factors.
    /// </summary>
    [Description("Scheduled Email: Sends an HTML/XML email based on a schedule.")]
    public class ScheduledEmail : FacileActionAdapterBase
    {
        #region [ Members ]

        
        // Constants      

        private const string ReportSettingsCategory = "snrSQLReportingDB";

        /// <summary>
        /// Default value for <see cref="EmailDatasource"/>.
        /// </summary>
        public const EmailDataSource DefaultEmailDataSource = EmailDataSource.File;

       



        // Fields
        private readonly Mail m_mailClient;
        private System.Timers.Timer m_timer;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="ScheduledEmail"/>.
        /// </summary>
        public ScheduledEmail()
        {
            m_mailClient = new Mail();
            m_mailClient.IsBodyHtml = true;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the Data Source used to create the Email Content 
        /// </summary>
        [ConnectionStringParameter]
        [Description("Sets the DataSource used for creating the email.")]
        [DefaultValue(DefaultEmailDataSource)]
        public EmailDataSource EmailDatasource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the e-mail address of the message sender.
        /// </summary>
        /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
        [ConnectionStringParameter,
        Description("Define the e-mail address of the message sender.")]
        public string From
        {
            get => m_mailClient.From;
            set => m_mailClient.From = value;
        }

        /// <summary>
        /// Gets or sets the comma-separated or semicolon-separated e-mail address list of the message recipients.
        /// </summary>
        /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
        [ConnectionStringParameter,
        Description("Define the comma-separated or semicolon-separated e-mail address list of the e-mail message recipients."),
        DefaultValue("")]
        public string ToRecipients
        {
            get => m_mailClient.ToRecipients;
            set => m_mailClient.ToRecipients = value;
        }

        /// <summary>
        /// Gets or sets the comma-separated or semicolon-separated e-mail address list of the message carbon copy (CC) recipients.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the comma-separated or semicolon-separated e-mail address list of the e-mail message carbon copy (CC) recipients."),
        DefaultValue("")]
        public string CcRecipients
        {
            get => m_mailClient.CcRecipients;
            set => m_mailClient.CcRecipients = value;
        }

        /// <summary>
        /// Gets or sets the comma-separated or semicolon-separated e-mail address list of the message blank carbon copy (BCC) recipients.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the comma-separated or semicolon-separated e-mail address list of the e-mail message blank carbon copy (BCC) recipients."),
        DefaultValue("")]
        public string BccRecipients
        {
            get => m_mailClient.BccRecipients;
            set => m_mailClient.BccRecipients = value;
        }

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the subject of the e-mail message.")]
        public string Subject
        {
            get => m_mailClient.Subject;
            set => m_mailClient.Subject = value;
        }

        /// <summary>
        /// Gets or sets the username used to authenticate to the SMTP server.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the username used to authenticate to the SMTP server."),
        DefaultValue("")]
        public string Username
        {
            get => m_mailClient.Username;
            set => m_mailClient.Username = value;
        }

        /// <summary>
        /// Gets or sets the password used to authenticate to the SMTP server.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the password used to authenticate to the SMTP server."),
        DefaultValue("")]
        public string Password
        {
            get => m_mailClient.Password;
            set => m_mailClient.Password = value;
        }

        /// <summary>
        /// Gets or sets the flag that determines whether to use SSL when communicating with the SMTP server.
        /// </summary>
        [ConnectionStringParameter,
        Description("Define the flag that determines whether to use SSL when communicating with the SMTP server."),
        DefaultValue(false)]
        public bool EnableSSL
        {
            get => m_mailClient.EnableSSL;
            set => m_mailClient.EnableSSL = value;
        }

        /// <summary>
        /// Gets or sets the name or IP address of the SMTP server to be used for sending the message.
        /// </summary>
        /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
        [ConnectionStringParameter,
        Description("Define the name or IP address of the SMTP server to be used for sending the e-mail message.")]
        public string SmtpServer
        {
            get => m_mailClient.SmtpServer;
            set => m_mailClient.SmtpServer = value;
        }

        /// <summary>
        /// Gets or sets the html Template to generate the email.
        /// </summary>
        /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
        [ConnectionStringParameter,
        Description("The html template file used to generate the email."),
        DefaultValue("")]
        [CustomConfigurationEditor("GSF.TimeSeries.UI.WPF.dll", "GSF.TimeSeries.UI.Editors.FileDialogEditor", "type=open; checkFileExists=true; defaultExt=.html; filter=HTML files|*.html|AllFiles|*.*")]
        public string TemplateFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the xml datafile used generate the email if <see cref="EmailDatasource"/> is set to <see cref="EmailDataSource.File"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Value being assigned is a null or empty string.</exception>
        [ConnectionStringParameter,
        Description("The xml datafile used to generate the email if a File is selected as DataSource or the .txt file with the SQL Query."),
        DefaultValue("")]
        [CustomConfigurationEditor("GSF.TimeSeries.UI.WPF.dll", "GSF.TimeSeries.UI.Editors.FileDialogEditor", "type=open; checkFileExists=true; defaultExt=.html; filter=HTML files|*.html|AllFiles|*.*")]
        public string DataFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Hour at which the Email should be send.
        /// </summary>
        [ConnectionStringParameter,
        Description("The hour at which this email should be sent (0-23)."),
        DefaultValue(0)]
        public int Hour
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets minute at which the Email should be send.
        /// </summary>
        [ConnectionStringParameter,
        Description("The minute at which this email should be sent (0-59)."),
        DefaultValue(0)]
        public int Minute
        {
            get;
            set;
        }

        /// <summary>
        /// Gets flag that determines if temporal processing is supported.
        /// </summary>
        public override bool SupportsTemporalProcessing => false;

        /// <summary>
        /// Gets a detailed status for this <see cref="ScheduledEmail"/>.
        /// </summary>
        public override string Status
        {
            get
            {

                StringBuilder status = new StringBuilder();

                const int descriptionLength = 35;

                status.AppendFormat("DataSource selected for email: {0}".PadLeft(descriptionLength), EmailDatasource.GetFormattedName());
                status.AppendLine();
                status.AppendFormat("Template file: {0}".PadLeft(descriptionLength), TemplateFile);
                status.AppendLine();
                status.AppendFormat("Scheduled to be sent at: {0}:{1}".PadLeft(descriptionLength+4), Hour,Minute);
                status.AppendLine();
                status.AppendFormat("Scheduled to be sent in: {0} s".PadLeft(descriptionLength+2), ComputeSeconds());
                status.AppendLine();
                if (EmailDatasource == EmailDataSource.File)
                {
                    status.AppendFormat("Data file: {0}".PadLeft(descriptionLength), DataFile);
                    status.AppendLine();
                }
                status.Append(base.Status);

                return status.ToString();
            }
        }

        

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes <see cref="ScheduledEmail"/>.
        /// </summary>
        public override void Initialize()
        {
            ParseConnectionString();
            // Start Timer for now we just fix it at 30 for testing
            // We do not reset the timer to avoid email flooding at this stage
            m_timer = new System.Timers.Timer();
            if (m_timer != null)
            {
                m_timer.Interval = ComputeSeconds()*1000;
                m_timer.AutoReset = true;
                m_timer.Elapsed += m_timerElapsed;
            }

            m_timer.Start();

        }

        private int ComputeSeconds()
        {
            DateTime now = DateTime.Now;
            int h = Hour - now.Hour;
            int m = Minute - now.Minute;
            int s = 0 - now.Second;
            if (s < 0)
            {
                s += 60;
                m -= 1;
            }

            if (m < 0)
            {
                h -= 1;
                m += 60;
            }
            if (h < 0)
                h += 24;

            m += h * 60;
            s += m * 60;

            return s;

        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ScheduledEmail"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (m_timer != null)
                        {
                            m_timer.Elapsed -= m_timerElapsed;
                            m_timer.Dispose();
                            m_timer = null;
                        }

                        if (m_mailClient != null)
                        {
                            m_mailClient.Dispose();
                        }
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Parses connection string.
        /// </summary>
        private void ParseConnectionString()
        {
            const string MissingRequiredMailSetting = "Missing required e-mail setting: \"{0}\"";

            // Parse all properties marked with ConnectionStringParameterAttribute from provided ConnectionString value
            ConnectionStringParser parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();
            parser.ParseConnectionString(ConnectionString, this);

            // Check Required Mail Settings
            if (From is null  || string.IsNullOrWhiteSpace(From))
                throw new ArgumentException(string.Format(MissingRequiredMailSetting, "from"));

            if (Subject is null || string.IsNullOrWhiteSpace(Subject))
                throw new ArgumentException(string.Format(MissingRequiredMailSetting, "subject"));

            if (SmtpServer is null || string.IsNullOrWhiteSpace(SmtpServer))
                throw new ArgumentException(string.Format(MissingRequiredMailSetting, "smtpServer"));

            if (TemplateFile is null || string.IsNullOrWhiteSpace(TemplateFile))
                throw new ArgumentException(string.Format(MissingRequiredMailSetting, "templateFile"));

        }

        /// <summary>
        /// Gets a short one-line status of this <see cref="AdapterBase"/>.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>A short one-line summary of the current status of this <see cref="AdapterBase"/>.</returns>
        public override string GetShortStatus(int maxLength)
        {
            return $"Email is scheduled for {Hour}:{Minute}".CenterText(maxLength);
        }

        /// <summary>
        /// Sends the  <see cref="ScheduledEmail"/>.
        /// </summary>
        [AdapterCommand("Sends the Email imidiately.", "Administrator", "Editor")]
        public void Send()
        {
            m_sendEmail();
        }


        private void m_timerElapsed(object sender, ElapsedEventArgs e)
        {

            OnStatusMessage(MessageLevel.Info, "Timer Expired");

            if (Monitor.TryEnter(m_timer))
            {
                try
                {
                    m_sendEmail();
                }
                finally
                {
                    m_timer.Interval = ComputeSeconds() * 1000;
                    OnStatusMessage(MessageLevel.Info, string.Format("Reset Timer to {0} seconds",m_timer.Interval));
                    Monitor.Exit(m_timer);
                }
            
            }
        }

        private void m_sendEmail()
        {
            OnStatusMessage(MessageLevel.Info, "Preparing Email");

            // Prepare Email to be sent.
            try
                {
                    //Read XLS Template File
                    string template = "";
                    using (StreamReader reader = new StreamReader(FilePath.GetAbsolutePath(TemplateFile)))
                        template = reader.ReadToEnd();

                    //Read Data
                    string data = "";
                    if (EmailDatasource == EmailDataSource.File)
                    {
                        using (StreamReader reader = new StreamReader(FilePath.GetAbsolutePath(DataFile)))
                            data = reader.ReadToEnd();
                    }
                    if (EmailDatasource == EmailDataSource.DataBase)
                    {
                        string sqlquery = "";
                        using (StreamReader reader = new StreamReader(FilePath.GetAbsolutePath(DataFile)))
                            sqlquery = reader.ReadToEnd();

                        sqlquery = string.Format(sqlquery, DateTime.Today.RoundDownToNearestDay());
                        using (AdoDataConnection connection = new AdoDataConnection(ReportSettingsCategory))
                            data = connection.ExecuteScalar<string>(sqlquery);
                    }


                    XDocument email = ApplyTemplate(template, data);

                    OnStatusMessage(MessageLevel.Info, "sending Email.....");

                    m_mailClient.Body = GetBody(email);
                    m_mailClient.Send();
                OnStatusMessage(MessageLevel.Info, "Email Sent Succesfully");
            }
                catch (Exception ex)
                {
                    OnStatusMessage(MessageLevel.Error, $"Failed to Send Email {ex.Message}");
                }
            
        }


        private string GetBody(XDocument htmlDocument)
        {
            return htmlDocument
                .ToString(SaveOptions.DisableFormatting)
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">");
        }

        private XDocument ApplyTemplate(string xlsTemplate, string xmlData)
        {
            string data = xmlData.Replace("&gt;", ">");
            data = data.Replace("&lt;", "<");

            string htmlText = data.ApplyXSLTransform(xlsTemplate);
            XDocument htmlDocument = XDocument.Parse(htmlText, LoadOptions.PreserveWhitespace);
            htmlDocument.TransformAll("format", element => element.Format());
            return htmlDocument;
            
        }

        #endregion
    }
}
