//******************************************************************************************************
//  HistorianQueryOperations.cs - Gbtc
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
//  08/16/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF;
using GSF.Web.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using openHistorian.Model;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Defines an interface for using <see cref="HistorianQueryOperations"/> within a SignalR hub.
    /// </summary>
    /// <remarks>
    /// This interface makes sure all hub methods needed by TrendMeasurements.cshtml get properly defined.
    /// </remarks>
    public interface IHistorianQueryOperations
    {
        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        IEnumerable<TrendValue> GetHistorianData(DateTime startTime, DateTime stopTime, long[] measurementIDs, Resolution resolution, int seriesLimit);
    }

    /// <summary>
    /// Represents hub operations for using <see cref="HistorianQueryHubClient"/> instances.
    /// </summary>
    /// <remarks>
    /// This hub client operations class makes sure a historian connection is created per SignalR session and only created when needed.
    /// </remarks>
    public class HistorianQueryOperations : HubClientOperationsBase<HistorianQueryHubClient>, IHistorianQueryOperations
    {
        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="HistorianQueryOperations"/> instance.
        /// </summary>
        /// <param name="hub">Parent hub.</param>
        /// <param name="logStatusMessageFunction">Delegate to use to log status messages, if any.</param>
        /// <param name="logExceptionFunction">Delegate to use to log exceptions, if any.</param>
        public HistorianQueryOperations(IHub hub, Action<string, UpdateType> logStatusMessageFunction = null, Action<Exception> logExceptionFunction = null) : base(hub, logStatusMessageFunction, logExceptionFunction)
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(DateTime startTime, DateTime stopTime, long[] measurementIDs, Resolution resolution, int seriesLimit)
        {
            return HubClient.GetHistorianData(startTime, stopTime, measurementIDs, resolution, seriesLimit);
        }

        #endregion
    }
}
