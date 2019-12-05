//******************************************************************************************************
//  ReportOperationsHubClient.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/10/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.IO;
using GSF.Web.Hubs;
using GSF.Web.Model;
using openHistorian.Model;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines enumeration of supported Report types.
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// Signal-to-Noise Ratio.
        /// </summary>
        SNR,
        /// <summary>
        /// Voltage Unbalances.
        /// </summary>
        Unbalance_V,
        /// <summary>
        /// Current Unbalances.
        /// </summary>
        Unbalance_I
    }

    /// <summary>
    /// Defines enumeration of supported Report Crtieria.
    /// </summary>
    public enum ReportCriteria
    {
        /// <summary>
        /// Mean.
        /// </summary>
        Mean,

        /// <summary>
        /// Maximum.
        /// </summary>
        Maximum,

        /// <summary>
        /// Time in Alert.
        /// </summary>
        AlertTime,
    }

    /// <summary>
    /// Represents a client instance of a SignalR Hub for report data operations.
    /// </summary>
    public class ReportOperationsHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private readonly ReportHistorianOperations m_historianOperations;
        private string m_databaseFile;
        private AdoDataConnection m_connection;
        private bool m_disposed;
        private bool m_writting;
        private readonly CancellationTokenSource m_cancellation;
        
        // These are for the Progressbar
        private DateTime m_endTime;
        private double m_totalTime;
        private double m_percentComplete;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ReportOperationsHubClient"/>.
        /// </summary>
        public ReportOperationsHubClient()
        {
            m_historianOperations = new ReportHistorianOperations();

            m_writting = false;
            // Override the Historian Instance with the value from the config File
            // Any changes after this will take effect

            // Access needed settings from specified categories in configuration file
            CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["reportSettings"];
            reportSettings.Add("historianInstance", "PPA" , "Default historian instance used for reporting");
            string historian = reportSettings["historianInstance"].ValueAs("PPA");
            SetSelectedInstanceName(historian);

            m_cancellation = new CancellationTokenSource();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ReportOperationsHubClient"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                // Dispose of Historian operations and connection
                // This is called on endSession and should be triggered in all cases
                m_historianOperations.Dispose();
                m_connection.Dispose();
                m_cancellation.Dispose();

                //also remove Database File to avoid filling up cache
                try
                {
                    if (disposing)
                    {
                        try
                        {
                            File.Delete(m_databaseFile);
                        }
                        catch
                        {
                            throw new Exception("Unable to delete temporary SQL Lite DB for Reports");
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
        /// Set selected Historian instance name.
        /// </summary>
        /// <param name="instanceName">Historian instance name that is selected by user.</param>
        public void SetSelectedInstanceName(string instanceName)
        {
            m_historianOperations.SetSelectedInstanceName(instanceName);
        }

        /// <summary>
        /// Gets selected historian instance name.
        /// </summary>
        /// <returns>Selected Historian instance name.</returns>
        public string GetSelectedInstanceName()
        {
            return m_historianOperations.GetSelectedInstanceName();
        }

        /// <summary>
        /// Gets Progress of current Report.
        /// </summary>
        /// <returns>Selected Historian instance name.</returns>
        public double GetReportProgress()
        {
            return m_percentComplete;
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public IEnumerable<string> GetInstanceNames()
        {
            return m_historianOperations.GetInstanceNames();
        }

        private void UpdatePercentage(ulong current)
        {
            m_percentComplete = (1.0D - new Ticks(m_endTime.Ticks - (long)current).ToSeconds() / m_totalTime) * 100.0D;
        }

        /// <summary>
        /// Updates the Report Data Source.
        /// </summary>
        /// <param name="startDate">Start date of the Report.</param>
        /// <param name="endDate">End date of the Report.</param>
        /// <param name="reportType"> Type of Report <see cref="ReportType"/>.</param>
        /// /// <param name="reportCriteria"> Criteria to create Report <see cref="ReportCriteria"/>.</param>
        /// <param name="numberOfRecords">Number of records included 0 for all records.</param>
        /// <param name="dataContext">DataContext from which the available reportingParameters are pulled <see cref="DataContext"/>.</param>
        public void UpdateReportSource(DateTime startDate, DateTime endDate, ReportCriteria reportCriteria, ReportType reportType, int numberOfRecords, DataContext dataContext)
        {
            m_endTime = endDate;
            m_percentComplete = 0;
            m_totalTime = (endDate - startDate).TotalSeconds;


            if (m_writting)
                m_cancellation.Cancel();

            m_writting = true;
            CancellationToken token = m_cancellation.Token;
            
            new Thread(() =>
            {
				try
				{
					string sourceFile = $"{FilePath.GetAbsolutePath("")}{Path.DirectorySeparatorChar}ReportTemplate.db";
					m_databaseFile = $"{ConfigurationCachePath}{Path.DirectorySeparatorChar}{ConnectionID}.db";
					File.Copy(sourceFile, m_databaseFile, true);

					string filterString = "";

					if (reportType == ReportType.SNR)
						filterString = "%-SNR";
					else if (reportType == ReportType.Unbalance_I)
						filterString = "%I-UBAL";
					else if (reportType == ReportType.Unbalance_V)
						filterString = "%V-UBAL";

					// Get AlertThreshold
					CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["reportSettings"];
					double threshold = 0;

					if (reportType == ReportType.SNR)
					{
						reportSettings.Add("DefaultSNRThreshold", "4.0", "Default SNR Alert threshold.");
						threshold = reportSettings["DefaultSNRThreshold"].ValueAs(threshold);
					}
					else if (reportType == ReportType.Unbalance_V)
					{
						reportSettings.Add("VUnbalanceThreshold", "4.0", "Voltage Unbalance Alert threshold.");
						threshold = reportSettings["VUnbalanceThreshold"].ValueAs(threshold);
					}
					else if (reportType == ReportType.Unbalance_I)
					{
						reportSettings.Add("IUnbalanceThreshold", "4.0", "Current Unbalance Alert threshold.");
						threshold = reportSettings["IUnbalanceThreshold"].ValueAs(threshold);
					}

					if (token.IsCancellationRequested)
					{
						m_writting = false;
						return;
					}

					List<ReportMeasurements> reportingMeasurements = GetFromStats(dataContext, startDate, endDate, reportType, token);

					if (token.IsCancellationRequested)
					{
						m_writting = false;
						return;
					}

					if (!reportingMeasurements.Any())
					{
                        TableOperations<ActiveMeasurement> tableOperations = new TableOperations<ActiveMeasurement>(dataContext.Connection);
						tableOperations.RootQueryRestriction[0] = $"{GetSelectedInstanceName()}:%";

						List<ActiveMeasurement> activeMeasuremnts = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterString).ToList();
						reportingMeasurements = activeMeasuremnts.Select(point => new ReportMeasurements(point)).ToList();

						// Pull Data From the Open Historian
                        if (token.IsCancellationRequested)
						{
							m_writting = false;
							return;
						}

						Progress<ulong> progress = new Progress<ulong>(UpdatePercentage);
						List<CondensedDataPoint> historiandata = m_historianOperations.ReadCondensed(startDate, endDate, activeMeasuremnts, threshold, token, progress).ToList();

						if (token.IsCancellationRequested)
						{
							m_writting = false;
							return;
						}

						//remove any that don't have data
						reportingMeasurements = reportingMeasurements.Where(item => historiandata.Select(point => point.PointID).Contains(item.PointID)).ToList();

						// Deal with the not-aggregated signals
						reportingMeasurements = reportingMeasurements.Select(item =>
						{
							CondensedDataPoint data = historiandata.Find(point => point.PointID == item.PointID);
							item.Max = data.max;
							item.Min = data.min;
							item.Mean = data.sum / data.totalPoints;
							item.NumberOfAlarms = data.alert;
							item.PercentAlarms = data.alert * 100.0D / data.totalPoints;
							item.StandardDeviation = Math.Sqrt((data.sqrsum - 2 * data.sum * item.Mean + data.totalPoints * item.Mean * item.Mean) / data.totalPoints);
							item.TimeInAlarm = item.NumberOfAlarms / item.FramesPerSecond.GetValueOrDefault().NotZero();

							return item;
						}
						).ToList();
                    }

                    if (reportCriteria == ReportCriteria.Mean)
						reportingMeasurements = reportingMeasurements.OrderByDescending(item => item.Mean).ToList();

					if (reportCriteria == ReportCriteria.AlertTime)
						reportingMeasurements = reportingMeasurements.OrderByDescending(item => item.TimeInAlarm).ToList();

					if (reportCriteria == ReportCriteria.Maximum)
						reportingMeasurements = reportingMeasurements.OrderByDescending(item => item.Max).ToList();

					string connectionString = $"Data Source={m_databaseFile}; Version=3; Foreign Keys=True; FailIfMissing=True";
					string dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";
					
                    m_connection = new AdoDataConnection(connectionString, dataProviderString);

					if (numberOfRecords == 0)
						numberOfRecords = reportingMeasurements.Count;

                    // Create Original Point Tag
                    reportingMeasurements = reportingMeasurements.Select(item =>
					{
						if (reportType == ReportType.SNR)
							item.PointTag = item.PointTag.Remove(item.PointTag.Length - 4);
						else
							item.PointTag = item.PointTag.Remove(item.PointTag.Length - 5);
						return item;
					}).ToList();

					if (token.IsCancellationRequested)
					{
						m_writting = false;
						return;
					}

					TableOperations<ReportMeasurements> tbl = new TableOperations<ReportMeasurements>(m_connection);
					
                    for (int i = 0; i < Math.Min(numberOfRecords, reportingMeasurements.Count); i++)
					{
						tbl.AddNewRecord(reportingMeasurements[i]);
					}

					m_writting = false;
					m_percentComplete = 100.0;
				}
				catch (Exception e)
				{
					LogException(e);
				}

				m_writting = false;
				m_percentComplete = 100.0;
			})
            {
                IsBackground = true,
                
            }.Start();
        }
        
        /// <summary>
        /// Returns the Table Operation Object that Queries are build against.
        /// </summary>
        /// <returns> Table Operations Object that is used to query report data.</returns>
        public TableOperations<ReportMeasurements> Table()
        {
            if (m_connection == null)
                return null;

            return new TableOperations<ReportMeasurements>(m_connection);
        }


        /// <summary>
        /// Gets the path for storing Report Database configurations.
        /// </summary>
        private static string ConfigurationCachePath
        {
            get
            {
                // Define default configuration cache directory relative to path of host application
                string configurationCachePath = string.Format("{0}{1}ConfigurationCache{1}", FilePath.GetAbsolutePath(""), Path.DirectorySeparatorChar);

                // Make sure configuration cache path setting exists within system settings section of config file
                ConfigurationFile configFile = ConfigurationFile.Current;
                CategorizedSettingsElementCollection systemSettings = configFile.Settings["systemSettings"];
                systemSettings.Add("ConfigurationCachePath", configurationCachePath, "Defines the path used to cache serialized phasor protocol configurations");

                // Retrieve configuration cache directory as defined in the config file
                configurationCachePath = FilePath.AddPathSuffix(systemSettings["ConfigurationCachePath"].Value);

                // Make sure configuration cache directory exists
                if (!Directory.Exists(configurationCachePath))
                    Directory.CreateDirectory(configurationCachePath);

                return configurationCachePath;
            }
        }

        /// <summary>
        /// Gets the reporting Measurment List from Aggregate Channels if available.
        /// </summary>
        /// <returns> List of ReportMeasurements to be added to Report.</returns>
        /// <param name="start">Start date of the Report.</param>
        /// <param name="end">End date of the Report.</param>
        /// <param name="type"> Type of Report <see cref="ReportType"/>.</param>
        /// <param name="cancelationToken"> Cancleation Token for the historian read operation.</param>
        /// <param name="dataContext">DataContext from which the available reportingParameters are pulled <see cref="DataContext"/>.</param>
        private List<ReportMeasurements> GetFromStats(DataContext dataContext, DateTime start, DateTime end, ReportType type, CancellationToken cancelationToken)
        {
            List<ReportMeasurements> result = new List<ReportMeasurements>();

            string filterstring = "";

            if (type == ReportType.SNR)
                filterstring = "%-SNR";
            else if (type == ReportType.Unbalance_I)
                filterstring = "%I-UBAL";
            else if (type == ReportType.Unbalance_V)
                filterstring = "%V-UBAL";

            TableOperations<ActiveMeasurement> tableOperations = new TableOperations<ActiveMeasurement>(dataContext.Connection);
            tableOperations.RootQueryRestriction[0] = $"{GetSelectedInstanceName()}:%";

            if (tableOperations.QueryRecordCountWhere("PointTag LIKE {0}", filterstring + ":SUM") > 0)
            {
                List<ActiveMeasurement> sumMeasurements = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterstring + ":SUM").ToList();
                List<ActiveMeasurement> squaredSumMeasurments = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterstring + ":SQR").ToList();
                List<ActiveMeasurement> minimumMeasurements = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterstring + ":MIN").ToList();
                List<ActiveMeasurement> maximumMeasurements = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterstring + ":MAX").ToList();
                List<ActiveMeasurement> countMeasurements = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterstring + ":NUM").ToList();
                List<ActiveMeasurement> alertMeasurements = tableOperations.QueryRecordsWhere("PointTag LIKE {0}", filterstring + ":ALT").ToList();

                result = sumMeasurements.Select(point => new ReportMeasurements(point)).ToList();

                // Pull Data From the Open Historian
                List<ActiveMeasurement> all = sumMeasurements.Concat(squaredSumMeasurments).Concat(minimumMeasurements).Concat(maximumMeasurements).Concat(countMeasurements).Concat(alertMeasurements).ToList();
                List<CondensedDataPoint> historiandata;

                if (cancelationToken.IsCancellationRequested)
                    return new List<ReportMeasurements>();

                try
                {
                    Progress<ulong> progress = new Progress<ulong>(UpdatePercentage);
                    historiandata = m_historianOperations.ReadCondensed(start, end, all, double.MaxValue, cancelationToken, progress).ToList();
                }
                catch
                {
                    return new List<ReportMeasurements>();
                }

                if (cancelationToken.IsCancellationRequested)
                    return new List<ReportMeasurements>();

                result = result.Where((item, index) =>
                {
                    ReportMeasurements sum = item;
                    string tag = item.PointTag.Remove(item.PointTag.Length - 4);

                    ActiveMeasurement squaredSumChannel = squaredSumMeasurments.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement minimumChannel = minimumMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement maximumChannel = maximumMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement countChannel = countMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement countAlertChannel = alertMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);

                    if (squaredSumChannel is null || minimumChannel is null || maximumChannel is null || countChannel is null || countAlertChannel is null)
                        return false;

                    return historiandata.Select(point => point.PointID).Contains(sum.PointID) &&
                           historiandata.Select(point => point.PointID).Contains(squaredSumChannel.PointID) &&
                           historiandata.Select(point => point.PointID).Contains(minimumChannel.PointID) &&
                           historiandata.Select(point => point.PointID).Contains(maximumChannel.PointID) &&
                           historiandata.Select(point => point.PointID).Contains(countAlertChannel.PointID) &&
                           historiandata.Select(point => point.PointID).Contains(countChannel.PointID);
                }).ToList();

                result = result.Select(item =>
                {
                    string tag = item.PointTag.Remove(item.PointTag.Length - 4);
                    
                    ActiveMeasurement squaredSumChannel = squaredSumMeasurments.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement minimumChannel = minimumMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement maximumChannel = maximumMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement countChannel = countMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement countAlertChannel = alertMeasurements.Find(channel => channel.PointTag.Remove(channel.PointTag.Length - 4) == tag);
                    ActiveMeasurement sumChannel = sumMeasurements.Find(meas => meas.PointTag.Remove(meas.PointTag.Length - 4) == tag);

                    double minimum = historiandata.Find(point => point.PointID == minimumChannel.PointID).min;
                    double maximum = historiandata.Find(point => point.PointID == maximumChannel.PointID).max;
                    double count = historiandata.Find(point => point.PointID == countChannel.PointID).sum;
                    double alarmCount = historiandata.Find(point => point.PointID == countAlertChannel.PointID).sum;
                    double summation = historiandata.Find(point => point.PointID == sumChannel.PointID).sum;
                    double squaredsum = historiandata.Find(point => point.PointID == squaredSumChannel.PointID).sum;

                    item.Max = maximum;
                    item.Min = minimum;
                    item.Mean = summation / count;
                    item.NumberOfAlarms = alarmCount;
                    item.PercentAlarms = alarmCount * 100.0D / count;
                    item.StandardDeviation = Math.Sqrt((squaredsum - 2 * summation * item.Mean + count * item.Mean * item.Mean) / count);
                    item.TimeInAlarm = item.NumberOfAlarms / item.FramesPerSecond.GetValueOrDefault().NotZero();

                    item.PointTag = item.PointTag.Remove(item.PointTag.Length - 4);
                    return item;
                }).ToList();
            }

            return result;
        }
        
        #endregion
    }
}