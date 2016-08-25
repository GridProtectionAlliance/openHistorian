//******************************************************************************************************
//  CsvDownloadHandler.ashx.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  07/29/2016 - Billy Ernest
//       Generated original version of source code.
//  08/10/2016 - J. Ritchie Carroll
//       Combined ASP.NET and self-hosted handlers into a single shared embedded resource.
//
//******************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Collections;
using GSF.Data;
using GSF.Data.Model;
using GSF.Reflection;
using GSF.Security;
using GSF.Web.Hosting;
using GSF.Web.Hubs;
using GSF.Web.Model;
using openHistorian.Model;

// ReSharper disable once CheckNamespace
namespace openHistorian
{
    /// <summary>
    /// Handles downloading of modeled table data as a comma-separated value file.
    /// </summary>
    public class CsvDownloadHandler : IHostedHttpHandler
    {
        #region [ Members ]

        // Constants
        private const string CsvContentType = "text/csv";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines if client cache should be enabled for rendered handler content.
        /// </summary>
        /// <remarks>
        /// If rendered handler content does not change often, the server and client will use the
        /// <see cref="IHostedHttpHandler.GetContentHash"/> to determine if the client needs to refresh the content.
        /// </remarks>
        public bool UseClientCache => false;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets hash of response content based on any <paramref name="request"/> parameters.
        /// </summary>
        /// <param name="request">HTTP request message.</param>
        /// <remarks>
        /// Value is only used when <see cref="IHostedHttpHandler.UseClientCache"/> is <c>true</c>.
        /// </remarks>
        public long GetContentHash(HttpRequestMessage request) => 0;

        /// <summary>
        /// Enables processing of HTTP web requests by a custom handler that implements the <see cref="IHostedHttpHandler"/> interface.
        /// </summary>
        /// <param name="request">HTTP request message.</param>
        /// <param name="response">HTTP response message.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        public Task ProcessRequestAsync(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            NameValueCollection requestParameters = request.RequestUri.ParseQueryString();

            response.Content = new PushStreamContent(async (stream, content, context) => 
            {
                try
                {
                    await CopyModelAsCsvToStreamAsync(requestParameters, stream, cancellationToken);
                }
                finally
                {
                    stream.Close();
                }
            }, new MediaTypeHeaderValue(CsvContentType));

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = requestParameters["FileName"] ?? "Export.csv"
            };

            return Task.CompletedTask;
        }

        private async Task CopyModelAsCsvToStreamAsync(NameValueCollection requestParameters, Stream responseStream, CancellationToken cancellationToken)
        {
            SecurityProviderCache.ValidateCurrentProvider();

            string selectedPointIDs = requestParameters["PointIDs"];

            if (string.IsNullOrEmpty(selectedPointIDs))
                throw new ArgumentNullException(nameof(selectedPointIDs), "Cannot download CSV data: no point IDs were specified.");

            Type modelType = typeof(Measurement);
            Type hubType = typeof(DataHub);
            IRecordOperationsHub hub;

            // Record operation tuple defines method name and allowed roles
            Tuple<string, string> queryRecordsOperation;
            string queryRoles;

            try
            {
                hub = Activator.CreateInstance(hubType) as IRecordOperationsHub;

                if ((object)hub == null)
                    throw new SecurityException($"Cannot download CSV data: hub type \"DataHub\" is not a IRecordOperationsHub, access cannot be validated.");

                Tuple<string, string>[] recordOperations;

                try
                {
                    // Get any authorized query roles as defined in hub records operations for modeled table, default to read allowed for query
                    recordOperations = hub.RecordOperationsCache.GetRecordOperations(modelType);

                    if ((object)recordOperations == null)
                        throw new NullReferenceException();
                }
                catch (KeyNotFoundException ex)
                {
                    throw new SecurityException($"Cannot download CSV data: hub type \"DataHub\" does not define record operations for \"Measurement\" table, access cannot be validated.", ex);
                }

                // Get record operation for querying records
                queryRecordsOperation = recordOperations[(int)RecordOperation.QueryRecords];

                if ((object)queryRecordsOperation == null)
                    throw new NullReferenceException();

                // Get any defined role restrictions for record query operation - access to CSV download will based on these roles
                queryRoles = string.IsNullOrEmpty(queryRecordsOperation.Item1) ? "*" : queryRecordsOperation.Item2 ?? "*";
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Cannot download CSV data: failed to instantiate hub type \"DataHub\" or access record operations, access cannot be validated.", ex);
            }

            using (DataContext dataContext = new DataContext())
            using (StreamWriter writer = new StreamWriter(responseStream))
            {
                // Validate current user has access to requested data
                if (!dataContext.UserIsInRole(queryRoles))
                    throw new SecurityException($"Cannot download CSV data: access is denied for user \"{Thread.CurrentPrincipal.Identity?.Name ?? "Undefined"}\", minimum required roles = {queryRoles.ToDelimitedString(", ")}.");

                //AdoDataConnection connection = dataContext.Connection;
                //ITableOperations table = dataContext.Table(modelType);
                //string[] fieldNames = table.GetFieldNames(false);

                // Write column headers
                //await writer.WriteLineAsync(string.Join(",", fieldNames.Select(fieldName => connection.EscapeIdentifier(fieldName, true))));
                await writer.FlushAsync();

                // cancellationToken.IsCancellationRequested

                // Write data pages

                //await writer.WriteLineAsync(string.Join(",", fieldNames.Select(fieldName => $"\"{table.GetFieldValue(record, fieldName)}\"")));

                //await writer.FlushAsync();                
            }
        }

        #endregion
    }
}