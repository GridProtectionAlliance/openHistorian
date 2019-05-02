//******************************************************************************************************
//  SetupInProgressScreen.xaml.cs - Gbtc
//
//  Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/09/2010 - Stephen C. Wills
//       Generated original version of source code.
//  09/19/2010 - J. Ritchie Carroll
//       Added code to stop key processes prior to modification of configuration files.
//       Fixed error with AdoMetadataProvider section updates.
//  02/28/2011 - Mehulbhai P Thakkar
//       Modified code to update ForceLoginDisplay settings for openHistorianManager config file.
//  03/02/2011 - J. Ritchie Carroll
//       Simplified code for XML update for ForceLoginDisplay.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using GSF;
using GSF.Communication;
using GSF.Data;
using GSF.Identity;
using GSF.Security;
using GSF.Security.Cryptography;
using Microsoft.Win32;

namespace ConfigurationSetupUtility.Screens
{
    /// <summary>
    /// Interaction logic for SetupInProgressScreen.xaml
    /// </summary>
    public partial class SetupInProgressScreen : UserControl, IScreen
    {
        #region [ Members ]

        // Constants
        private const string SQLiteDataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.109.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";

        // Fields
        private bool m_canGoForward;
        private bool m_canGoBack;
        private bool m_canCancel;
        private readonly IScreen m_nextScreen;
        private Dictionary<string, object> m_state;
        private string m_oldConnectionString;
        private string m_oldDataProviderString;
        private bool m_defaultNodeAdded;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="SetupInProgressScreen"/> class.
        /// </summary>
        public SetupInProgressScreen()
        {
            InitializeComponent();
            m_nextScreen = new SetupCompleteScreen();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the screen to be displayed when the user clicks the "Next" button.
        /// </summary>
        public IScreen NextScreen
        {
            get
            {
                return m_nextScreen;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user can advance to
        /// the next screen from the current screen.
        /// </summary>
        public bool CanGoForward
        {
            get
            {
                return m_canGoForward;
            }
            private set
            {
                m_canGoForward = value;
                UpdateNavigation();
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user can return to
        /// the previous screen from the current screen.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return m_canGoBack;
            }
            private set
            {
                m_canGoBack = value;
                UpdateNavigation();
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user can cancel the
        /// setup process from the current screen.
        /// </summary>
        public bool CanCancel
        {
            get
            {
                return m_canCancel;
            }
            private set
            {
                m_canCancel = value;
                UpdateNavigation();
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the user input is valid on the current page.
        /// </summary>
        public bool UserInputIsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Collection shared among screens that represents the state of the setup.
        /// </summary>
        public Dictionary<string, object> State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
                m_canGoBack = false;
                m_canCancel = false;
                ThreadPool.QueueUserWorkItem(SetUpConfiguration);
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

        // Called when this screen is ready to set up the user's configuration.
        private void SetUpConfiguration(object state)
        {
            string configurationType = m_state["configurationType"].ToString();
            ClearStatusMessages();

            if (m_state.ContainsKey("oldConnectionString") && !string.IsNullOrWhiteSpace(m_state["oldConnectionString"].ToString()))
                m_oldConnectionString = m_state["oldConnectionString"].ToString();
            else
                m_oldConnectionString = null;

            if (m_state.ContainsKey("oldDataProviderString") && !string.IsNullOrWhiteSpace(m_state["oldDataProviderString"].ToString()))
                m_oldDataProviderString = m_state["oldDataProviderString"].ToString();
            else
                m_oldDataProviderString = null;

            // Attempt to establish crypto keys in case they do not exist
            try
            {
                "SetupString".Encrypt(App.CipherLookupKey, CipherStrength.Aes256);
            }
            catch
            {
                // Keys will be established at run-time otherwise
            }

            if (configurationType == "database")
                SetUpDatabase();
            else if (configurationType == "xml")
                SetUpXmlConfiguration();
            else
                SetUpWebServiceConfiguration();
        }

        // Called when the setup utility is about to set up the database
        private void SetUpDatabase()
        {
            string databaseType = m_state["newDatabaseType"].ToString();

            if (databaseType == "SQLServer")
                SetUpSqlServerDatabase();
            else if (databaseType == "MySQL")
                SetUpMySqlDatabase();
            else if (databaseType == "Oracle")
                SetUpOracleDatabase();
            else if (databaseType == "PostgreSQL")
                SetUpPostgresDatabase();
            else
                SetUpSqliteDatabase();
        }

        // Called when the user has asked to set up a MySQL database.
        private void SetUpMySqlDatabase()
        {
            MySqlSetup mySqlSetup = null;

            try
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                string dataProviderString = null;

                mySqlSetup = m_state["mySqlSetup"] as MySqlSetup;
                m_state["newConnectionString"] = mySqlSetup.ConnectionString;

                // Get user customized data provider string
                dataProviderString = mySqlSetup.DataProviderString;

                if (string.IsNullOrWhiteSpace(dataProviderString))
                    dataProviderString = "AssemblyName={MySql.Data, Version=6.5.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d}; ConnectionType=MySql.Data.MySqlClient.MySqlConnection; AdapterType=MySql.Data.MySqlClient.MySqlDataAdapter";

                m_state["newDataProviderString"] = dataProviderString;

                if (!existing || migrate)
                {
                    if (!CheckIfDatabaseExists(mySqlSetup.ConnectionString, dataProviderString, mySqlSetup.DatabaseName))
                    {
                        List<string> scriptNames = new List<string>();
                        bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                        bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);
                        bool enableAuditLog = Convert.ToBoolean(m_state["enableAuditLog"]);
                        bool createNewUser = Convert.ToBoolean(m_state["createNewMySqlUser"]);
                        int progress = 0;

                        // Determine which scripts need to be run.
                        scriptNames.Add("openHistorian.sql");
                        if (initialDataScript)
                        {
                            scriptNames.Add("InitialDataSet.sql");
                            if (sampleDataScript)
                                scriptNames.Add("SampleDataSet.sql");
                        }

                        if (enableAuditLog)
                            scriptNames.Add("AuditLog.sql");

                        foreach (string scriptName in scriptNames)
                        {
                            string scriptPath = Directory.GetCurrentDirectory() + "\\Database scripts\\MySQL\\" + scriptName;
                            AppendStatusMessage(string.Format("Attempting to run {0} script...", scriptName));
                            mySqlSetup.ExecuteScript(scriptPath);
                            progress += 85 / scriptNames.Count;
                            UpdateProgressBar(progress);
                            AppendStatusMessage(string.Format("{0} ran successfully.", scriptName));
                            AppendStatusMessage(string.Empty);
                        }

                        // Set up the initial historian.
                        if (Convert.ToBoolean(m_state["setupHistorian"]))
                            SetUpInitialHistorian(mySqlSetup.ConnectionString, dataProviderString);

                        // Create new MySQL database user.
                        if (createNewUser)
                        {
                            string user = m_state["newMySqlUserName"].ToString();
                            string pass = m_state["newMySqlUserPassword"].ToString();
                            AppendStatusMessage(string.Format("Attempting to create new user {0}...", user));

                            mySqlSetup.ExecuteStatement(string.Format("GRANT SELECT, UPDATE, INSERT, DELETE ON {0}.* TO {1} IDENTIFIED BY '{2}'", mySqlSetup.DatabaseName, user, pass));

                            mySqlSetup.UserName = user;
                            mySqlSetup.Password = pass;

                            UpdateProgressBar(90);
                            AppendStatusMessage("New database user created successfully.");
                            AppendStatusMessage(string.Empty);
                        }

                        if (!migrate)
                        {
                            SetUpStatisticsHistorian(mySqlSetup.ConnectionString, dataProviderString);
                            SetupAdminUserCredentials(mySqlSetup.ConnectionString, dataProviderString);
                            UpdateProgressBar(95);
                        }
                    }
                    else
                    {
                        this.CanGoBack = true;
                        ScreenManager sm = m_state["screenManager"] as ScreenManager;
                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            while (!(sm.CurrentScreen is MySqlDatabaseSetupScreen))
                                sm.GoToPreviousScreen();
                        });
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(mySqlSetup.ConnectionString, dataProviderString);
                }

                // Modify the openHistorian configuration file.
                ModifyConfigFiles(mySqlSetup.ConnectionString, dataProviderString, Convert.ToBoolean(m_state["encryptMySqlConnectionStrings"]));
                SaveOldConnectionString();

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                ((App)Application.Current).ErrorLogger.Log(ex);
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        // Called when the user has asked to set up a SQL Server database.
        private void SetUpSqlServerDatabase()
        {
            try
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                SqlServerSetup adminSqlServerSetup = m_state["sqlServerSetup"] as SqlServerSetup;
                m_state["newConnectionString"] = adminSqlServerSetup.ConnectionString;
                m_state["newDataProviderString"] = adminSqlServerSetup.DataProviderString;

                // Create a copy of the SqlServerSetup so that it can be manipulated independently
                SqlServerSetup sqlServerSetup = new SqlServerSetup();
                sqlServerSetup.ConnectionString = adminSqlServerSetup.ConnectionString;
                sqlServerSetup.DataProviderString = adminSqlServerSetup.DataProviderString;
                sqlServerSetup.Timeout = "5";

                if (!existing || migrate)
                {
                    if (!CheckIfDatabaseExists(sqlServerSetup.NonPooledConnectionString, sqlServerSetup.DataProviderString, sqlServerSetup.DatabaseName))
                    {
                        List<string> scriptNames = new List<string>();
                        bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                        bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);
                        bool enableAuditLog = Convert.ToBoolean(m_state["enableAuditLog"]);
                        int progress = 0;

                        // Determine which scripts need to be run.
                        scriptNames.Add("openHistorian.sql");

                        if (initialDataScript)
                        {
                            scriptNames.Add("InitialDataSet.sql");

                            if (sampleDataScript)
                                scriptNames.Add("SampleDataSet.sql");
                        }

                        if (enableAuditLog)
                            scriptNames.Add("AuditLog.sql");

                        foreach (string scriptName in scriptNames)
                        {
                            string scriptPath = Directory.GetCurrentDirectory() + "\\Database scripts\\SQL Server\\" + scriptName;
                            AppendStatusMessage(string.Format("Attempting to run {0} script...", scriptName));
                            sqlServerSetup.ExecuteScript(scriptPath);
                            progress += 80 / scriptNames.Count;
                            UpdateProgressBar(progress);
                            AppendStatusMessage(string.Format("{0} ran successfully.", scriptName));
                            AppendStatusMessage(string.Empty);
                        }

                        // Set up the initial historian.
                        if (Convert.ToBoolean(m_state["setupHistorian"]))
                            SetUpInitialHistorian(sqlServerSetup.NonPooledConnectionString, sqlServerSetup.DataProviderString);

                        // Create new SQL Server database user.
                        if (Convert.ToBoolean(m_state["createNewSqlServerUser"]))
                        {
                            string userName = m_state["newSqlServerUserName"].ToString();
                            string password = m_state["newSqlServerUserPassword"].ToString();

                            AppendStatusMessage(string.Format("Attempting to create new login {0}...", userName));
                            sqlServerSetup.CreateLogin(userName, password);
                            AppendStatusMessage("Database login created successfully.");

                            AppendStatusMessage(string.Format("Attempting to grant access to database {0} for login {1}...", sqlServerSetup.DatabaseName, userName));
                            sqlServerSetup.GrantDatabaseAccess(userName, userName, "db_datareader");
                            sqlServerSetup.GrantDatabaseAccess(userName, userName, "db_datawriter");
                            AppendStatusMessage("Database access granted successfully.");

                            sqlServerSetup.UserName = userName;
                            sqlServerSetup.Password = password;

                            sqlServerSetup.IntegratedSecurity = null;

                            AppendStatusMessage("");
                            UpdateProgressBar(90);
                        }
                        else if ((object)sqlServerSetup.IntegratedSecurity != null)
                        {
                            const string GroupName = "openHistorian Admins";
                            string host = sqlServerSetup.HostName.Split('\\')[0].Trim();

                            bool useGroupLogin = UserInfo.LocalGroupExists(GroupName) && (host == "." || Transport.IsLocalAddress(host));
                            string serviceAccountName = GetServiceAccountName();
                            string groupAccountName = useGroupLogin ? string.Format(@"{0}\{1}", Environment.MachineName, GroupName) : null;

                            if ((object)serviceAccountName != null && serviceAccountName.Equals("LocalSystem", StringComparison.InvariantCultureIgnoreCase))
                                serviceAccountName = @"NT Authority\System";

                            string[] loginNames = new string[] { groupAccountName, serviceAccountName };

                            foreach (string loginName in loginNames)
                            {
                                if ((object)loginName != null)
                                {
                                    AppendStatusMessage(string.Format("Attempting to add Windows authenticated database login for {0}...", loginName));
                                    sqlServerSetup.CreateLogin(loginName);
                                    AppendStatusMessage("Database login created successfully.");

                                    AppendStatusMessage(string.Format("Attempting to grant access to database {0} for login {1}...", sqlServerSetup.DatabaseName, loginName));
                                    sqlServerSetup.GrantDatabaseAccess(loginName, loginName, "db_datareader");
                                    sqlServerSetup.GrantDatabaseAccess(loginName, loginName, "db_datawriter");
                                    AppendStatusMessage("Database access granted successfully.");

                                    AppendStatusMessage("");
                                }
                            }

                            UpdateProgressBar(90);
                        }

                        if (!migrate)
                        {
                            SetUpStatisticsHistorian(sqlServerSetup.NonPooledConnectionString, sqlServerSetup.DataProviderString);
                            SetupAdminUserCredentials(sqlServerSetup.NonPooledConnectionString, sqlServerSetup.DataProviderString);
                            UpdateProgressBar(95);
                        }
                    }
                    else
                    {
                        CanGoBack = true;

                        if (m_state.TryGetValue("screenManager", out object obj))
                        {
                            ScreenManager screenManager = obj as ScreenManager;

                            if ((object)screenManager != null)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    while (!(screenManager.CurrentScreen is SqlServerDatabaseSetupScreen))
                                        screenManager.GoToPreviousScreen();
                                });
                            }
                        }
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(sqlServerSetup.NonPooledConnectionString, sqlServerSetup.DataProviderString);
                }

                // Modify the openHistorian configuration file.
                ModifyConfigFiles(sqlServerSetup.ConnectionString, sqlServerSetup.DataProviderString, Convert.ToBoolean(m_state["encryptSqlServerConnectionStrings"]));
                SaveOldConnectionString();

                // Now that config files have been modified, we can get the old connection string before database
                // migration if the database is in fact being migrated. We open a connection to query database
                // user info so that can be migrated to the new database.
                bool migrateUsers =
                    migrate &&
                    (object)sqlServerSetup.IntegratedSecurity != null &&
                    m_state.ContainsKey("oldConnectionString") &&
                    m_state.ContainsKey("oldDataProviderString");

                if (migrateUsers)
                {
                    string oldConnectionString = m_state["oldConnectionString"].ToString();
                    string oldDataProviderString = m_state["oldDataProviderString"].ToString();

                    using (AdoDataConnection connection = new AdoDataConnection(oldConnectionString, oldDataProviderString))
                    {
                        if (connection.IsSQLServer)
                        {
                            AppendStatusMessage("Attempting to grant database access to existing user accounts and groups...");

                            string existingUsersQuery =
                                "SELECT " +
                                "    sysusers.name UserName, " +
                                "    syslogins.name LoginName, " +
                                "    sysroles.name RoleName " +
                                "FROM " +
                                "    sys.sysusers JOIN " +
                                "    sys.database_role_members ON sysusers.uid = database_role_members.member_principal_id JOIN " +
                                "    sys.sysusers sysroles ON database_role_members.role_principal_id = sysroles.uid JOIN " +
                                "    sys.syslogins ON sysusers.sid = syslogins.sid";

                            DataTable existingUsers = connection.RetrieveData(existingUsersQuery);
                            string[] adminRoles = { "db_datareader", "db_datawriter" };

                            foreach (DataRow row in existingUsers.Rows)
                            {
                                string userName = row.ConvertField<string>("UserName");
                                string loginName = row.ConvertField<string>("LoginName");
                                string roleName = row.ConvertField<string>("RoleName");

                                string[] roles = (roleName != "openHistorianAdminRole")
                                    ? new[] { roleName }
                                    : adminRoles;

                                if (userName == "dbo")
                                    userName = loginName;

                                AppendStatusMessage($"Granting database access to {loginName}...");

                                foreach (string role in roles)
                                    adminSqlServerSetup.GrantDatabaseAccess(userName, loginName, role);

                                AppendStatusMessage("Database access granted successfully.");
                            }

                            AppendStatusMessage("");
                        }
                    }
                }

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                ((App)Application.Current).ErrorLogger.Log(ex);
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        // Called when the user has asked to set up an Oracle database.
        private void SetUpOracleDatabase()
        {
            OracleSetup oracleSetup = null;

            try
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                string dataProviderString;

                oracleSetup = m_state["oracleSetup"] as OracleSetup;
                m_state["newConnectionString"] = oracleSetup.ConnectionString;

                // Get user customized data provider string
                dataProviderString = oracleSetup.DataProviderString;

                if (string.IsNullOrWhiteSpace(dataProviderString))
                    dataProviderString = OracleSetup.DefaultDataProviderString;

                m_state["newDataProviderString"] = dataProviderString;

                if (!existing || migrate)
                {
                    if (!oracleSetup.CreateNewSchema || !CheckIfDatabaseExists(oracleSetup.AdminConnectionString, dataProviderString, oracleSetup.SchemaUserName))
                    {
                        IDbConnection dbConnection = null;
                        List<string> scriptNames = new List<string>();
                        bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                        bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);
                        bool enableAuditLog = Convert.ToBoolean(m_state["enableAuditLog"]);
                        bool createNewSchema = oracleSetup.CreateNewSchema;
                        int progress = 0;

                        // Determine which scripts need to be run.
                        scriptNames.Add("openHistorian.sql");
                        if (initialDataScript)
                        {
                            scriptNames.Add("InitialDataSet.sql");
                            if (sampleDataScript)
                                scriptNames.Add("SampleDataSet.sql");
                        }

                        if (enableAuditLog)
                            scriptNames.Add("AuditLog.sql");

                        // Create new Oracle database user.
                        if (createNewSchema)
                        {
                            string user = oracleSetup.SchemaUserName;

                            AppendStatusMessage(string.Format("Attempting to create new user {0}...", user));
                            oracleSetup.ExecuteStatement(string.Format("CREATE TABLESPACE {0}_TS DATAFILE '{1}.dbf' SIZE 20M AUTOEXTEND ON", user.TruncateRight(27), user));
                            oracleSetup.ExecuteStatement(string.Format("CREATE TABLESPACE {0}_INDEX DATAFILE '{1}_index.dbf' SIZE 20M AUTOEXTEND ON", user.TruncateRight(24), user));
                            oracleSetup.ExecuteStatement(string.Format("CREATE USER {0} IDENTIFIED BY {1} DEFAULT TABLESPACE {2}_TS", user, oracleSetup.SchemaPassword, user.TruncateRight(27)));
                            oracleSetup.ExecuteStatement(string.Format("GRANT UNLIMITED TABLESPACE TO {0}", user));
                            oracleSetup.ExecuteStatement(string.Format("GRANT CREATE SESSION TO {0}", user));

                            UpdateProgressBar(8);
                            AppendStatusMessage("New database user created successfully.");
                            AppendStatusMessage(string.Empty);
                        }

                        try
                        {
                            oracleSetup.OpenAdminConnection(ref dbConnection);
                            oracleSetup.ExecuteStatement(dbConnection, string.Format("ALTER SESSION SET CURRENT_SCHEMA = {0}", oracleSetup.SchemaUserName));

                            foreach (string scriptName in scriptNames)
                            {
                                string scriptPath = Directory.GetCurrentDirectory() + "\\Database scripts\\Oracle\\" + scriptName;
                                AppendStatusMessage(string.Format("Attempting to run {0} script...", scriptName));
                                oracleSetup.ExecuteScript(dbConnection, scriptPath);
                                progress += 90 / scriptNames.Count;
                                UpdateProgressBar(progress);
                                AppendStatusMessage(string.Format("{0} ran successfully.", scriptName));
                                AppendStatusMessage(string.Empty);
                            }
                        }
                        finally
                        {
                            if ((object)dbConnection != null)
                                dbConnection.Dispose();
                        }

                        // Set up the initial historian.
                        if (Convert.ToBoolean(m_state["setupHistorian"]))
                            SetUpInitialHistorian(oracleSetup.ConnectionString, dataProviderString);

                        if (!migrate)
                        {
                            SetUpStatisticsHistorian(oracleSetup.ConnectionString, dataProviderString);
                            SetupAdminUserCredentials(oracleSetup.ConnectionString, dataProviderString);
                            UpdateProgressBar(95);
                        }
                    }
                    else
                    {
                        this.CanGoBack = true;
                        ScreenManager sm = m_state["screenManager"] as ScreenManager;
                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            while (!(sm.CurrentScreen is OracleDatabaseSetupScreen))
                                sm.GoToPreviousScreen();
                        });
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(oracleSetup.ConnectionString, dataProviderString);
                }

                // Modify the openHistorian configuration file.
                string connectionString = oracleSetup.ConnectionString;
                ModifyConfigFiles(connectionString, dataProviderString, oracleSetup.EncryptConnectionString);
                SaveOldConnectionString();

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                ((App)Application.Current).ErrorLogger.Log(ex);
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        // Called when the user has asked to set up a sqlite database.
        private void SetUpSqliteDatabase()
        {
            try
            {
                const string GroupName = "openHistorian Admins";
                DirectorySecurity destinationSecurity;
                string loginName;

                string filePath = null;
                string destination = m_state["sqliteDatabaseFilePath"].ToString();
                string destinationDirectory = Path.GetDirectoryName(destination);
                string connectionString = "Data Source=" + destination + "; Version=3; Foreign Keys=True; FailIfMissing=True";
                string dataProviderString = SQLiteDataProviderString;
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                m_state["newConnectionString"] = connectionString;
                m_state["newDataProviderString"] = dataProviderString;

                if (!existing || migrate)
                {
                    bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                    bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);

                    if (!initialDataScript)
                        filePath = Directory.GetCurrentDirectory() + "\\Database scripts\\SQLite\\" + App.BaseSqliteConfig;
                    else if (!sampleDataScript)
                        filePath = Directory.GetCurrentDirectory() + "\\Database scripts\\SQLite\\" + App.SqliteInitialData;
                    else
                        filePath = Directory.GetCurrentDirectory() + "\\Database scripts\\SQLite\\" + App.SqliteSampleData;

                    UpdateProgressBar(2);
                    AppendStatusMessage(string.Format("Attempting to copy file {0} to {1}...", filePath, destination));

                    // Create directory and set permissions
                    if ((object)destinationDirectory != null)
                    {
                        if (!Directory.Exists(destinationDirectory))
                            Directory.CreateDirectory(destinationDirectory);

                        loginName = UserInfo.LocalGroupExists(GroupName) ? string.Format(@"{0}\{1}", Environment.MachineName, GroupName) : GetServiceAccountName();

                        if ((object)loginName != null && !loginName.Equals("Local System", StringComparison.InvariantCultureIgnoreCase))
                        {
                            destinationSecurity = Directory.GetAccessControl(destinationDirectory);
                            destinationSecurity.AddAccessRule(new FileSystemAccessRule(loginName, FileSystemRights.ListDirectory, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                            destinationSecurity.AddAccessRule(new FileSystemAccessRule(loginName, FileSystemRights.DeleteSubdirectoriesAndFiles, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                            destinationSecurity.AddAccessRule(new FileSystemAccessRule(loginName, FileSystemRights.Read, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                            destinationSecurity.AddAccessRule(new FileSystemAccessRule(loginName, FileSystemRights.Read, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                            destinationSecurity.AddAccessRule(new FileSystemAccessRule(loginName, FileSystemRights.Write, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                            destinationSecurity.AddAccessRule(new FileSystemAccessRule(loginName, FileSystemRights.Write, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                            Directory.SetAccessControl(destinationDirectory, destinationSecurity);
                        }
                    }

                    // Copy the file to the specified path.
                    File.Copy(filePath, destination, true);
                    UpdateProgressBar(90);
                    AppendStatusMessage("File copy successful.");
                    AppendStatusMessage(string.Empty);

                    // Set up the initial historian.
                    if (Convert.ToBoolean(m_state["setupHistorian"]))
                        SetUpInitialHistorian(connectionString, dataProviderString);

                    if (!migrate)
                    {
                        SetUpStatisticsHistorian(connectionString, dataProviderString);
                        SetupAdminUserCredentials(connectionString, dataProviderString);
                        UpdateProgressBar(95);
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(connectionString, dataProviderString);
                }

                // Modify the openHistorian configuration file.
                ModifyConfigFiles(connectionString, dataProviderString, false);
                SaveOldConnectionString();

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                ((App)Application.Current).ErrorLogger.Log(ex);
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        // Called when the user has asked to set up an Oracle database.
        private void SetUpPostgresDatabase()
        {
            try
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                PostgresSetup postgresSetup = m_state["postgresSetup"] as PostgresSetup;
                string databaseName = postgresSetup.DatabaseName;

                m_state["newConnectionString"] = postgresSetup.ConnectionString;
                m_state["newDataProviderString"] = PostgresSetup.DataProviderString;

                if (!existing || migrate)
                {
                    bool dbExists;
                    bool cancelDBSetup;

                    m_state["cancelDBSetup"] = false;

                    try
                    {
                        postgresSetup.DatabaseName = null;
                        dbExists = CheckIfDatabaseExists(postgresSetup.AdminConnectionString, PostgresSetup.DataProviderString, databaseName);
                    }
                    finally
                    {
                        postgresSetup.DatabaseName = databaseName;
                    }

                    cancelDBSetup = Convert.ToBoolean(m_state["cancelDBSetup"]);

                    if (!cancelDBSetup)
                    {
                        IDbConnection dbConnection = null;
                        List<string> scriptNames = new List<string>();
                        bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                        bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);
                        bool enableAuditLog = Convert.ToBoolean(m_state["enableAuditLog"]);
                        int progress = 0;

                        // Determine which scripts need to be run.
                        scriptNames.Add("openHistorian.sql");
                        if (initialDataScript)
                        {
                            scriptNames.Add("InitialDataSet.sql");
                            if (sampleDataScript)
                                scriptNames.Add("SampleDataSet.sql");
                        }

                        if (enableAuditLog)
                            scriptNames.Add("AuditLog.sql");

                        // Create new Oracle database user.
                        if (!dbExists)
                        {
                            try
                            {
                                postgresSetup.DatabaseName = null;
                                AppendStatusMessage($"Attempting to create new database {databaseName}...");
                                postgresSetup.ExecuteStatement($"CREATE DATABASE {databaseName}");
                            }
                            finally
                            {
                                postgresSetup.DatabaseName = databaseName;
                            }

                            if (!string.IsNullOrEmpty(postgresSetup.RoleName))
                            {
                                AppendStatusMessage($"Attempting to create new role {postgresSetup.RoleName}...");
                                
                                if ((object)postgresSetup.RolePassword != null && postgresSetup.RolePassword.Length > 0)
                                    postgresSetup.CreateLogin(postgresSetup.RoleName.ToLower(), postgresSetup.RolePassword.ToUnsecureString());
                                else
                                    postgresSetup.CreateLogin(postgresSetup.RoleName.ToLower());
                            }

                            UpdateProgressBar(8);
                            AppendStatusMessage("New database created successfully.");
                            AppendStatusMessage(string.Empty);
                        }

                        try
                        {
                            postgresSetup.OpenAdminConnection(ref dbConnection);

                            foreach (string scriptName in scriptNames)
                            {
                                string scriptPath = Directory.GetCurrentDirectory() + "\\Database scripts\\PostgreSQL\\" + scriptName;
                                AppendStatusMessage($"Attempting to run {scriptName} script...");
                                postgresSetup.ExecuteScript(dbConnection, scriptPath);
                                progress += 90 / scriptNames.Count;
                                UpdateProgressBar(progress);
                                AppendStatusMessage($"{scriptName} ran successfully.");
                                AppendStatusMessage(string.Empty);
                            }
                        }
                        finally
                        {
                            if ((object)dbConnection != null)
                                dbConnection.Dispose();
                        }

                        // Grant access to the database for the new user
                        if (!string.IsNullOrEmpty(postgresSetup.RoleName))
                        {
                            AppendStatusMessage($"Attempting to grant database access to specified role...");
                            postgresSetup.GrantDatabaseAccess(postgresSetup.RoleName.ToLower());
                            AppendStatusMessage($"Successfully granted database access.");
                            AppendStatusMessage($"");
                        }

                        // Set up the initial historian.
                        if (Convert.ToBoolean(m_state["setupHistorian"]))
                            SetUpInitialHistorian(postgresSetup.ConnectionString + "; Pooling=false", PostgresSetup.DataProviderString);

                        if (!migrate)
                        {
                            SetUpStatisticsHistorian(postgresSetup.ConnectionString + "; Pooling=false", PostgresSetup.DataProviderString);
                            SetupAdminUserCredentials(postgresSetup.ConnectionString + "; Pooling=false", PostgresSetup.DataProviderString);
                            UpdateProgressBar(95);
                        }
                    }
                    else
                    {
                        this.CanGoBack = true;
                        ScreenManager sm = m_state["screenManager"] as ScreenManager;
                        this.Dispatcher.Invoke((Action)delegate ()
                        {
                            while (!(sm.CurrentScreen is PostgresDatabaseSetupScreen))
                                sm.GoToPreviousScreen();
                        });
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(postgresSetup.ConnectionString, PostgresSetup.DataProviderString);
                }

                // Modify the openHistorian configuration file.
                string connectionString = postgresSetup.ConnectionString;
                ModifyConfigFiles(connectionString, PostgresSetup.DataProviderString, postgresSetup.EncryptConnectionString);
                SaveOldConnectionString();

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                ((App)Application.Current).ErrorLogger.Log(ex);
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        /// <summary>
        /// Gets the account name that the openHistorian service is running under.
        /// </summary>
        /// <returns>The account name that the openHistorian service is running under.</returns>
        private string GetServiceAccountName()
        {
            SelectQuery selectQuery = new SelectQuery(string.Format("select name, startname from Win32_Service where name = '{0}'", "openHistorian"));

            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(selectQuery))
            {
                ManagementObject service = managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault();
                return ((object)service != null) ? service["startname"].ToString() : null;
            }
        }

        /// <summary>
        /// Checks if user requested database already exists.
        /// </summary>
        /// <param name="connectionString">Connection string to the database server.</param>
        /// <param name="dataProviderString">Data provider string.</param>
        /// <param name="databaseName">Name of the database to check for.</param>
        /// <returns>returns true if database exists and user says no to database delete, false if database does not exist or user says yes to database delete.</returns>
        private bool CheckIfDatabaseExists(string connectionString, string dataProviderString, string databaseName)
        {
            AppendStatusMessage(string.Format("Checking if database {0} already exists.", databaseName));

            Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = dataProviderSettings["AssemblyName"];
            string connectionTypeName = dataProviderSettings["ConnectionType"];

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            IDbConnection connection = null;

            try
            {
                int dbCount = 0;

                try
                {
                    connection = (IDbConnection)Activator.CreateInstance(connectionType);
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    IDbCommand command = connection.CreateCommand();

                    if (m_state["newDatabaseType"].ToString() == "SQLServer")
                        command.CommandText = string.Format("SELECT COUNT(*) FROM sys.databases WHERE name = '{0}'", databaseName);
                    else if (m_state["newDatabaseType"].ToString() == "Oracle")
                        command.CommandText = string.Format("SELECT COUNT(*) FROM all_users WHERE USERNAME = '{0}'", databaseName.ToUpper());
                    else if (m_state["newDatabaseType"].ToString() == "PostgreSQL")
                        command.CommandText = string.Format("SELECT COUNT(*) FROM pg_database WHERE datname = '{0}'", databaseName.ToLower());
                    else
                        command.CommandText = string.Format("SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}'", databaseName);

                    dbCount = Convert.ToInt32(command.ExecuteScalar());
                }
                catch
                {
                    // If we cannot open connection then assume database does not exist. If for some other reason, connection or query failed then during script run, it will fail gracefully.
                    return false;
                }
                finally
                {
                    if (connection != null)
                        connection.Dispose();
                }

                MessageBoxResult messageBoxResult;

                if (dbCount > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    if (m_state["newDatabaseType"].ToString() != "PostgreSQL")
                    {
                        sb.AppendFormat("Database \"{0}\" already exists.\r\n", databaseName);
                        sb.AppendLine();
                        sb.AppendLine("    Click YES to delete existing database.");
                        sb.AppendLine("    Click NO to go back to change database name.");
                        sb.AppendLine();
                        sb.AppendLine("WARNING: If you delete the existing database ALL configuration in that database will be permanently deleted.");

                        messageBoxResult = MessageBox.Show(sb.ToString(), "Database Exists!", MessageBoxButton.YesNo);
                    }
                    else
                    {
                        sb.AppendFormat("Database \"{0}\" already exists.\r\n", databaseName);
                        sb.AppendLine();
                        sb.AppendLine("    Click YES to delete existing database.");
                        sb.AppendLine("    Click NO to apply scripts to existing database.");
                        sb.AppendLine("    Click CANCEL to go back to change database name.");
                        sb.AppendLine();
                        sb.AppendLine("WARNING: If you delete the existing database ALL configuration in that database will be permanently deleted.");

                        messageBoxResult = MessageBox.Show(sb.ToString(), "Database Exists!", MessageBoxButton.YesNoCancel);
                        m_state["cancelDBSetup"] = (messageBoxResult == MessageBoxResult.Cancel);
                    }

                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        if (m_state["newDatabaseType"].ToString() == "SQLServer")
                        {
                            SqlServerSetup sqlServerSetup = m_state["sqlServerSetup"] as SqlServerSetup;
                            sqlServerSetup.DatabaseName = "master";

                            try
                            {
                                sqlServerSetup.ExecuteStatement(string.Format("USE [master] ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE DROP DATABASE {0}", databaseName));
                                AppendStatusMessage(string.Format("Dropped database {0} successfully.", databaseName));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("Failed to delete database {0} due to exception: {1}", databaseName, ex.Message), "Delete Database Failed");
                            }

                            sqlServerSetup.DatabaseName = databaseName;
                        }
                        else if (m_state["newDatabaseType"].ToString() == "Oracle")
                        {
                            OracleSetup oracleSetup = m_state["oracleSetup"] as OracleSetup;

                            try
                            {
                                oracleSetup.ExecuteStatement(string.Format("DROP USER {0} CASCADE", databaseName));
                                oracleSetup.ExecuteStatement(string.Format("DROP TABLESPACE {0}_TS INCLUDING CONTENTS AND DATAFILES", databaseName.TruncateRight(27)));
                                oracleSetup.ExecuteStatement(string.Format("DROP TABLESPACE {0}_INDEX INCLUDING CONTENTS AND DATAFILES", databaseName.TruncateRight(24)));
                                AppendStatusMessage(string.Format("Dropped database {0} successfully.", databaseName));
                            }
                            catch
                            {
                                MessageBox.Show(string.Format("Failed to delete database {0}", databaseName), "Delete Database Failed");
                            }
                        }
                        else if (m_state["newDatabaseType"].ToString() == "PostgreSQL")
                        {
                            PostgresSetup postgresSetup = m_state["postgresSetup"] as PostgresSetup;

                            try
                            {
                                postgresSetup.ExecuteStatement($"DROP DATABASE {databaseName.ToLower()}");
                            }
                            catch (Exception)
                            {
                                MessageBox.Show($"Failed to delete database {databaseName}", "Delete Database Failed");
                            }
                        }
                        else
                        {
                            try
                            {
                                MySqlSetup mySqlSetup = m_state["mySqlSetup"] as MySqlSetup;
                                mySqlSetup.ExecuteStatement(string.Format("DROP DATABASE {0}", databaseName));
                                AppendStatusMessage(string.Format("Dropped database {0} successfully.", databaseName));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(string.Format("Failed to delete database {0} due to exception: {1}", databaseName, ex.Message), "Delete Database Failed");
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        private string[] GetExistingLoginNames(string connectionString, string dataProviderString)
        {
            const string SelectQuery = "SELECT Name FROM UserAccount WHERE LockedOut = 0 " +
                "UNION ALL SELECT Name FROM SecurityGroup";

            string[] loginNames;

            Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = dataProviderSettings["AssemblyName"];
            string connectionTypeName = dataProviderSettings["ConnectionType"];
            string adapterTypeName = dataProviderSettings["AdapterType"];

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            Type adapterType = assembly.GetType(adapterTypeName);
            IDbConnection connection = null;
            DataTable loginNamesTable;

            try
            {
                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = connectionString;
                connection.Open();

                loginNamesTable = connection.RetrieveData(adapterType, SelectQuery);

                loginNames = loginNamesTable.Select()
                    .Select(row => row["Name"].ToNonNullString())
                    .Select(UserInfo.SIDToAccountName)
                    .ToArray();
            }
            finally
            {
                if ((object)connection != null)
                    connection.Dispose();
            }

            return loginNames;
        }

        // Called when the user has asked to set up an XML configuration.
        private void SetUpXmlConfiguration()
        {
            try
            {
                // Modify the openHistorian configuration file.
                ModifyConfigFiles(m_state["xmlFilePath"].ToString(), string.Empty, false);

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        // Called when the user has asked to set up a web service configuration.
        private void SetUpWebServiceConfiguration()
        {
            try
            {
                // Modify the openHistorian configuration file.
                ModifyConfigFiles(m_state["webServiceUrl"].ToString(), string.Empty, false);

                // Remove cached configuration since it will
                // likely be different from the new configuration
                RemoveCachedConfiguration();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
        }

        // Sets up the initial historian in new configurations.
        private void SetUpInitialHistorian(string connectionString, string dataProviderString)
        {
            bool initialDataScript = Convert.ToBoolean(m_state["initialDataScript"]);
            bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);

            string historianAssemblyName = m_state["historianAssemblyName"].ToString();
            string historianTypeName = m_state["historianTypeName"].ToString();
            string historianAcronym = m_state["historianAcronym"].ToString();
            string historianName = m_state["historianName"].ToString();
            string historianDescription = m_state["historianDescription"].ToString();
            string historianConnectionString = m_state["historianConnectionString"].ToString();

            Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = dataProviderSettings["AssemblyName"];
            string connectionTypeName = dataProviderSettings["ConnectionType"];

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            IDbConnection connection = null;

            try
            {
                IDbCommand historianCommand;
                string nodeIdQueryString = null;

                AppendStatusMessage("Attempting to set up the initial historian...");

                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = connectionString;
                connection.Open();

                // Set up default node.
                bool defaultNodeCreatedHere = false;
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (!migrate)
                    defaultNodeCreatedHere = ManageDefaultNode(connection, sampleDataScript, m_defaultNodeAdded);

                if (string.IsNullOrWhiteSpace(nodeIdQueryString))
                    nodeIdQueryString = "'" + m_state["selectedNodeId"].ToString() + "'";

                if (defaultNodeCreatedHere)
                    AddRolesForNode(connection, nodeIdQueryString);

                // Set up initial historian.
                historianCommand = connection.CreateCommand();

                if (sampleDataScript)
                    historianCommand.CommandText = string.Format("UPDATE Historian SET AssemblyName='{0}', TypeName='{1}', Acronym='{2}', Name='{3}', Description='{4}', ConnectionString='{5}'", historianAssemblyName, historianTypeName, historianAcronym, historianName, historianDescription, historianConnectionString);
                else
                    historianCommand.CommandText = string.Format("INSERT INTO Historian(NodeID, Acronym, Name, AssemblyName, TypeName, ConnectionString, IsLocal, Description, LoadOrder, Enabled) VALUES({0}, '{3}', '{4}', '{1}', '{2}', '{6}', 0, '{5}', 0, 1)", nodeIdQueryString, historianAssemblyName, historianTypeName, historianAcronym, historianName, historianDescription, historianConnectionString);

                historianCommand.ExecuteNonQuery();

                // Report success to the user.
                AppendStatusMessage("Successfully set up initial historian.");
                AppendStatusMessage(string.Empty);
                UpdateProgressBar(95);
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        // Sets up the statistics historian in new configurations.
        private void SetUpStatisticsHistorian(string connectionString, string dataProviderString)
        {
            bool initialDataScript = Convert.ToBoolean(m_state["initialDataScript"]);
            bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);

            Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = dataProviderSettings["AssemblyName"];
            string connectionTypeName = dataProviderSettings["ConnectionType"];

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            IDbConnection connection = null;

            try
            {
                string nodeIdQueryString = null;
                int statHistorianCount;

                AppendStatusMessage("Attempting to set up the statistics historian...");

                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = connectionString;
                connection.Open();

                // Set up default node.
                bool defaultNodeCreatedHere = false;
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (!migrate)
                    defaultNodeCreatedHere = ManageDefaultNode(connection, sampleDataScript, m_defaultNodeAdded);

                if (string.IsNullOrWhiteSpace(nodeIdQueryString))
                    nodeIdQueryString = "'" + m_state["selectedNodeId"].ToString() + "'";

                if (defaultNodeCreatedHere)
                    AddRolesForNode(connection, nodeIdQueryString);

                // Set up statistics historian.
                statHistorianCount = Convert.ToInt32(connection.ExecuteScalar(string.Format("SELECT COUNT(*) FROM Historian WHERE Acronym = 'STAT' AND NodeID = {0}", nodeIdQueryString)));

                if (statHistorianCount == 0)
                    connection.ExecuteNonQuery(string.Format("INSERT INTO Historian(NodeID, Acronym, Name, AssemblyName, TypeName, ConnectionString, IsLocal, Description, LoadOrder, Enabled) VALUES({0}, 'STAT', 'Statistics Archive', 'HistorianAdapters.dll', 'HistorianAdapters.LocalOutputAdapter', '', 1, 'Local historian used to archive system statistics', 9999, 1)", nodeIdQueryString));

                // Report success to the user.
                AppendStatusMessage("Successfully set up statistics historian.");
                AppendStatusMessage(string.Empty);
                UpdateProgressBar(95);
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        // Sets up administrative user credentials in the database.
        private void SetupAdminUserCredentials(string connectionString, string dataProviderString)
        {
            bool sampleDataScript = Convert.ToBoolean(m_state["initialDataScript"]) && Convert.ToBoolean(m_state["sampleDataScript"]);

            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
            Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = dataProviderSettings["AssemblyName"];
            string connectionTypeName = dataProviderSettings["ConnectionType"];
            string accountName = string.Empty;
            string adminRoleID = string.Empty;
            string adminUserID = string.Empty;

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            IDbConnection connection = null;

            try
            {
                string nodeIdQueryString = null;

                AppendStatusMessage("Attempting to set up administrative user...");

                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = connectionString;
                connection.Open();

                bool defaultNodeCreatedHere = false;
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (!migrate)
                    defaultNodeCreatedHere = ManageDefaultNode(connection, sampleDataScript, m_defaultNodeAdded);

                if (string.IsNullOrWhiteSpace(nodeIdQueryString))
                    nodeIdQueryString = "'" + m_state["selectedNodeId"].ToString() + "'";

                if (defaultNodeCreatedHere)
                    AddRolesForNode(connection, nodeIdQueryString);

                // Get Administrative RoleID
                IDbCommand roleIdCommand;
                IDataReader roleIdReader = null;

                // Get the node ID from the database.
                roleIdCommand = connection.CreateCommand();
                roleIdCommand.CommandText = "SELECT ID FROM ApplicationRole WHERE Name = 'Administrator'";
                using (roleIdReader = roleIdCommand.ExecuteReader())
                {
                    if (roleIdReader.Read())
                        adminRoleID = roleIdReader["ID"].ToNonNullString();
                }

                bool oracle = connection.GetType().Name == "OracleConnection";
                char paramChar = oracle ? ':' : '@';

                // Add Administrative User.                
                IDbCommand adminCredentialCommand = connection.CreateCommand();
                if (m_state["authenticationType"].ToString() == "windows")
                {
                    IDbDataParameter nameParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter createdByParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter updatedByParameter = adminCredentialCommand.CreateParameter();

                    accountName = UserInfo.UserNameToSID(m_state["adminUserName"].ToString());

                    nameParameter.ParameterName = paramChar + "name";
                    createdByParameter.ParameterName = paramChar + "createdBy";
                    updatedByParameter.ParameterName = paramChar + "updatedBy";

                    nameParameter.Value = accountName;
                    createdByParameter.Value = Thread.CurrentPrincipal.Identity.Name;
                    updatedByParameter.Value = Thread.CurrentPrincipal.Identity.Name;

                    adminCredentialCommand.Parameters.Add(nameParameter);
                    adminCredentialCommand.Parameters.Add(createdByParameter);
                    adminCredentialCommand.Parameters.Add(updatedByParameter);

                    if (oracle)
                        adminCredentialCommand.CommandText = string.Format("INSERT INTO UserAccount(Name, DefaultNodeID, CreatedBy, UpdatedBy) Values (:name, {0}, :createdBy, :updatedBy)", nodeIdQueryString);
                    else
                        adminCredentialCommand.CommandText = string.Format("INSERT INTO UserAccount(Name, DefaultNodeID, CreatedBy, UpdatedBy) Values (@name, {0}, @createdBy, @updatedBy)", nodeIdQueryString);
                }
                else
                {
                    IDbDataParameter nameParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter passwordParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter firstNameParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter lastNameParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter createdByParameter = adminCredentialCommand.CreateParameter();
                    IDbDataParameter updatedByParameter = adminCredentialCommand.CreateParameter();

                    accountName = m_state["adminUserName"].ToString();

                    nameParameter.ParameterName = paramChar + "name";
                    passwordParameter.ParameterName = paramChar + "password";
                    firstNameParameter.ParameterName = paramChar + "firstName";
                    lastNameParameter.ParameterName = paramChar + "lastName";
                    createdByParameter.ParameterName = paramChar + "createdBy";
                    updatedByParameter.ParameterName = paramChar + "updatedBy";

                    nameParameter.Value = accountName;
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

                    if (oracle)
                        adminCredentialCommand.CommandText = string.Format("INSERT INTO UserAccount(Name, Password, FirstName, LastName, DefaultNodeID, UseADAuthentication, CreatedBy, UpdatedBy) Values " +
                            "(:name, :password, :firstName, :lastName, {0}, 0, :createdBy, :updatedBy)", nodeIdQueryString);
                    else
                        adminCredentialCommand.CommandText = string.Format("INSERT INTO UserAccount(Name, Password, FirstName, LastName, DefaultNodeID, UseADAuthentication, CreatedBy, UpdatedBy) Values " +
                            "(@name, @password, @firstName, @lastName, {0}, 0, @createdBy, @updatedBy)", nodeIdQueryString);
                }

                adminCredentialCommand.ExecuteNonQuery();

                // Get the admin user ID from the database.
                IDataReader userIdReader = null;
                IDbDataParameter newNameParameter = adminCredentialCommand.CreateParameter();

                newNameParameter.ParameterName = paramChar + "name";
                newNameParameter.Value = accountName;

                adminCredentialCommand.CommandText = "SELECT ID FROM UserAccount WHERE Name = " + paramChar + "name";
                adminCredentialCommand.Parameters.Clear();
                adminCredentialCommand.Parameters.Add(newNameParameter);
                using (userIdReader = adminCredentialCommand.ExecuteReader())
                {
                    if (userIdReader.Read())
                        adminUserID = userIdReader["ID"].ToNonNullString();
                }

                // Assign Administrative User to Administrator Role.
                if (!string.IsNullOrEmpty(adminRoleID) && !string.IsNullOrEmpty(adminUserID))
                {
                    adminUserID = "'" + adminUserID + "'";
                    adminRoleID = "'" + adminRoleID + "'";
                    adminCredentialCommand.CommandText = string.Format("INSERT INTO ApplicationRoleUserAccount(ApplicationRoleID, UserAccountID) VALUES ({0}, {1})", adminRoleID, adminUserID);
                    adminCredentialCommand.ExecuteNonQuery();
                }

                // Report success to the user.
                AppendStatusMessage("Successfully set up credentials for administrative user.");
                AppendStatusMessage(string.Empty);
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Checks to see if sample database script was selected to be run. If not, then create default node otherwise assign default nodeID to m_state["selectedNodeId"]
        /// </summary>
        /// <param name="connection">IDbConnection used for database operation</param>
        /// <param name="sampleDataScript">Indicates if sample database script was selected to be run</param>
        /// <param name="defaultNodeHasBeenAdded">indicates if default node has been added previously</param>
        /// <returns>true if new node was added otherwise false</returns>
        private bool ManageDefaultNode(IDbConnection connection, bool sampleDataScript, bool defaultNodeHasBeenAdded)
        {
            bool defaultNodeCreated = false;
            IDbCommand nodeIdCommand;
            IDataReader nodeIdReader = null;
            string nodeId = null;

            // Set up default node if it has not been added to in the SetupDefaultHistorian method above.            
            if (!sampleDataScript && !m_defaultNodeAdded)
            {
                IDbCommand nodeCommand = connection.CreateCommand();
                nodeCommand.CommandText = "INSERT INTO Node(Name, CompanyID, Description, Settings, MenuType, MenuData, Master, LoadOrder, Enabled) " +
                                          "VALUES('Default', NULL, 'Default node', 'RemoteStatusServerConnectionString={server=localhost:8510; integratedSecurity=true}; dataPublisherPort=6175', 'File', 'Menu.xml', 1, 0, 1)";
                nodeCommand.ExecuteNonQuery();
                m_defaultNodeAdded = true;
                defaultNodeCreated = true;
            }

            // Get the node ID from the database.
            nodeIdCommand = connection.CreateCommand();
            nodeIdCommand.CommandText = "SELECT ID FROM Node WHERE Name = 'Default'";

            using (nodeIdReader = nodeIdCommand.ExecuteReader())
            {
                if (nodeIdReader.Read())
                    nodeId = nodeIdReader["ID"].ToNonNullString();

                m_state["selectedNodeId"] = nodeId;
            }

            return defaultNodeCreated;
        }

        /// <summary>
        /// Creates a brand new node based on the selected node ID.
        /// </summary>
        /// <param name="connectionString">Connection string to the database in which the node is to be created.</param>
        /// <param name="dataProviderString">Data provider string used to create database connection.</param>
        private void CreateNewNode(string connectionString, string dataProviderString)
        {
            string insertQuery = "INSERT INTO Node(Name, Description, MenuData, Enabled) VALUES(@name, @description, 'Menu.xml', 1)";
            string updateQuery = "UPDATE Node SET ID = {0} WHERE Name = @name";

            Dictionary<string, string> dataProviderSettings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = dataProviderSettings["AssemblyName"];
            string connectionTypeName = dataProviderSettings["ConnectionType"];

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            IDbConnection connection = null;

            Guid nodeID;
            string nodeIDQueryString = null;
            string name = string.Empty;
            string description = string.Empty;

            AppendStatusMessage("Creating new node...");

            if (!m_state.ContainsKey("selectedNodeId"))
                throw new InvalidOperationException("Attempted to create new node without node selected.");

            if (!m_state.ContainsKey("newNodeName"))
                throw new InvalidOperationException("Attempted to create new node without a name.");

            nodeID = (Guid)m_state["selectedNodeId"];
            name = m_state["newNodeName"].ToString();

            if (m_state.ContainsKey("newNodeDescription"))
                description = m_state["newNodeDescription"].ToNonNullString();

            if (string.IsNullOrWhiteSpace(nodeIDQueryString))
                nodeIDQueryString = "'" + nodeID.ToString() + "'";

            try
            {
                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = connectionString;
                connection.Open();

                // Oracle uses a different character for parameterized queries
                if (connection.GetType().Name == "OracleConnection")
                {
                    insertQuery = insertQuery.Replace('@', ':');
                    updateQuery = updateQuery.Replace('@', ':');
                }

                connection.ExecuteNonQuery(insertQuery, name, description);
                connection.ExecuteNonQuery(string.Format(updateQuery, nodeIDQueryString), name);

                AddRolesForNode(connection, nodeIDQueryString);
            }
            finally
            {
                if ((object)connection != null)
                    connection.Dispose();
            }

            AppendStatusMessage("Successfully created new node.");
        }

        /// <summary>
        /// Adds three default roles for newly added node (Administrator, Editor, Viewer).
        /// </summary>
        /// <param name="connection">IDbConnection to be used for database operations.</param>
        /// <param name="nodeID">Node ID to which three roles are being assigned</param>        
        private void AddRolesForNode(IDbConnection connection, string nodeID)
        {
            // When a new node added, also add 3 roles to it (Administrator, Editor, Viewer).
            IDbCommand adminCredentialCommand;
            adminCredentialCommand = connection.CreateCommand();
            adminCredentialCommand.CommandText = string.Format("INSERT INTO ApplicationRole(Name, Description, NodeID, UpdatedBy, CreatedBy) VALUES('Administrator', 'Administrator Role', {0}, '{1}', '{2}')", nodeID, Thread.CurrentPrincipal.Identity.Name, Thread.CurrentPrincipal.Identity.Name);
            adminCredentialCommand.ExecuteNonQuery();

            adminCredentialCommand.CommandText = string.Format("INSERT INTO ApplicationRole(Name, Description, NodeID, UpdatedBy, CreatedBy) VALUES('Editor', 'Editor Role', {0}, '{1}', '{2}')", nodeID, Thread.CurrentPrincipal.Identity.Name, Thread.CurrentPrincipal.Identity.Name);
            adminCredentialCommand.ExecuteNonQuery();

            adminCredentialCommand.CommandText = string.Format("INSERT INTO ApplicationRole(Name, Description, NodeID, UpdatedBy, CreatedBy) VALUES('Viewer', 'Viewer Role', {0}, '{1}', '{2}')", nodeID, Thread.CurrentPrincipal.Identity.Name, Thread.CurrentPrincipal.Identity.Name);
            adminCredentialCommand.ExecuteNonQuery();
        }

        // Attempt to stop key processes/services before modifying their configuration files
        private void AttemptToStopKeyProcesses()
        {
            m_state["restarting"] = false;

            try
            {
                Process[] instances = Process.GetProcessesByName(App.Manager);

                if (instances.Length > 0)
                {
                    int total = 0;
                    AppendStatusMessage("Attempting to stop running instances of the openHistorian Manager...");

                    // Terminate all instances of openHistorian Manager running on the local computer
                    foreach (Process process in instances)
                    {
                        process.Kill();
                        total++;
                    }

                    if (total > 0)
                        AppendStatusMessage(string.Format("Stopped {0} openHistorian Manager instance{1}.", total, total > 1 ? "s" : ""));

                    // Add an extra line for visual separation of process termination status
                    AppendStatusMessage("");
                }
            }
            catch (Exception ex)
            {
                AppendStatusMessage("Failed to terminate running instances of the openHistorian Manager: " + ex.Message + "\r\nModifications continuing anyway...\r\n");
            }

            // Attempt to access service controller for the openHistorian
            ServiceController openHistorianServiceController = ServiceController.GetServices().SingleOrDefault(svc => string.Compare(svc.ServiceName, "openHistorian", true) == 0);

            if (openHistorianServiceController != null)
            {
                try
                {
                    if (openHistorianServiceController.Status == ServiceControllerStatus.Running)
                    {
                        AppendStatusMessage("Attempting to stop the openHistorian Windows service...");

                        openHistorianServiceController.Stop();

                        // Can't wait forever for service to stop, so we time-out after 20 seconds
                        openHistorianServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20.0D));

                        if (openHistorianServiceController.Status == ServiceControllerStatus.Stopped)
                        {
                            m_state["restarting"] = true;
                            AppendStatusMessage("Successfully stopped the openHistorian Windows service.");
                        }
                        else
                            AppendStatusMessage("Failed to stop the openHistorian Windows service after trying for 20 seconds.\r\nModifications continuing anyway...");

                        // Add an extra line for visual separation of service termination status
                        AppendStatusMessage("");
                    }
                }
                catch (Exception ex)
                {
                    AppendStatusMessage("Failed to stop the openHistorian Windows service: " + ex.Message + "\r\nModifications continuing anyway...\r\n");
                }
            }

            // If the openHistorian service failed to stop or it is installed as stand-alone debug application, we try to stop any remaining running instances
            try
            {
                Process[] instances = Process.GetProcessesByName("openHistorian");

                if (instances.Length > 0)
                {
                    int total = 0;
                    AppendStatusMessage("Attempting to stop running instances of the openHistorian...");

                    // Terminate all instances of openHistorian running on the local computer
                    foreach (Process process in instances)
                    {
                        process.Kill();
                        total++;
                    }

                    if (total > 0)
                        AppendStatusMessage(string.Format("Stopped {0} openHistorian instance{1}.", total, total > 1 ? "s" : ""));

                    // Add an extra line for visual separation of process termination status
                    AppendStatusMessage("");
                }
            }
            catch (Exception ex)
            {
                AppendStatusMessage("Failed to terminate running instances of the openHistorian: " + ex.Message + "\r\nModifications continuing anyway...\r\n");
            }
        }

        // Modifies the configuration files to contain the given connection string and data provider string.
        private void ModifyConfigFiles(string connectionString, string dataProviderString, bool encrypted)
        {
            // Before modification of configuration files we try to stop key process
            AttemptToStopKeyProcesses();

            object webManagerDir = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\openHistorianManagerServices", "Installation Path", null) ?? Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Wow6432Node\\openHistorianManagerServices", "Installation Path", null);
            bool applyChangesToService = Convert.ToBoolean(m_state["applyChangesToService"]);
            bool applyChangesToLocalManager = Convert.ToBoolean(m_state["applyChangesToLocalManager"]);
            bool applyChangesToWebManager = Convert.ToBoolean(m_state["applyChangesToWebManager"]);
            string configFile;

            AppendStatusMessage("Attempting to modify configuration files...");

            configFile = Directory.GetCurrentDirectory() + "\\" + App.ApplicationConfig;

            if (applyChangesToService && File.Exists(configFile))
                ModifyConfigFile(configFile, connectionString, dataProviderString, encrypted, true);

            configFile = Directory.GetCurrentDirectory() + "\\" + App.ManagerConfig;

            if (applyChangesToLocalManager && File.Exists(configFile))
                ModifyConfigFile(configFile, connectionString, dataProviderString, encrypted, false);

            if (webManagerDir != null)
            {
                configFile = webManagerDir.ToString() + "\\Web.config";

                if (applyChangesToWebManager && File.Exists(configFile))
                    ModifyConfigFile(configFile, connectionString, dataProviderString, encrypted, false);
            }

            AppendStatusMessage("Modification of configuration files was successful.");
        }

        // Modifies the configuration file with the given file name to contain the given connection string and data provider string.
        private void ModifyConfigFile(string configFileName, string connectionString, string dataProviderString, bool encrypted, bool serviceConfigFile)
        {
            // Replace all instances of "TVA." with "GSF." to fix errors caused by namespace changes when upgrading
            File.WriteAllText(configFileName, File.ReadAllText(configFileName).Replace("TVA.", "GSF."));

            // Modify system settings.
            XmlDocument configFile = new XmlDocument();
            configFile.Load(configFileName);
            XmlNode categorizedSettings = configFile.SelectSingleNode("configuration/categorizedSettings");
            XmlNode systemSettings = configFile.SelectSingleNode("configuration/categorizedSettings/systemSettings");

            bool databaseConfigurationType = (m_state["configurationType"].ToString() == "database");

            if (encrypted)
                connectionString = Cipher.Encrypt(connectionString, App.CipherLookupKey, App.CryptoStrength);

            foreach (XmlNode child in systemSettings.ChildNodes)
            {
                if (child.Attributes != null && child.Attributes["name"] != null)
                {
                    if (child.Attributes["name"].Value == "DataProviderString")
                    {
                        // Retrieve the old data provider string from the config file.
                        if (m_oldDataProviderString == null)
                        {
                            m_oldDataProviderString = child.Attributes["value"].Value;

                            if (m_oldDataProviderString.Contains("System.Data.SQLite"))
                                m_oldDataProviderString = SQLiteDataProviderString;

                            if (serviceConfigFile)
                                m_state["oldDataProviderString"] = m_oldDataProviderString;
                        }

                        child.Attributes["value"].Value = dataProviderString;
                    }
                    else if (child.Attributes["name"].Value == "ConnectionString")
                    {
                        if (m_oldConnectionString == null)
                        {
                            // Retrieve the old connection string from the config file.
                            m_oldConnectionString = child.Attributes["value"].Value;

                            if (serviceConfigFile)
                                m_state["oldConnectionString"] = m_oldConnectionString;

                            if (Convert.ToBoolean(child.Attributes["encrypted"].Value))
                                m_oldConnectionString = Cipher.Decrypt(m_oldConnectionString, App.CipherLookupKey, App.CryptoStrength);
                        }

                        // Modify the config file settings to the new values.
                        child.Attributes["value"].Value = connectionString;
                        child.Attributes["encrypted"].Value = encrypted.ToString();
                    }
                    else if (child.Attributes["name"].Value == "NodeID")
                    {
                        if (m_state.ContainsKey("selectedNodeId"))
                        {
                            // Change the node ID in the configuration file to
                            // the ID that the user selected in the previous step.
                            string selectedNodeId = m_state["selectedNodeId"].ToString();
                            child.Attributes["value"].Value = selectedNodeId;
                        }
                        else
                        {
                            // Select the node that's in the configuration file in order
                            // to run validation routines at the end of the setup.
                            m_state["selectedNodeId"] = child.Attributes["value"].Value;
                        }
                    }
                }
            }

            if (serviceConfigFile && databaseConfigurationType)
            {
                XmlNode errorLoggerNode = configFile.SelectSingleNode("configuration/categorizedSettings/errorLogger");

                // Ensure that error logger category exists
                if ((object)errorLoggerNode == null)
                {
                    errorLoggerNode = configFile.CreateElement("errorLogger");
                    configFile.SelectSingleNode("configuration/categorizedSettings").AppendChild(errorLoggerNode);
                }

                // Make sure LogToDatabase setting exists
                XmlNode logToDatabaseNode = errorLoggerNode.SelectNodes("add").Cast<XmlNode>()
                                                           .SingleOrDefault(node => node.Attributes != null && node.Attributes["name"].Value == "LogToDatabase");

                if ((object)logToDatabaseNode == null)
                {
                    XmlElement addElement = configFile.CreateElement("add");

                    XmlAttribute attribute = configFile.CreateAttribute("name");
                    attribute.Value = "LogToDatabase";
                    addElement.Attributes.Append(attribute);

                    attribute = configFile.CreateAttribute("value");
                    attribute.Value = (m_state["newDatabaseType"].ToString() != "SQLite").ToString();
                    addElement.Attributes.Append(attribute);

                    attribute = configFile.CreateAttribute("description");
                    attribute.Value = "True if an encountered exception is logged to the database; otherwise False.";
                    addElement.Attributes.Append(attribute);

                    attribute = configFile.CreateAttribute("encrypted");
                    attribute.Value = "false";
                    addElement.Attributes.Append(attribute);

                    errorLoggerNode.AppendChild(addElement);
                }
                else if (m_state["newDatabaseType"].ToString() == "SQLite")
                {
                    logToDatabaseNode.Attributes["value"].Value = "False";
                }
            }

            // Make sure sttpDataPublisher settings exist
            XmlNode sttpDataPublisherNode = configFile.SelectSingleNode("configuration/categorizedSettings/sttpdatapublisher");
            if (serviceConfigFile && (object)sttpDataPublisherNode == null)
            {
                sttpDataPublisherNode = configFile.CreateElement("sttpdatapublisher");

                XmlElement addElement = configFile.CreateElement("add");

                XmlAttribute attribute = configFile.CreateAttribute("name");
                attribute.Value = "ConfigurationString";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("value");
                attribute.Value = "port=7175";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("description");
                attribute.Value = "Data required by the server to initialize.";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("encrypted");
                attribute.Value = "false";
                addElement.Attributes.Append(attribute);

                sttpDataPublisherNode.AppendChild(addElement);
                configFile.SelectSingleNode("configuration/categorizedSettings").AppendChild(sttpDataPublisherNode);
            }

            // Make sure sttpsDataPublisher settings exist
            XmlNode sttpsDataPublisher = configFile.SelectSingleNode("configuration/categorizedSettings/sttpsdatapublisher");
            if (serviceConfigFile && (object)sttpsDataPublisher == null)
            {
                sttpsDataPublisher = configFile.CreateElement("sttpsdatapublisher");

                // Add ConfigurationString setting
                XmlElement addElement = configFile.CreateElement("add");

                XmlAttribute attribute = configFile.CreateAttribute("name");
                attribute.Value = "ConfigurationString";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("value");
                attribute.Value = "port=7177";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("description");
                attribute.Value = "Data required by the server to initialize.";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("encrypted");
                attribute.Value = "false";
                addElement.Attributes.Append(attribute);

                sttpsDataPublisher.AppendChild(addElement);

                // Add CertificateFile setting
                addElement = configFile.CreateElement("add");

                attribute = configFile.CreateAttribute("name");
                attribute.Value = "CertificateFile";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("value");
                attribute.Value = "Eval(systemSettings.LocalCertificate)";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("description");
                attribute.Value = "Path to the local certificate used by this server for authentication.";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("encrypted");
                attribute.Value = "false";
                addElement.Attributes.Append(attribute);

                sttpsDataPublisher.AppendChild(addElement);
                configFile.SelectSingleNode("configuration/categorizedSettings").AppendChild(sttpsDataPublisher);
            }

            // Make sure alarm services settings exist
            XmlNode alarmServicesNode = configFile.SelectSingleNode("configuration/categorizedSettings/alarmservicesAlarmService");

            if (serviceConfigFile && (object)alarmServicesNode == null)
            {
                alarmServicesNode = configFile.CreateElement("alarmservicesAlarmService");

                XmlElement addElement = configFile.CreateElement("add");

                XmlAttribute attribute = configFile.CreateAttribute("name");
                attribute.Value = "Endpoints";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("value");
                attribute.Value = "http.rest://localhost:5019/alarmservices";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("description");
                attribute.Value = "Semicolon delimited list of URIs where the web service can be accessed.";
                addElement.Attributes.Append(attribute);

                attribute = configFile.CreateAttribute("encrypted");
                attribute.Value = "false";
                addElement.Attributes.Append(attribute);

                alarmServicesNode.AppendChild(addElement);
                configFile.SelectSingleNode("configuration/categorizedSettings").AppendChild(alarmServicesNode);
            }

            // Modify ADO metadata provider sections.
            IEnumerable<XmlNode> adoProviderSections = categorizedSettings.ChildNodes.Cast<XmlNode>().Where(node => node.Name.EndsWith("AdoMetadataProvider"));

            foreach (XmlNode section in adoProviderSections)
            {
                XmlNode connectionNode = section.ChildNodes.Cast<XmlNode>().SingleOrDefault(node => node.Name == "add" && node.Attributes != null && node.Attributes["name"].Value == "ConnectionString");
                XmlNode dataProviderNode = section.ChildNodes.Cast<XmlNode>().SingleOrDefault(node => node.Name == "add" && node.Attributes != null && node.Attributes["name"].Value == "DataProviderString");

                if (connectionNode != null && dataProviderNode != null)
                {
                    connectionNode.Attributes["value"].Value = "Eval(SystemSettings.ConnectionString)";
                    connectionNode.Attributes["encrypted"].Value = "False";
                    dataProviderNode.Attributes["value"].Value = "Eval(SystemSettings.DataProviderString)";
                    dataProviderNode.Attributes["encrypted"].Value = "False";
                }
            }

            // Update the data publisher default shared secret, if defined
            XmlNode dataPublisherPassword = configFile.SelectSingleNode("configuration/categorizedSettings/dataPublisher/add[@name = 'SharedSecret']");

            if (dataPublisherPassword != null)
            {
                string existingPassword = dataPublisherPassword.Attributes["value"].Value;

                if (Convert.ToBoolean(dataPublisherPassword.Attributes["encrypted"].Value))
                {
                    try
                    {
                        existingPassword = Cipher.Decrypt(existingPassword, App.CipherLookupKey, App.CryptoStrength);
                    }
                    catch
                    {
                        existingPassword = "openHistorian";
                    }
                }

                // During upgrade from older versions this password will be defaulted to openHistorian
                if (string.Compare(existingPassword, "openHistorian", true) == 0)
                {
                    dataPublisherPassword.Attributes["value"].Value = "TSF-E1CCE965-39A6-4476-8C60-EF02D8212F16";
                    dataPublisherPassword.Attributes["encrypted"].Value = "False";
                }
            }

            // The following change will be done only for openHistorianManager configuration.
            if (Convert.ToBoolean(m_state["applyChangesToLocalManager"]) && m_state.ContainsKey("allowPassThroughAuthentication"))
            {
                XmlNode forceLoginDisplayValue = configFile.SelectSingleNode("configuration/userSettings/openHistorianManager.Properties.Settings/setting[@name = 'ForceLoginDisplay']/value");

                if (forceLoginDisplayValue != null)
                    forceLoginDisplayValue.InnerXml = Convert.ToBoolean(m_state["allowPassThroughAuthentication"]) ? "False" : "True";
            }

            configFile.Save(configFileName);
        }

        // Saves the old connection string as an OleDB connection string.
        private void SaveOldConnectionString()
        {
            if ((object)m_oldDataProviderString != null)
            {
                // Determine the type of connection string.
                if (m_oldDataProviderString.Contains("System.Data.SqlClient.SqlConnection"))
                {
                    // Assume it's a SQL Server ODBC connection string.
                    m_state["oldDatabaseType"] = "SQLServer";
                }
                else if (m_oldDataProviderString.Contains("MySql.Data.MySqlClient.MySqlConnection"))
                {
                    // Assume it's a MySQL ODBC connection string.
                    m_state["oldDatabaseType"] = "MySQL";
                }
                else if (m_oldDataProviderString.Contains("Oracle.DataAccess.Client.OracleConnection"))
                {
                    // Assume it's a Oracle ODBC connection string.
                    m_state["oldDatabaseType"] = "Oracle";
                }
                else if (m_oldDataProviderString.Contains("System.Data.SQLite.SQLiteConnection"))
                {
                    // Assume it's a SQLite ODBC connection string.
                    m_state["oldDatabaseType"] = "SQLite";
                }
                else if (m_oldDataProviderString.Contains("Npgsql.NpgsqlConnection"))
                {
                    // Assume it's a PostgreSQL ODBC connection string.
                    m_state["oldDatabaseType"] = "PostgreSQL";
                }
                else
                {
                    // Assume it's a generic ODBC connection string.
                    m_state["oldDatabaseType"] = "Unspecified";
                }
            }
        }

        // Removes the cached configuration files.
        private void RemoveCachedConfiguration()
        {
            string configurationCachePath = Path.Combine(Directory.GetCurrentDirectory(), "ConfigurationCache");
            string binaryCachePath = Path.Combine(configurationCachePath, "SystemConfiguration.bin");
            string xmlCachePath = Path.Combine(configurationCachePath, "SystemConfiguration.xml");

            if (File.Exists(binaryCachePath))
                File.Delete(binaryCachePath);

            if (File.Exists(xmlCachePath))
                File.Delete(xmlCachePath);
        }

        // Updates the progress bar to have the specified value.
        private void UpdateProgressBar(int value)
        {
            if (Dispatcher.CheckAccess())
                m_progressBar.Value = value;
            else
                Dispatcher.Invoke(new Action<int>(UpdateProgressBar), value);
        }

        // Clears the status messages on the setup status text box.
        private void ClearStatusMessages()
        {
            if (Dispatcher.CheckAccess())
                m_setupStatusTextBox.Text = string.Empty;
            else
                Dispatcher.Invoke(new Action(ClearStatusMessages), null);
        }

        // Updates the setup status text box to include the specified message.
        private void AppendStatusMessage(string message)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(new Action<string>(AppendStatusMessage), message);
            else
            {
                m_setupStatusTextBox.AppendText(message + Environment.NewLine);
                m_setupStatusTextBox.ScrollToEnd();
            }
        }

        // Allows the user to proceed to the next screen if the setup succeeded.
        private void OnSetupSucceeded()
        {
            AppendStatusMessage("Operation succeeded. Click next to continue.");
            UpdateProgressBar(100);
            CanGoForward = true;
        }

        // Allows the user to go back to previous screens or cancel the setup if it failed.
        private void OnSetupFailed()
        {
            AppendStatusMessage("Operation failed. Click the back button to try again.");
            UpdateProgressBar(0);
            m_canGoBack = true;
            CanCancel = true;
        }

        #endregion
    }
}