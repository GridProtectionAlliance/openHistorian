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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.IO;
using GSF.Snap.Services;
using GSF.Threading;
using GSF.Web.Hubs;
using GSF.Web.Model;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using CancellationToken = GSF.Threading.CancellationToken;
using Random = GSF.Security.Cryptography.Random;

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
    /// Represents a client instance of a SignalR Hub for report data operations.
    /// </summary>
    public class ReportOperationsHubClient : HubClientBase
    {
        #region [ Members ]

        // Fields
        private ReportType reportType;
        private int numberOfRecords;
        private HistorianOperationsHubClient historianOperations;
        private string databaseFile;
        private AdoDataConnection connection;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ReportOperationsHubClient"/>.
        /// </summary>
        public ReportOperationsHubClient()
        {
            this.historianOperations = new HistorianOperationsHubClient();
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
                this.historianOperations.Dispose();
                this.connection.Dispose();

                //also remove Database File to avoid filling up cache
                try
                {
                    if (disposing)
                    {
                        try
                        {
                            System.IO.File.Delete(this.databaseFile);
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
            this.historianOperations.SetSelectedInstanceName(instanceName);
        }

        /// <summary>
        /// Gets selected historian instance name.
        /// </summary>
        /// <returns>Selected Historian instance name.</returns>
        public string GetSelectedInstanceName()
        {
            return this.historianOperations.GetSelectedInstanceName();
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public IEnumerable<string> GetInstanceNames()
        {
            return this.historianOperations.GetInstanceNames();
        }

        /// <summary>
        /// Updates the Report Data Source.
        /// </summary>
        /// <param name="startDate">Start date of the Report.</param>
        /// <param name="endDate">End date of the Report.</param>
        /// <param name="reportType"> Type of Report <see cref="ReportType"/>.</param>
        /// <param name="number">Number of records included 0 for all records.</param>
        /// <param name="dataContext">DataContext from which the available reportingParameters are pulled <see cref="DataContext"/>.</param>
        public void UpdateReportSource(DateTime startDate, DateTime endDate, ReportType reportType, int number, DataContext dataContext)
        {
            string sourceFile = string.Format("{0}{1}ReportTemplate.db", FilePath.GetAbsolutePath(""), Path.DirectorySeparatorChar);
            this.databaseFile = string.Format("{0}{1}{2}.db",ConfigurationCachePath, Path.DirectorySeparatorChar,this.ConnectionID) ;
            System.IO.File.Copy(sourceFile, this.databaseFile, true);

            this.reportType = reportType;
            this.numberOfRecords = number;

            string filterstring = "";

            if (reportType == ReportType.SNR)
                filterstring = "%-SNR";
            else if (reportType == ReportType.Unbalance_I)
                filterstring = "%I-UBAL";
            else if (reportType == ReportType.Unbalance_V)
                filterstring = "%V-UBAL";

            List<ActiveMeasurement> activeMeasuremnts = new TableOperations<ActiveMeasurement>(dataContext.Connection).
                QueryRecordsWhere("PointTag LIKE {0}", filterstring).ToList();

            List<ReportMeasurements> reportingMeasurements = activeMeasuremnts.Select(item => (ReportMeasurements)item).ToList();

            // Pull Data From the Open Historian

            Parallel.ForEach(reportingMeasurements, item =>
            {

                List<TrendValue> points;

                try
                {
                    points = this.historianOperations.GetHistorianData(this.GetSelectedInstanceName(), startDate, endDate, new ulong[1] { item.PointID },
                        openHistorian.Adapters.Resolution.Full, 999, true).ToList();
                }
                catch
                {
                    points = new List<TrendValue>();
                }

                if (points.Count() > 1)
                {
                    item.Mean = points.Select(point => point.Value).Average();
                    item.Min = points.Select(point => point.Value).Min();
                    item.Max = points.Select(point => point.Value).Max();
                    item.NumberOfAlarms = points.Where(point => point.Value > 4).Count();
                    item.PercentAlarms = (item.NumberOfAlarms * 100.0D) / points.Count();
                }
                else
                {
                    item.Mean = 0.0D;
                    item.Min = 0.0D;
                    item.Max = 0.0D;
                    item.NumberOfAlarms = 0.0D;
                    item.PercentAlarms = 0.0D;
                }

                if (points.Count() < 2)
                    item.StandardDeviation = 0.0D;
                else
                {
                    double sum = points.Sum(point => Math.Pow(point.Value - item.Mean, 2));
                    item.StandardDeviation = Math.Sqrt(sum / (points.Count() - 1));
                }

                item.TimeInAlarm = 0;


            });

            reportingMeasurements = reportingMeasurements.OrderByDescending(item => item.Mean).ToList();

            string connectionstring = String.Format("Data Source={0}; Version=3; Foreign Keys=True; FailIfMissing=True", this.databaseFile);
            string dataProviderstring = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";
            this.connection = new AdoDataConnection(connectionstring, dataProviderstring);

            if (this.numberOfRecords == 0)
                this.numberOfRecords = reportingMeasurements.Count();

            TableOperations<ReportMeasurements> tbl = new TableOperations<ReportMeasurements>(connection);
            for (int i=0; i < Math.Min(this.numberOfRecords, reportingMeasurements.Count());i++)
            {
                tbl.AddNewRecord(reportingMeasurements[i]);
            }
          
            
            //Also make sure instance selection is taken care of in DataHub
            // And then switch over cshtml to original Creation of JS Script except we need to add setup Report...
            //based on the Data Context from datahub and OpenHistorian Data

        }


        /// <summary>
        /// Returns the Table Operation Object that Queries are build against.
        /// </summary>
        /// <returns> Table Operations Object that is used to query report data.</returns>
        public TableOperations<ReportMeasurements> Table()
        {
            return (new TableOperations<ReportMeasurements>(this.connection));
        }


        /// <summary>
        /// Gets the path for storing Report Database configurations.
        /// </summary>
        private static string ConfigurationCachePath
        {
            get
            {
                // Define default configuration cache directory relative to path of host application
                string s_configurationCachePath = string.Format("{0}{1}ConfigurationCache{1}", FilePath.GetAbsolutePath(""), Path.DirectorySeparatorChar);

                // Make sure configuration cache path setting exists within system settings section of config file
                ConfigurationFile configFile = ConfigurationFile.Current;
                CategorizedSettingsElementCollection systemSettings = configFile.Settings["systemSettings"];
                systemSettings.Add("ConfigurationCachePath", s_configurationCachePath, "Defines the path used to cache serialized phasor protocol configurations");

                // Retrieve configuration cache directory as defined in the config file
                s_configurationCachePath = FilePath.AddPathSuffix(systemSettings["ConfigurationCachePath"].Value);

                // Make sure configuration cache directory exists
                if (!Directory.Exists(s_configurationCachePath))
                    Directory.CreateDirectory(s_configurationCachePath);

                return s_configurationCachePath;
            }
        }

        #endregion
    }
}