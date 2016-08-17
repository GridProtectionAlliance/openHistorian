//******************************************************************************************************
//  DataSubscriptionOperations.cs - Gbtc
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

namespace openHistorian
{
    /// <summary>
    /// Defines an interface for using <see cref="DataSubscriptionOperations"/> within a SignalR hub.
    /// </summary>
    /// <remarks>
    /// This interface makes sure all hub methods needed by GraphMeasurements.cshtml get properly defined.
    /// </remarks>
    public interface IDataSubscriptionOperations
    {
        IEnumerable<MeasurementValue> GetMeasurements();

        IEnumerable<MeasurementValue> GetStats();

        IEnumerable<StatusLight> GetLights();

        IEnumerable<DeviceDetail> GetDeviceDetails();

        IEnumerable<MeasurementDetail> GetMeasurementDetails();

        IEnumerable<PhasorDetail> GetPhasorDetails();

        IEnumerable<SchemaVersion> GetSchemaVersion();

        void InitializeSubscriptions();

        void TerminateSubscriptions();

        void UpdateFilters(string filterExpression);

        void StatSubscribe(string filterExpression);
    }

    /// <summary>
    /// Represents hub operations for using <see cref="DataSubscriptionHubClient"/> instances.
    /// </summary>
    /// <remarks>
    /// This hub client operations class makes sure a data subscription is created per SignalR session and only created when needed.
    /// </remarks>
    public class DataSubscriptionOperations : HubClientOperationsBase<DataSubscriptionHubClient>, IDataSubscriptionOperations
    {
        /// <summary>
        /// Creates a new <see cref="DataSubscriptionOperations"/> instance.
        /// </summary>
        /// <param name="hub">Parent hub.</param>
        /// <param name="logStatusMessageFunction">Delegate to use to log status messages, if any.</param>
        /// <param name="logExceptionFunction">Delegate to use to log exceptions, if any.</param>
        public DataSubscriptionOperations(IHub hub, Action<string, UpdateType> logStatusMessageFunction = null, Action<Exception> logExceptionFunction = null) : base(hub, logStatusMessageFunction, logExceptionFunction)
        {
        }

        public IEnumerable<MeasurementValue> GetMeasurements()
        {
            return HubClient.GetMeasurements();
        }

        public IEnumerable<MeasurementValue> GetStats()
        {
            return HubClient.GetStatistics();
        }

        public IEnumerable<StatusLight> GetLights()
        {
            return HubClient.GetStatusLights();
        }

        public IEnumerable<DeviceDetail> GetDeviceDetails()
        {
            return HubClient.GetDeviceDetails();
        }

        public IEnumerable<MeasurementDetail> GetMeasurementDetails()
        {
            return HubClient.GetMeasurementDetails();
        }

        public IEnumerable<PhasorDetail> GetPhasorDetails()
        {
            return HubClient.GetPhasorDetails();
        }

        public IEnumerable<SchemaVersion> GetSchemaVersion()
        {
            return HubClient.GetSchemaVersion();
        }

        public void InitializeSubscriptions()
        {
            HubClient.InitializeSubscriptions();
        }

        public void TerminateSubscriptions()
        {
            HubClient.TerminateSubscriptions();
        }

        public void UpdateFilters(string filterExpression)
        {
            HubClient.UpdatePrimaryDataSubscription(filterExpression);
        }

        public void StatSubscribe(string filterExpression)
        {
            HubClient.UpdateStatisticsDataSubscription(filterExpression);
        }
    }
}
