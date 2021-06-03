//******************************************************************************************************
//  FeedbackController.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  05/21/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a REST based API for openHistorian feedback, e.g., from external applications.
    /// </summary>
    public class FeedbackController : ApiController
    {
        private static readonly MemoryCache s_updateCompletedCache;

        static FeedbackController()
        {
            s_updateCompletedCache = new MemoryCache($"{nameof(FeedbackController)}-UpdateCompletedCache");
        }

        /// <summary>
        /// Checks if UpdateCOMTRADECounters has been marked as completed for user session.
        /// </summary>
        /// <param name="operationHandle">Handle to historian operation state.</param>
        /// <returns><c>true</c> if completed; otherwise, <c>false</c>.</returns>
        public static bool CheckIfUpdateCOMTRADECountersIsCompleted(string operationHandle)
        {
            if (s_updateCompletedCache.Get(operationHandle) is null)
                return false;

            s_updateCompletedCache.Remove(operationHandle);
            return true;
        }

        /// <summary>
        /// Validates that feedback controller is responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Notifies openHistorian that the UpdateCOMTRADECounters application for given session has completed.
        /// </summary>
        /// <param name="operationHandle">Handle to historian operation state.</param>
        [HttpGet]
        public HttpResponseMessage SendUpdateCOMTRADECountersCompleteNotification(string operationHandle)
        {
            s_updateCompletedCache.Add(operationHandle, true, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30.0D) });
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
