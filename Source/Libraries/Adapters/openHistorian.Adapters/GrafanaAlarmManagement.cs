//******************************************************************************************************
//  GrafanaAlarmManagement.cs - Gbtc
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
//  01/26/2020 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

namespace openHistorian.Adapters
{
    /// <summary>
    /// Represents a collection of functions to manage alarms in Grafana and match them to openHistorian alarms.
    /// </summary>
    /// <remarks>
    /// Limit lines are generated in the Grafana data source. This class is only used for the Alert Table panel.
    /// </remarks>
    //public class GrafanaAlarmManagement
    //{
    //    private const string GrafanaAlarmQuery = @"SELECT alert.id, alert.state, tag.value FROM 
    //                                                   alert LEFT JOIN alert_rule_tag ON alert.id = alert_rule_tag.alert_id LEFT JOIN tag ON
    //                                                   tag.id = alert_rule_tag.tag_id
    //                                               WHERE tag.key = 'OpenHistorianID' AND dashboard_id = {0} AND panel_id = {1}";

    //    /// <summary>
    //    /// Checks if the alarms in Grafana match the alarms in openHistorian and updates them if necessary.
    //    /// </summary>
    //    /// <param name="query">The Grafana query including the panelID and dashboardID.</param>
    //    /// <param name="alarms">Relevant alarms in the OpenHistorian.</param>
    //    public static void UpdateAlerts(QueryRequest query, List<GrafanaAlarm> alarms)
    //    {
    //        // Start by getting the Alarms that are in the Grafana instance
    //        using (AdoDataConnection connection = GetGrafanaDB())
    //        {
    //            DataTable grafanaAlarms = connection.RetrieveData(GrafanaAlarmQuery, query.dashboardId, query.panelId);

    //            List<DataRow> alarmsToUpdate = grafanaAlarms.AsEnumerable()
    //                .Where(item => CheckUpdate(alarms, item))
    //                .ToList();

    //            // TODO: Should these alarms be removed? Result currently unused.
    //            HashSet<int> alarmIDs = new HashSet<int>(alarms.Select(alarm => alarm.ID));
    //            List<DataRow> removeAlarms = grafanaAlarms.AsEnumerable().Where(row => alarmIDs.Contains(row.Field<int>("value"))).ToList();

    //            HashSet<int> alarmRecordIDs = new HashSet<int>(grafanaAlarms.AsEnumerable().Select(row => row.Field<int>("value")));
    //            List<GrafanaAlarm> alarmsToAdd = alarms.Where(alarm => alarmRecordIDs.Contains(alarm.ID)).ToList();

    //            alarmsToUpdate.ForEach(alarmRecord => Update(alarms, alarmRecord, connection));

    //            alarmsToAdd.ForEach(alarm => Add(alarm, query, connection));
    //        }
    //    }

    //    private static bool CheckUpdate(List<GrafanaAlarm> alarms, DataRow alarmRecord)
    //    {
    //        int index = alarms.FindIndex(item => item.ID == Convert.ToInt32(alarmRecord["value"]));

    //        if (index == -1)
    //            return false;

    //        Alarm alarmState = AlarmAdapter.Default?.GetAlarmStatus(alarms[index].SignalID)?.FirstOrDefault(item => item.ID == alarms[index].ID);

    //        if (alarmState is null || alarmRecord.Field<string>("state") == "unknown")
    //            return false;

    //        if (alarmState.State == GSF.TimeSeries.AlarmState.Cleared && alarmRecord.Field<string>("state") == "ok")
    //            return false;

    //        if (alarmState.State == GSF.TimeSeries.AlarmState.Raised && alarmRecord.Field<string>("state") == "alerting")
    //            return false;

    //        return true;
    //    }

    //    private static void Update(List<GrafanaAlarm> alarms, DataRow alarmRecord, AdoDataConnection connection)
    //    {
    //        int index = alarms.FindIndex(item => item.ID == alarmRecord.Field<int>("value"));
    //        string recordState = "unknown";

    //        if (index == -1)
    //        {
    //            connection.ExecuteNonQuery("UPDATE alert SET state = {0} WHERE ID = {1}", recordState, alarmRecord.Field<int>("id"));
    //            return;
    //        }

    //        Alarm alarmState = AlarmAdapter.Default?.GetAlarmStatus(alarms[index].SignalID)?.FirstOrDefault(item => item.ID == alarms[index].ID);

    //        if (alarmState != null)
    //        {
    //            if (alarmState.State == GSF.TimeSeries.AlarmState.Cleared)
    //                recordState = "ok";
    //            else if (alarmState.State == GSF.TimeSeries.AlarmState.Raised)
    //                recordState = "alerting";
    //        }

    //        connection.ExecuteNonQuery("UPDATE alert SET state = {0} WHERE ID = {1}", recordState, alarmRecord.Field<int>("id"));
    //    }

    //    private static void Add(GrafanaAlarm alarm, QueryRequest query, AdoDataConnection connection)
    //    {
    //        const string insertSQL = "INSERT INTO alert (dashboard_id, version, org_id, panel_id, name,state, new_state_date, message, settings, frequency, handler, severity, silenced, execution_error, state_changes, created, updated) VALUES " +
    //                                 "({0}, 0, 1, {1}, {2}, {3}, {4}, '', '', 60, 0, 0, 0, '', 0, {5}, {6})";

    //        // Start by adding the Alarm
    //        Alarm alarmState = AlarmAdapter.Default?.GetAlarmStatus(alarm.SignalID)?.FirstOrDefault(item => item.ID == alarm.ID);

    //        if (alarmState is null)
    //            return;

    //        string recordState = "ok";

    //        if (alarmState.State == GSF.TimeSeries.AlarmState.Raised)
    //            recordState = "alerting";

    //        connection.ExecuteNonQuery(insertSQL, query.dashboardId, query.panelId, alarm.Description, recordState, alarmState.TimeRaised, alarm.CreatedOn, DateTime.UtcNow);

    //        // Note: the 'last_insert_rowid' function is specific to SQLite
    //        int alarmID = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");

    //        // Check if Tag for this alarm already Exists
    //        int tagID = connection.ExecuteScalar<int>("SELECT id FROM tag WHERE key = 'OpenHistorianID' AND value = {0}", alarm.ID);

    //        if (tagID == 0)
    //        {
    //            connection.ExecuteNonQuery("INSERT INTO tag (key, value) VALUES ('OpenHistorianID', {0})", alarm.ID);
    //            tagID = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
    //        }

    //        connection.ExecuteNonQuery("INSERT INTO alert_rule_tag (alert_id, tag_id) VALUES ({0}, {1})", alarmID, tagID);
    //    }

    //    private static AdoDataConnection GetGrafanaDB()
    //    {
    //        const string defaultGrafanaDB = ".\\Grafana\\data\\grafana.db";
    //        const string dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";

    //        CategorizedSettingsElementCollection alarmSettings = ConfigurationFile.Current.Settings["OHGrafanaAlarmService"];

    //        alarmSettings.Add("GrafanaDB", defaultGrafanaDB, "Path to the Grafana database file used for adding alarms from the openHistorian");
    //        string grafanaDB = alarmSettings["GrafanaDB"].ValueAs(defaultGrafanaDB);

    //        return new AdoDataConnection($"Data Source={grafanaDB}; Version=3; Foreign Keys=True; FailIfMissing=True", dataProviderString);
    //    }
    //}
}