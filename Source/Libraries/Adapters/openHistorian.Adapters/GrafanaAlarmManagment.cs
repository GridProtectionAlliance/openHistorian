//******************************************************************************************************
//  GrafanaAlarmManagment.cs - Gbtc
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
//  09/15/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GrafanaAdapters;
using GSF;
using GSF.Configuration;
using GSF.Data;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using DataQualityMonitoring;
using GSF.TimeSeries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openHistorian.Snap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using CancellationToken = System.Threading.CancellationToken;
using GSF.TimeSeries.Adapters;

namespace openHistorian.Adapters
{
    


    /// <summary>
    /// Represents a Collection of Static Functions to manage Alarms in Grafana and matching them to the OH Alarms.
    /// Note that the limit lines are generated in the Grafana datasource. This is only for the Alert Table panel
    /// </summary>
    public class GrafanaAlarmManagment
    {
        #region [ Members ]

        const string GrafanaAlarmQuery = @" SELECT alert.id, alert.state, tag.value  FROM 
                                                alert LEFT JOIN alert_rule_tag ON alert.id = alert_rule_tag.alert_id LEFT JOIN
                                                tag ON tag.id = alert_rule_tag.tag_id
                                            WHERE tag.key = 'OpenHistorianID' AND dashboard_id = {0} AND panel_id = {1}";




        #endregion


        #region [Methods]
        /// <summary>
        /// This Function checks if the Alarms in Grafana match the alarms in the OH and updates them if neccesarry
        /// </summary>
        /// <param name="query"> The Grafana Query including the panelID and DashboardID </param>
        /// <param name="OHalarms"> List of relevant alarms in the OpenHistorian </param>
        public static void UpdateAlerts(QueryRequest query, List<GrafanaAlarm> OHalarms)
        {
            // Start by getting the Alarms that are in the Grafana Instrance
            using (AdoDataConnection connection = GetGrafanaDB())
            {
                DataTable grafanaAlarms = connection.RetrieveData(GrafanaAlarmQuery, query.dashboardId, query.panelId);

                List<DataRow> updateAlarms = grafanaAlarms.AsEnumerable()
                    .Where(item => CheckUpdate(OHalarms, item))
                    .ToList();

                List<int> ohIDs = OHalarms.Select(item => item.ID).ToList();

                List<DataRow> removeAlarms = grafanaAlarms.AsEnumerable().Where(item => CheckRemove(item,ohIDs)).ToList();

                List<int>  grafanaOHIDs = grafanaAlarms.AsEnumerable().Select(item => Convert.ToInt32(item["value"])).ToList();

                List<GrafanaAlarm> addAlarms = OHalarms.Where(item => CheckAdd(item, grafanaOHIDs)).ToList();

                updateAlarms.ForEach(item => Update(OHalarms, item, connection));

                addAlarms.ForEach(item => Add(item, query, connection));


            }

         
        }

        private static bool CheckUpdate(List<GrafanaAlarm> OHalarms, DataRow GrafanaAlarm)
        {
            int OHindex = OHalarms.FindIndex(item => item.ID == Convert.ToInt32(GrafanaAlarm["value"]));

            if (OHindex == -1)
                return false;

            Alarm OHstate = AlarmAdapter.Default?.GetAlarmStatus(OHalarms[OHindex].SignalID).Where(item => item.ID == OHalarms[OHindex].ID).FirstOrDefault();

            if (OHstate == null && GrafanaAlarm.Field<string>("state") == "unknown")
                return false;

            if (OHstate.State == GSF.TimeSeries.AlarmState.Cleared && GrafanaAlarm.Field<string>("state") == "ok")
                return false;

            if (OHstate.State == GSF.TimeSeries.AlarmState.Raised && GrafanaAlarm.Field<string>("state") == "alerting")
                return false;

            return true;
        }

        private static bool CheckAdd(GrafanaAlarm OHalarm, List<int> GrafanaOHAlarmIDs)
        {
            int index = GrafanaOHAlarmIDs.FindIndex(item => item == OHalarm.ID);

            if (index == -1)
                return true;
            else
                return false;
        }

        private static bool CheckRemove(DataRow GrafanaAlarm, List<int> OHAlarmIDs)
        {
            int index = OHAlarmIDs.FindIndex(item => item == Convert.ToInt32(GrafanaAlarm.Field<string>("value")));

            if (index == -1)
                return true;
            else
                return false;
        }

        private static void Update(List<GrafanaAlarm> OHalarms, DataRow GrafanaAlarm, AdoDataConnection connection)
        {
            int index = OHalarms.FindIndex(item => item.ID == Convert.ToInt32(GrafanaAlarm["value"]));
            string state = "unknown";
            if (index == -1)
            {
                connection.ExecuteNonQuery("UPDATE alert SET state = {0} WHERE ID = {1}", state, Convert.ToInt32(GrafanaAlarm["id"]));
                return;
            }

            Alarm ohState = AlarmAdapter.Default?.GetAlarmStatus(OHalarms[index].SignalID).Where(item => item.ID == OHalarms[index].ID).FirstOrDefault();

            if (ohState.State == GSF.TimeSeries.AlarmState.Cleared)
                state = "ok";
            else if (ohState.State == GSF.TimeSeries.AlarmState.Raised)
                state = "alerting";

            connection.ExecuteNonQuery("UPDATE alert SET state = {0} WHERE ID = {1}", state, Convert.ToInt32(GrafanaAlarm["id"]));
        }

