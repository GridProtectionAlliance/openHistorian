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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Data.Model;
using GSF.Identity;
using GSF.IO;
using GSF.Web.Model;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using openHistorian.Model;
using RecordRestriction = GSF.Data.Model.RecordRestriction;

namespace openHistorian
{
    [AuthorizeHubRole]
    public class DataHub : Hub, IRecordOperationsHub
    {
        #region [ Members ]

        // Fields
        private readonly DataContext m_dataContext;
        private DataSubscriptionHubClient m_dataSubscriptionHubClient;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public DataHub()
        {
            m_dataContext = new DataContext(exceptionHandler: LogException);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets <see cref="IRecordOperationsHub.RecordOperationsCache"/> for SignalR hub.
        /// </summary>
        public RecordOperationsCache RecordOperationsCache => s_recordOperationsCache;

        private DataSubscriptionHubClient DataSubscriptionHubClient
        {
            get
            {
                return m_dataSubscriptionHubClient ?? (m_dataSubscriptionHubClient = s_dataSubscriptionHubClients.GetOrAdd(Context.ConnectionId, id => new DataSubscriptionHubClient(Clients.Client(Context.ConnectionId))));
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DataHub"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        m_dataContext?.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        public override Task OnConnected()
        {
            s_connectCount++;
            Program.Host.LogStatusMessage($"DataHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {s_connectCount}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                DataSubscriptionHubClient client;

                // Dispose of data hub client when client connection is disconnected
                if (s_dataSubscriptionHubClients.TryRemove(Context.ConnectionId, out client))
                    client.Dispose();

                m_dataSubscriptionHubClient = null;

                s_connectCount--;
                Program.Host.LogStatusMessage($"DataHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {s_connectCount}");
            }

            return base.OnDisconnected(stopCalled);
        }

        private void LogStatusMessage(string message, UpdateType type = UpdateType.Information)
        {
            dynamic hubClient = Clients.Client(Context.ConnectionId);

            if (hubClient != null)
                hubClient.sendErrorMessage(message, type == UpdateType.Information ? 2000 : -1);

            Program.Host.LogStatusMessage(message, type);

        }

        private void LogException(Exception ex)
        {
            dynamic hubClient = Clients.Client(Context.ConnectionId);

            if (hubClient != null)
                hubClient.sendInfoMessage(ex.Message, -1);

            Program.Host.LogException(ex);
        }

        #endregion

        #region [ Static ]

        // Static Properties

        /// <summary>
        /// Gets the hub connection ID for the current thread.
        /// </summary>
        public static string CurrentConnectionID => s_connectionID.Value;

        // Static Fields
        private static readonly ConcurrentDictionary<string, DataSubscriptionHubClient> s_dataSubscriptionHubClients;
        private static volatile int s_connectCount;
        private static readonly ThreadLocal<string> s_connectionID = new ThreadLocal<string>();
        private static readonly RecordOperationsCache s_recordOperationsCache;

        // Static Methods

        /// <summary>
        /// Gets statically cached instance of <see cref="RecordOperationsCache"/> for <see cref="DataHub"/> instances.
        /// </summary>
        /// <returns>Statically cached instance of <see cref="RecordOperationsCache"/> for <see cref="DataHub"/> instances.</returns>
        public static RecordOperationsCache GetRecordOperationsCache() => s_recordOperationsCache;

        // Static Constructor
        static DataHub()
        {
            s_dataSubscriptionHubClients = new ConcurrentDictionary<string, DataSubscriptionHubClient>(StringComparer.OrdinalIgnoreCase);
            // Analyze and cache record operations of data hub
            s_recordOperationsCache = new RecordOperationsCache(typeof(DataHub));
        }

        #endregion

        // Client-side script functionality

        #region [ Company Table Operations ]

        [RecordOperation(typeof(Company), RecordOperation.QueryRecordCount)]
        public int QueryCompanyCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return m_dataContext.Table<Company>().QueryRecordCount();

            return m_dataContext.Table<Company>().QueryRecordCount(new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(Company), RecordOperation.QueryRecords)]
        public IEnumerable<Company> QueryCompanies(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return m_dataContext.Table<Company>().QueryRecords(sortField, ascending, page, pageSize);

            return m_dataContext.Table<Company>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.DeleteRecord)]
        public void DeleteCompany(int id)
        {
            m_dataContext.Table<Company>().DeleteRecord(id);
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

            m_dataContext.Table<Company>().AddNewRecord(company);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.UpdateRecord)]
        public void UpdateCompany(Company company)
        {
            company.UpdatedBy = company.CreatedBy;
            company.UpdatedOn = company.CreatedOn;

            m_dataContext.Table<Company>().UpdateRecord(company);
        }

        #endregion

