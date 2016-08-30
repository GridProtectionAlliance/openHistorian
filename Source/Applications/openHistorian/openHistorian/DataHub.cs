//******************************************************************************************************
//  DataHub.cs - Gbtc
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
//  01/14/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Data.Model;
using GSF.Identity;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using openHistorian.Adapters;
using openHistorian.Model;
using RecordRestriction = GSF.Data.Model.RecordRestriction;

namespace openHistorian
{
    [AuthorizeHubRole]
    public class DataHub : RecordOperationsHub<DataHub>, IHistorianQueryOperations, IDataSubscriptionOperations, IDirectoryBrowserOperations
    {
        #region [ Members ]

        // Fields
        private readonly HistorianQueryOperations m_historianQueryOperations;
        private readonly DataSubscriptionOperations m_dataSubscriptionOperations;

        #endregion

        #region [ Constructors ]

        public DataHub() : base(Program.Host.LogStatusMessage, Program.Host.LogException)
        {
            Action<string, UpdateType> logStatusMessage = (message, updateType) => LogStatusMessage(message, updateType);
            Action<Exception> logException = ex => LogException(ex);

            m_historianQueryOperations = new HistorianQueryOperations(this, logStatusMessage, logException);
            m_dataSubscriptionOperations = new DataSubscriptionOperations(this, logStatusMessage, logException);
        }

        #endregion

        #region [ Methods ]

        public override Task OnConnected()
        {
            Program.Host.LogStatusMessage($"DataHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                // Dispose any associated hub operations associated with current SignalR client
                m_historianQueryOperations?.EndSession();
                m_dataSubscriptionOperations?.EndSession();

                Program.Host.LogStatusMessage($"DataHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}");
            }

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        // Client-side script functionality

        #region [ ActiveMeasurement View Operations ]
 
        [RecordOperation(typeof(ActiveMeasurement), RecordOperation.QueryRecordCount)]
        public int QueryActiveMeasurementCount(string filterText)
        {
            TableOperations<ActiveMeasurement> tableOperations = DataContext.Table<ActiveMeasurement>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);

            if ((object)restriction == null)
            {
                restriction = new RecordRestriction("ID LIKE {0}", $"{HistorianQueryHubClient.InstanceName}:%");
            }
            else
            {
                List<object> parameters = new List<object>(restriction.Parameters);
                parameters.Add($"{HistorianQueryHubClient.InstanceName}:%");
                restriction = new RecordRestriction($"({restriction.FilterExpression}) AND ID LIKE {{{restriction.Parameters.Length}}}", parameters.ToArray());
            }

            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(ActiveMeasurement), RecordOperation.QueryRecords)]
        public IEnumerable<ActiveMeasurement> QueryActiveMeasurements(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<ActiveMeasurement> tableOperations = DataContext.Table<ActiveMeasurement>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);

            if ((object)restriction == null)
            {
                restriction = new RecordRestriction("ID LIKE {0}", $"{HistorianQueryHubClient.InstanceName}:%");
            }
            else
            {
                List<object> parameters = new List<object>(restriction.Parameters);
                parameters.Add($"{HistorianQueryHubClient.InstanceName}:%");
                restriction = new RecordRestriction($"({restriction.FilterExpression}) AND ID LIKE {{{restriction.Parameters.Length}}}", parameters.ToArray());
            }

            return DataContext.Table<ActiveMeasurement>().QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        [RecordOperation(typeof(ActiveMeasurement), RecordOperation.CreateNewRecord)]
        public ActiveMeasurement NewActiveMeasurement()
        {
            return new ActiveMeasurement();
        }

        #endregion

