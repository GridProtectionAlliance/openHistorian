//******************************************************************************************************
//  NodeSelectionScreen.xaml.cs - Gbtc
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
//  10/14/2010 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using GSF;
using GSF.Configuration;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for NodeSelectionScreen.xaml
    /// </summary>
    public partial class NodeSelectionScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Nested Types

        private class NodeInfo
        {
            public string Name
            {
                get;
                set;
            }

            public string Company
            {
                get;
                set;
            }

            public string Description
            {
                get;
                set;
            }

            public Guid Id
            {
                get;
                set;
            }
        }

        // Fields
        private Dictionary<string, object> m_state;
        private IList<NodeInfo> m_dbNodes;
        private IList<NodeInfo> m_nodeList;
        private NodeInfo m_newNode;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="NodeSelectionScreen"/> class.
        /// </summary>
        public NodeSelectionScreen()
        {
            InitializeComponent();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the screen to be displayed when the user clicks the "Next" button.
        /// </summary>
        public IScreen NextScreen => (IScreen)m_state["setupReadyScreen"];

        /// <summary>
        /// Gets a boolean indicating whether the user can advance to
        /// the next screen from the current screen.
        /// </summary>
        public bool CanGoForward => true;

        /// <summary>
        /// Gets a boolean indicating whether the user can return to
        /// the previous screen from the current screen.
        /// </summary>
        public bool CanGoBack => true;

        /// <summary>
        /// Gets a boolean indicating whether the user can cancel the
        /// setup process from the current screen.
        /// </summary>
        public bool CanCancel => true;

        /// <summary>
        /// Gets a boolean indicating whether the user input is valid on the current page.
        /// </summary>
        public bool UserInputIsValid => true;

        /// <summary>
        /// Collection shared among screens that represents the state of the setup.
        /// </summary>
        public Dictionary<string, object> State
        {
            get => m_state;
            set
            {
                m_state = value;
                InitializeState();
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

        // Initializes the state keys to their default values.
        private void InitializeState()
        {
            // This method is called either now or when the
            // data grid is loaded, whichever happens last.
            if (m_dataGrid.IsLoaded)
                UpdateDataGrid();

            if (!m_state.ContainsKey("createNewNode"))
                m_state.Add("createNewNode", false);
        }

        private void UpdateDataGrid()
        {
            IDbConnection connection;

            Guid nodeId;
            int defaultIndex;

            bool applyChangesToService;
            bool applyChangesToLocalManager;

            applyChangesToService = Convert.ToBoolean(m_state["applyChangesToService"]);
            applyChangesToLocalManager = Convert.ToBoolean(m_state["applyChangesToLocalManager"]);

            // Avoid accessing the database again
            // if we already have database nodes.
            if (m_dbNodes is null)
            {
                connection = GetDatabaseConnection();
                m_dbNodes = GetNodes(connection);
            }

            m_nodeList = new List<NodeInfo>(m_dbNodes);

            if (m_nodeList.Count > 0)
            {
                m_infoTextBlock.Text = "Please select the node you would like the openHistorian to use.";
            }
            else
            {
                // Inform the user that the node list could not be found.
                m_infoTextBlock.Text = "The Configuration Setup Utility encountered some difficulty"
                                       + " retrieving the node list from your existing database."
                                       + " This will not affect your ability to complete the setup.";
            }

            if (applyChangesToService)
                nodeId = GetNodeIdFromConfigFile(App.ApplicationConfig);
            else if (applyChangesToLocalManager)
                nodeId = GetNodeIdFromConfigFile(App.ManagerConfig);
            else
                nodeId = Guid.Empty;

            defaultIndex = m_nodeList.TakeWhile(info => info.Id != nodeId).Count();

            if (defaultIndex == m_nodeList.Count)
                defaultIndex = 0;

            // If the configuration file node is not already in the list,
            // add it as a possible selection in case the user does not wish to change it.
            if (applyChangesToService)
                TryAddNodeInfo(App.ApplicationConfig);

            if (applyChangesToLocalManager)
                TryAddNodeInfo(App.ManagerConfig);

            if (m_nodeList.Count == 1)
            {
                if (m_state["screenManager"] is ScreenManager manager)
                {
                    manager.GoToNextScreen(false);
                    return;
                }
            }

            m_dataGrid.ItemsSource = new ObservableCollection<NodeInfo>(m_nodeList);
            m_dataGrid.SelectedIndex = defaultIndex;
        }

        private void TryAddNodeInfo(string configFileName)
        {
            Guid nodeId = GetNodeIdFromConfigFile(configFileName);

            if (m_nodeList.All(info => info.Id != nodeId))
            {
                m_nodeList.Add(new NodeInfo()
                {
                    Name = "ConfigFile",
                    Company = GetCompanyNameFromConfigFile(),
                    Description = $"This node was found in {configFileName}.",
                    Id = nodeId
                });
            }
        }

        // Updates the new node with the name entered by the user.
        private void UpdateNewNode()
        {
            string nodeName = m_newNodeTextBox.Text;

            if (string.IsNullOrWhiteSpace(nodeName))
                return;

            if (m_newNode is null)
            {
                m_newNode = new NodeInfo();
                m_newNode.Company = GetCompanyNameFromConfigFile();
                m_newNode.Description = "Node created by Configuration Setup Utility";
                m_newNode.Id = Guid.NewGuid();

                m_nodeList.Add(m_newNode);

                m_state["newNodeDescription"] = m_newNode.Description;
            }

            m_newNode.Name = nodeName;

            // Deselect first so that new
            // selection will not be overlooked
            m_dataGrid.SelectedIndex = -1;
            m_dataGrid.ItemsSource = new ObservableCollection<NodeInfo>(m_nodeList);
            m_dataGrid.SelectedIndex = m_nodeList.IndexOf(m_newNode);

            m_state["newNodeName"] = m_newNode.Name;
        }

        // Gets a database connection based on the selections the user made earlier in the setup.
        private IDbConnection GetDatabaseConnection()
        {
            if (m_state.ContainsKey("updateConfiguration") && Convert.ToBoolean(m_state["updateConfiguration"]))
                return GetConnectionFromConfigFile();
            else
            {
                string databaseType = m_state["newDatabaseType"].ToString();

                if (databaseType == "SQLServer")
                    return GetSqlServerConnection();
                else if (databaseType == "MySQL")
                    return GetMySqlConnection();
                else if (databaseType == "Oracle")
                    return GetOracleConnection();
                else if (databaseType == "PostgreSQL")
                    return GetPostgresConnection();
                else
                    return GetSqliteDatabaseConnection();
            }
        }

        // Gets a database connection to the SQL Server database configured earlier in the setup.
        private IDbConnection GetSqlServerConnection()
        {
            return !(m_state["sqlServerSetup"] is SqlServerSetup sqlSetup) ? null : new SqlConnection(sqlSetup.ConnectionString);
        }

        // Gets a database connection to the MySQL database configured earlier in the setup.
        private IDbConnection GetMySqlConnection()
        {
            MySqlSetup sqlSetup = m_state["mySqlSetup"] as MySqlSetup;
            string connectionString = sqlSetup is null ? null : sqlSetup.ConnectionString;
            string dataProviderString = sqlSetup is null ? null : sqlSetup.DataProviderString;
            return GetConnection(connectionString, dataProviderString);
        }

        // Gets a database connection to the Oracle database configured earlier in the setup.
        private IDbConnection GetOracleConnection()
        {
            OracleSetup oracleSetup = m_state["oracleSetup"] as OracleSetup;
            string connectionString = oracleSetup is null ? null : oracleSetup.ConnectionString;
            string dataProviderString = oracleSetup is null ? OracleSetup.DefaultDataProviderString : oracleSetup.DataProviderString;
            return GetConnection(connectionString, dataProviderString);
        }

        private IDbConnection GetPostgresConnection()
        {
            PostgresSetup postgresSetup = m_state["postgresSetup"] as PostgresSetup;
            string connectionString = postgresSetup?.ConnectionString;
            string dataProviderString = PostgresSetup.DataProviderString;
            return GetConnection(connectionString, dataProviderString);
        }

        // Gets a database connection to the SQLite database configured earlier in the setup.
        private IDbConnection GetSqliteDatabaseConnection()
        {
            string databaseFileName = m_state["sqliteDatabaseFilePath"].ToString();
            string connectionString = "Data Source=" + databaseFileName + "; Version=3";
            string dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";
            return GetConnection(connectionString, dataProviderString);
        }

        // Gets a database connection object using the connection information that is stored in the configuration file.
        private IDbConnection GetConnectionFromConfigFile()
        {
            IDbConnection connection = null;
            string configFileName = Directory.GetCurrentDirectory() + "\\" + App.ApplicationConfig;

            ConfigurationFile configFile;
            string connectionString;
            string dataProviderString;

            try
            {
                configFile = ConfigurationFile.Open(configFileName);
                connectionString = configFile.Settings["systemSettings"]["ConnectionString"]?.Value;
                dataProviderString = configFile.Settings["systemSettings"]["DataProviderString"]?.Value;

                if ((object)connectionString != null && (object)dataProviderString != null)
                    connection = GetConnection(connectionString, dataProviderString);
            }
            catch
            {
                // Ignore file not found and similar exceptions.
                // Failure to retrieve the node list should not
                // interrupt the setup.
            }

            return connection;
        }

        // Gets a database connection object using the given connection string and data provider string.
        private IDbConnection GetConnection(string connectionString, string dataProviderString)
        {
            Dictionary<string, string> settings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = settings["AssemblyName"].ToNonNullString();
            string connectionTypeName = settings["ConnectionType"].ToNonNullString();
            string adapterTypeName = settings["AdapterType"].ToNonNullString();

            if (string.IsNullOrEmpty(connectionTypeName) || string.IsNullOrEmpty(adapterTypeName))
                return null;

            try
            {
                IDbConnection connection;
                Assembly assembly;
                Type connectionType, adapterType;

                assembly = Assembly.Load(new AssemblyName(assemblyName));
                connectionType = assembly.GetType(connectionTypeName);
                adapterType = assembly.GetType(adapterTypeName);
                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = connectionString;

                return connection;
            }
            catch
            {
                // Ignore errors and return null.
                return null;
            }
        }

        // Gets the list of nodes that are stored in the existing database.
        private IList<NodeInfo> GetNodes(IDbConnection connection)
        {
            List<NodeInfo> nodes = new List<NodeInfo>();

            if (connection != null)
            {
                try
                {
                    IDbCommand command;
                    IDataReader reader;

                    connection.Open();
                    command = connection.CreateCommand();
                    command.CommandText = "SELECT ID, Name, CompanyName AS Company, Description FROM NodeDetail WHERE Enabled <> 0";

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (Guid.TryParse(reader["ID"].ToNonNullString(), out Guid nodeId))
                            {
                                nodes.Add(new NodeInfo()
                                {
                                    Name = reader["Name"].ToNonNullString(),
                                    Company = reader["Company"].ToNonNullString(),
                                    Description = reader["Description"].ToNonNullString(),
                                    Id = nodeId
                                });
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions since failure to retrieve
                    // the node list should not interrupt the setup.
                }
                finally
                {
                    connection.Dispose();
                }
            }

            return nodes;
        }

        // Gets the node ID that is currently stored in the config file.
        private Guid GetNodeIdFromConfigFile(string configFileName)
        {
            string nodeIDString;

            nodeIDString = GetValueOfSystemSetting(configFileName, "NodeID").ToNonNullString();

            if (Guid.TryParse(nodeIDString, out Guid nodeID))
                return nodeID;
            
            return Guid.NewGuid();
        }

        // Gets the company acronym that is currently stored in the config file.
        private string GetCompanyNameFromConfigFile()
        {
            return GetValueOfSystemSetting(App.ApplicationConfig, "CompanyName");
        }

        // Gets the value of the config file system setting with the given name.
        private string GetValueOfSystemSetting(string configFileName, string settingName)
        {
            string configFilePath = Directory.GetCurrentDirectory() + '\\' + configFileName;
            XmlDocument doc = new XmlDocument();

            IEnumerable<XmlNode> systemSettings;
            XmlNode idNode;
            string value = null;

            try
            {
                doc.Load(configFilePath);
                systemSettings = doc.SelectNodes("configuration/categorizedSettings/systemSettings/add").Cast<XmlNode>();
                idNode = systemSettings.SingleOrDefault(node => node.Attributes != null && node.Attributes["name"].Value == settingName);

                if (idNode != null)
                    value = idNode.Attributes["value"].Value;
            }
            catch
            {
                // Ignore file not found and similar exceptions.
                // Failure to retrieve a node from the configuration
                // file will result in a newly generated GUID and
                // should not interrupt the setup.
            }

            return value;
        }

        // Occurs when the data grid is loaded.
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // This method is called either now or when the state
            // object is initialized, whichever happens last.
            if (m_state != null)
                UpdateDataGrid();
        }

        // Occurs when the user selects a different node to be used by the openHistorian.
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedItem = m_dataGrid.SelectedItem;

            if (m_state != null && selectedItem is NodeInfo info)
                m_state["selectedNodeId"] = info.Id;

            if (m_state != null)
                m_state["createNewNode"] = selectedItem != null && selectedItem == m_newNode;
        }

        // Occurs when the user uses the keyboard when the new node text box is in focus.
        private void NewNodeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateNewNode();
                e.Handled = true;
            }
        }

        // Occurs when the user clicks the "New Node" button.
        private void NewNodeButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateNewNode();
        }

        #endregion
    }
}