        #region [ Vendor Table Operations ]

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecordCount)]
        public int QueryVendorCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return m_dataContext.Table<Vendor>().QueryRecordCount();

            return m_dataContext.Table<Vendor>().QueryRecordCount(new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecords)]
        public IEnumerable<Vendor> QueryVendors(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return m_dataContext.Table<Vendor>().QueryRecords(sortField, ascending, page, pageSize);

            return m_dataContext.Table<Vendor>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Acronym LIKE {0} OR Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.DeleteRecord)]
        public void DeleteVendor(int id)
        {
            m_dataContext.Table<Vendor>().DeleteRecord(id);
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

            m_dataContext.Table<Vendor>().AddNewRecord(vendor);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.UpdateRecord)]
        public void UpdateVendor(Vendor vendor)
        {
            vendor.UpdatedBy = vendor.CreatedBy;
            vendor.UpdatedOn = vendor.CreatedOn;

            m_dataContext.Table<Vendor>().UpdateRecord(vendor);
        }

        #endregion

        #region [ VendorDevice Table Operations ]

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecordCount)]
        public int QueryVendorDeviceCount(string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return m_dataContext.Table<VendorDevice>().QueryRecordCount();

            return m_dataContext.Table<VendorDevice>().QueryRecordCount(new RecordRestriction("Name LIKE {0}", $"%{filterText}%"));
        }

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecords)]
        public IEnumerable<VendorDevice> QueryVendorDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            if (string.IsNullOrWhiteSpace(filterText))
                return m_dataContext.Table<VendorDevice>().QueryRecords(sortField, ascending, page, pageSize);

            return m_dataContext.Table<VendorDevice>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("Name LIKE {0}", $"%{filterText}%"));
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.DeleteRecord)]
        public void DeleteVendorDevice(int id)
        {
            m_dataContext.Table<VendorDevice>().DeleteRecord(id);
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

            m_dataContext.Table<VendorDevice>().AddNewRecord(vendorDevice);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.UpdateRecord)]
        public void UpdateVendorDevice(VendorDevice vendorDevice)
        {
            vendorDevice.UpdatedBy = vendorDevice.CreatedBy;
            vendorDevice.UpdatedOn = vendorDevice.CreatedOn;

            m_dataContext.Table<VendorDevice>().UpdateRecord(vendorDevice);
        }

        #endregion

        #region [ Data Subscription Operations ]

        // These functions are dependent on subscriptions to data where each client connection can customize the subscriptions, so an instance
        // of the DataHubSubscriptionClient is created per SignalR DataHub client connection to manage the subscription life-cycles.

        public IEnumerable<MeasurementValue> GetMeasurements()
        {
            return DataSubscriptionHubClient.Measurements;
        }

        public IEnumerable<DeviceDetail> GetDeviceDetails()
        {
            return DataSubscriptionHubClient.DeviceDetails;
        }

        public IEnumerable<MeasurementDetail> GetMeasurementDetails()
        {
            return DataSubscriptionHubClient.MeasurementDetails;
        }

        public IEnumerable<PhasorDetail> GetPhasorDetails()
        {
            return DataSubscriptionHubClient.PhasorDetails;
        }

        public IEnumerable<SchemaVersion> GetSchemaVersion()
        {
            return DataSubscriptionHubClient.SchemaVersion;
        }

        public IEnumerable<MeasurementValue> GetStats()
        {
            return DataSubscriptionHubClient.Statistics;
        }

        public IEnumerable<StatusLight> GetLights()
        {
            return DataSubscriptionHubClient.StatusLights;
        }

        public void InitializeSubscriptions()
        {
            DataSubscriptionHubClient.InitializeSubscriptions();
        }

        public void TerminateSubscriptions()
        {
            DataSubscriptionHubClient.TerminateSubscriptions();
        }

        public void UpdateFilters(string filterExpression)
        {
            DataSubscriptionHubClient.UpdatePrimaryDataSubscription(filterExpression);
        }

        public void StatSubscribe(string filterExpression)
        {
            DataSubscriptionHubClient.UpdateStatisticsDataSubscription(filterExpression);
        }

        #endregion

        #region [ DirectoryBrowser Hub Operations ]

        public IEnumerable<string> LoadDirectories(string rootFolder, bool showHidden)
        {
            if (string.IsNullOrWhiteSpace(rootFolder))
                return Directory.GetLogicalDrives();

            IEnumerable<string> directories = Directory.GetDirectories(rootFolder);

            if (!showHidden)
                directories = directories.Where(path => !new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.Hidden));

            return new[] { "..\\" }.Concat(directories.Select(path => FilePath.AddPathSuffix(FilePath.GetLastDirectoryName(path))));
        }

        public bool IsLogicalDrive(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            DirectoryInfo info = new DirectoryInfo(path);
            return info.FullName == info.Root.FullName;
        }

        public string ResolvePath(string path)
        {
            if (IsLogicalDrive(path) && Path.GetFullPath(path) == path)
                return path;

            return Path.GetFullPath(FilePath.GetAbsolutePath(Environment.ExpandEnvironmentVariables(path)));
        }

        public string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public void CreatePath(string path)
        {
            Directory.CreateDirectory(path);
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

        #endregion
    }
}
