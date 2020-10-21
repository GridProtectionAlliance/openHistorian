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
//  10/10/2019 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;
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

        // Constants      

        private const string ReportSettingsCategory = "snrSQLReportingDB";

        // Fields
        private string m_databaseFile;
        private AdoDataConnection m_connection;
        private bool m_disposed;
        private bool m_writing;
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
            m_writing = false;
            
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
            if (m_disposed)
                return;

            // Dispose of Historian operations and connection
            // This is called on endSession and should be triggered in all cases
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

        /// <summary>
        /// Gets Progress of current Report.
        /// </summary>
        /// <returns>Selected Historian instance name.</returns>
        public double GetReportProgress()
        {
            return m_percentComplete;
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

            if (m_writing)
                m_cancellation.Cancel();

            m_writing = true;
            CancellationToken token = m_cancellation.Token;
            
            new Thread(() =>
            {
				try
				{
					string sourceFile = $"{FilePath.GetAbsolutePath("")}{Path.DirectorySeparatorChar}ReportTemplate.db";
					m_databaseFile = $"{ConfigurationCachePath}{Path.DirectorySeparatorChar}{ConnectionID}.db";
					File.Copy(sourceFile, m_databaseFile, true);

					if (token.IsCancellationRequested)
					{
						m_writing = false;
						return;
					}

					List<ReportMeasurements> reportingMeasurements = GetFromStats(dataContext, startDate, endDate, reportType, reportCriteria, numberOfRecords, token);

					if (token.IsCancellationRequested)
					{
						m_writing = false;
						return;
					}

					string connectionString = $"Data Source={m_databaseFile}; Version=3; Foreign Keys=True; FailIfMissing=True";
					string dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";

                    // Make this a single Transaction for speed purpose
                    using (TransactionScope scope = new TransactionScope())
                    {

                        m_connection = new AdoDataConnection(connectionString, dataProviderString);
                        

                        // Remove Points that are all NaN
                        reportingMeasurements = reportingMeasurements.Where(item =>
                        {
                            return (!(double.IsNaN(item.Mean)));
                        }).ToList();



                        if (numberOfRecords == 0)
                            numberOfRecords = reportingMeasurements.Count;


                        if (token.IsCancellationRequested)
                        {
                            m_writing = false;
                            return;
                        }



                        TableOperations<ReportMeasurements> reportMeasurements = new TableOperations<ReportMeasurements>(m_connection);

                        for (int i = 0; i < Math.Min(numberOfRecords, reportingMeasurements.Count); i++)
                            reportMeasurements.AddNewRecord(reportingMeasurements[i]);
                        

                        scope.Complete();
                    }
					m_writing = false;
					m_percentComplete = 100.0;
				}
				catch (Exception e)
				{
					LogException(e);
				}

				m_writing = false;
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
            return m_connection == null ? null : new TableOperations<ReportMeasurements>(m_connection);
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
        /// <param name="criteria"> Criteria for sorting this Report <see cref="ReportCriteria"/></param>
        /// <param name="numberRecords"> Number of Records that are being obtained. </param>
        /// <param name="cancelationToken"> Cancleation Token for the historian read operation.</param>
        /// <param name="dataContext">DataContext from which the available reportingParameters are pulled <see cref="DataContext"/>.</param>
        private List<ReportMeasurements> GetFromStats(DataContext dataContext, DateTime start, DateTime end, ReportType type, ReportCriteria criteria, int numberRecords, CancellationToken cancelationToken)
        {
            List<ReportMeasurements> result = new List<ReportMeasurements>();

            // For now lump I and V together since we have to change all of this logic anyway to get away from SignalID back to Point Tags eventually
            // and get away from SQLLite files - waiting for ritchies comments before implementing that
            // Also using SignalID for everything this will be adjusted once I figured out how to combine the 2 SQL DB
            string idCollumn = (type == ReportType.SNR? "SignalID" : "PositivePhaseSignalID");
            string sortCollumn = "Max";
            
            // Alert Time is currently not available in SQL Reporting  - needs to be removed from UI
            switch (criteria)
            {
                case (ReportCriteria.AlertTime):
                    sortCollumn = "Min";
                    break;
                case (ReportCriteria.Maximum):
                    sortCollumn = "Max";
                    break;
                case (ReportCriteria.Mean):
                    sortCollumn = "Mean";
                    break;
            }

            string sqlQuery = $@"SELECT TOP {numberRecords}
                                    {idCollumn} AS ID,
                                    {idCollumn} AS SignalID,
                                    {idCollumn} AS PointTag,
                                    {idCollumn} AS SignalReference,
                                    (SUM(SUM)/SUM(Count)) AS Mean,
                                    MAX(Maximum) AS Max,
                                    Min(Minimum) AS Min,
                                    SQRT((1/SUM(COUNT))*(SUM(SquaredSum)-2*SUM(Sum)*Sum(Sum)/SUM(Count)+SUM(Sum)*Sum(Sum)/SUM(Count))) AS StandardDeviation,
                                    MAX(Maximum) AS NumberOfAlarms, 
                                    MAX(Maximum) as PercentAlarms,
                                    MAX(Maximum) as TimeInAlarm
                                FROM {(type == ReportType.SNR? "SNRSummary" : "UnbalanceSummary")}
                                WHERE Date >= '{start.ToString("yyyy-MM-dd")}' AND Date <= '{end.ToString("yyyy-MM-dd")}' 
                                GROUP BY {idCollumn}
                                ORDER BY {sortCollumn}";

            DataTable reportTbl;

            using (AdoDataConnection connection = new AdoDataConnection(ReportSettingsCategory))
                reportTbl = connection.RetrieveData(sqlQuery);

            foreach (DataRow row in reportTbl.Rows)
            {
                result.Add(new ReportMeasurements() {
                    ID = row.Field<Guid>("ID").ToString(),
                    SignalID = row.Field<Guid>("SignalID"),
                    PointTag = row.Field<Guid>("PointTag").ToString(),
                    SignalReference = row.Field<Guid>("SignalReference").ToString(),
                    Mean = row.Field<double>("Mean"),
                    Max = row.Field<double>("Max"),
                    Min = row.Field<double>("Min"),
                    StandardDeviation = row.Field<double>("StandardDeviation"),
                    NumberOfAlarms = row.Field<double>("NumberOfAlarms"),
                    PercentAlarms = row.Field<double>("PercentAlarms"),
                    TimeInAlarm = row.Field<double>("TimeInAlarm"),
                    FramesPerSecond = 30
                });
            }

            return result;
        }
        
        #endregion
    }
}