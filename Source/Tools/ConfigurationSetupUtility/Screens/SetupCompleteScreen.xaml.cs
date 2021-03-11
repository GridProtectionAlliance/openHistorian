//******************************************************************************************************
//  SetupCompleteScreen.xaml.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
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
//  09/10/2010 - Stephen C. Wills
//       Generated original version of source code.
//  09/19/2010 - J. Ritchie Carroll
//       Modified code to take into account that service will normally be stopped on this screen.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using GSF;
using GSF.Data;
using GSF.IO;
using GSF.Identity;
using GSF.Security;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for SetupCompleteScreen.xaml
    /// </summary>
    public partial class SetupCompleteScreen : IScreen
    {
        #region [ Members ]

        // Fields
        private Dictionary<string, object> m_state;
        private ServiceController m_openHistorianServiceController;
        private readonly string m_updateTagNamesExecutable;
        private bool m_runUpdateTagNamesPostMigration;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="SetupCompleteScreen"/> class.
        /// </summary>
        public SetupCompleteScreen()
        {
            InitializeComponent();
            InitializeopenHistorianServiceController();
            InitializeServiceCheckboxState();
            InitializeManagerCheckboxState();

            m_updateTagNamesExecutable = FilePath.GetAbsolutePath("UpdateTagNames.exe");

            if (File.Exists(m_updateTagNamesExecutable))
                return;

            m_updateTagNamesPrefix.Visibility = Visibility.Collapsed;
            m_updateTagNames.Visibility = Visibility.Collapsed;
            m_updateTagNamesSuffix.Visibility = Visibility.Hidden;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the screen to be displayed when the user clicks the "Next" button.
        /// </summary>
        public IScreen NextScreen => null;

        /// <summary>
        /// Gets a boolean indicating whether the user can advance to
        /// the next screen from the current screen.
        /// </summary>
        public bool CanGoForward => true;

        /// <summary>
        /// Gets a boolean indicating whether the user can return to
        /// the previous screen from the current screen.
        /// </summary>
        public bool CanGoBack => false;

        /// <summary>
        /// Gets a boolean indicating whether the user can cancel the
        /// setup process from the current screen.
        /// </summary>
        public bool CanCancel => false;

        /// <summary>
        /// Gets a boolean indicating whether the user input is valid on the current page.
        /// </summary>
        public bool UserInputIsValid
        {
            get
            {
                // Very little input to validate on this page, but we take this opportunity to execute final shut-down operations
                if (m_state != null)
                {
                    try
                    {
                        bool existing = Convert.ToBoolean(m_state["existing"]);
                        bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                        // Validate needed end-point bindings for Grafana interfaces
                        ValidateGrafanaBindings();

                        // Make sure needed assembly bindings exist in config file (required for self-hosted web server)
                        RunHiddenConsoleApp("ValidateAssemblyBindings.exe", App.ApplicationConfig);
                        RunHiddenConsoleApp("ValidateAssemblyBindings.exe", App.ManagerConfig);

                        if (migrate)
                        {
                            const string SerializedSchemaPath = "SerializedSchema.bin";

                            string dataFolder = FilePath.GetApplicationDataFolder();
                            string dataMigrationUtilityUserSettingsFolder = dataFolder + "\\..\\DataMigrationUtility";
                            string userSettingsFile = dataMigrationUtilityUserSettingsFolder + "\\Settings.xml";

                            string newConnectionString = m_state["newConnectionString"].ToString();
                            string oldConnectionString = m_state.ContainsKey("oldConnectionString") ? m_state["oldConnectionString"].ToString() : string.Empty;
                            string newDataProviderString = m_state["newDataProviderString"].ToString();
                            string oldDataProviderString = m_state.ContainsKey("oldDataProviderString") ? m_state["oldDataProviderString"].ToString() : string.Empty;
                            string newDatabaseType = m_state["newDatabaseType"].ToString().Replace(" ", "");

                            if (!Directory.Exists(dataMigrationUtilityUserSettingsFolder))
                                Directory.CreateDirectory(dataMigrationUtilityUserSettingsFolder);

                            oldConnectionString += $"; dataProviderString={{ {oldDataProviderString} }}; serializedSchema={SerializedSchemaPath}";
                            newConnectionString += $"; dataProviderString={{ {newDataProviderString} }}; serializedSchema={SerializedSchemaPath}";

                            XDocument doc = new XDocument(
                                                new XElement("settings",
                                                    new XElement("applicationSettings",
                                                        new XElement("add", new XAttribute("name", "FromConnectionString"),
                                                                            new XAttribute("value", oldConnectionString)),
                                                        new XElement("add", new XAttribute("name", "ToConnectionString"),
                                                                            new XAttribute("value", newConnectionString)),
                                                        new XElement("add", new XAttribute("name", "ToDataType"),
                                                                            new XAttribute("value", newDatabaseType)),
                                                        new XElement("add", new XAttribute("name", "UseFromConnectionForRI"),
                                                                            new XAttribute("value", string.Empty)),
                                                        new XElement("add", new XAttribute("name", "FromDataType"),
                                                                            new XAttribute("value", m_state.ContainsKey("oldDatabaseType") ? m_state["oldDatabaseType"].ToString() : "Unspecified"))
                                                    )
                                                )
                                            );

                            doc.Save(userSettingsFile);

                            try
                            {
                                DependencyObject parent = VisualTreeHelper.GetParent(this);
                                
                                while (parent != null && !(parent is Window))
                                    parent = VisualTreeHelper.GetParent(parent);

                                if (parent is Window mainWindow)
                                    mainWindow.WindowState = WindowState.Minimized;
                            }
                            catch
                            {
                                // Nothing to do if we fail to minimize...
                            }

                            // Run the DataMigrationUtility.
                            using (Process migrationProcess = new Process())
                            {
                                migrationProcess.StartInfo.FileName = "DataMigrationUtility.exe";
                                migrationProcess.StartInfo.Arguments = "-install";
                                migrationProcess.Start();
                                migrationProcess.WaitForExit();
                            }

                            // During migrations - make sure time format is upgraded to support higher-resolution timestamps
                            ValidateDefaultConfigurationSettings();
                        }

                        //ValidateErrorTemplateConfiguration();

                        // Always make sure time series startup operations are defined in the database.
                        ValidateTimeSeriesStartupOperations();

                        // Always make sure new configuration entity records are defined in the database.
                        ValidateConfigurationEntity();

                        // Always make sure openHistorian specific protocols exist
                        ValidateProtocols();

                        // Always make sure that node settings defines the alarm service URL.
                        ValidateNodeSettings();

                        // Always make sure that all three needed roles are available for each defined node(s) in the database.
                        ValidateSecurityRoles();

                        // If tag name updates was postponed till after migration, run the application now
                        if (m_runUpdateTagNamesPostMigration)
                            RunUpdateTagNames();

                        bool runningService = m_serviceStartCheckBox?.IsChecked.GetValueOrDefault() ?? false;

                        // If the user requested it, start or restart the openHistorian service.
                        if (runningService)
                        {
                            try
                            {
                            #if DEBUG
                                Process.Start(App.ApplicationExe);
                            #else
                                try
                                {
                                    DependencyObject parent = VisualTreeHelper.GetParent(this);
                                    Window mainWindow;

                                    while (parent != null && !(parent is Window))
                                        parent = VisualTreeHelper.GetParent(parent);

                                    mainWindow = parent as Window;

                                    if (mainWindow != null)
                                        mainWindow.WindowState = WindowState.Minimized;
                                }
                                catch
                                {
                                    // Nothing to do if we fail to minimize...
                                }                                
                                
                                m_openHistorianServiceController.Refresh();

                                if (m_openHistorianServiceController.Status != ServiceControllerStatus.Running)
                                    m_openHistorianServiceController.Start();
                            #endif
                            }
                            catch
                            {
                                MessageBox.Show("The configuration utility was unable to start openHistorian service, you will need to manually start the service.", "Cannot Start Windows Service", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        // If the user requested it, start the openHistorian Manager.
                        if (m_managerStartCheckBox?.IsChecked.GetValueOrDefault() ?? false)
                        {
                            // Try to launch web app if service is running
                            bool runWebManager = runningService;

                            if (runWebManager)
                            {
                                try
                                {
                                    Process process = new Process
                                    {
                                        StartInfo =
                                        {
                                            UseShellExecute = true,
                                            FileName = "http://localhost:8180/"
                                        }
                                    };

                                    process.Start();
                                }
                                catch
                                {
                                    // For now, fall-back on WPF manager if we can't launch a browser
                                    runWebManager = false;
                                }
                            }

                            if (!runWebManager)
                            {
                                if (UserAccountControl.IsUacEnabled && UserAccountControl.IsCurrentProcessElevated)
                                {
                                    try
                                    {
                                        UserAccountControl.CreateProcessAsStandardUser(App.ManagerExe);
                                    }
                                    catch
                                    {
                                        Process.Start(App.ManagerExe);
                                    }
                                }
                                else
                                {
                                    Process.Start(App.ManagerExe);
                                }
                            }
                        }
                    }
                    finally
                    {
                        m_openHistorianServiceController?.Close();
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Collection shared among screens that represents the state of the setup.
        /// </summary>
        public Dictionary<string, object> State
        {
            get => m_state;
            set
            {
                m_state = value;

                if (Convert.ToBoolean(m_state["restarting"]))
                    m_serviceStartCheckBox.Content = "Restart the openHistorian";
            }
        }

        /// <summary>
        /// Allows the screen to update the navigation buttons after a change is made
        /// that would affect the user's ability to navigate to other screens.
        /// </summary>
        public Action UpdateNavigation
        {
            get;
            set;
        }

        #endregion

        #region [ Methods ]

        // Initializes the openHistorian service controller.
        private void InitializeopenHistorianServiceController()
        {
            ServiceController[] services = ServiceController.GetServices();
            m_openHistorianServiceController = services.SingleOrDefault(svc => string.Compare(svc.ServiceName, "openHistorian", StringComparison.OrdinalIgnoreCase) == 0);
        }

        // Initializes the state of the openHistorian service checkbox.
        private void InitializeServiceCheckboxState()
        {
        #if DEBUG
            bool serviceInstalled = File.Exists(App.ApplicationExe);
        #else
            bool serviceInstalled = m_openHistorianServiceController != null;
        #endif

            m_serviceStartCheckBox.IsChecked = serviceInstalled;
            m_serviceStartCheckBox.IsEnabled = serviceInstalled;
        }

        // Initializes the state of the openHistorian Manager checkbox.
        private void InitializeManagerCheckboxState()
        {
            bool managerInstalled = File.Exists(App.ManagerExe);
            m_managerStartCheckBox.IsChecked = managerInstalled;
            m_managerStartCheckBox.IsEnabled = managerInstalled;
        }

        private void ValidateGrafanaBindings()
        {
            string configFileName = Path.Combine(Directory.GetCurrentDirectory(), App.ApplicationConfig);

            if (!File.Exists(configFileName))
                return;

            XmlDocument configFile = new XmlDocument();
            configFile.Load(configFileName);

            XmlNode categorizedSettings = configFile.SelectSingleNode("configuration/categorizedSettings");

            if (categorizedSettings is null)
            {
                XmlNode config = configFile.SelectSingleNode("configuration");

                if (config is null)
                    return;

                categorizedSettings = configFile.CreateElement("categorizedSettings");
                config.AppendChild(categorizedSettings);
            }

            const string GrafanaArchiveBinding =
                @"<{0}GrafanaDataService>
                  <add name=""Endpoints"" value=""{1}"" description=""Semicolon delimited list of URIs where the web service can be accessed."" encrypted=""false"" />
                  <add name=""Contract"" value=""GrafanaAdapters.IGrafanaDataService, GrafanaAdapters"" description=""Assembly qualified name of the contract interface implemented by the web service."" encrypted=""false"" />
                  <add name=""Singleton"" value=""True"" description=""True if the web service is singleton; otherwise False."" encrypted=""false"" />
                  <add name=""SecurityPolicy"" value="""" description=""Assembly qualified name of the authorization policy to be used for securing the web service."" encrypted=""false"" />
                  <add name=""PublishMetadata"" value=""True"" description=""True if the web service metadata is to be published at all the endpoints; otherwise False."" encrypted=""false"" />
                  <add name=""AllowCrossDomainAccess"" value=""False"" description=""True to allow Silverlight and Flash cross-domain access to the web service."" encrypted=""false"" />
                  <add name=""AllowedDomainList"" value=""*"" description=""Comma separated list of domain names for Silverlight and Flash cross-domain access to use when allowCrossDomainAccess is true. Use * for domain wildcards, e.g., *.consoto.com."" encrypted=""false"" />
                  <add name=""CloseTimeout"" value=""00:02:00"" description=""Maximum time allowed for a connection to close before raising a timeout exception."" encrypted=""false"" />
                  <add name=""Enabled"" value=""{2}"" description=""{3}"" encrypted=""false"" />
                </{0}GrafanaDataService>";

            if (configFile.SelectSingleNode("configuration/categorizedSettings/statGrafanaDataService") is null)
            {
                string archiveBinding = string.Format(GrafanaArchiveBinding, "stat", "http.rest://localhost:6356/api/grafana", "True", "Determines if web service should be enabled at startup.");

                XmlDocument grafanaBindingsXml = new XmlDocument();
                grafanaBindingsXml.LoadXml(archiveBinding);

                XmlDocumentFragment grafanaBindings = configFile.CreateDocumentFragment();
                grafanaBindings.InnerXml = grafanaBindingsXml.InnerXml;

                categorizedSettings.AppendChild(grafanaBindings);
            }

            bool setupHistorian = false;

            if (m_state.TryGetValue("setupHistorian", out object setupHistorianValue))
                setupHistorian = setupHistorianValue.ToNonNullNorWhiteSpace("False").ParseBoolean();

            if (setupHistorian)
            {
                string historianAcronym;

                m_state.TryGetValue("historianAcronym", out object historianAcronymValue);
                historianAcronym = historianAcronymValue.ToNonNullNorWhiteSpace("ppa").ToLowerInvariant();

                m_state.TryGetValue("historianTypeName", out object historianTypeName);

                if (historianTypeName.ToNonNullString().Equals("openHistorian.Adapters.LocalOutputAdapter") && configFile.SelectSingleNode($"configuration/categorizedSettings/{historianAcronym}GrafanaDataService") is null)
                {
                    // In the case of the openHistorian 2.0, we do not want to enable WCF based Grafana binding for primary archive,
                    // instead we want user to use the MVC API based Grafana controller that is hosted off of the main web UI port,
                    // so we disable the WCF binding
                    string archiveBinding = string.Format(GrafanaArchiveBinding, historianAcronym, "http.rest://localhost:8180/unused", "False", "Do not enable, service already available through primary web interface at /api/grafana.");

                    XmlDocument grafanaBindingsXml = new XmlDocument();
                    grafanaBindingsXml.LoadXml(archiveBinding);

                    XmlDocumentFragment grafanaBindings = configFile.CreateDocumentFragment();
                    grafanaBindings.InnerXml = grafanaBindingsXml.InnerXml;

                    categorizedSettings.AppendChild(grafanaBindings);
                }
            }

            configFile.Save(configFileName);
        }

        private void ValidateDefaultConfigurationSettings()
        {
            string configFileName = Path.Combine(Directory.GetCurrentDirectory(), App.ApplicationConfig);

            if (!File.Exists(configFileName))
                return;

            bool configFileUpdated = false;
            XmlDocument configFile = new XmlDocument();
            configFile.Load(configFileName);

            XmlNode timeformat = configFile.SelectSingleNode("configuration/categorizedSettings/systemSettings/add[@name='TimeFormat']");

            if (timeformat != null)
            {
                XmlAttribute value = timeformat.Attributes?["value"];

                if (value != null)
                {
                    // For migrations, make sure time format supports high-resolution timestamps
                    if (!value.Value.Contains(".ffffff"))
                    {
                        value.Value = "HH:mm:ss.ffffff";
                        configFileUpdated = true;
                    }
                }
            }

            XmlNode authRedirectExpression = configFile.SelectSingleNode("configuration/categorizedSettings/systemSettings/add[@name='AuthFailureRedirectResourceExpression']");

            if (authRedirectExpression != null)
            {
                XmlAttribute value = authRedirectExpression.Attributes?["value"];

                if (value != null)
                {
                    // Make sure non-api based grafana calls will redirect when unauthenticated
                    if (!value.Value.Contains("/grafana"))
                    {
                        value.Value = @"^/$|^/.+\.cshtml$|^/.+\.vbhtml$|^/grafana(?!/api/).*$";
                        configFileUpdated = true;
                    }
                }
            }

            if (configFileUpdated)
                configFile.Save(configFileName);
        }

        //private void ValidateErrorTemplateConfiguration()
        //{
        //    string configFileName = Path.Combine(Directory.GetCurrentDirectory(), App.ApplicationConfig);

        //    if (!File.Exists(configFileName))
        //        return;

        //    bool configFileUpdated = false;
        //    XmlDocument configFile = new XmlDocument();
        //    configFile.Load(configFileName);

        //    XmlElement errorTemplateName = configFile.SelectSingleNode("configuration/categorizedSettings/systemSettings/add[@name='ErrorTemplateName']") as XmlElement;

        //    if (errorTemplateName?.Attributes is null)
        //    {
        //        XmlElement systemSettings = configFile.SelectSingleNode("configuration/categorizedSettings/systemSettings") as XmlElement;

        //        if (systemSettings != null)
        //        {
        //            errorTemplateName = configFile.CreateElement("add");

        //            errorTemplateName.SetAttribute("name", "ErrorTemplateName");
        //            errorTemplateName.SetAttribute("value", "Error.cshtml");
        //            errorTemplateName.SetAttribute("description", "Defines the template file name to use when a Razor compile or execution exception occurs. Leave blank for none.");
        //            errorTemplateName.SetAttribute("encrypted", "false");

        //            systemSettings.AppendChild(errorTemplateName);
        //            configFileUpdated = true;
        //        }
        //    }

        //    if (configFileUpdated)
        //        configFile.Save(configFileName);
        //}

        private void ValidateTimeSeriesStartupOperations()
        {
            const string CountQuery = "SELECT COUNT(*) FROM DataOperation WHERE MethodName = 'PerformTimeSeriesStartupOperations'";
            const string InsertQuery = "INSERT INTO DataOperation(Description, AssemblyName, TypeName, MethodName, Arguments, LoadOrder, Enabled) VALUES('Time Series Startup Operations', 'GSF.TimeSeries.dll', 'GSF.TimeSeries.TimeSeriesStartupOperations', 'PerformTimeSeriesStartupOperations', '', 0, 1)";

            IDbConnection connection = null;
            
            try
            {
                connection = OpenNewConnection();
                int timeSeriesStartupOperationsCount = Convert.ToInt32(connection.ExecuteScalar(CountQuery));

                if (timeSeriesStartupOperationsCount == 0)
                    connection.ExecuteNonQuery(InsertQuery);
            }
            finally
            {
                connection?.Dispose();
            }
        }

        private void ValidateConfigurationEntity()
        {
            const string CountNodeInfoQuery = "SELECT COUNT(*) FROM ConfigurationEntity WHERE RuntimeName = 'NodeInfo'";
            const string InsertNodeInfoQuery = "INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('NodeInfo', 'NodeInfo', 'Defines information about the nodes in the database', 18, 1)";

            const string CountCompressionSettingsQuery = "SELECT COUNT(*) FROM ConfigurationEntity WHERE RuntimeName = 'CompressionSettings'";
            const string InsertCompressionSettingsQuery = "INSERT INTO ConfigurationEntity(SourceName, RuntimeName, Description, LoadOrder, Enabled) VALUES('NodeCompressionSetting', 'CompressionSettings', 'Defines information about measurement compression settings', 19, 1)";

            IDbConnection connection = null;
            
            try
            {
                connection = OpenNewConnection();

                int configurationEntityCount = Convert.ToInt32(connection.ExecuteScalar(CountNodeInfoQuery));

                if (configurationEntityCount == 0)
                    connection.ExecuteNonQuery(InsertNodeInfoQuery);

                configurationEntityCount = Convert.ToInt32(connection.ExecuteScalar(CountCompressionSettingsQuery));

                if (configurationEntityCount == 0)
                    connection.ExecuteNonQuery(InsertCompressionSettingsQuery);
            }
            finally
            {
                connection?.Dispose();
            }
        }

        private void ValidateProtocols()
        {
            const string CountModbusQuery = "SELECT COUNT(*) FROM Protocol WHERE Acronym = 'Modbus'";
            const string InsertModbusQuery = "INSERT INTO Protocol(Acronym, Name, Type, Category, AssemblyName, TypeName, LoadOrder) VALUES('Modbus', 'Modbus Poller', 'Measurement', 'Device', 'ModbusAdapters.dll', 'ModbusAdapters.ModbusPoller', 13)";

            const string CountComtradeQuery = "SELECT COUNT(*) FROM Protocol WHERE Acronym = 'COMTRADE'";
            const string InsertComtradeQuery = "INSERT INTO Protocol(Acronym, Name, Type, Category, AssemblyName, TypeName, LoadOrder) VALUES('COMTRADE', 'COMTRADE Import', 'Measurement', 'Imported', 'TestingAdapters.dll', 'TestingAdapters.VirtualInputAdapter', 14)";

            IDbConnection connection = null;
            
            try
            {
                connection = OpenNewConnection();

                int configurationEntityCount = Convert.ToInt32(connection.ExecuteScalar(CountModbusQuery));

                if (configurationEntityCount == 0)
                    connection.ExecuteNonQuery(InsertModbusQuery);

                configurationEntityCount = Convert.ToInt32(connection.ExecuteScalar(CountComtradeQuery));

                if (configurationEntityCount == 0)
                    connection.ExecuteNonQuery(InsertComtradeQuery);
            }
            finally
            {
                connection?.Dispose();
            }
        }

        private void ValidateNodeSettings()
        {
            const string SettingsQuery = "SELECT Settings FROM Node WHERE ID = '{0}'";
            string updateQuery = "UPDATE Node SET Settings = @settings WHERE ID = '{0}'";
            IDbConnection connection = null;
            
            try
            {
                // Ensure that there is a selected node ID
                if (!m_state.TryGetValue("selectedNodeId", out object selectedNodeId) || string.IsNullOrWhiteSpace(selectedNodeId.ToNonNullString()))
                    return;

                // Open new connection
                string nodeIDQueryString = selectedNodeId.ToString();
                connection = OpenNewConnection();

                // Get node settings from the database
                object nodeSettingsConnectionString = connection.ExecuteScalar(string.Format(SettingsQuery, nodeIDQueryString));
                Dictionary<string, string> nodeSettings = nodeSettingsConnectionString.ToNonNullString().ParseKeyValuePairs();

                // If the AlarmServiceUrl does not exist in node settings, add it and then update the database record
                if (nodeSettings.TryGetValue("AlarmServiceUrl", out string _))
                    return;

                if (connection.GetType().Name == "OracleConnection")
                    updateQuery = updateQuery.Replace('@', ':');

                nodeSettings.Add("AlarmServiceUrl", "http://localhost:5019/alarmservices");
                nodeSettingsConnectionString = nodeSettings.JoinKeyValuePairs();
                connection.ExecuteNonQuery(string.Format(updateQuery, nodeIDQueryString), nodeSettingsConnectionString);
            }
            finally
            {
                // Dispose of the connection if it was opened
                connection?.Dispose();
            }
        }

        private void ValidateSecurityRoles()
        {
            IDbConnection connection = OpenNewConnection();

            if (connection is null)
                return;

            try
            {
                IDataReader nodeReader;
                IDbCommand nodeCommand;

                nodeCommand = connection.CreateCommand();
                nodeCommand.CommandText = "SELECT ID FROM Node";

                // For each Node in new database make sure all roles exist
                using (nodeReader = nodeCommand.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(nodeReader);
                    nodeReader.Close();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string nodeID = $"'{row["ID"].ToNonNullString()}'";
                        IDbCommand command = connection.CreateCommand();

                        command.CommandText = $"SELECT COUNT(*) FROM ApplicationRole WHERE NodeID = {nodeID} AND Name = 'Administrator'";
                        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                            AddRolesForNode(connection, nodeID, "Administrator");
                        else // Verify an admin user exists for the node and attached to administrator role.
                            VerifyAdminUser(connection, nodeID);

                        command.CommandText = $"SELECT COUNT(*) FROM ApplicationRole WHERE NodeID = {nodeID} AND Name = 'Editor'";
                        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                            AddRolesForNode(connection, nodeID, "Editor");

                        command.CommandText = $"SELECT COUNT(*) FROM ApplicationRole WHERE NodeID = {nodeID} AND Name = 'Viewer'";
                        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                            AddRolesForNode(connection, nodeID, "Viewer");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to validate application roles: " + ex.Message, "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// Adds role for newly added node (Administrator, Editor, Viewer).
        /// </summary>
        /// <param name="connection">IDbConnection to be used for database operations.</param>
        /// <param name="nodeID">Node ID to which three roles are being assigned</param>
        /// <param name="roleName">Role name to add.</param> 
        private void AddRolesForNode(IDbConnection connection, string nodeID, string roleName)
        {
            IDbCommand adminCredentialCommand = connection.CreateCommand();

            switch (roleName)
            {
                case "Administrator":
                    adminCredentialCommand.CommandText = $"INSERT INTO ApplicationRole(Name, Description, NodeID, UpdatedBy, CreatedBy) VALUES('Administrator', 'Administrator Role', {nodeID}, '{Thread.CurrentPrincipal.Identity.Name}', '{Thread.CurrentPrincipal.Identity.Name}')";
                    break;
                case "Editor":
                    adminCredentialCommand.CommandText = $"INSERT INTO ApplicationRole(Name, Description, NodeID, UpdatedBy, CreatedBy) VALUES('Editor', 'Editor Role', {nodeID}, '{Thread.CurrentPrincipal.Identity.Name}', '{Thread.CurrentPrincipal.Identity.Name}')";
                    break;
                default:
                    adminCredentialCommand.CommandText = $"INSERT INTO ApplicationRole(Name, Description, NodeID, UpdatedBy, CreatedBy) VALUES('Viewer', 'Viewer Role', {nodeID}, '{Thread.CurrentPrincipal.Identity.Name}', '{Thread.CurrentPrincipal.Identity.Name}')";
                    break;
            }

            adminCredentialCommand.ExecuteNonQuery();

            if (roleName == "Administrator") //verify admin user exists for the node and attached to administrator role.
                VerifyAdminUser(connection, nodeID);
        }

        private void VerifyAdminUser(IDbConnection connection, string nodeID)
        {
            // Lookup administrator role ID
            IDbCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT ID FROM ApplicationRole WHERE Name = 'Administrator' AND NodeID = {nodeID}";
            string adminRoleID = command.ExecuteScalar().ToNonNullString();
            string databaseType = m_state["newDatabaseType"].ToString();

            adminRoleID = adminRoleID.StartsWith("'") ? adminRoleID : "'" + adminRoleID + "'";

            // Check if there is any user associated with the administrator role ID in the ApplicationRoleUserAccount table.
            // if so that means there is at least one user associated with that role. So we do not need to take any action.
            // if not that means, user provided on the screen must be attached to this role. Also check if that user exists in 
            // the UserAccount table. If so, then get the ID otherwise add user and retrieve ID.
            command.CommandText = $"SELECT COUNT(*) FROM ApplicationRoleUserAccount WHERE ApplicationRoleID = {adminRoleID}";

            if (Convert.ToInt32(command.ExecuteScalar()) == 0)
            {
                if (m_state.ContainsKey("adminUserName")) //i.e. if security setup screen was displayed during setup.
                {
                    command.CommandText = $"Select ID FROM UserAccount WHERE Name = '{m_state["adminUserName"]}'";
                    string adminUserID = command.ExecuteScalar().ToNonNullString();

                    if (!string.IsNullOrEmpty(adminUserID)) //if user exists then attach it to admin role and we'll be done with it.
                    {
                        adminUserID = adminUserID.StartsWith("'") ? adminUserID : "'" + adminUserID + "'";
                        command.CommandText = $"INSERT INTO ApplicationRoleUserAccount(ApplicationRoleID, UserAccountID) VALUES({adminRoleID}, {adminUserID})";
                        command.ExecuteNonQuery();
                    }
                    else //we need to add user to the UserAccount table and then attach it to admin role.
                    {
                        bool databaseIsOracle = databaseType == "Oracle";
                        char paramChar = databaseIsOracle ? ':' : '@';

                        // Add Administrative User.                
                        IDbCommand adminCredentialCommand = connection.CreateCommand();
                        if (m_state["authenticationType"].ToString() == "windows")
                        {
                            IDbDataParameter nameParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter createdByParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter updatedByParameter = adminCredentialCommand.CreateParameter();

                            nameParameter.ParameterName = paramChar + "name";
                            createdByParameter.ParameterName = paramChar + "createdBy";
                            updatedByParameter.ParameterName = paramChar + "updatedBy";

                            nameParameter.Value = m_state["adminUserName"].ToString();
                            createdByParameter.Value = Thread.CurrentPrincipal.Identity.Name;
                            updatedByParameter.Value = Thread.CurrentPrincipal.Identity.Name;

                            adminCredentialCommand.Parameters.Add(nameParameter);
                            adminCredentialCommand.Parameters.Add(createdByParameter);
                            adminCredentialCommand.Parameters.Add(updatedByParameter);

                            if (databaseIsOracle)
                                adminCredentialCommand.CommandText = $"INSERT INTO UserAccount(Name, DefaultNodeID, CreatedBy, UpdatedBy) VALUES(:name, {nodeID}, :createdBy, :updatedBy)";
                            else
                                adminCredentialCommand.CommandText = $"INSERT INTO UserAccount(Name, DefaultNodeID, CreatedBy, UpdatedBy) VALUES(@name, {nodeID}, @createdBy, @updatedBy)";
                        }
                        else
                        {
                            IDbDataParameter nameParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter passwordParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter firstNameParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter lastNameParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter createdByParameter = adminCredentialCommand.CreateParameter();
                            IDbDataParameter updatedByParameter = adminCredentialCommand.CreateParameter();

                            nameParameter.ParameterName = paramChar + "name";
                            passwordParameter.ParameterName = paramChar + "password";
                            firstNameParameter.ParameterName = paramChar + "firstName";
                            lastNameParameter.ParameterName = paramChar + "lastName";
                            createdByParameter.ParameterName = paramChar + "createdBy";
                            updatedByParameter.ParameterName = paramChar + "updatedBy";

                            nameParameter.Value = m_state["adminUserName"].ToString();
                            passwordParameter.Value = SecurityProviderUtility.EncryptPassword(m_state["adminPassword"].ToString());
                            firstNameParameter.Value = m_state["adminUserFirstName"].ToString();
                            lastNameParameter.Value = m_state["adminUserLastName"].ToString();
                            createdByParameter.Value = Thread.CurrentPrincipal.Identity.Name;
                            updatedByParameter.Value = Thread.CurrentPrincipal.Identity.Name;

                            adminCredentialCommand.Parameters.Add(nameParameter);
                            adminCredentialCommand.Parameters.Add(passwordParameter);
                            adminCredentialCommand.Parameters.Add(firstNameParameter);
                            adminCredentialCommand.Parameters.Add(lastNameParameter);
                            adminCredentialCommand.Parameters.Add(createdByParameter);
                            adminCredentialCommand.Parameters.Add(updatedByParameter);

                            if (databaseIsOracle)
                                adminCredentialCommand.CommandText = "INSERT INTO UserAccount(Name, Password, FirstName, LastName, DefaultNodeID, UseADAuthentication, CreatedBy, UpdatedBy) VALUES" + $"(:name, :password, :firstName, :lastName, {nodeID}, 0, :createdBy, :updatedBy)";
                            else
                                adminCredentialCommand.CommandText = "INSERT INTO UserAccount(Name, Password, FirstName, LastName, DefaultNodeID, UseADAuthentication, CreatedBy, UpdatedBy) VALUES" + $"(@name, @password, @firstName, @lastName, {nodeID}, 0, @createdBy, @updatedBy)";
                        }

                        adminCredentialCommand.ExecuteNonQuery();
                        IDbDataParameter newNameParameter = adminCredentialCommand.CreateParameter();

                        newNameParameter.ParameterName = paramChar + "name";
                        newNameParameter.Value = m_state["adminUserName"].ToString();

                        adminCredentialCommand.CommandText = "SELECT ID FROM UserAccount WHERE Name = " + paramChar + "name";
                        adminCredentialCommand.Parameters.Clear();
                        adminCredentialCommand.Parameters.Add(newNameParameter);

                        // Get the admin user ID from the database.
                        IDataReader userIdReader;

                        using (userIdReader = adminCredentialCommand.ExecuteReader())
                        {
                            if (userIdReader.Read())
                                adminUserID = userIdReader["ID"].ToNonNullString();
                        }

                        // Assign Administrative User to Administrator Role.
                        if (!string.IsNullOrEmpty(adminRoleID) && !string.IsNullOrEmpty(adminUserID))
                        {
                            adminUserID = adminUserID.StartsWith("'") ? adminUserID : "'" + adminUserID + "'";
                            adminCredentialCommand.CommandText = $"INSERT INTO ApplicationRoleUserAccount(ApplicationRoleID, UserAccountID) VALUES ({adminRoleID}, {adminUserID})";
                            adminCredentialCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private IDbConnection OpenNewConnection()
        {
            IDbConnection connection = null;

            try
            {
                string databaseType = m_state["newDatabaseType"].ToString();
                string connectionString, dataProviderString;

                if (databaseType == "SQLServer")
                {
                    SqlServerSetup sqlServerSetup = m_state["sqlServerSetup"] as SqlServerSetup;
                    connectionString = sqlServerSetup?.ConnectionString;
                    dataProviderString = sqlServerSetup?.DataProviderString;

                    if (string.IsNullOrWhiteSpace(dataProviderString))
                        dataProviderString = "AssemblyName={System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089}; ConnectionType=System.Data.SqlClient.SqlConnection; AdapterType=System.Data.SqlClient.SqlDataAdapter";
                }
                else if (databaseType == "MySQL")
                {
                    MySqlSetup mySqlSetup = m_state["mySqlSetup"] as MySqlSetup;
                    connectionString = mySqlSetup?.ConnectionString;
                    dataProviderString = mySqlSetup?.DataProviderString;

                    if (string.IsNullOrWhiteSpace(dataProviderString))
                        dataProviderString = "AssemblyName={MySql.Data, Version=6.5.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d}; ConnectionType=MySql.Data.MySqlClient.MySqlConnection; AdapterType=MySql.Data.MySqlClient.MySqlDataAdapter";
                }
                else if (databaseType == "Oracle")
                {
                    OracleSetup oracleSetup = m_state["oracleSetup"] as OracleSetup;
                    connectionString = oracleSetup?.ConnectionString;
                    dataProviderString = oracleSetup?.DataProviderString;

                    if (string.IsNullOrWhiteSpace(dataProviderString))
                        dataProviderString = OracleSetup.DefaultDataProviderString;
                }
                else if (databaseType == "PostgreSQL")
                {
                    PostgresSetup postgresSetup = m_state["postgresSetup"] as PostgresSetup;
                    connectionString = postgresSetup?.ConnectionString;
                    dataProviderString = PostgresSetup.DataProviderString;
                }
                else
                {
                    string destination = m_state["sqliteDatabaseFilePath"].ToString();
                    connectionString = "Data Source=" + destination + "; Version=3";
                    dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";
                }

                if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(dataProviderString))
                {
                    Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
                    string assemblyName = dataProviderSettings["AssemblyName"];
                    string connectionTypeName = dataProviderSettings["ConnectionType"];

                    Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
                    Type connectionType = assembly.GetType(connectionTypeName);
                    connection = (IDbConnection)Activator.CreateInstance(connectionType);
                    connection.ConnectionString = connectionString;
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open new database connection: " + ex.Message, "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                connection?.Dispose();
                connection = null;
            }

            return connection;
        }

        private void m_updateTagNames_Click(object sender, RoutedEventArgs e)
        {
            bool existing = Convert.ToBoolean(m_state["existing"]);
            bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

            if (migrate)
            {
                m_runUpdateTagNamesPostMigration = true;
                MessageBox.Show("Request received to update tag names. Since database is being migrated to new schema, the update tag names utility will run after database migration utility.", "Execution Order Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                RunUpdateTagNames();
            }
        }

        private void RunUpdateTagNames()
        {
            try
            {
                // Run the UpdateTagNames utility.
                using (Process migrationProcess = new Process())
                {
                    migrationProcess.StartInfo.FileName = m_updateTagNamesExecutable;
                    migrationProcess.Start();
                    migrationProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run update tag names utility - attempt to run the tool manually later: " + ex.Message, "Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunHiddenConsoleApp(string application, string arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = application,
                    Arguments = arguments,
                    WorkingDirectory = FilePath.GetAbsolutePath("")
                };

                // Pre-start console process for quick update responses
                Process process = new Process { StartInfo = startInfo };
                process.Start();
                process.WaitForExit(10000);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to execute \"{application} {arguments}\": {ex.Message}", "Execution Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}