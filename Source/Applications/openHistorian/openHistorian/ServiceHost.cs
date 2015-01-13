//******************************************************************************************************
//  ServiceHost.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/02/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using GSF;
using GSF.Configuration;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.TimeSeries;
using GSF.Units;
using Timer = System.Timers.Timer;

namespace openHistorian
{
    public class ServiceHost : ServiceHostBase
    {
        #region [ Members ]

        // Constants
        private const int DefaultMaximumDiagnosticLogSize = 10;

        // Fields
        private string m_diagnosticLogPath;
        private long m_maximumDiagnosticLogSize;
        private Timer m_logCurtailmentTimer;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ServiceHost"/> instance.
        /// </summary>
        public ServiceHost()
        {
            ServiceName = "openHistorian";
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServiceHost"/> object and optionally releases the managed resources.
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
                        if ((object)m_logCurtailmentTimer != null)
                        {
                            m_logCurtailmentTimer.Stop();
                            m_logCurtailmentTimer.Elapsed -= m_logCurtailmentTimer_Elapsed;
                            m_logCurtailmentTimer.Dispose();
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
        /// Event handler for service starting operations.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments containing command line arguments passed into service at startup.</param>
        protected override void ServiceStartingHandler(object sender, EventArgs<string[]> e)
        {
            // Handle base class service starting procedures
            base.ServiceStartingHandler(sender, e);

            // Make sure openHistorian specific default service settings exist
            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];

            systemSettings.Add("CompanyName", "Grid Protection Alliance", "The name of the company who owns this instance of the openHistorian.");
            systemSettings.Add("CompanyAcronym", "GPA", "The acronym representing the company who owns this instance of the openHistorian.");
            systemSettings.Add("MemoryPoolSize", "0.0", "The fixed memory pool size in Gigabytes. Leave at zero for dynamically calculated setting.");
            systemSettings.Add("MemoryPoolTargetUtilization", "Low", "The target utilization level for the memory pool. One of 'Low', 'Medium', or 'High'.");
            systemSettings.Add("DiagnosticLogPath", FilePath.GetAbsolutePath(""), "Path for diagnostic logs.");
            systemSettings.Add("MaximumDiagnosticLogSize", DefaultMaximumDiagnosticLogSize, "The combined maximum size for the diagnostic logs in whole Megabytes; curtailment happens hourly. Set to zero for no limit.");

            // Set maximum buffer size
            double memoryPoolSize = systemSettings["MemoryPoolSize"].ValueAs(0.0D);

            if (memoryPoolSize > 0.0D)
                Globals.MemoryPool.SetMaximumBufferSize((long)(memoryPoolSize * SI2.Giga));

            TargetUtilizationLevels targetLevel;

            if (!Enum.TryParse(systemSettings["MemoryPoolTargetUtilization"].Value, false, out targetLevel))
                targetLevel = TargetUtilizationLevels.High;

            Globals.MemoryPool.SetTargetUtilizationLevel(targetLevel);

            // Set default logging path
            m_diagnosticLogPath = systemSettings["DiagnosticLogPath"].Value;

            if (string.IsNullOrWhiteSpace(m_diagnosticLogPath) || !Directory.Exists(m_diagnosticLogPath))
                m_diagnosticLogPath = FilePath.GetAbsolutePath("");

            GSF.Diagnostics.Logger.SetLoggingPath(m_diagnosticLogPath);

            // Get maximum diagnostic log size
            m_maximumDiagnosticLogSize = SI2.Mega * systemSettings["MaximumDiagnosticLogSize"].ValueAs(DefaultMaximumDiagnosticLogSize);

            if (m_maximumDiagnosticLogSize > 0)
            {
                m_logCurtailmentTimer = new Timer(Time.SecondsPerHour * 1000.0D);
                m_logCurtailmentTimer.AutoReset = true;
                m_logCurtailmentTimer.Elapsed += m_logCurtailmentTimer_Elapsed;
                m_logCurtailmentTimer.Enabled = true;
            }
        }

        private void m_logCurtailmentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(m_logCurtailmentTimer))
                return;

            try
            {
                long totalBytes = 0L;
                int curtailmentStartIndex = -1;

                FileInfo[] logFiles =
                    FilePath.GetFileList(Path.Combine(m_diagnosticLogPath, "*.logbin")).
                    OrderByDescending(file => Convert.ToInt64(FilePath.GetFileNameWithoutExtension(file))).
                    Select(file => new FileInfo(file)).
                    ToArray();

                for (int i = 1; i < logFiles.Length; i++)
                {
                    totalBytes += logFiles.LongLength;

                    if (totalBytes > m_maximumDiagnosticLogSize)
                    {
                        curtailmentStartIndex = i - 1;
                        break;
                    }
                }

                if (curtailmentStartIndex > -1)
                    for (int i = curtailmentStartIndex; i < logFiles.Length; i++)
                        logFiles[i].Delete();
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException(string.Format("Failed to curtail diagnostic logs due to an exception: {0}", ex.Message), ex));
            }
            finally
            {
                Monitor.Exit(m_logCurtailmentTimer);
            }
        }

        #endregion
    }
}