        #region [ Measurement Table Operations ]

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecordCount)]
        public int QueryMeasurementCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Measurement>().QueryRecordCount();

            return DataContext.Table<Measurement>().QueryRecordCount(new RecordRestriction("PointTag LIKE {0} OR AlternateTag LIKE {0} OR Description LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecords)]
        public IEnumerable<Measurement> QueryMeasurements(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Measurement>().QueryRecords(sortField, ascending, page, pageSize);

            return DataContext.Table<Measurement>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("PointTag LIKE {0} OR AlternateTag LIKE {0} OR Description LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.DeleteRecord)]
        public void DeleteMeasurement(int id)
        {
            DataContext.Table<Measurement>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Measurement), RecordOperation.CreateNewRecord)]
        public Measurement NewMeasurement()
        {
            return new Measurement();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.AddNewRecord)]
        public void AddNewMeasurement(Measurement measurement)
        {
            measurement.CreatedBy = GetCurrentUserID();
            measurement.CreatedOn = DateTime.UtcNow;
            measurement.UpdatedBy = measurement.CreatedBy;
            measurement.UpdatedOn = measurement.CreatedOn;

            DataContext.Table<Measurement>().AddNewRecord(measurement);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.UpdateRecord)]
        public void UpdateMeasurement(Measurement measurement)
        {
            measurement.UpdatedBy = GetCurrentUserID();
            measurement.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Measurement>().UpdateRecord(measurement);
        }

        #endregion

        #region [ Device Table Operations ]

        [RecordOperation(typeof(Device), RecordOperation.QueryRecordCount)]
        public int QueryDeviceCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Device>().QueryRecordCount();

            return DataContext.Table<Device>().QueryRecordCount(new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(Device), RecordOperation.QueryRecords)]
        public IEnumerable<Device> QueryDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Device>().QueryRecords(sortField, ascending, page, pageSize);

            return DataContext.Table<Device>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.DeleteRecord)]
        public void DeleteDevice(int id)
        {
            DataContext.Table<Device>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Device), RecordOperation.CreateNewRecord)]
        public Device NewDevice()
        {
            return new Device();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.AddNewRecord)]
        public void AddNewDevice(Device device)
        {
            if (device.HistorianID == -1)
                device.HistorianID = null;

            if (device.VendorDeviceID == -1)
                device.VendorDeviceID = null;

            device.NodeID = Program.Host.Model.Global.NodeID;
            device.CreatedBy = GetCurrentUserID();
            device.CreatedOn = DateTime.UtcNow;
            device.UpdatedBy = device.CreatedBy;
            device.UpdatedOn = device.CreatedOn;

            DataContext.Table<Device>().AddNewRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.UpdateRecord)]
        public void UpdateDevice(Device device)
        {
            if (device.HistorianID == -1)
                device.HistorianID = null;

            if (device.VendorDeviceID == -1)
                device.VendorDeviceID = null;

            device.UpdatedBy = GetCurrentUserID();
            device.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Device>().UpdateRecord(device);
        }

        #endregion

        #region [ Company Table Operations ]

        [RecordOperation(typeof(Company), RecordOperation.QueryRecordCount)]
        public int QueryCompanyCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Company>().QueryRecordCount();

            return DataContext.Table<Company>().QueryRecordCount(new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(Company), RecordOperation.QueryRecords)]
        public IEnumerable<Company> QueryCompanies(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Company>().QueryRecords(sortField, ascending, page, pageSize);

            return DataContext.Table<Company>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.DeleteRecord)]
        public void DeleteCompany(int id)
        {
            DataContext.Table<Company>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Company), RecordOperation.CreateNewRecord)]
        public Company NewCompany()
        {
            return new Company();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.AddNewRecord)]
        public void AddNewCompany(Company company)
        {
            company.CreatedBy = GetCurrentUserID();
            company.CreatedOn = DateTime.UtcNow;
            company.UpdatedBy = company.CreatedBy;
            company.UpdatedOn = company.CreatedOn;

            DataContext.Table<Company>().AddNewRecord(company);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.UpdateRecord)]
        public void UpdateCompany(Company company)
        {
            company.UpdatedBy = GetCurrentUserID();
            company.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Company>().UpdateRecord(company);
        }

        #endregion

        #region [ Vendor Table Operations ]

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecordCount)]
        public int QueryVendorCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Vendor>().QueryRecordCount();

            return DataContext.Table<Vendor>().QueryRecordCount(new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecords)]
        public IEnumerable<Vendor> QueryVendors(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<Vendor>().QueryRecords(sortField, ascending, page, pageSize);

            return DataContext.Table<Vendor>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.DeleteRecord)]
        public void DeleteVendor(int id)
        {
            DataContext.Table<Vendor>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Vendor), RecordOperation.CreateNewRecord)]
        public Vendor NewVendor()
        {
            return new Vendor();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.AddNewRecord)]
        public void AddNewVendor(Vendor vendor)
        {
            vendor.CreatedBy = GetCurrentUserID();
            vendor.CreatedOn = DateTime.UtcNow;
            vendor.UpdatedBy = vendor.CreatedBy;
            vendor.UpdatedOn = vendor.CreatedOn;

            DataContext.Table<Vendor>().AddNewRecord(vendor);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.UpdateRecord)]
        public void UpdateVendor(Vendor vendor)
        {
            vendor.UpdatedBy = GetCurrentUserID();
            vendor.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Vendor>().UpdateRecord(vendor);
        }

        #endregion

        #region [ VendorDevice Table Operations ]

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecordCount)]
        public int QueryVendorDeviceCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<VendorDevice>().QueryRecordCount();

            return DataContext.Table<VendorDevice>().QueryRecordCount(new RecordRestriction("Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecords)]
        public IEnumerable<VendorDevice> QueryVendorDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return DataContext.Table<VendorDevice>().QueryRecords(sortField, ascending, page, pageSize);

            return DataContext.Table<VendorDevice>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.DeleteRecord)]
        public void DeleteVendorDevice(int id)
        {
            DataContext.Table<VendorDevice>().DeleteRecord(id);
        }

        [RecordOperation(typeof(VendorDevice), RecordOperation.CreateNewRecord)]
        public VendorDevice NewVendorDevice()
        {
            return new VendorDevice();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.AddNewRecord)]
        public void AddNewVendorDevice(VendorDevice vendorDevice)
        {
            vendorDevice.CreatedBy = GetCurrentUserID();
            vendorDevice.CreatedOn = DateTime.UtcNow;
            vendorDevice.UpdatedBy = vendorDevice.CreatedBy;
            vendorDevice.UpdatedOn = vendorDevice.CreatedOn;

            DataContext.Table<VendorDevice>().AddNewRecord(vendorDevice);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.UpdateRecord)]
        public void UpdateVendorDevice(VendorDevice vendorDevice)
        {
            vendorDevice.UpdatedBy = GetCurrentUserID();
            vendorDevice.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<VendorDevice>().UpdateRecord(vendorDevice);
        }

        #endregion

        #region [ Historian Query Operations ]

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <returns>Enumeration of <see cref="MeasurementValue"/> instances read for time range.</returns>
        public IEnumerable<MeasurementValue> GetHistorianData(DateTime startTime, DateTime stopTime, long[] measurementIDs, Resolution resolution, int seriesLimit)
        {
            return m_historianQueryOperations.GetHistorianData(startTime, stopTime, measurementIDs, resolution, seriesLimit);
        }

        #endregion

        #region [ Data Subscription Operations ]

        // These functions are dependent on subscriptions to data where each client connection can customize the subscriptions, so an instance
        // of the DataHubSubscriptionClient is created per SignalR DataHub client connection to manage the subscription life-cycles.

        public IEnumerable<MeasurementValue> GetMeasurements()
        {
            return m_dataSubscriptionOperations.GetMeasurements();
        }

        public IEnumerable<DeviceDetail> GetDeviceDetails()
        {
            return m_dataSubscriptionOperations.GetDeviceDetails();
        }

        public IEnumerable<MeasurementDetail> GetMeasurementDetails()
        {
            return m_dataSubscriptionOperations.GetMeasurementDetails();
        }

        public IEnumerable<PhasorDetail> GetPhasorDetails()
        {
            return m_dataSubscriptionOperations.GetPhasorDetails();
        }

        public IEnumerable<SchemaVersion> GetSchemaVersion()
        {
            return m_dataSubscriptionOperations.GetSchemaVersion();
        }

        public IEnumerable<MeasurementValue> GetStats()
        {
            return m_dataSubscriptionOperations.GetStats();
        }

        public IEnumerable<StatusLight> GetLights()
        {
            return m_dataSubscriptionOperations.GetLights();
        }

        public void InitializeSubscriptions()
        {
            m_dataSubscriptionOperations.InitializeSubscriptions();
        }

        public void TerminateSubscriptions()
        {
            m_dataSubscriptionOperations.TerminateSubscriptions();
        }

        public void UpdateFilters(string filterExpression)
        {
            m_dataSubscriptionOperations.UpdateFilters(filterExpression);
        }

        public void StatSubscribe(string filterExpression)
        {
            m_dataSubscriptionOperations.StatSubscribe(filterExpression);
        }

        #endregion

        #region [ DirectoryBrowser Operations ]

        public IEnumerable<string> LoadDirectories(string rootFolder, bool showHidden)
        {
            return DirectoryBrowserOperations.LoadDirectories(rootFolder, showHidden);
        }

        public bool IsLogicalDrive(string path)
        {
            return DirectoryBrowserOperations.IsLogicalDrive(path);
        }

        public string ResolvePath(string path)
        {
            return DirectoryBrowserOperations.ResolvePath(path);
        }

        public string CombinePath(string path1, string path2)
        {
            return DirectoryBrowserOperations.CombinePath(path1, path2);
        }

        public void CreatePath(string path)
        {
            DirectoryBrowserOperations.CreatePath(path);
        }

        #endregion

        #region [ Miscellaneous Functions ]

        /// <summary>
        /// Gets current user ID.
        /// </summary>
        /// <returns>Current user ID.</returns>
        public string GetCurrentUserID()
        {
            return Thread.CurrentPrincipal.Identity?.Name ?? UserInfo.CurrentUserID;
        }

        /// <summary>
        /// Gets elapsed time between two dates as a range.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <returns>Elapsed time between two dates as a range.</returns>
        public string GetElapsedTimeString(DateTime startTime, DateTime stopTime)
        {
            return new Ticks(stopTime - startTime).ToElapsedTimeString(2);
        }

        #endregion
    }
}