        private static void Add(GrafanaAlarm alarm, QueryRequest query, AdoDataConnection connection)
        {
            //Start by adding the Alarm
            Alarm ohState = AlarmAdapter.Default?.GetAlarmStatus(alarm.SignalID).Where(item => item.ID ==alarm.ID).FirstOrDefault();
            string state = "ok";
            if (ohState.State == GSF.TimeSeries.AlarmState.Raised)
                state = "alerting";

            DateTime changed = ohState.TimeRaised;
            string inserQuery = "INSERT INTO alert (dashboard_id,version,org_id,panel_id,name,state,new_state_date,message,settings, frequency, handler,severity,silenced,execution_error,state_changes,created, updated) VALUES";
            inserQuery = inserQuery + "({0}, 0, 1, {1}, {2}, {3}, {4}, '', '', 60, 0, 0, 0, '', 0, {5}, {6})";
             
            connection.ExecuteNonQuery(inserQuery, query.dashboardId, query.panelId, alarm.Description, state, changed, alarm.CreatedOn, DateTime.UtcNow);

            int grafanaAlarmID = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");

            // Check if Tag for this alarm already Exists
            int grafanaTagID = connection.ExecuteScalar<int>("SELECT id FROM tag WHERE key = 'OpenHistorianID' AND value = {0}", Convert.ToString(alarm.ID));

            if (grafanaTagID == 0)
            {
                connection.ExecuteNonQuery("INSERT INTO tag (key,value) VALUES ('OpenHistorianID',{0})", alarm.ID);
                grafanaTagID = connection.ExecuteScalar<int>("SELECT last_insert_rowid()");
            }

            connection.ExecuteNonQuery("INSERT INTO alert_rule_tag (alert_id, tag_id) VALUES ({0},{1})", grafanaAlarmID,grafanaTagID);
        }
        #endregion

      
        private static AdoDataConnection GetGrafanaDB()
        {
            CategorizedSettingsElementCollection reportSettings = ConfigurationFile.Current.Settings["OHGrafanaAlarmService"];
            string dBName = "";

            reportSettings.Add("GrafanaDB", ".\\Grafana\\data\\grafana.db", "Path to the Grafana DataBase File used for adding alarms from the OH");
            dBName = reportSettings["GrafanaDB"].ValueAs(dBName);


            string connectionString = $"Data Source={dBName}; Version=3; Foreign Keys=True; FailIfMissing=True";
            string dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";

            return new AdoDataConnection(connectionString, dataProviderString);
        }


       
        
