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
using System.Linq;
using System.Threading;
using GSF.Data;
using GSF.Data.Model;
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
    /// Defines enumeration of supported Report Criteria.
    /// </summary>
    public enum ReportCriteria
    {
        /// <summary>
        /// Mean.
        /// </summary>
        Mean=0,

        /// <summary>
        /// Maximum.
        /// </summary>
        Maximum=1,

        /// <summary>
        /// Time in Alarm.
        /// </summary>
        TimeInAlarm=2,

        /// <summary>
        /// Standard Deviation.
        /// </summary>
        StandardDev=3,

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
        private List<ReportMeasurements> m_listing;
        private string m_currentSort;
        private bool m_currentAscending;
        private bool m_disposed;
        private bool m_writing;
        private readonly CancellationTokenSource m_cancellation;
        
        // These are for the Progressbar
        private double m_percentComplete;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ReportOperationsHubClient"/>.
        /// </summary>
        public ReportOperationsHubClient()
        {
            m_writing = false;
            m_listing = null;
            m_currentSort = "";
            m_currentAscending = false;
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
            m_cancellation.Dispose();


           
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
            
        }

        /// <summary>
        /// Gets Progress of current Report.
        /// </summary>
        /// <returns>Selected Historian instance name.</returns>
        public double GetReportProgress()
        {
            return m_percentComplete;
        }

        private void UpdatePercentage(int current, int total)
        {
            m_percentComplete = (1.0D -current / (double)total) * 100.0D;
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
            m_percentComplete = 0;

            if (m_writing)
                m_cancellation.Cancel();

            m_writing = true;
            CancellationToken token = m_cancellation.Token;
            
            new Thread(() =>
            {
                try
                {
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

                    m_listing = reportingMeasurements;
                    
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
            string idCollumn = type == ReportType.SNR? "SignalID" : "PositivePhaseSignalID";
            string sortCollumn = "Max";
            string typerestriction = $" AND SignalType = '{(type == ReportType.Unbalance_I? "I" : "V")}'";


            switch (criteria)
            {
                case ReportCriteria.Maximum:
                    sortCollumn = "Max";
                    break;
                case ReportCriteria.Mean:
                    sortCollumn = "Mean";
                    break;
                case ReportCriteria.StandardDev:
                    sortCollumn = "StandardDeviation";
                    break;
                case ReportCriteria.TimeInAlarm:
                    sortCollumn = "PercentAlarms";
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
                                    SQRT((1/CAST(SUM(COUNT) AS Float))*(SUM(SquaredSum)-2*SUM(Sum)*Sum(Sum)/CAST(SUM(Count) AS Float)+SUM(Sum)*Sum(Sum)/SUM(Count))) AS StandardDeviation,
                                    SUM(AlarmCount) AS NumberOfAlarms, 
                                    (CAST(SUM(AlarmActiveCount)AS float)/CAST(SUM(Count) AS Float)) as PercentAlarms
                                FROM {(type == ReportType.SNR? "SNRSummary" : "UnbalanceSummary")}
                                WHERE Date >= '{start.ToString("yyyy-MM-dd")}' AND Date <= '{end.ToString("yyyy-MM-dd")}'{(type == ReportType.SNR? "" : typerestriction)} 
                                GROUP BY {idCollumn}
                                ORDER BY {sortCollumn}";

            DataTable reportTbl;

            using (AdoDataConnection connection = new AdoDataConnection(ReportSettingsCategory))
                reportTbl = connection.RetrieveData(sqlQuery);

            using (AdoDataConnection connection = new AdoDataConnection("Systemsettings"))
            {
                TableOperations<ActiveMeasurement> activeMeasurementTbl = new TableOperations<ActiveMeasurement>(connection);
                TableOperations<Phasor> phasorTbl = new TableOperations<Phasor>(connection);
                int i = 0;

                foreach (DataRow row in reportTbl.Rows)
                {
                    UpdatePercentage(i, numberRecords);
                    i++;

                    Guid signalID = row.Field<Guid>("SignalID");
                    ActiveMeasurement sourceMeasurement = activeMeasurementTbl.QueryRecordWhere("SignalID = {0}", signalID);
                    
                    if (sourceMeasurement is null)
                        continue;

                    Phasor phasor = phasorTbl.QueryRecordWhere("ID = {0}", sourceMeasurement.PhasorID);

                    result.Add(new ReportMeasurements()
                    {
                        SignalID = signalID,
                        PointTag = phasor != null && type != ReportType.SNR? phasor.Label : sourceMeasurement.PointTag,
                        SignalReference = sourceMeasurement.SignalReference,
                        SignalType = sourceMeasurement.SignalType,
                        DeviceName = sourceMeasurement.Device,
                        Mean = row.Field<double>("Mean"),
                        Max = row.Field<double>("Max"),
                        Min = row.Field<double>("Min"),
                        StandardDeviation = row.Field<double>("StandardDeviation"),
                        AlarmCount = row.Field<int>("NumberOfAlarms"),
                        PercentInAlarm = row.Field<double>("PercentAlarms")*100.0D,
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the number of <see cref="ReportMeasurements"/> in the report.
        /// </summary>
        public int GetCount() => m_listing is null ? 0 : m_listing.Count();

        /// <summary>
        /// Gets the <see cref="ReportMeasurements"/> for a given page (and sorted by a given Field).
        /// </summary>
        /// <param name="ascending"> If the data should be sorted Ascending or Descending</param>
        /// <param name="page"> the (1 based) pageIndex</param>
        /// <param name="pageSize"> The number of records in a single page. </param>
        /// <param name="sortField">The Field by which the data is sorted. </param>
        public IEnumerable<ReportMeasurements> GetData(string sortField, bool ascending, int page, int pageSize)
        {
            if (m_listing is null)
                return new List<ReportMeasurements>();

            if (string.Compare(m_currentSort,sortField,true) != 0)
            {
                switch(sortField)
                {
                    case "PointTag":
                        Sort(ascending, item => item.PointTag);
                        break;
                    case "DeviceName":
                        Sort(ascending, item => item.DeviceName);
                        break;
                    case "Mean":
                        Sort(ascending, item => item.Mean);
                        break;
                    case "Max":
                        Sort(ascending, item => item.Max);
                        break;
                    case "Min":
                        Sort(ascending, item => item.Min);
                        break;
                }
                m_currentAscending = ascending;
                m_currentSort = sortField;

            }
            if (m_currentAscending != ascending)
            {
                m_listing.Reverse();
                m_currentAscending = ascending;
            }

            int start = (page-1)*pageSize;
            return m_listing.Skip(start).Take(pageSize);
        }

        private void Sort<T>(bool ascending, Func<ReportMeasurements,T> keySelector)
        {
            if (ascending)
                m_listing = m_listing.OrderBy(keySelector).ToList();
            else
                m_listing = m_listing.OrderByDescending(keySelector).ToList();
        }
        #endregion
    }
}