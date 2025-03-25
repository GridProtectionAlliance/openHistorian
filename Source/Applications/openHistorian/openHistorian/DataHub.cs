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

// Ignore Spelling: json

using GSF;
using GSF.COMTRADE;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.Identity;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using ModbusAdapters;
using ModbusAdapters.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using openHistorian.Adapters;
using openHistorian.Model;
using PhasorWebUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using GSF.IO;
using Measurement = openHistorian.Model.Measurement;
using Microsoft.Ajax.Utilities;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace openHistorian
{
    [AuthorizeHubRole]
    public class DataHub : RecordOperationsHub<DataHub>, IHistorianOperations, IDirectoryBrowserOperations, IModbusOperations
    {
        #region [ Members ]

        // Fields
        private readonly HistorianOperations m_historianOperations;
        private readonly ReportOperations m_reportOperations;
        private readonly ModbusOperations m_modbusOperations;

        #endregion

        #region [ Constructors ]

        public DataHub() : base(Program.Host.LogWebHostStatusMessage, Program.Host.LogException)
        {
            void logStatusMessage(string message, UpdateType updateType) => LogStatusMessage(message, updateType);
            void logException(Exception ex) => LogException(ex);

            m_historianOperations = new HistorianOperations(this, logStatusMessage, logException);
            m_modbusOperations = new ModbusOperations(this, logStatusMessage, logException);
            m_reportOperations = new ReportOperations(this, logStatusMessage, logException);

        }

        #endregion

        #region [ Methods ]

        public override Task OnConnected()
        {
            LogStatusMessage($"DataHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}", UpdateType.Information, false);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                // Dispose any associated hub operations associated with current SignalR client
                m_historianOperations?.EndSession();
                m_modbusOperations?.EndSession();
                m_reportOperations?.EndSession();

                LogStatusMessage($"DataHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}", UpdateType.Information, false);
            }

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static int s_modbusProtocolID;
        private static int s_comtradeProtocolID;
        private static int s_ieeeC37_118ID;
        private static int s_virtualProtocolID;
        //private static int s_vphmSignalTypeID;
        private static int s_calcSignalTypeID;
        private static string s_dateTimeFormat;

        // Static Constructor
        static DataHub()
        {
            ModbusPoller.ProgressUpdated += (sender, args) => ProgressUpdated(sender, new EventArgs<string, List<ProgressUpdate>>(null, new List<ProgressUpdate>() { args.Argument }));
        }

        private static void ProgressUpdated(object sender, EventArgs<string, List<ProgressUpdate>> e)
        {
            string deviceName = null;

            if (sender is ModbusPoller modbusPoller)
                deviceName = modbusPoller.Name;

            if (deviceName is null)
                return;

            string clientID = e.Argument1;

            List<object> updates = e.Argument2
                .Select(update => update.AsExpandoObject())
                .ToList();

            if (clientID is null)
                GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.All.deviceProgressUpdate(deviceName, updates);
            else
                GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.Client(clientID).deviceProgressUpdate(deviceName, updates);
        }

        #endregion

        // Client-side script functionality
        #region [ Report View Operations ]

        [RecordOperation(typeof(ReportMeasurements), RecordOperation.QueryRecordCount)]
        public int QuerySNRMeasurmentCount(string _)
        {
            return m_reportOperations.GetCount();
        }

        [RecordOperation(typeof(ReportMeasurements), RecordOperation.QueryRecords)]
        public IEnumerable<ReportMeasurements> QueryReportMeasurments(string sortField, bool ascending, int page, int pageSize, string _)
        {
            return m_reportOperations.GetData(sortField, ascending, page, pageSize);
        }

        [RecordOperation(typeof(ReportMeasurements), RecordOperation.CreateNewRecord)]
        public ReportMeasurements NewReportMeasurement()
        {
            //This is not really allowed need to check if we can disable that
            return new ReportMeasurements();
        }

        /// <summary>
        /// Set selected Report Sources.
        /// </summary>
        /// <param name="reportType">Type of the report <see cref="ReportType"/>.</param>
        /// <param name="reportCriteria">Criteria of the report <see cref="ReportCriteria"/>.</param>
        /// <param name="number">Depth of the report (0 is all).</param>
        /// <param name="start">Start time of the report.</param>
        /// <param name="end">End time of the report.</param>
        /// <returns> Flag to ensure this is completed before creating a report.</returns>
        public bool SetReportingSource(int reportType, int reportCriteria, int number, DateTime start, DateTime end)
        {
            m_reportOperations.UpdateReportSource(start, end, (ReportCriteria)reportCriteria, (ReportType)reportType, number, DataContext);
            return true;
        }

        /// <summary>
        /// Gets Progress of Setting Current Reporting Source.
        /// </summary>
        public double SetReportingSourceProgress()
        {
            return this.m_reportOperations.GetReportProgress();
        }

        #endregion

        #region [ ActiveMeasurement View Operations ]

        [RecordOperation(typeof(ActiveMeasurement), RecordOperation.QueryRecordCount)]
        public int QueryActiveMeasurementCount(string filterText)
        {
            TableOperations<ActiveMeasurement> tableOperations = DataContext.Table<ActiveMeasurement>();
            tableOperations.RootQueryRestriction[0] = $"{GetSelectedInstanceName()}:%";
            return tableOperations.QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(ActiveMeasurement), RecordOperation.QueryRecords)]
        public IEnumerable<ActiveMeasurement> QueryActiveMeasurements(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<ActiveMeasurement> tableOperations = DataContext.Table<ActiveMeasurement>();
            if (!tableOperations.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            tableOperations.RootQueryRestriction[0] = $"{GetSelectedInstanceName()}:%";
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [RecordOperation(typeof(ActiveMeasurement), RecordOperation.CreateNewRecord)]
        public ActiveMeasurement NewActiveMeasurement()
        {
            return DataContext.Table<ActiveMeasurement>().NewRecord();
        }

        #endregion

        #region [ Device Table Operations ]

        [RecordOperation(typeof(Device), RecordOperation.QueryRecordCount)]
        public int QueryDeviceCount(Guid nodeID, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND NOT (ProtocolID = {1} AND AccessID = {2})", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Device), RecordOperation.QueryRecords)]
        public IEnumerable<Device> QueryDevices(Guid nodeID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();
            if (!deviceTable.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND NOT (ProtocolID = {1} AND AccessID = {2})", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }
        
        public int QueryEnabledDeviceCount(Guid nodeID, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND NOT (ProtocolID = {1} AND AccessID = {2})", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                new RecordRestriction("Acronym NOT LIKE 'SYSTEM!%'") +
                new RecordRestriction("Enabled <> 0") +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecordCount(restriction);
        }

        public IEnumerable<Device> QueryEnabledDevices(Guid nodeID, int limit, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND NOT (ProtocolID = {1} AND AccessID = {2})", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                new RecordRestriction("Acronym NOT LIKE 'SYSTEM!%'") +
                new RecordRestriction("Enabled <> 0") +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecords("Acronym", restriction, limit);
        }

        public Device QueryDevice(string acronym)
        {
            return DataContext.Table<Device>().QueryRecordWhere("Acronym = {0}", acronym) ?? NewDevice();
        }

        public Device QueryDeviceByID(int deviceID)
        {
            return DataContext.Table<Device>().QueryRecordWhere("ID = {0}", deviceID) ?? NewDevice();
        }

        public IEnumerable<Device> QueryChildDevices(int deviceID)
        {
            return DataContext.Table<Device>().QueryRecordsWhere("ParentID = {0}", deviceID);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.DeleteRecord)]
        public void DeleteDevice(int id)
        {
            try
            {
                Device device = QueryDeviceByID(id);
                File.Delete(PhasorConfigController.GetJsonConfigurationFileName(device.Acronym));
            }
            catch (Exception ex)
            {
                LogPublisher log = Logger.CreatePublisher(typeof(DataHub), MessageClass.Component);
                log.Publish(MessageLevel.Warning, "Error Message", "Failed to delete cached device config", null, ex);
            }

            TableOperations<Device> deviceTable = DataContext.Table<Device>();            
            deviceTable.DeleteRecordWhere("ParentID = {0}", id);
            deviceTable.DeleteRecord(id);
        }

        [RecordOperation(typeof(Device), RecordOperation.CreateNewRecord)]
        public Device NewDevice()
        {
            return DataContext.Table<Device>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.AddNewRecord)]
        public void AddNewDevice(Device device)
        {
            if ((device.ProtocolID ?? 0) == 0)
                device.ProtocolID = ModbusProtocolID;

            DataContext.Table<Device>().AddNewRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.UpdateRecord)]
        public void UpdateDevice(Device device)
        {
            if ((device.ProtocolID ?? 0) == 0)
                device.ProtocolID = ModbusProtocolID;

            DataContext.Table<Device>().UpdateRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void AddNewOrUpdateDevice(Device device)
        {
            DataContext.Table<Device>().AddNewOrUpdateRecord(device);
        }

        public void RemoveDeviceCalculations(int deviceID)
        {
            Device device = QueryDeviceByID(deviceID);

            if (device is null || device.ID == 0)
                return;

            DataContext.Connection.ExecuteNonQuery($"DELETE FROM CustomActionAdapter WHERE TypeName = 'DynamicCalculator.DynamicCalculator' AND AdapterName LIKE '{device.Acronym}%'");
            DataContext.Connection.ExecuteNonQuery($"DELETE FROM Measurement WHERE DeviceID = {device.ID} AND SignalTypeID = {CalcSignalTypeID}");
        }

        public void RemoveAllDeviceCalculations()
        {
            DataContext.Connection.ExecuteNonQuery("DELETE FROM CustomActionAdapter WHERE TypeName = 'DynamicCalculator.DynamicCalculator'");
            DataContext.Connection.ExecuteNonQuery("DELETE FROM CustomActionAdapter WHERE TypeName = 'PowerCalculations.BulkSequenceCalculator' AND AdapterName <> 'BULK_SEQ'");
            DataContext.Connection.ExecuteNonQuery($"DELETE FROM Measurement WHERE SignalTypeID = {CalcSignalTypeID}");
        }

        #endregion

        #region [ DeviceGroup Table Operations ]

        [RecordOperation(typeof(DeviceGroup), RecordOperation.QueryRecordCount)]
        public int QueryDeviceGroupCount(Guid nodeID, string filterText)
        {
            TableOperations<DeviceGroup> deviceGroupTable = DataContext.Table<DeviceGroup>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND ProtocolID = {1} AND AccessID = {2}", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                deviceGroupTable.GetSearchRestriction(filterText);

            return deviceGroupTable.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(DeviceGroup), RecordOperation.QueryRecords)]
        public IEnumerable<DeviceGroup> QueryDeviceGroups(Guid nodeID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<DeviceGroup> deviceGroupTable = DataContext.Table<DeviceGroup>();
            if (!deviceGroupTable.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND ProtocolID = {1} AND AccessID = {2}", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                deviceGroupTable.GetSearchRestriction(filterText);

            return deviceGroupTable.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public DeviceGroup QueryDeviceGroup(Guid nodeID, int id)
        {
            return DataContext.Table<DeviceGroup>().QueryRecordWhere("NodeID = {0} AND ID = {1}", nodeID, id);
        }

        public IEnumerable<Device> QueryDeviceGroupDevices(Guid nodeID, int id)
        {
            DeviceGroup deviceGroup = QueryDeviceGroup(nodeID, id);

            if (string.IsNullOrWhiteSpace(deviceGroup?.ConnectionString))
                return Enumerable.Empty<Device>();

            Dictionary<string, string> settings = deviceGroup.ConnectionString.ParseKeyValuePairs();

            if (!settings.TryGetValue("deviceIDs", out string deviceIDs) || string.IsNullOrWhiteSpace(deviceIDs))
                return Enumerable.Empty<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0} AND NOT (ProtocolID = {1} AND AccessID = {2})", nodeID, VirtualProtocolID, DeviceGroup.DefaultAccessID) +
                $"ID IN ({deviceIDs})";

            return DataContext.Table<Device>().QueryRecords(restriction);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(DeviceGroup), RecordOperation.DeleteRecord)]
        public void DeleteDeviceGroup(int id)
        {
            DataContext.Table<DeviceGroup>().DeleteRecord(id);
        }

        [RecordOperation(typeof(DeviceGroup), RecordOperation.CreateNewRecord)]
        public DeviceGroup NewDeviceGroup()
        {
            return DataContext.Table<DeviceGroup>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(DeviceGroup), RecordOperation.AddNewRecord)]
        public void AddNewDeviceGroup(DeviceGroup deviceGroup)
        {
            deviceGroup.ProtocolID = VirtualProtocolID;
            deviceGroup.AccessID = DeviceGroup.DefaultAccessID;
            DataContext.Table<DeviceGroup>().AddNewRecord(deviceGroup);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(DeviceGroup), RecordOperation.UpdateRecord)]
        public void UpdateDeviceGroup(DeviceGroup deviceGroup)
        {
            deviceGroup.ProtocolID = VirtualProtocolID;
            deviceGroup.AccessID = DeviceGroup.DefaultAccessID;
            DataContext.Table<DeviceGroup>().UpdateRecord(deviceGroup);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void AddNewOrUpdateDeviceGroup(DeviceGroup deviceGroup)
        {
            deviceGroup.ProtocolID = VirtualProtocolID;
            deviceGroup.AccessID = DeviceGroup.DefaultAccessID;
            DataContext.Table<DeviceGroup>().AddNewOrUpdateRecord(deviceGroup);
        }

        #endregion

        #region [ DeviceGroupClass Table Operations ]

        [RecordOperation(typeof(DeviceGroupClass), RecordOperation.QueryRecordCount)]
        public int QueryDeviceGroupClassCount(string filterText)
        {
            return DataContext.Table<DeviceGroupClass>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(DeviceGroupClass), RecordOperation.QueryRecords)]
        public IEnumerable<DeviceGroupClass> QueryDeviceGroupClasses(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<DeviceGroupClass> tableOperations = DataContext.Table<DeviceGroupClass>();
            if (!tableOperations.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public DateTime QueryDeviceGroupClassLastUpdated()
        {
            return DataContext.Table<DeviceGroupClass>().QueryRecords().Max(deviceGroupClass => deviceGroupClass.UpdatedOn);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(DeviceGroupClass), RecordOperation.DeleteRecord)]
        public void DeleteDeviceGroupClass(int id)
        {
            DataContext.Table<DeviceGroupClass>().DeleteRecord(id);
        }

        [RecordOperation(typeof(DeviceGroupClass), RecordOperation.CreateNewRecord)]
        public DeviceGroupClass NewDeviceGroupClass()
        {
            return DataContext.Table<DeviceGroupClass>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(DeviceGroupClass), RecordOperation.AddNewRecord)]
        public void AddNewDeviceGroupClass(DeviceGroupClass deviceGroupClass)
        {
            DataContext.Table<DeviceGroupClass>().AddNewRecord(deviceGroupClass);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(DeviceGroupClass), RecordOperation.UpdateRecord)]
        public void UpdateDeviceGroupClass(DeviceGroupClass deviceGroupClass)
        {
            DataContext.Table<DeviceGroupClass>().UpdateRecord(deviceGroupClass);
        }

        #endregion

        #region [ Measurement Table Operations ]

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecordCount)]
        public int QueryMeasurementCount(string filterText)
        {
            return DataContext.Table<Measurement>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecords)]
        public IEnumerable<Measurement> QueryMeasurements(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Measurement> tableOperations = DataContext.Table<Measurement>();
            if (!tableOperations.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public Measurement QueryMeasurement(string signalReference)
        {
            return DataContext.Table<Measurement>().QueryRecordWhere("SignalReference = {0}", signalReference) ?? NewMeasurement();
        }

        public Measurement QueryMeasurementByPointTag(string pointTag)
        {
            return DataContext.Table<Measurement>().QueryRecordWhere("PointTag = {0}", pointTag) ?? NewMeasurement();
        }

        public Measurement QueryMeasurementBySignalID(Guid signalID)
        {
            return DataContext.Table<Measurement>().QueryRecordWhere("SignalID = {0}", signalID) ?? NewMeasurement();
        }

        public IEnumerable<Measurement> QueryDeviceMeasurements(int deviceID)
        {
            return DataContext.Table<Measurement>().QueryRecordsWhere("DeviceID = {0}", deviceID);
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
            return DataContext.Table<Measurement>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.AddNewRecord)]
        public void AddNewMeasurement(Measurement measurement)
        {
            DataContext.Table<Measurement>().AddNewRecord(measurement);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.UpdateRecord)]
        public void UpdateMeasurement(Measurement measurement)
        {
            DataContext.Table<Measurement>().UpdateRecord(measurement);
        }

        public void AddNewOrUpdateMeasurement(Measurement measurement)
        {
            DataContext.Table<Measurement>().AddNewOrUpdateRecord(measurement);
        }

        #endregion

        #region [ PhasorDetail Table Operations ]

        [RecordOperation(typeof(PhasorDetail), RecordOperation.QueryRecordCount)]
        public int QueryPhasorCount(int deviceID, string filterText)
        {
            TableOperations<PhasorDetail> phasorDetailTable = DataContext.Table<PhasorDetail>();

            RecordRestriction restriction = (deviceID > 0 ?
                new RecordRestriction("DeviceID = {0}", deviceID) : null) +
                phasorDetailTable.GetSearchRestriction(filterText);

            return phasorDetailTable.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(PhasorDetail), RecordOperation.QueryRecords)]
        public IEnumerable<PhasorDetail> QueryPhasors(int deviceID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<PhasorDetail> phasorDetailTable = DataContext.Table<PhasorDetail>();
            if (!phasorDetailTable.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            RecordRestriction restriction = (deviceID > 0 ?
                new RecordRestriction("DeviceID = {0}", deviceID) : null) +
                phasorDetailTable.GetSearchRestriction(filterText);

            return DataContext.Table<PhasorDetail>().QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        #endregion

        #region [ CustomActionAdapter Table Operations ]

        [RecordOperation(typeof(CustomActionAdapter), RecordOperation.QueryRecordCount)]
        public int QueryCustomActionAdapterCount(string filterText)
        {
            return DataContext.Table<CustomActionAdapter>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(CustomActionAdapter), RecordOperation.QueryRecords)]
        public IEnumerable<CustomActionAdapter> QueryCustomActionAdapters(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<CustomActionAdapter> tableOperations = DataContext.Table<CustomActionAdapter>();
            if (!tableOperations.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(CustomActionAdapter), RecordOperation.DeleteRecord)]
        public void DeleteCustomActionAdapter(int id)
        {
            DataContext.Table<CustomActionAdapter>().DeleteRecord(id);
        }

        [RecordOperation(typeof(CustomActionAdapter), RecordOperation.CreateNewRecord)]
        public CustomActionAdapter NewCustomActionAdapter()
        {
            return DataContext.Table<CustomActionAdapter>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(CustomActionAdapter), RecordOperation.AddNewRecord)]
        public void AddNewCustomActionAdapter(CustomActionAdapter customActionAdapter)
        {
            DataContext.Table<CustomActionAdapter>().AddNewRecord(customActionAdapter);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(CustomActionAdapter), RecordOperation.UpdateRecord)]
        public void UpdateCustomActionAdapter(CustomActionAdapter customActionAdapter)
        {
            DataContext.Table<CustomActionAdapter>().UpdateRecord(customActionAdapter);
        }

        public void AddNewOrUpdateCustomActionAdapter(CustomActionAdapter customActionAdapter)
        {
            TableOperations<CustomActionAdapter> customActionAdapterTable = DataContext.Table<CustomActionAdapter>();

            if (customActionAdapterTable.QueryRecordCountWhere("AdapterName = {0}", customActionAdapter.AdapterName) == 0)
            {
                AddNewCustomActionAdapter(customActionAdapter);
            }
            else
            {
                CustomActionAdapter existingActionAdapter = customActionAdapterTable.QueryRecordWhere("AdapterName = {0}", customActionAdapter.AdapterName);
                
                existingActionAdapter.AssemblyName = customActionAdapter.AssemblyName;
                existingActionAdapter.TypeName = customActionAdapter.TypeName;
                existingActionAdapter.ConnectionString = customActionAdapter.ConnectionString;
                existingActionAdapter.LoadOrder = customActionAdapter.LoadOrder;
                existingActionAdapter.Enabled = customActionAdapter.Enabled;
                existingActionAdapter.UpdatedBy = customActionAdapter.UpdatedBy;
                existingActionAdapter.UpdatedOn = customActionAdapter.UpdatedOn;
                
                UpdateCustomActionAdapter(existingActionAdapter);
            }
        }

        #endregion

        #region [ OscEvents Table Operations ]

        [RecordOperation(typeof(OscEvents), RecordOperation.QueryRecordCount)]
        public int QueryOscEventsCount(int parentID, string filterText)
        {
            TableOperations<OscEvents> oscEventsTable = DataContext.Table<OscEvents>();

            RecordRestriction restriction = (parentID > 0 ?
                new RecordRestriction("ParentID = {0}", parentID) : 
                new RecordRestriction("ParentID IS NULL")) +
                oscEventsTable.GetSearchRestriction(filterText);

            return DataContext.Table<OscEvents>().QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(OscEvents), RecordOperation.QueryRecords)]
        public IEnumerable<OscEvents> QueryOscEvents(int parentID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<OscEvents> oscEventsTable = DataContext.Table<OscEvents>();
            if (!oscEventsTable.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            RecordRestriction restriction = (parentID > 0 ?
                new RecordRestriction("ParentID = {0}", parentID) :
                new RecordRestriction("ParentID IS NULL")) +
                oscEventsTable.GetSearchRestriction(filterText);

            return DataContext.Table<OscEvents>().QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public int QueryAssociatedEventCount(int parentID)
        {
            return DataContext.Table<OscEvents>().QueryRecordCountWhere("ParentID = {0}", parentID);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(OscEvents), RecordOperation.DeleteRecord)]
        public void DeleteOscEvents(int id)
        {
            DataContext.Table<OscEvents>().DeleteRecord(id);
        }

        [RecordOperation(typeof(OscEvents), RecordOperation.CreateNewRecord)]
        public OscEvents NewOscEvents()
        {
            return DataContext.Table<OscEvents>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(OscEvents), RecordOperation.AddNewRecord)]
        public void AddNewOscEvents(OscEvents oscEvents)
        {
            DataContext.Table<OscEvents>().AddNewRecord(oscEvents);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(OscEvents), RecordOperation.UpdateRecord)]
        public void UpdateOscEvents(OscEvents oscEvents)
        {
            DataContext.Table<OscEvents>().UpdateRecord(oscEvents);
        }

        #endregion

        #region [ EventMarker Table Operations ]

        [RecordOperation(typeof(EventMarker), RecordOperation.QueryRecordCount)]
        public int QueryEventmarkerCount(int parentID, string filterText)
        {
            TableOperations<EventMarker> eventMarkerTable = DataContext.Table<EventMarker>();

            RecordRestriction restriction = (parentID > 0 ?
                new RecordRestriction("ParentID = {0}", parentID) :
                new RecordRestriction("ParentID IS NULL")) +
                eventMarkerTable.GetSearchRestriction(filterText);

            return DataContext.Table<EventMarker>().QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(EventMarker), RecordOperation.QueryRecords)]
        public IEnumerable<EventMarker> QueryEventMarker(int parentID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<EventMarker> eventMarkerTable = DataContext.Table<EventMarker>();
            if (!eventMarkerTable.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            RecordRestriction restriction = (parentID > 0 ?
                new RecordRestriction("ParentID = {0}", parentID) :
                new RecordRestriction("ParentID IS NULL")) +
                eventMarkerTable.GetSearchRestriction(filterText);

            return DataContext.Table<EventMarker>().QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public int QueryAssociatedEventMarkerCount(int parentID)
        {
            return DataContext.Table<EventMarker>().QueryRecordCountWhere("ParentID = {0}", parentID);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(EventMarker), RecordOperation.DeleteRecord)]
        public void DeleteEventMarker(int id)
        {
            DataContext.Table<EventMarker>().DeleteRecord(id);
        }

        [RecordOperation(typeof(EventMarker), RecordOperation.CreateNewRecord)]
        public EventMarker NewEventMarker()
        {
            return DataContext.Table<EventMarker>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(EventMarker), RecordOperation.AddNewRecord)]
        public void AddNewEventMarker(EventMarker eventMarker)
        {
            DataContext.Table<EventMarker>().AddNewRecord(eventMarker);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(EventMarker), RecordOperation.UpdateRecord)]
        public void UpdateEventMarkers(EventMarker eventMarker)
        {
            DataContext.Table<EventMarker>().UpdateRecord(eventMarker);
        }

        #endregion

        #region [ Historian Table Operations ]

        [RecordOperation(typeof(Historian), RecordOperation.QueryRecordCount)]
        public int QueryHistorianCount(string filterText)
        {
            return DataContext.Table<Historian>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Historian), RecordOperation.QueryRecords)]
        public IEnumerable<Historian> QueryHistorians(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Historian> tableOperations = DataContext.Table<Historian>();
            if (!tableOperations.FieldExists(sortField))
                throw new InvalidOperationException($"\"{sortField}\" is not a valid field");

            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public Historian QueryHistorian(string acronym)
        {
            return DataContext.Table<Historian>().QueryRecordWhere("Acronym = {0}", acronym);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Historian), RecordOperation.DeleteRecord)]
        public void DeleteHistorian(int id)
        {
            DataContext.Table<Historian>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Historian), RecordOperation.CreateNewRecord)]
        public Historian NewHistorian()
        {
            return DataContext.Table<Historian>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Historian), RecordOperation.AddNewRecord)]
        public void AddNewHistorian(Historian historian)
        {
            DataContext.Table<Historian>().AddNewRecord(historian);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Historian), RecordOperation.UpdateRecord)]
        public void UpdateHistorian(Historian historian)
        {
            DataContext.Table<Historian>().UpdateRecord(historian);
        }

        #endregion

        #region [ Historian Operations ]

        /// <summary>
        /// Set selected Historian instance name.
        /// </summary>
        /// <param name="instanceName">Instance name that is selected by user.</param>
        public void SetSelectedInstanceName(string instanceName)
        {
            m_historianOperations.SetSelectedInstanceName(instanceName);
        }

        /// <summary>
        /// Gets selected instance name.
        /// </summary>
        /// <returns>Selected instance name.</returns>
        public string GetSelectedInstanceName()
        {
            return m_historianOperations.GetSelectedInstanceName();
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public IEnumerable<string> GetInstanceNames() => m_historianOperations.GetInstanceNames();

        /// <summary>
        /// Begins a new historian read operation.
        /// </summary>
        /// <param name="totalValues">Total values or timespan to read, if known in advance.</param>
        /// <returns>New operational state handle.</returns>
        public uint BeginHistorianRead(long totalValues)
        {
            return m_historianOperations.BeginHistorianRead(totalValues);
        }

        /// <summary>
        /// Begins a new historian write operation.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="values">Enumeration of <see cref="TrendValue"/> instances to write.</param>
        /// <param name="totalValues">Total values or timespan to write, if known in advance.</param>
        /// <param name="timestampType">Type of timestamps.</param>
        /// <returns>New operational state handle.</returns>
        public uint BeginHistorianWrite(string instanceName, IEnumerable<TrendValue> values, long totalValues, TimestampType timestampType)
        {
            return m_historianOperations.BeginHistorianWrite(instanceName, values, totalValues, timestampType);
        }

        /// <summary>
        /// Gets current historian operation state for specified handle.
        /// </summary>
        /// <param name="operationHandle">Handle to historian operation state.</param>
        /// <returns>Current historian operation state.</returns>
        public HistorianOperationState GetHistorianOperationState(uint operationHandle)
        {
            return m_historianOperations.GetHistorianOperationState(operationHandle);
        }

        /// <summary>
        /// Cancels a historian operation.
        /// </summary>
        /// <param name="operationHandle">Handle to historian operation state.</param>
        /// <returns><c>true</c> if operation was successfully terminated; otherwise, <c>false</c>.</returns>
        public bool CancelHistorianOperation(uint operationHandle)
        {
            return m_historianOperations.CancelHistorianOperation(operationHandle);
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(string instanceName, DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit)
        {
            return m_historianOperations.GetHistorianData(instanceName, startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit);
        }

        /// <summary>
        /// Read historian data from server.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
        /// <param name="resolution">Resolution for data query.</param>
        /// <param name="seriesLimit">Maximum number of points per series.</param>
        /// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
        /// <param name="timestampType">Type of timestamps.</param>
        /// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
        public IEnumerable<TrendValue> GetHistorianData(string instanceName, DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit, TimestampType timestampType)
        {
            return m_historianOperations.GetHistorianData(instanceName, startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit, timestampType);
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

        #region [ Modbus Operations ]

        public Task<bool> ModbusConnect(string connectionString)
        {
            return m_modbusOperations.ModbusConnect(connectionString);
        }

        public void ModbusDisconnect()
        {
            m_modbusOperations.ModbusDisconnect();
        }

        public async Task<bool[]> ReadDiscreteInputs(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadDiscreteInputs(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading discrete inputs starting @ {startAddress}: {ex.Message}", ex));
                return Array.Empty<bool>();
            }
        }

        public async Task<bool[]> ReadCoils(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadCoils(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading coil values starting @ {startAddress}: {ex.Message}", ex));
                return Array.Empty<bool>();
            }
        }

        public async Task<ushort[]> ReadInputRegisters(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadInputRegisters(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading input registers starting @ {startAddress}: {ex.Message}", ex));
                return Array.Empty<ushort>();
            }
        }

        public async Task<ushort[]> ReadHoldingRegisters(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadHoldingRegisters(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading holding registers starting @ {startAddress}: {ex.Message}", ex));
                return Array.Empty<ushort>();
            }
        }

        public async Task WriteCoils(ushort startAddress, bool[] data)
        {
            try
            {
                await m_modbusOperations.WriteCoils(startAddress, data);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while writing coil values starting @ {startAddress}: {ex.Message}", ex));
            }
        }

        public async Task WriteHoldingRegisters(ushort startAddress, ushort[] data)
        {
            try
            {
                await m_modbusOperations.WriteHoldingRegisters(startAddress, data);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while writing holding registers starting @ {startAddress}: {ex.Message}", ex));
            }
        }

        public string DeriveString(ushort[] values)
        {
            return m_modbusOperations.DeriveString(values);
        }

        public float DeriveSingle(ushort highValue, ushort lowValue)
        {
            return m_modbusOperations.DeriveSingle(highValue, lowValue);
        }

        public double DeriveDouble(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return m_modbusOperations.DeriveDouble(b3, b2, b1, b0);
        }

        public int DeriveInt32(ushort highValue, ushort lowValue)
        {
            return m_modbusOperations.DeriveInt32(highValue, lowValue);
        }

        public uint DeriveUInt32(ushort highValue, ushort lowValue)
        {
            return m_modbusOperations.DeriveUInt32(highValue, lowValue);
        }

        public long DeriveInt64(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return m_modbusOperations.DeriveInt64(b3, b2, b1, b0);
        }

        public ulong DeriveUInt64(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return m_modbusOperations.DeriveUInt64(b3, b2, b1, b0);
        }

        #endregion

        #region [ Data Export Operations ]

        public string GenerateCacheID() =>
            Guid.NewGuid().ToString();

        public uint BeginDataExport(string startTime, string endTime)
        {
            long startTicks, endTicks;

            try
            {
                startTicks = DateTime.ParseExact(startTime, DateTimeFormat, null, DateTimeStyles.AdjustToUniversal).Ticks;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"{nameof(startTime)}\" parameter value \"{startTime}\". Expected format is \"{DateTimeFormat}\". Error message: {ex.Message}", nameof(startTime), ex);
            }

            try
            {
                endTicks = DateTime.ParseExact(endTime, DateTimeFormat, null, DateTimeStyles.AdjustToUniversal).Ticks;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"{nameof(endTime)}\" parameter value \"{endTime}\". Expected format is \"{DateTimeFormat}\". Error message: {ex.Message}", nameof(endTime), ex);
            }

            if (startTicks > endTicks)
                throw new ArgumentOutOfRangeException(nameof(startTime), "Cannot export data: start time exceeds end time.");

            return BeginHistorianRead(endTicks - startTicks);
        }

        #endregion

        #region [ COMTRADE Operations ]

        public Schema LoadCOMTRADEConfiguration(string configFile)
        {
            return new Schema(configFile);
        }

        public uint BeginCOMTRADEDataLoad(string instanceName, string configFile, int deviceID, bool inferTimeFromSampleRates)
        {
            Schema schema = new Schema(configFile);
            Dictionary<int, int> indexToPointID = new Dictionary<int, int>();

            // Establish channel index to point ID mapping
            foreach (Measurement measurement in QueryDeviceMeasurements(deviceID))
            {
                string alternateTag = measurement.AlternateTag;

                if (string.IsNullOrWhiteSpace(alternateTag))
                    continue;

                if (alternateTag.Length > 7 && alternateTag.StartsWith("ANALOG:") && int.TryParse(alternateTag.Substring(7), out int index))
                    indexToPointID[index - 1] = measurement.PointID;
                else if (alternateTag.Length > 8 && alternateTag.StartsWith("DIGITAL:") && int.TryParse(alternateTag.Substring(8), out index))
                    indexToPointID[schema.TotalAnalogChannels + index - 1] = measurement.PointID;
            }

            return BeginHistorianWrite(instanceName, ReadCOMTRADEValues(schema, indexToPointID, inferTimeFromSampleRates), schema.TotalSamples * indexToPointID.Count, TimestampType.Ticks);
        }

        private IEnumerable<TrendValue> ReadCOMTRADEValues(Schema schema, Dictionary<int, int> indexToPointID, bool inferTimeFromSampleRates)
        {
            using Parser parser = new Parser
            {
                Schema = schema,
                InferTimeFromSampleRates = inferTimeFromSampleRates
            };

            parser.OpenFiles();

            while (parser.ReadNext())
            {
                double timestamp = parser.Timestamp.Ticks;

                for (int i = 0; i < schema.TotalChannels; i++)
                {
                    if (indexToPointID.TryGetValue(i, out int pointID))
                        yield return new TrendValue
                        {
                            ID = pointID,
                            Timestamp = timestamp,
                            Value = parser.Values[i]
                        };
                }
            }
        }

        #endregion

        #region [ Miscellaneous Functions ]

        private int ModbusProtocolID => s_modbusProtocolID != 0 ? s_modbusProtocolID : s_modbusProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Modbus'");

        private int ComtradeProtocolID => s_comtradeProtocolID != 0 ? s_comtradeProtocolID : s_comtradeProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='COMTRADE'");

        private int IeeeC37_118ID => s_ieeeC37_118ID != 0 ? s_ieeeC37_118ID : s_ieeeC37_118ID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='IeeeC37_118V1'");

        private int VirtualProtocolID => s_virtualProtocolID != 0 ? s_virtualProtocolID : s_virtualProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='VirtualInput'");

        //private int VphmSignalTypeID => s_vphmSignalTypeID != 0 ? s_vphmSignalTypeID : (s_vphmSignalTypeID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM SignalType WHERE Acronym='VPHM'"));

        private int CalcSignalTypeID => s_calcSignalTypeID != 0 ? s_calcSignalTypeID : s_calcSignalTypeID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM SignalType WHERE Acronym='CALC'");

        private static string DateTimeFormat => s_dateTimeFormat ??= Program.Host.Model.Global.DateTimeFormat;

        /// <summary>
        /// Gets protocol ID for "ModbusPoller" protocol.
        /// </summary>
        public int GetModbusProtocolID() => ModbusProtocolID;

        /// <summary>
        /// Gets protocol ID for "COMTRADE" protocol.
        /// </summary>
        public int GetComtradeProtocolID() => ComtradeProtocolID;

        public string GetProtocolCategory(int protocolID) => 
            DataContext.Table<Protocol>().QueryRecordWhere("ID = {0}", protocolID)?.Category ?? "Undefined";

        /// <summary>
        /// Determines if directory exists from server's perspective.
        /// </summary>
        /// <param name="path">Directory path to test for existence.</param>
        /// <returns><c>true</c> if directory exists; otherwise, <c>false</c>.</returns>
        public bool DirectoryExists(string path) => 
            Directory.Exists(path);

        /// <summary>
        /// Determines if file exists from server's perspective.
        /// </summary>
        /// <param name="path">Path and file name to test for existence.</param>
        /// <returns><c>true</c> if file exists; otherwise, <c>false</c>.</returns>
        public bool FileExists(string path) => 
            File.Exists(path);

        /// <summary>
        /// Requests that the device send the current list of progress updates.
        /// </summary>
        public void QueryDeviceStatus()
        {
            // Typically used with FTP down-loaders...
        }

        /// <summary>
        /// Gets current user ID.
        /// </summary>
        /// <returns>Current user ID.</returns>
        public string GetCurrentUserID() => 
            Thread.CurrentPrincipal.Identity?.Name ?? UserInfo.CurrentUserID;

        /// <summary>
        /// Gets elapsed time between two dates as a range.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <returns>Elapsed time between two dates as a range.</returns>
        public Task<string> GetElapsedTimeString(DateTime startTime, DateTime stopTime) => 
            Task.Factory.StartNew(() => new Ticks(stopTime - startTime).ToElapsedTimeString(2));

        /// <summary>
        /// Checks if UpdateCOMTRADECounters has been marked as completed for user session.
        /// </summary>
        /// <param name="operationHandle">Handle to historian operation state.</param>
        /// <returns><c>true</c> if completed; otherwise, <c>false</c>.</returns>
        public bool CheckIfUpdateCOMTRADECountersIsCompleted(uint operationHandle) =>
            FeedbackController.CheckIfUpdateCOMTRADECountersIsCompleted(operationHandle.ToString());

        /// <summary>
        /// Saves JSON content to a local file path.
        /// </summary>
        /// <param name="targetFilePath">Target directory or file path for JSON file.</param>
        /// <param name="json">JSON file content.</param>
        /// <exception cref="SecurityException">
        ///   <para>Cannot save JSON file outside local file path.</para>
        ///   <para>OR</para>
        ///   <para>Cannot save JSON files without the .json extension.</para>
        /// </exception>
        /// <returns>URL to download filename.</returns>
        public string SaveJSONFile(string targetFilePath, string json)
        {
            string localPath = FilePath.GetAbsolutePath(@"Grafana\public");
            targetFilePath = FilePath.GetAbsolutePath(targetFilePath);

            // Prevent file saves outside local file path
            if (!targetFilePath.StartsWith(localPath, StringComparison.OrdinalIgnoreCase))
                throw new SecurityException(@"Path access error: Cannot save JSON file outside Grafana\public file path.");

            // Prevent saving data that is not valid JSON (helps prevent possible function abuse)
            if (!IsValidJson(json))
                throw new InvalidOperationException("JSON content is not valid.");

            if (string.IsNullOrEmpty(Path.GetFileName(targetFilePath)) || string.IsNullOrEmpty(Path.GetExtension(targetFilePath)))
                targetFilePath = Path.Combine(targetFilePath, $"{DateTime.Now:s}Merge.json".Replace(':', '.'));

            // Prevent saving files that are not given the .json file extension (helps prevent possible function abuse)
            if (!string.Equals(Path.GetExtension(targetFilePath), ".json", StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityException("File type error: Cannot save JSON files without the .json extension.");

            string directory = Path.GetDirectoryName(targetFilePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            File.WriteAllText(targetFilePath, json);

            return targetFilePath.Substring(localPath.Length).Replace('\\', '/');
        }

        /// <summary>
        /// Determines if <paramref name="input"/> string is valid JSON.
        /// </summary>
        /// <param name="input">Input string to test for valid JSON.</param>
        /// <returns><c>true</c> if <paramref name="input"/> is valid JSON; otherwise, <c>false</c>.</returns>
        public bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();
            
            // Check if input starts and ends with a valid JSON array or object token
            if ((!input.StartsWith("{") || !input.EndsWith("}")) && (!input.StartsWith("[") || !input.EndsWith("]")))
                return false;

            try
            {
                JToken.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