        /*
       

        #region [ Properties ]


        #endregion

        #region [ Methods ]
        /// <summary>
        /// Checks if Alarms for a Grafana Panel are set correctly and sets them if not.
        /// </summary>
        /// <param name="dashboardID"> Grafana DashboardID</param>
        /// <param name="panelID"> Grafana Panel ID </param>
        /// <param name="targets"> List of Targets of the panel and dashboard </param>
        /// <param name="alarms"> List of Alarms for this set of targets </param>
        public static void CheckAlarms(int dashboardID, int panelID, List<Target> targets, List<GrafanaAlarm> alarms)
        {
            Tuple<int, int> key = new Tuple<int, int>(dashboardID, panelID);
            List<Target> oldtargets = new List<Target>();

            if (!m_grafanaPanels.TryGetValue(key, out oldtargets))
            {
                CleanAlarms(dashboardID, panelID);
                CreateAlarms(dashboardID, panelID, alarms);
                m_grafanaPanels.Add(key, targets);
                return;
            }

            if (oldtargets.Count() != targets.Count())
            {
                CleanAlarms(dashboardID, panelID);
                CreateAlarms(dashboardID, panelID, alarms);
                m_grafanaPanels[key] = targets;
                return;
            }


            // Now we need to check if the number stays the same but the channels have changed....
            bool hasChanged = false;
            foreach (Target target in targets)
            {
                if (oldtargets.Where(item => item.target == target.target).Count() < 1)
                {
                    hasChanged = true;
                    break;
                }
            }

            if (hasChanged)
            {
                CleanAlarms(dashboardID, panelID);
                CreateAlarms(dashboardID, panelID, alarms);
                m_grafanaPanels[key] = targets;
                return;
            }
        }

        private static void CleanAlarms(int dashboardID, int panelID)
        {
            lock (m_locked)
            {
                using (AdoDataConnection connection = GetGrafanaDB())
                {
                    // First Delete Alarm Objects
                    connection.ExecuteNonQuery("DELETE FROM alert WHERE dashboard_id = {0} AND panel_id = {1}", dashboardID, panelID);
                    //Next Remove Alarms from Dashboard ib Database
                    dynamic dashboard = JsonConvert.DeserializeObject<dynamic>(connection.ExecuteScalar<string>("SELECT data FROM dashboard WHERE id= {0}", dashboardID));
                    List<dynamic> panels = GetPanels(dashboard);

                    int panelIndex = panels.FindIndex(item => item["id"] == panelID);

                    if (panelIndex == -1)
                        return;

                    dynamic alert;
                    if (TryGetKey(panels[panelIndex], "alert", out alert))
                    {
                        panels[panelIndex].Remove("alert");
                    }
                    if (TryGetKey(panels[panelIndex], "thresholds", out alert))
                    {
                        panels[panelIndex].Remove("thresholds");

                    }


                    string updatedData = JsonConvert.SerializeObject(CreateDashboard(dashboard, panels));
                    connection.ExecuteNonQuery("UPDATE dashboard SET data = {0} WHERE id= {1}", updatedData, dashboardID);

                    //panel = panels.Select(item => JsonConvert.DeserializeObject<GrafanaPanel>(item)).Where(item => item.id == panelID).First();
                }
            }

        }

        private static void CreateAlarms(int dashboardID, int panelID, List<GrafanaAlarm> alarms)
        {
            // Add any Active alarms to the Panel as lines
            dynamic threshholds = new JArray();

            foreach (GrafanaAlarm alarm in alarms)
            {
                dynamic alert = new JObject();
                alert["colorMode"] = "critical";
                alert["fill"] = true;
                alert["line"] = true;
                if (alarm.Operation == (int)AlarmOperation.Equal)
                    alert["fill"] = false;
                if ((alarm.Operation == (int)AlarmOperation.GreaterOrEqual)||(alarm.Operation == (int)AlarmOperation.GreaterThan))
                    alert["op"] = "gt";
                if ((alarm.Operation == (int)AlarmOperation.LessOrEqual) || (alarm.Operation == (int)AlarmOperation.LessThan))
                    alert["op"] = "lt";

                alert["value"] = alarm.SetPoint;
                threshholds.Add(alert);
            }


            lock (m_locked)
            {
                
                using (AdoDataConnection connection = GetGrafanaDB())
                {
                    //Get Dashboard data
                    dynamic dashboard = JsonConvert.DeserializeObject<dynamic>(connection.ExecuteScalar<string>("SELECT data FROM dashboard WHERE id= {0}", dashboardID));
                    List<dynamic> panels = GetPanels(dashboard);

                    int panelIndex = panels.FindIndex(item => item["id"] == panelID);

                   
                    dynamic oldAlert;
                    if (panelIndex == -1)
                        return;

                    if (TryGetKey(panels[panelIndex], "thresholds", out oldAlert))
                    {
                        panels[panelIndex]["thresholds"] = threshholds;
                    }
                    else
                    {
                        panels[panelIndex].Add("thresholds");
                        panels[panelIndex]["thresholds"] = threshholds;
                    }


                    string updatedData = JsonConvert.SerializeObject(CreateDashboard(dashboard, panels));
                    connection.ExecuteNonQuery("UPDATE dashboard SET data = {0} WHERE id= {1}", updatedData, dashboardID);


                    //Add Alarms to the Database
                    foreach (GrafanaAlarm alarm in alarms)
                    {
                        Alarm stateAlarm = AlarmAdapter.Default?.GetAlarmStatus(alarm.SignalID).Where(item => item.ID == alarm.ID).FirstOrDefault();
                        string state = "ok";
                        if (stateAlarm.State == GSF.TimeSeries.AlarmState.Raised)
                            state = "alert";
                        DateTime changed = stateAlarm.TimeRaised;
                        string query = "INSERT INTO alert (dashboard_id,version,org_id,panel_id,name,state,new_state_date,message,settings, frequency, handler,severity,silenced,execution_error,state_changes,created, updated) VALUES";
                        query = query + "({0}, 0, 1, {1}, {2}, {3}, {4}, '', '', 60, 0, 0, 0, '', 0, {5}, {6})";
                        // 
                        connection.ExecuteNonQuery(query, dashboardID, panelID, alarm.Description,state,changed,alarm.CreatedOn,DateTime.UtcNow);
                    }

                    

                }
            }


        }

       
           
        private static bool TryGetKey(dynamic input, int key, out dynamic result)
        {
            try
            {
                result = input[key];
                return true;
            }
            catch 
            {
                result = null;
                return false;
            }
        }

        private static bool TryGetKey(dynamic input, string key, out dynamic result)
        {
            try
            {
                result = input[key];
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        #region  [Deserialize Grafana Panels]

        private static List<dynamic> GetPanels(dynamic dashboard)
        {
            List<dynamic> result = new List<dynamic>();

            int index = 0;
            dynamic p;

            while (TryGetKey(dashboard["panels"],index, out p))
            {
                result.Add(p);
                index ++;
            }

            return result;

        }

        private static dynamic CreateDashboard(dynamic dashboard, List<dynamic> panels)
        {
            dynamic panel = dashboard["panels"];

            int index = 0;

            foreach (dynamic entry in panels)
            {
                panel[index] = entry;
                index++;
            }

            dashboard["panels"] = panel;

            return dashboard;
        }
        #endregion

        #endregion
        */

    }
}
