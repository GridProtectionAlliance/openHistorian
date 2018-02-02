//******************************************************************************************************
//  GrafanaDeviceStatusController.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  12/26/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using openHistorian.Adapters.Model;
using GrafanaAdapters;
using GSF.Data;
using GSF.Data.Model;
using CancellationToken = System.Threading.CancellationToken;
using ValidateAntiForgeryTokenAttribute = System.Web.Mvc.ValidateAntiForgeryTokenAttribute;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for a simple JSON based Grafana "device status" based data source,
    /// accessible from Grafana data source as http://localhost:8180/api/grafanadevicestatus
    /// </summary>
    public class GrafanaDeviceController : GrafanaController
    {
        #region [ Methods ]

        /// <summary>
        /// Queries current alarm device state.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IEnumerable<AlarmDeviceStateView>> GetAlarmState(QueryRequest request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
                {
                    using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                    {
                        return new TableOperations<AlarmDeviceStateView>(connection).QueryRecords();
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// Queries current data availability.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IEnumerable<DataAvailability>> GetDataAvailability(QueryRequest request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
                {
                    using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                    {
                        return new TableOperations<DataAvailability>(connection).QueryRecords();
                    }
                },
                cancellationToken);
        }

        #endregion
    }
}