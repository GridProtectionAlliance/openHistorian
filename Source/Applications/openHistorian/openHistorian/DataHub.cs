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
using GSF.IO;
using GSF.PhasorProtocols;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using ModbusAdapters;
using ModbusAdapters.Model;
using openHistorian.Adapters;
using openHistorian.Model;
using PhasorProtocolAdapters;
using PowerCalculations.PowerMultiCalculator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Measurement = openHistorian.Model.Measurement;
using PowerCalculation = openHistorian.Adapters.PowerCalculation;
using SignalType = openHistorian.Model.SignalType;

namespace openHistorian
{
    [AuthorizeHubRole]
    public class DataHub : RecordOperationsHub<DataHub>, IHistorianOperations, IDataSubscriptionOperations, IDirectoryBrowserOperations, IModbusOperations
    {
        #region [ Members ]

        // Fields
        private readonly HistorianOperations m_historianOperations;
        private readonly DataSubscriptionOperations m_dataSubscriptionOperations;
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
            DataContext.Table<Device>().DeleteRecord(id);
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

        #region [ PowerCalculation Table Operations ]

        [RecordOperation(typeof(PowerCalculation), RecordOperation.QueryRecordCount)]
        public int QueryPowerCalculationCount(string filterText)
        {
            return DataContext.Table<PowerCalculation>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(PowerCalculation), RecordOperation.QueryRecords)]
        public IEnumerable<PowerCalculation> QueryPowerCalculations(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<PowerCalculation>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(PowerCalculation), RecordOperation.DeleteRecord)]
        public void DeletePowerCalculation(int id)
        {
            DataContext.Table<PowerCalculation>().DeleteRecord(id);
        }

        [RecordOperation(typeof(PowerCalculation), RecordOperation.CreateNewRecord)]
        public PowerCalculation NewPowerCalculation()
        {
            return DataContext.Table<PowerCalculation>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(PowerCalculation), RecordOperation.AddNewRecord)]
        public void AddNewPowerCalculation(PowerCalculation powerCalculation)
        {
            DataContext.Table<PowerCalculation>().AddNewRecord(powerCalculation);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(PowerCalculation), RecordOperation.UpdateRecord)]
        public void UpdatePowerCalculation(PowerCalculation powerCalculation)
        {
            DataContext.Table<PowerCalculation>().UpdateRecord(powerCalculation);
        }

        public PowerCalculation QueryPowerCalculationForDescriptionOrInputs(string circuitDescription, Guid voltageAngleSignalID, Guid voltageMagSignalID, Guid currentAngleSignalID, Guid currentMagSignalID)
        {
            return DataContext.Table<PowerCalculation>().QueryRecordWhere($"CircuitDescription LIKE '{circuitDescription}%' OR (VoltageAngleSignalID = {{0}} AND VoltageMagSignalID = {{1}} AND CurrentAngleSignalID = {{2}} AND CurrentMagSignalID = {{3}})", voltageAngleSignalID, voltageMagSignalID, currentAngleSignalID, currentMagSignalID) ?? NewPowerCalculation();
        }

        public PowerCalculation QueryPowerCalculationForInputs(string circuitDescription, Guid voltageAngleSignalID, Guid voltageMagSignalID, Guid currentAngleSignalID, Guid currentMagSignalID)
        {
            return DataContext.Table<PowerCalculation>().QueryRecordWhere("VoltageAngleSignalID = {0} AND VoltageMagSignalID = {1} AND CurrentAngleSignalID = {2} AND CurrentMagSignalID = {3}", voltageAngleSignalID, voltageMagSignalID, currentAngleSignalID, currentMagSignalID) ?? NewPowerCalculation();
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
        /// Set selected instance name.
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

        #region [ Synchrophasor Device Wizard Operations ]

        public IEnumerable<SignalType> LoadSignalTypes(string source)
        {
            return DataContext.Table<SignalType>().QueryRecordsWhere("Source = {0}", source);
        }

        public string CreatePointTag(string deviceAcronym, string signalTypeAcronym)
        {
            return CommonPhasorServices.CreatePointTag(Program.Host.Model.Global.CompanyAcronym, deviceAcronym, null, signalTypeAcronym);
        }

        public string CreateIndexedPointTag(string deviceAcronym, string signalTypeAcronym, int signalIndex)
        {
            return CommonPhasorServices.CreatePointTag(Program.Host.Model.Global.CompanyAcronym, deviceAcronym, null, signalTypeAcronym, null, signalIndex);
        }

        public string CreatePhasorPointTag(string deviceAcronym, string signalTypeAcronym, string phasorLabel, string phase, int signalIndex, int baseKV)
        {
            return CommonPhasorServices.CreatePointTag(Program.Host.Model.Global.CompanyAcronym, deviceAcronym, null, signalTypeAcronym, phasorLabel, signalIndex, string.IsNullOrWhiteSpace(phase) ? '_' : phase.Trim()[0], baseKV);
        }

        #region [ Old Code ]

        // Saving BaseKV into its own Phasor table field now...

        //public void SaveNominalVoltage(string signalReference, int? nominalVoltage)
        //{
        //    if (string.IsNullOrWhiteSpace(signalReference) || nominalVoltage == null)
        //        return;

        //    Measurement voltageMagnitude = QueryMeasurement(signalReference);

        //    if (voltageMagnitude.PointID == 0)
        //        return;

        //    // Nominal voltage is stored along with the VPHM measurement that is associated with the active power output measurement. Since this
        //    // value is locally calculated, the metadata for this point is "owned" locally and will not be overwritten by GEP synchronizations
        //    PowerCalculation powerCalculation = DataContext.Table<PowerCalculation>().QueryRecordWhere("VoltageMagSignalID = {0}", voltageMagnitude.SignalID);

        //    if (powerCalculation == null)
        //        return;

        //    Measurement activePower = QueryMeasurementBySignalID(powerCalculation.ActivePowerOutputSignalID.GetValueOrDefault());

        //    if (activePower.PointID == 0)
        //        return;

        //    string description = activePower.Description ?? "";
        //    Dictionary<string, string> settings;

        //    // Look for possible settings beyond last " -- " marker in description:
        //    int index = description.LastIndexOf(" -- ", StringComparison.Ordinal);

        //    if (index > -1)
        //    {
        //        try
        //        {
        //            description = description.Substring(index + 4);
        //            settings = description.ParseKeyValuePairs();
        //        }
        //        catch
        //        {
        //            settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //        }
        //    }
        //    else
        //    {
        //        settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //    }

        //    settings["nominalVoltage"] = nominalVoltage.ToString();
        //    activePower.Description = $"{description} -- {settings.JoinKeyValuePairs()}";

        //    UpdateMeasurement(activePower);
        //}

        //private int? LookupNominalVoltage(Phasor phasor)
        //{
        //    if (phasor.BaseKV > 0)
        //        return phasor.BaseKV;

        //    if (phasor.Type != 'V')
        //        return null;

        //    Measurement voltageMagnitude = DataContext.Table<Measurement>().QueryRecordWhere("DeviceID = {0} AND PhasorSourceIndex = {1} AND SignalTypeID = {2}", phasor.DeviceID, phasor.SourceIndex, VphmSignalTypeID);

        //    if (voltageMagnitude == null)
        //        return null;

        //    // Nominal voltage is stored along with the VPHM measurement that is associated with the active power output measurement. Since this
        //    // value is locally calculated, the metadata for this point is "owned" locally and will not be overwritten by GEP synchronizations
        //    PowerCalculation powerCalculation = DataContext.Table<PowerCalculation>().QueryRecordWhere("VoltageMagSignalID = {0}", voltageMagnitude.SignalID);

        //    if (powerCalculation == null)
        //        return null;

        //    Measurement activePower = QueryMeasurementBySignalID(powerCalculation.ActivePowerOutputSignalID.GetValueOrDefault());

        //    if (activePower.PointID == 0)
        //        return null;

        //    string description = activePower.Description;

        //    if (string.IsNullOrWhiteSpace(description))
        //        return null;

        //    // Look for possible settings beyond last " -- " marker in description:
        //    int index = description.LastIndexOf(" -- ", StringComparison.Ordinal);

        //    if (index < 0)
        //        return null;

        //    try
        //    {
        //        Dictionary<string, string> settings = description.Substring(index + 4).ParseKeyValuePairs();

        //        if (settings.TryGetValue("nominalVoltage", out string setting) && int.TryParse(setting, out int nominalVoltage))
        //            return nominalVoltage;
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        #endregion

        public ConfigurationFrame ExtractConfigurationFrame(int deviceID)
        {
            Device device = QueryDeviceByID(deviceID);

            if (device.ID == 0)
                return new ConfigurationFrame();

            ConfigurationFrame derivedFrame = new ConfigurationFrame
            {
                IDCode = (ushort)device.AccessID,
                StationName = device.Name,
                IDLabel = device.Acronym,
                FrameRate = (ushort)(device.FramesPerSecond ?? Program.Host.Model.Global.DefaultCalculationFramesPerSecond),
                ConnectionString = device.ConnectionString,
                ProtocolID = device.ProtocolID ?? IeeeC37_118ID
            };

            if (device.ParentID == null)
            {
                IEnumerable<Device> devices = QueryChildDevices(deviceID);

                foreach (Device childDevice in devices)
                {
                    // Create new configuration cell
                    ConfigurationCell derivedCell = new ConfigurationCell
                    {
                        ID = childDevice.ID,
                        ParentID = device.ID,
                        IDCode = (ushort)childDevice.AccessID,
                        StationName = childDevice.Name,
                        IDLabel = childDevice.Acronym
                    };

                    derivedCell.FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" };

                    // Extract phasor definitions
                    foreach (Phasor phasor in QueryPhasorsForDevice(childDevice.ID))
                        derivedCell.PhasorDefinitions.Add(new PhasorDefinition { ID = phasor.ID, Label = phasor.Label, PhasorType = phasor.Type == 'V' ? "Voltage" : "Current", Phase = phasor.Phase.ToString(), DestinationPhasorID = phasor.DestinationPhasorID, NominalVoltage = phasor.BaseKV, SourceIndex = phasor.SourceIndex });

                    // Add cell to frame
                    derivedFrame.Cells.Add(derivedCell);
                }

                if (derivedFrame.Cells.Count > 0)
                {
                    derivedFrame.IsConcentrator = true;
                }
                else
                {
                    // This is a directly connected device
                    derivedFrame.IsConcentrator = false;

                    ConfigurationCell derivedCell = new ConfigurationCell
                    {
                        ID = device.ID,
                        ParentID = null,
                        IDCode = derivedFrame.IDCode,
                        StationName = device.Name,
                        IDLabel = device.Acronym
                    };

                    derivedCell.FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" };

                    // Extract phasor definitions
                    foreach (Phasor phasor in QueryPhasorsForDevice(device.ID))
                        derivedCell.PhasorDefinitions.Add(new PhasorDefinition { ID = phasor.ID, Label = phasor.Label, PhasorType = phasor.Type == 'V' ? "Voltage" : "Current", Phase = phasor.Phase.ToString(), DestinationPhasorID = phasor.DestinationPhasorID, NominalVoltage = phasor.BaseKV, SourceIndex = phasor.SourceIndex });

                    // Add cell to frame
                    derivedFrame.Cells.Add(derivedCell);
                }
            }
            else
            {
                derivedFrame.IsConcentrator = true;

                // Create new configuration cell
                ConfigurationCell derivedCell = new ConfigurationCell
                {
                    ID = device.ID,
                    ParentID = null,
                    IDCode = (ushort)device.AccessID,
                    StationName = device.Name,
                    IDLabel = device.Acronym
                };

                derivedCell.FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" };

                // Extract phasor definitions
                foreach (Phasor phasor in QueryPhasorsForDevice(device.ID))
                    derivedCell.PhasorDefinitions.Add(new PhasorDefinition { ID = phasor.ID, Label = phasor.Label, PhasorType = phasor.Type == 'V' ? "Voltage" : "Current", Phase = phasor.Phase.ToString(), DestinationPhasorID = phasor.DestinationPhasorID, NominalVoltage = phasor.BaseKV, SourceIndex = phasor.SourceIndex });

                // Add cell to frame
                derivedFrame.Cells.Add(derivedCell);
            }

            return derivedFrame;
        }

        public ConfigurationFrame LoadConfigurationFrame(string sourceData)
        {
            string connectionString = "";

            IConfigurationFrame GetConfigurationFrame()
            {
                try
                {
                    ConnectionSettings connectionSettings;
                    SoapFormatter formatter = new SoapFormatter
                    {
                        AssemblyFormat = FormatterAssemblyStyle.Simple,
                        TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
                        Binder = Serialization.LegacyBinder
                    };

                    using (MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes(sourceData)))
                        connectionSettings = formatter.Deserialize(source) as ConnectionSettings;

                    if ((object)connectionSettings != null)
                    {
                        connectionString = connectionSettings.ConnectionString;

                        Dictionary<string, string> connectionStringKeyValues = connectionString.ParseKeyValuePairs();

                        connectionString = "transportProtocol=" + connectionSettings.TransportProtocol + ";" + connectionStringKeyValues.JoinKeyValuePairs();

                        if ((object)connectionSettings.ConnectionParameters != null)
                        {
                            switch (connectionSettings.PhasorProtocol)
                            {
                                case PhasorProtocol.BPAPDCstream:
                                    GSF.PhasorProtocols.BPAPDCstream.ConnectionParameters bpaParameters = connectionSettings.ConnectionParameters as GSF.PhasorProtocols.BPAPDCstream.ConnectionParameters;
                                    if ((object)bpaParameters != null)
                                        connectionString += "; iniFileName=" + bpaParameters.ConfigurationFileName + "; refreshConfigFileOnChange=" + bpaParameters.RefreshConfigurationFileOnChange + "; parseWordCountFromByte=" + bpaParameters.ParseWordCountFromByte;
                                    break;
                                case PhasorProtocol.FNET:
                                    GSF.PhasorProtocols.FNET.ConnectionParameters fnetParameters = connectionSettings.ConnectionParameters as GSF.PhasorProtocols.FNET.ConnectionParameters;
                                    if ((object)fnetParameters != null)
                                        connectionString += "; timeOffset=" + fnetParameters.TimeOffset + "; stationName=" + fnetParameters.StationName + "; frameRate=" + fnetParameters.FrameRate + "; nominalFrequency=" + (int)fnetParameters.NominalFrequency;
                                    break;
                                case PhasorProtocol.SelFastMessage:
                                    GSF.PhasorProtocols.SelFastMessage.ConnectionParameters selParameters = connectionSettings.ConnectionParameters as GSF.PhasorProtocols.SelFastMessage.ConnectionParameters;
                                    if ((object)selParameters != null)
                                        connectionString += "; messagePeriod=" + selParameters.MessagePeriod;
                                    break;
                                case PhasorProtocol.IEC61850_90_5:
                                    GSF.PhasorProtocols.IEC61850_90_5.ConnectionParameters iecParameters = connectionSettings.ConnectionParameters as GSF.PhasorProtocols.IEC61850_90_5.ConnectionParameters;
                                    if ((object)iecParameters != null)
                                        connectionString += "; useETRConfiguration=" + iecParameters.UseETRConfiguration + "; guessConfiguration=" + iecParameters.GuessConfiguration + "; parseRedundantASDUs=" + iecParameters.ParseRedundantASDUs + "; ignoreSignatureValidationFailures=" + iecParameters.IgnoreSignatureValidationFailures + "; ignoreSampleSizeValidationFailures=" + iecParameters.IgnoreSampleSizeValidationFailures;
                                    break;
                                case PhasorProtocol.Macrodyne:
                                    GSF.PhasorProtocols.Macrodyne.ConnectionParameters macrodyneParameters = connectionSettings.ConnectionParameters as GSF.PhasorProtocols.Macrodyne.ConnectionParameters;
                                    if ((object)macrodyneParameters != null)
                                        connectionString += "; protocolVersion=" + macrodyneParameters.ProtocolVersion + "; iniFileName=" + macrodyneParameters.ConfigurationFileName + "; refreshConfigFileOnChange=" + macrodyneParameters.RefreshConfigurationFileOnChange + "; deviceLabel=" + macrodyneParameters.DeviceLabel;
                                    break;
                            }
                        }

                        connectionString += "; accessID=" + connectionSettings.PmuID;
                        connectionString += "; phasorProtocol=" + connectionSettings.PhasorProtocol;

                        using (CommonPhasorServices phasorServices = new CommonPhasorServices())
                        {
                            phasorServices.StatusMessage += (sender, e) => LogStatusMessage(e.Argument.Replace("**", ""));
                            phasorServices.ProcessException += (sender, e) => LogException(e.Argument);
                            return phasorServices.RequestDeviceConfiguration(connectionString);
                        }
                    }

                    using (MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes(sourceData)))
                        return formatter.Deserialize(source) as IConfigurationFrame;
                }
                catch
                {
                    return new ConfigurationErrorFrame();
                }
            }

            IConfigurationFrame sourceFrame = GetConfigurationFrame();

            if (sourceFrame is ConfigurationErrorFrame)
                return new ConfigurationFrame();

            ConfigurationFrame derivedFrame;

            // Create a new simple concrete configuration frame for JSON serialization converted from equivalent configuration information
            int protocolID = 0, deviceID = 0, phasorID = -1; // Start phasor ID's at less than -1 since associated voltage == -1 is reserved as unselected
        
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
                protocolID = DataContext.Table<Protocol>().QueryRecordWhere("Acronym = {0}", settings["phasorProtocol"]).ID;
            }

            derivedFrame = new ConfigurationFrame
            {
                IDCode = sourceFrame.IDCode,
                FrameRate = sourceFrame.FrameRate,
                ConnectionString = connectionString,
                ProtocolID = protocolID
            };

            foreach (IConfigurationCell sourceCell in sourceFrame.Cells)
            {
                // Create new derived configuration cell
                ConfigurationCell derivedCell = new ConfigurationCell
                {
                    ID = --deviceID, // Provide a negative index so any database lookup will return null
                    ParentID = null,
                    IDCode = sourceCell.IDCode,
                    StationName = sourceCell.StationName,
                    IDLabel = sourceCell.IDLabel
                };

                // Create equivalent derived frequency definition
                IFrequencyDefinition sourceFrequency = sourceCell.FrequencyDefinition;

                if (sourceFrequency != null)
                    derivedCell.FrequencyDefinition = new FrequencyDefinition { Label = sourceFrequency.Label };

                int sourceIndex = 0;

                // Create equivalent derived phasor definitions
                foreach (IPhasorDefinition sourcePhasor in sourceCell.PhasorDefinitions)
                    derivedCell.PhasorDefinitions.Add(new PhasorDefinition { ID = --phasorID, Label = sourcePhasor.Label, PhasorType = sourcePhasor.PhasorType.ToString(), SourceIndex = ++sourceIndex });

                // Create equivalent derived analog definitions (assuming analog type = SinglePointOnWave)
                foreach (IAnalogDefinition sourceAnalog in sourceCell.AnalogDefinitions)
                    derivedCell.AnalogDefinitions.Add(new AnalogDefinition { Label = sourceAnalog.Label, AnalogType = sourceAnalog.AnalogType.ToString() });

                // Create equivalent derived digital definitions
                foreach (IDigitalDefinition sourceDigital in sourceCell.DigitalDefinitions)
                    derivedCell.DigitalDefinitions.Add(new DigitalDefinition { Label = sourceDigital.Label });

                // Add cell to frame
                derivedFrame.Cells.Add(derivedCell);
            }

            derivedFrame.IsConcentrator = derivedFrame.Cells.Count > 1;

            return derivedFrame;
        }

        public IEnumerable<string> GetTemplateTypes()
        {
            List<string> templateTypes = new List<string>(FilePath.GetFileList(FilePath.GetAbsolutePath("*.TagTemplate")).Select(FilePath.GetFileNameWithoutExtension));
            templateTypes.Insert(0, "None");
            return templateTypes;
        }

        public IEnumerable<TagTemplate> LoadTemplate(string templateType)
        {
            List<TagTemplate> tagTemplates = new List<TagTemplate>();
 
            foreach (string line in File.ReadLines(FilePath.GetAbsolutePath($"{templateType}.TagTemplate")))
            {
                // Skip comment lines
                if (line.TrimStart().StartsWith("#"))
                    continue;

                string[] parts = line.Split('\t');

                if (parts.Length == 5)
                {
                    tagTemplates.Add(new TagTemplate
                    {
                        TagName = parts[0].Trim(),
                        Inputs = parts[1].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(input => input.Trim()).ToArray(),
                        Equation = parts[2].Trim(),
                        Type = parts[3].Trim(),
                        Description = parts[4].Trim()
                    });
                }
            }

            return tagTemplates;
        }

        public string GetProtocolCategory(int protocolID)
        {
            return DataContext.Table<Protocol>().QueryRecordWhere("ID = {0}", protocolID).Category;
        }

        public void ValidateCalculatorConfigurations(int? historianID)
        {
            const int Avg = 0, Max = 1, Min = 2;
            PowerCalculationConfigurationValidation.ValidateDatabaseDefinitions();        
            TableOperations<Measurement> measurementTable = DataContext.Table<Measurement>();

            // Look for existing frequency average
            if (measurementTable.QueryRecordCountWhere("SignalReference = 'SYSTEM!FREQ-AVG-FQ'") > 0)
                return;

            TableOperations<CustomActionAdapter> customActionAdapterTable = DataContext.Table<CustomActionAdapter>();
            CustomActionAdapter avgFreqAdapter = customActionAdapterTable.QueryRecordWhere("TypeName = {0}", typeof(PowerCalculations.AverageFrequency).FullName) ?? NewCustomActionAdapter();
            Measurement[] measurements = GetCalculatedFrequencyMeasurements(historianID);

            double lagTime = Program.Host.Model.Global.DefaultCalculationLagTime;

            // Reduce lag-time since dynamic calculations can depend on average frequency
            lagTime -= lagTime > 1.0 ? 1.0 : 0.5;

            if (lagTime < 0.1)
                lagTime = 0.1;

            avgFreqAdapter.AdapterName = "PHASOR!AVERAGEFREQ";
            avgFreqAdapter.AssemblyName = "PowerCalculations.dll";
            avgFreqAdapter.TypeName = typeof(PowerCalculations.AverageFrequency).FullName;
            avgFreqAdapter.NodeID = Program.Host.Model.Global.NodeID;
            avgFreqAdapter.ConnectionString = $"InputMeasurementKeys={{FILTER ActiveMeasurements WHERE SignalType = 'FREQ'}}; OutputMeasurements={{{measurements[Avg].SignalID};{measurements[Max].SignalID};{measurements[Min].SignalID}}}; LagTime={lagTime}; LeadTime={Program.Host.Model.Global.DefaultCalculationLeadTime}; FramesPerSecond={Program.Host.Model.Global.DefaultCalculationFramesPerSecond}";
            avgFreqAdapter.Enabled = true;

            customActionAdapterTable.AddNewOrUpdateRecord(avgFreqAdapter);            
        }

        private Measurement[] GetCalculatedFrequencyMeasurements(int? historianID)
        {
            SignalType freqSignalType = DataContext.Table<SignalType>().QueryRecordWhere("Acronym = 'FREQ'");

            if (freqSignalType.ID == 0)
                throw new InvalidOperationException("Failed to find 'FREQ' signal type");

            Measurement avgFreqMeasurement = NewMeasurement();
            Measurement maxFreqMeasurement = NewMeasurement();
            Measurement minFreqMeasurement = NewMeasurement();

            avgFreqMeasurement.PointTag = "SYSTEM!FREQ-AVG-FQ";
            avgFreqMeasurement.SignalReference = "SYSTEM!FREQ-AVG-FQ";
            avgFreqMeasurement.SignalTypeID = freqSignalType.ID;
            avgFreqMeasurement.HistorianID = historianID;
            avgFreqMeasurement.Description = "Average System Frequency";
            avgFreqMeasurement.Internal = true;
            avgFreqMeasurement.Enabled = true;

            maxFreqMeasurement.PointTag = "SYSTEM!FREQ-MAX-FQ";
            maxFreqMeasurement.SignalReference = "SYSTEM!FREQ-MAX-FQ";
            maxFreqMeasurement.SignalTypeID = freqSignalType.ID;
            maxFreqMeasurement.HistorianID = historianID;
            maxFreqMeasurement.Description = "Maximum System Frequency";
            maxFreqMeasurement.Internal = true;
            maxFreqMeasurement.Enabled = true;

            minFreqMeasurement.PointTag = "SYSTEM!FREQ-MIN-FQ";
            minFreqMeasurement.SignalReference = "SYSTEM!FREQ-MIN-FQ";
            minFreqMeasurement.SignalTypeID = freqSignalType.ID;
            minFreqMeasurement.HistorianID = historianID;
            minFreqMeasurement.Description = "Minimum System Frequency";
            minFreqMeasurement.Internal = true;
            minFreqMeasurement.Enabled = true;

            AddNewOrUpdateMeasurement(avgFreqMeasurement);
            AddNewOrUpdateMeasurement(maxFreqMeasurement);
            AddNewOrUpdateMeasurement(minFreqMeasurement);

            Measurement[] measurements = new Measurement[3];

            measurements[0] = QueryMeasurement("SYSTEM!FREQ-AVG-FQ");
            measurements[1] = QueryMeasurement("SYSTEM!FREQ-MAX-FQ");
            measurements[2] = QueryMeasurement("SYSTEM!FREQ-MIN-FQ");

            return measurements;
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