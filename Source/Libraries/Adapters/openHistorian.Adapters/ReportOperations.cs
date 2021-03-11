//******************************************************************************************************
//  ReportOperations.cs - Gbtc
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
using GSF;
using GSF.Web.Hubs;
using GSF.Web.Model;
using Microsoft.AspNet.SignalR.Hubs;
using openHistorian.Model;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines an interface for using <see cref="ReportOperations"/> within a SignalR hub.
    /// </summary>
    /// <remarks>
    /// This interface makes sure all hub methods needed by pages using report operations get properly defined.
    /// </remarks>
    public interface IReportOperations
    {
       

        /// <summary>
        /// Updates the Report Data Source.
        /// </summary>
        /// <param name="startDate">Start date of the Report.</param>
        /// <param name="endDate">End date of the Report.</param>
        /// <param name="reportType"> Type of Report <see cref="ReportType"/>.</param>
        /// <param name="number">Number of records included 0 for all records.</param>
        /// <param name="reportCriteria"> Crtieria to use for Report <see cref="ReportCriteria"/>.</param>
        /// <param name="dataContext">DataContext from which the available reportingParameters are pulled <see cref="DataContext"/>.</param>
        void UpdateReportSource(DateTime startDate, DateTime endDate, ReportCriteria reportCriteria, ReportType reportType, int number, DataContext dataContext);

        /// <summary>
        /// Returns the number of rows for the report.
        /// </summary>
        /// <returns>Table Operations Object that is used to query report data.</returns>
        int GetCount();

        /// <summary>
        /// Gets Progress of current Report generation.
        /// </summary>
        double GetReportProgress();
    }

    /// <summary>
    /// Represents hub operations for using <see cref="ReportOperationsHubClient"/> instances.
    /// </summary>
    /// <remarks>
    /// This hub client operations class makes sure a report connection is created per SignalR session and only created when needed.
    /// </remarks>
    public class ReportOperations : HubClientOperationsBase<ReportOperationsHubClient>, IReportOperations
    {
        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="HistorianOperations"/> instance.
        /// </summary>
        /// <param name="hub">Parent hub.</param>
        /// <param name="logStatusMessageFunction">Delegate to use to log status messages, if any.</param>
        /// <param name="logExceptionFunction">Delegate to use to log exceptions, if any.</param>
        public ReportOperations(IHub hub, Action<string, UpdateType> logStatusMessageFunction = null, Action<Exception> logExceptionFunction = null) : base(hub, logStatusMessageFunction, logExceptionFunction)
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates the Report Data Source.
        /// </summary>
        /// <param name="startDate">Start date of the Report.</param>
        /// <param name="endDate">End date of the Report.</param>
        /// <param name="reportType"> Type of Report <see cref="ReportType"/>.</param>
        /// <param name="reportCriteria"> Crtieria to use for Report <see cref="ReportCriteria"/>.</param>
        /// <param name="number">Number of records included 0 for all records.</param>
        /// <param name="dataContext">DataContext from which the available reportingParameters are pulled <see cref="DataContext"/>.</param>
        public void UpdateReportSource(DateTime startDate, DateTime endDate, ReportCriteria reportCriteria, ReportType reportType, int number, DataContext dataContext)
        {
            HubClient.UpdateReportSource(startDate, endDate, reportCriteria, reportType, number, dataContext);
        }

        /// <summary>
        /// Gets the number of <see cref="ReportMeasurements"/> in the report.
        /// </summary>
        public int GetCount() => HubClient.GetCount();

        /// <summary>
        /// Gets Progress of current Report.
        /// </summary>
        public double GetReportProgress() => HubClient.GetReportProgress();

        /// <summary>
        /// Gets the <see cref="ReportMeasurements"/> for a given page (and sorted by a given Field).
        /// </summary>
        /// <param name="ascending"> If the data should be sorted Ascending or Descending</param>
        /// <param name="page"> the (1 based) pageIndex</param>
        /// <param name="pageSize"> The number of records in a single page. </param>
        /// <param name="sortField">The Field by which the data is sorted. </param>
        public IEnumerable<ReportMeasurements> GetData(string sortField,bool ascending,int page,int pageSize) => HubClient.GetData(sortField,ascending,page,pageSize);

        #endregion
    }
}
