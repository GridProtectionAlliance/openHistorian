//******************************************************************************************************
//  ImportedMeasurement.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  09/27/2017 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web.Http;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Web.Security;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace openHistorian.Model
{
    public class ImportedMeasurement
    {
        public Guid? NodeID { get; set; }

        public Guid? SourceNodeID { get; set; }

        public Guid? SignalID { get; set; }

        [StringLength(200)]
        public string Source { get; set; }

        public long PointID { get; set; }

        [StringLength(200)]
        public string PointTag { get; set; }

        [StringLength(200)]
        public string AlternateTag { get; set; }

        [StringLength(4)]
        public string SignalTypeAcronym { get; set; }

        [StringLength(200)]
        public string SignalReference { get; set; }

        public int? FramesPerSecond { get; set; }

        [StringLength(200)]
        public string ProtocolAcronym { get; set; }

        [StringLength(200)]
        [DefaultValue("Frame")]
        public string ProtocolType { get; set; }

        public int? PhasorID { get; set; }

        [FieldDataType(DbType.String)]
        public char? PhasorType { get; set; }

        [FieldDataType(DbType.String)]
        public char? Phase { get; set; }

        [DefaultValue(0.0D)]
        public double Adder { get; set; }

        [DefaultValue(1.0D)]
        public double Multiplier { get; set; }

        [StringLength(200)]
        public string CompanyAcronym { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string Description { get; set; }

        [DefaultValue(false)]
        public bool Enabled { get; set; }
    }

    public class ImportedMeasurementsController : ApiController
    {
        #region [ Properties ]

        private Guid NodeID
        {
            get
            {
                ConfigurationFile configurationFile = ConfigurationFile.Current;
                CategorizedSettingsElementCollection systemSettings = configurationFile.Settings["systemSettings"];
                string nodeIDSetting = systemSettings["NodeID"].Value;

                if (!Guid.TryParse(nodeIDSetting, out Guid nodeID))
                    return Guid.Empty;

                return nodeID;
            }
        }

        private string Source
        {
            get
            {
                string username = User.Identity.Name;

                return new string(username
                    .ToUpper()
                    .Select(c => char.IsLetterOrDigit(c) ? c : '_')
                    .ToArray());
            }
        }

        #endregion

        #region [ Methods ]

        // This generates a request verification token that will need to be added to the headers
        // of a web request before calling ImportMeasurements or DeleteMeasurement since these
        // methods validate the header token to prevent CSRF attacks in a browser. Browsers will
        // not allow this HTTP GET based method to be called from remote sites due to Same-Origin
        // policies unless CORS has been configured to explicitly to allow it, as such posting to
        // ImportMeasurements or DeleteMeasurement (which is allowed from any site) will fail
        // unless this header token is made available. The actual header name used to store the
        // verification token is controlled by the local configuration.
        [HttpGet]
        public HttpResponseMessage GenerateRequestVerficationToken()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Request.GenerateRequestVerficationHeaderToken(), Encoding.UTF8, "text/plain")
            };
        }

        [HttpGet]
        public IEnumerable<ImportedMeasurement> FindAll()
        {
            return QueryImportedMeasurements();
        }

        [HttpGet]
        public IEnumerable<ImportedMeasurement> FindByID(long id)
        {
            return QueryImportedMeasurements(new RecordRestriction("PointID = {0}", id));
        }

        [HttpGet]
        public IEnumerable<ImportedMeasurement> FindByPointTag(string id)
        {
            return QueryImportedMeasurements(new RecordRestriction("PointTag = {0}", id));
        }

        [HttpGet]
        public IEnumerable<ImportedMeasurement> FindByAlternateTag(string id)
        {
            return QueryImportedMeasurements(new RecordRestriction("AlternateTag = {0}", id));
        }

        [HttpPost]
        [ValidateRequestVerificationToken, SuppressMessage("Security", "SG0016", Justification = "CSRF vulnerability handled via ValidateRequestVerificationToken.")]
        public void ImportMeasurements(IEnumerable<ImportedMeasurement> measurements)
        {
            foreach (ImportedMeasurement measurement in measurements)
                CreateOrUpdate(measurement);
        }

        [HttpDelete]
        [ValidateRequestVerificationToken, SuppressMessage("Security", "SG0016", Justification = "CSRF vulnerability handled via ValidateRequestVerificationToken.")]
        public void DeleteMeasurement(long id)
        {
            long pointID = id;

            using (AdoDataConnection connection = CreateDbConnection())
            {
                TableOperations<ImportedMeasurement> importedMeasurementTable = new TableOperations<ImportedMeasurement>(connection);

                RecordRestriction recordRestriction =
                    new RecordRestriction("NodeID = {0}", NodeID) &
                    new RecordRestriction("Source = {0}", Source) &
                    new RecordRestriction("PointID = {0}", pointID);

                importedMeasurementTable.DeleteRecord(recordRestriction);
            }
        }

        private IEnumerable<ImportedMeasurement> QueryImportedMeasurements(RecordRestriction recordRestriction = null)
        {
            using (AdoDataConnection connection = CreateDbConnection())
            {
                TableOperations<ImportedMeasurement> importedMeasurementTable = new TableOperations<ImportedMeasurement>(connection);

                RecordRestriction queryRestriction =
                    new RecordRestriction("NodeID = {0}", NodeID) &
                    new RecordRestriction("Source = {0}", Source);

                if ((object)recordRestriction != null)
                    queryRestriction &= recordRestriction;

                return importedMeasurementTable.QueryRecords(queryRestriction);
            }
        }

        private void CreateOrUpdate(ImportedMeasurement measurement)
        {
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required;

            TransactionOptions transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            measurement.NodeID = NodeID;
            measurement.Source = Source;

            using (TransactionScope transactionScope = new TransactionScope(transactionScopeOption, transactionOptions))
            using (AdoDataConnection connection = CreateDbConnection())
            {
                TableOperations<ImportedMeasurement> importedMeasurementTable = new TableOperations<ImportedMeasurement>(connection);

                RecordRestriction recordRestriction =
                    new RecordRestriction("NodeID = {0}", measurement.NodeID) &
                    new RecordRestriction("Source = {0}", measurement.Source) &
                    new RecordRestriction("PointID = {0}", measurement.PointID);

                int measurementCount = importedMeasurementTable.UpdateRecord(measurement, recordRestriction);

                if (measurementCount == 0)
                    importedMeasurementTable.AddNewRecord(measurement);

                transactionScope.Complete();
            }
        }

        private AdoDataConnection CreateDbConnection()
        {
            return new AdoDataConnection("systemSettings");
        }

        #endregion
    }
}
