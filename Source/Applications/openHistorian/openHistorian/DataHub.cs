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

using GSF;
using GSF.COMTRADE;
using GSF.Data.Model;
using GSF.Identity;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using ModbusAdapters;
using ModbusAdapters.Model;
using openHistorian.Adapters;
using openHistorian.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Measurement = openHistorian.Model.Measurement;

namespace openHistorian
{
    [AuthorizeHubRole]
    public class DataHub : RecordOperationsHub<DataHub>, IHistorianOperations, IDataSubscriptionOperations, IDirectoryBrowserOperations, IModbusOperations
    {
        #region [ Members ]

        // Fields
        private readonly HistorianOperations m_historianOperations;
        private readonly DataSubscriptionOperations m_dataSubscriptionOperations;
        private readonly ReportOperations m_reportOperations;
        private readonly ModbusOperations m_modbusOperations;

        #endregion

        #region [ Constructors ]

        public DataHub() : base(Program.Host.LogWebHostStatusMessage, Program.Host.LogException)
        {
            Action<string, UpdateType> logStatusMessage = (message, updateType) => LogStatusMessage(message, updateType);
            Action<Exception> logException = ex => LogException(ex);

            m_historianOperations = new HistorianOperations(this, logStatusMessage, logException);
            m_dataSubscriptionOperations = new DataSubscriptionOperations(this, logStatusMessage, logException);
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
                m_dataSubscriptionOperations?.EndSession();
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
        //private static int s_vphmSignalTypeID;

        // Static Constructor
        static DataHub()
        {
            ModbusPoller.ProgressUpdated += (sender, args) => ProgressUpdated(sender, new EventArgs<string, List<ProgressUpdate>>(null, new List<ProgressUpdate>() { args.Argument }));
        }

        private static void ProgressUpdated(object sender, EventArgs<string, List<ProgressUpdate>> e)
        {
            string deviceName = null;

            ModbusPoller modbusPoller = sender as ModbusPoller;

            if ((object)modbusPoller != null)
                deviceName = modbusPoller.Name;

            if ((object)deviceName == null)
                return;

            string clientID = e.Argument1;

            List<object> updates = e.Argument2
                .Select(update => update.AsExpandoObject())
                .ToList();

            if ((object)clientID != null)
                GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.Client(clientID).deviceProgressUpdate(deviceName, updates);
            else
                GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.All.deviceProgressUpdate(deviceName, updates);
        }

        #endregion

        // Client-side script functionality
        #region [ Report View Operations ]
        [RecordOperation(typeof(ReportMeasurements), RecordOperation.QueryRecordCount)]
        public int QuerySNRMeasurmentCount(string filterText)
        {
            TableOperations<ReportMeasurements> tableOperations = m_reportOperations.Table();
            if (tableOperations == null)
                return 0;
            return tableOperations.QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(ReportMeasurements), RecordOperation.QueryRecords)]
        public IEnumerable<ReportMeasurements> QueryReportMeasurments(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<ReportMeasurements> tableOperations = m_reportOperations.Table();

            if (tableOperations == null)
                return (new List<ReportMeasurements>());
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }


        [RecordOperation(typeof(ReportMeasurements), RecordOperation.CreateNewRecord)]
        public ReportMeasurements NewReportMeasurement()
        {
            //This is not really allowed need to check if we can disable that
            return m_reportOperations.Table().NewRecord();
        }

        /// <summary>
        /// Set selected Report Sources.
        /// </summary>
        /// <param name="ReportType">Type of the report requested <see cref="ReportType"/>.</param>
        /// <param name="number">Depth of the report (0 is all).</param>
        /// <param name="start">Start time of the report.</param>
        /// <param name="end">End time of the report.</param>
        /// <returns> Flag to ensure this is completed before creating a report.</returns>
        public bool SetReportingSource(int ReportType, int ReportCriteria, int number, DateTime start, DateTime end)
        {
            this.m_reportOperations.UpdateReportSource(start, end, (ReportCriteria)ReportCriteria, (ReportType)ReportType, number, DataContext);
            return true;
        }

