//******************************************************************************************************
//  HistorianQueryController.cs - Gbtc
//
//  Copyright © 2018, Grid Protection Alliance.  All Rights Reserved.
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
//  06/13/2018 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using GSF.Snap.Services;
using GSF.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openHistorian.Model;
using openHistorian.Net;
using CancellationToken = System.Threading.CancellationToken;

#pragma warning disable 0649

namespace openHistorian.Adapters
{
    /// <summary>
    /// Web API controller for querying historian data.
    /// </summary>
    public class HistorianQueryController : ApiController
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class QueryParameters
        {
            public string instanceName;
            public DateTime startTime;
            public DateTime stopTime;
            public ulong[] measurementIDs;
            public Resolution resolution;
            public int seriesLimit;
            public bool forceLimit;
            public TimestampType timestampType = TimestampType.UnixMilliseconds;
        }

        private CancellationTokenSource m_linkedTokenSource;
        private bool m_disposed;

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        [HttpPost]
        public async Task<IEnumerable<TrendValue>> GetHistorianData(CancellationToken cancellationToken)
        {
            QueryParameters queryParameters;

            using (Stream contentStream = await Request.Content.ReadAsStreamAsync())
            using (StreamReader contentReader = new StreamReader(contentStream))
            using (JsonTextReader jsonReader = new JsonTextReader(contentReader))
            {
                JObject jsonObject = await JObject.LoadAsync(jsonReader);
                queryParameters = jsonObject.ToObject<QueryParameters>();
            }

            string instanceName = queryParameters.instanceName;
            DateTime startTime = queryParameters.startTime;
            DateTime stopTime = queryParameters.stopTime;
            ulong[] measurementIDs = queryParameters.measurementIDs;
            Resolution resolution = queryParameters.resolution;
            int seriesLimit = queryParameters.seriesLimit;
            bool forceLimit = queryParameters.forceLimit;

            // Try to ensure another linked cancellation token is not created after dispose,
            // because another call to Dispose() won't clean it up
            if (m_disposed)
                throw new ObjectDisposedException(nameof(HistorianQueryController));

            // Cancel any running operation
            CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            using (CancellationTokenSource oldTokenSource = Interlocked.Exchange(ref m_linkedTokenSource, linkedTokenSource))
            {
                oldTokenSource?.Cancel();
            }

            SnapServer server = GetServer(instanceName)?.Host;
            ICancellationToken compatibleToken = new CompatibleCancellationToken(linkedTokenSource);
            IEnumerable<TrendValue> values = TrendValueAPI.GetHistorianData(server, instanceName, startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit, compatibleToken);

            switch (queryParameters.timestampType)
            {
                case TimestampType.Ticks:
                    return values.Select(value =>
                    {
                        value.Timestamp = value.Timestamp * 10000.0D + 621355968000000000.0D;
                        return value;
                    });
                case TimestampType.UnixSeconds:
                    return values.Select(value =>
                    {
                        value.Timestamp = value.Timestamp / 1000.0D;
                        return value;
                    });
                default:
                    return values;
            }
        }

        private HistorianServer GetServer(string instanceName)
        {
            if (LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter historianAdapter))
                return historianAdapter?.Server;

            return null;
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally,
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (disposing)
                {
                    using (CancellationTokenSource linkedTokenSource = Interlocked.Exchange(ref m_linkedTokenSource, null))
                    {
                        linkedTokenSource?.Cancel();
                    }
                }
            }
            finally
            {
                m_disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