        /// <summary>
        /// Set selected Report instance name.
        /// </summary>
        /// <param name="instanceName">Instance name that is selected by user.</param>
        public void SetSelectedReportInstanceName(string instanceName)
        {
            m_reportOperations.SetSelectedInstanceName(instanceName);
        }

        /// <summary>
        /// Gets selected Report instance name.
        /// </summary>
        /// <returns>Selected Reportinstance name.</returns>
        public string GetSelectedReportInstanceName()
        {
            return m_reportOperations.GetSelectedInstanceName();
        }

        /// <summary>
        /// Gets loaded historian adapter instance names to Report from.
        /// </summary>
        /// <returns>Report Historian adapter instance names.</returns>
        public IEnumerable<string> GetReportInstanceNames() => m_reportOperations.GetInstanceNames();

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

        private int ModbusProtocolID => s_modbusProtocolID != 0 ? s_modbusProtocolID : (s_modbusProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Modbus'"));

        private int ComtradeProtocolID => s_comtradeProtocolID != 0 ? s_comtradeProtocolID : (s_comtradeProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='COMTRADE'"));

        private int IeeeC37_118ID => s_ieeeC37_118ID != 0 ? s_ieeeC37_118ID : (s_ieeeC37_118ID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='IeeeC37_118V1'"));

        //private int VphmSignalTypeID => s_vphmSignalTypeID != 0 ? s_vphmSignalTypeID : (s_vphmSignalTypeID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM SignalType WHERE Acronym='VPHM'"));

        /// <summary>
        /// Gets protocol ID for "ModbusPoller" protocol.
        /// </summary>
        public int GetModbusProtocolID() => ModbusProtocolID;

        /// <summary>
        /// Gets protocol ID for "COMTRADE" protocol.
        /// </summary>
        public int GetComtradeProtocolID() => ComtradeProtocolID;

        [RecordOperation(typeof(Device), RecordOperation.QueryRecordCount)]
        public int QueryDeviceCount(Guid nodeID, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0}", nodeID) +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Device), RecordOperation.QueryRecords)]
        public IEnumerable<Device> QueryDevices(Guid nodeID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0}", nodeID) +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public IEnumerable<Device> QueryEnabledDevices(Guid nodeID, int limit, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0}", nodeID) +
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
            // TODO: Delete associated custom action adapters (generated by tag templates)
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

            // TODO: Update name, if changed, of associated custom action adapters (generated by tag templates)
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void AddNewOrUpdateDevice(Device device)
        {
            DataContext.Table<Device>().AddNewOrUpdateRecord(device);
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
            return DataContext.Table<Measurement>().QueryRecords(sortField, ascending, page, pageSize, filterText);
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

        #region [ Phasor Table Operations ]

        [RecordOperation(typeof(Phasor), RecordOperation.QueryRecordCount)]
        public int QueryPhasorCount(string filterText)
        {
            return DataContext.Table<Phasor>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Phasor), RecordOperation.QueryRecords)]
        public IEnumerable<Phasor> QueryPhasors(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Phasor>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Phasor), RecordOperation.DeleteRecord)]
        public void DeletePhasor(int id)
        {
            DataContext.Table<Phasor>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Phasor), RecordOperation.CreateNewRecord)]
        public Phasor NewPhasor()
        {
            return DataContext.Table<Phasor>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Phasor), RecordOperation.AddNewRecord)]
        public void AddNewPhasor(Phasor phasor)
        {
            DataContext.Table<Phasor>().AddNewRecord(phasor);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Phasor), RecordOperation.UpdateRecord)]
        public void UpdatePhasor(Phasor phasor)
        {
            DataContext.Table<Phasor>().UpdateRecord(phasor);
        }

        public Phasor QueryPhasorForDevice(int deviceID, int sourceIndex)
        {
            return DataContext.Table<Phasor>().QueryRecordWhere("DeviceID = {0} AND SourceIndex = {1}", deviceID, sourceIndex) ?? NewPhasor();
        }

        public IEnumerable<Phasor> QueryPhasorsForDevice(int deviceID)
        {
            return DataContext.Table<Phasor>().QueryRecordsWhere("DeviceID = {0}", deviceID).OrderBy(phasor => phasor.SourceIndex);
        }

        public int QueryPhasorCountForDevice(int deviceID)
        {
            return DataContext.Connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Phasor WHERE DeviceID = {0}", deviceID);
        }

        public int DeletePhasorsForDevice(int deviceID)
        {
            return DataContext.Connection.ExecuteScalar<int>("DELETE FROM Phasor WHERE DeviceID = {0}", deviceID);
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
            return DataContext.Table<CustomActionAdapter>().QueryRecords(sortField, ascending, page, pageSize, filterText);
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

        #region [ Historian Table Operations ]

        [RecordOperation(typeof(Historian), RecordOperation.QueryRecordCount)]
        public int QueryHistorianCount(string filterText)
        {
            return DataContext.Table<Historian>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Historian), RecordOperation.QueryRecords)]
        public IEnumerable<Historian> QueryHistorians(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Historian>().QueryRecords(sortField, ascending, page, pageSize, filterText);
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
        /// Begins a new historian write operation.
        /// </summary>
        /// <param name="instanceName">Historian instance name.</param>
        /// <param name="values">Enumeration of <see cref="TrendValue"/> instances to write.</param>
        /// <param name="totalValues">Total values to write, if known in advance.</param>
        /// <param name="timestampType">Type of timestamps.</param>
        /// <returns>New operational state handle.</returns>
        public uint BeginHistorianWrite(string instanceName, IEnumerable<TrendValue> values, long totalValues, TimestampType timestampType)
        {
            return m_historianOperations.BeginHistorianWrite(instanceName, values, totalValues, timestampType);
        }

        /// <summary>
        /// Gets current historian write operation state for specified handle.
        /// </summary>
        /// <param name="operationHandle">Handle to historian write operation state.</param>
        /// <returns>Current historian write operation state.</returns>
        public HistorianWriteOperationState GetHistorianWriteState(uint operationHandle)
        {
            return m_historianOperations.GetHistorianWriteState(operationHandle);
        }

        /// <summary>
        /// Cancels a historian write operation.
        /// </summary>
        /// <param name="operationHandle">Handle to historian write operation state.</param>
        /// <returns><c>true</c> if operation was successfully terminated; otherwise, <c>false</c>.</returns>
        public bool CancelHistorianWrite(uint operationHandle)
        {
            return m_historianOperations.CancelHistorianWrite(operationHandle);
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
                return new bool[0];
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
                return new bool[0];
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
                return new ushort[0];
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
                return new ushort[0];
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
            using (Parser parser = new Parser
            {
                Schema = schema,
                InferTimeFromSampleRates = inferTimeFromSampleRates
            })
            {
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
        }

        #endregion

        #region [ Miscellaneous Functions ]

        /// <summary>
        /// Determines if directory exists from server's perspective.
        /// </summary>
        /// <param name="path">Directory path to test for existence.</param>
        /// <returns><c>true</c> if directory exists; otherwise, <c>false</c>.</returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Determines if file exists from server's perspective.
        /// </summary>
        /// <param name="path">Path and file name to test for existence.</param>
        /// <returns><c>true</c> if file exists; otherwise, <c>false</c>.</returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

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
        public Task<string> GetElapsedTimeString(DateTime startTime, DateTime stopTime)
        {
            return Task.Factory.StartNew(() => new Ticks(stopTime - startTime).ToElapsedTimeString(2));
        }

        #endregion
    }
}