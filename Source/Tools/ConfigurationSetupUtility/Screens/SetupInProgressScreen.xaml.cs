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
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using GSF;
using GSF.Communication;
using GSF.Data;
using GSF.Identity;
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

            // Establish crypto keys in case they do not exist
            try
            {
                "SetupString".Encrypt(App.CipherLookupKey, CipherStrength.Aes256);
                //"SetupString".Encrypt(DataPublisher.CipherLookupKey, CipherStrength.Aes256);
            }
            catch
            {
                //AppendStatusMessage(string.Format("WARNING: Failed to establish crypto keys due to exception: {0}", ex.Message));
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
            string databaseType = m_state["databaseType"].ToString();

            if (databaseType == "sql server")
                SetUpSqlServerDatabase();
            else if (databaseType == "mysql")
                SetUpMySqlDatabase();
            else if (databaseType == "oracle")
                SetUpOracleDatabase();
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
                string adminUserName, adminPassword;
                object dataProviderStringValue;
                string dataProviderString = null;

                mySqlSetup = m_state["mySqlSetup"] as MySqlSetup;
                mySqlSetup.OutputDataReceived += MySqlSetup_OutputDataReceived;
                mySqlSetup.ErrorDataReceived += MySqlSetup_ErrorDataReceived;
                m_state["newOleDbConnectionString"] = mySqlSetup.OleDbConnectionString;
                adminUserName = mySqlSetup.UserName;
                adminPassword = mySqlSetup.Password;

                // Get user customized data provider string
                if (m_state.TryGetValue("mySqlDataProviderString", out dataProviderStringValue))
                    dataProviderString = dataProviderStringValue.ToString();

                if (string.IsNullOrWhiteSpace(dataProviderString))
                    dataProviderString = "AssemblyName={MySql.Data, Version=6.3.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d}; ConnectionType=MySql.Data.MySqlClient.MySqlConnection; AdapterType=MySql.Data.MySqlClient.MySqlDataAdapter";

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

                            if (!mySqlSetup.ExecuteScript(scriptPath))
                            {
                                OnSetupFailed();
                                return;
                            }

                            progress += 90 / scriptNames.Count;
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

                            if (!mySqlSetup.ExecuteStatement(string.Format("GRANT SELECT, UPDATE, INSERT, DELETE ON {0}.* TO {1} IDENTIFIED BY '{2}'", mySqlSetup.DatabaseName, user, pass)))
                            {
                                // If we couldn't grant the necessary permissions to
                                // the database user, then the setup should fail.
                                OnSetupFailed();
                                return;
                            }

                            mySqlSetup.UserName = user;
                            mySqlSetup.Password = pass;

                            UpdateProgressBar(98);
                            AppendStatusMessage("New database user created successfully.");
                            AppendStatusMessage(string.Empty);
                        }

                        if (!migrate)
                        {
                            SetUpStatisticsHistorian(mySqlSetup.ConnectionString, dataProviderString);
                            SetupAdminUserCredentials(mySqlSetup.ConnectionString, dataProviderString);
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

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
            finally
            {
                if (mySqlSetup != null)
                {
                    mySqlSetup.OutputDataReceived -= MySqlSetup_OutputDataReceived;
                    mySqlSetup.ErrorDataReceived -= MySqlSetup_ErrorDataReceived;
                }
            }
        }

        // Called when the user has asked to set up a SQL Server database.
        private void SetUpSqlServerDatabase()
        {
            SqlServerSetup sqlServerSetup = null;

            try
            {
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);
                string adminUserName, adminPassword;
                object dataProviderStringValue;
                string dataProviderString = null;
                bool createNewUser = false;
                bool useIntegratedSecurity = false;

                sqlServerSetup = m_state["sqlServerSetup"] as SqlServerSetup;
                sqlServerSetup.OutputDataReceived += SqlServerSetup_OutputDataReceived;
                sqlServerSetup.ErrorDataReceived += SqlServerSetup_ErrorDataReceived;
                m_state["newOleDbConnectionString"] = sqlServerSetup.OleDbConnectionString;
                adminUserName = sqlServerSetup.UserName;
                adminPassword = sqlServerSetup.Password;

                // Get user customized data provider string
                if (m_state.TryGetValue("sqlServerDataProviderString", out dataProviderStringValue))
                    dataProviderString = dataProviderStringValue.ToString();

                if (string.IsNullOrWhiteSpace(dataProviderString))
                    dataProviderString = "AssemblyName={System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089}; ConnectionType=System.Data.SqlClient.SqlConnection; AdapterType=System.Data.SqlClient.SqlDataAdapter";

                // Check to see if user requested to use integrated authentication
                if (m_state.ContainsKey("useSqlServerIntegratedSecurity"))
                    useIntegratedSecurity = Convert.ToBoolean(m_state["useSqlServerIntegratedSecurity"]);

                if (!existing || migrate)
                {
                    if (!CheckIfDatabaseExists(sqlServerSetup.ConnectionString, dataProviderString, sqlServerSetup.DatabaseName))
                    {
                        List<string> scriptNames = new List<string>();
                        bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                        bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);
                        bool enableAuditLog = Convert.ToBoolean(m_state["enableAuditLog"]);
                        int progress = 0;

                        createNewUser = Convert.ToBoolean(m_state["createNewSqlServerUser"]);

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

                            if (!sqlServerSetup.ExecuteScript(scriptPath))
                            {
                                OnSetupFailed();
                                return;
                            }

                            progress += 90 / scriptNames.Count;
                            UpdateProgressBar(progress);
                            AppendStatusMessage(string.Format("{0} ran successfully.", scriptName));
                            AppendStatusMessage(string.Empty);
                        }

                        // Set up the initial historian.
                        if (Convert.ToBoolean(m_state["setupHistorian"]))
                            SetUpInitialHistorian(sqlServerSetup.ConnectionString, dataProviderString);

                        // Create new SQL Server database user.
                        if (createNewUser)
                        {
                            string user = m_state["newSqlServerUserName"].ToString();
                            string pass = m_state["newSqlServerUserPassword"].ToString();

                            AppendStatusMessage(string.Format("Attempting to create new user {0}...", user));
                            string db = sqlServerSetup.DatabaseName;

                            sqlServerSetup.DatabaseName = "master";
                            if (!sqlServerSetup.ExecuteStatement(string.Format("IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'{0}') CREATE LOGIN [{0}] WITH PASSWORD=N'{1}', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF", user, pass)))
                            {
                                OnSetupFailed();
                                return;
                            }

                            sqlServerSetup.DatabaseName = db;
                            if (!sqlServerSetup.ExecuteStatement(string.Format("CREATE USER [{0}] FOR LOGIN [{0}]", user)))
                            {
                                OnSetupFailed();
                                return;
                            }

                            if (!sqlServerSetup.ExecuteStatement("CREATE ROLE [openHistorianAdminRole] AUTHORIZATION [dbo]"))
                            {
                                OnSetupFailed();
                                return;
                            }

                            if (!sqlServerSetup.ExecuteStatement(string.Format("EXEC sp_addrolemember N'openHistorianAdminRole', N'{0}'", user)))
                            {
                                OnSetupFailed();
                                return;
                            }

                            if (!sqlServerSetup.ExecuteStatement(string.Format("EXEC sp_addrolemember N'db_datareader', N'openHistorianAdminRole'")))
                            {
                                OnSetupFailed();
                                return;
                            }

                            if (!sqlServerSetup.ExecuteStatement(string.Format("EXEC sp_addrolemember N'db_datawriter', N'openHistorianAdminRole'")))
                            {
                                OnSetupFailed();
                                return;
                            }

                            sqlServerSetup.UserName = user;
                            sqlServerSetup.Password = pass;

                            UpdateProgressBar(98);
                            AppendStatusMessage("New database user created successfully.");
                            AppendStatusMessage(string.Empty);
                        }
                        else if (useIntegratedSecurity)
                        {
                            const string GroupName = "openHistorian Admins";

                            string host = sqlServerSetup.HostName.Split('\\')[0].Trim();
                            string db = sqlServerSetup.DatabaseName;
                            string[] loginNames;
                            bool useGroupLogin;

                            useGroupLogin = UserInfo.LocalGroupExists(GroupName) && (host == "." || Transport.IsLocalAddress(host));
                            loginNames = new string[] { (useGroupLogin ? string.Format(@"{0}\{1}", Environment.MachineName, GroupName) : null), GetServiceAccountName() };

                            foreach (string loginName in loginNames)
                            {
                                if ((object)loginName != null && !loginName.Equals("LocalSystem", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    AppendStatusMessage(string.Format("Attempting to add Windows authenticated database login for {0}...", loginName));

                                    sqlServerSetup.DatabaseName = "master";
                                    if (!sqlServerSetup.ExecuteStatement(string.Format("IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'{0}') CREATE LOGIN [{0}] FROM WINDOWS WITH DEFAULT_DATABASE=[master]", loginName)))
                                    {
                                        OnSetupFailed();
                                        return;
                                    }

                                    sqlServerSetup.DatabaseName = db;
                                    if (!sqlServerSetup.ExecuteStatement(string.Format("CREATE USER [{0}] FOR LOGIN [{0}]", loginName)))
                                    {
                                        OnSetupFailed();
                                        return;
                                    }

                                    if (!sqlServerSetup.ExecuteStatement("CREATE ROLE [openHistorianAdminRole] AUTHORIZATION [dbo]"))
                                    {
                                        OnSetupFailed();
                                        return;
                                    }

                                    if (!sqlServerSetup.ExecuteStatement(string.Format("EXEC sp_addrolemember N'openHistorianAdminRole', N'{0}'", loginName)))
                                    {
                                        OnSetupFailed();
                                        return;
                                    }

                                    if (!sqlServerSetup.ExecuteStatement(string.Format("EXEC sp_addrolemember N'db_datareader', N'openHistorianAdminRole'")))
                                    {
                                        OnSetupFailed();
                                        return;
                                    }

                                    if (!sqlServerSetup.ExecuteStatement(string.Format("EXEC sp_addrolemember N'db_datawriter', N'openHistorianAdminRole'")))
                                    {
                                        OnSetupFailed();
                                        return;
                                    }

                                    UpdateProgressBar(98);
                                    AppendStatusMessage("Database login created successfully.");
                                    AppendStatusMessage(string.Empty);
                                }
                            }
                        }

                        if (!migrate)
                        {
                            SetUpStatisticsHistorian(sqlServerSetup.ConnectionString, dataProviderString);
                            SetupAdminUserCredentials(sqlServerSetup.ConnectionString, dataProviderString);
                        }
                    }
                    else
                    {
                        this.CanGoBack = true;
                        ScreenManager sm = m_state["screenManager"] as ScreenManager;
                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            while (!(sm.CurrentScreen is SqlServerDatabaseSetupScreen))
                                sm.GoToPreviousScreen();
                        });
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(sqlServerSetup.ConnectionString, dataProviderString);
                }

                // Modify the openHistorian configuration file.
                string connectionString;

                if (useIntegratedSecurity && !createNewUser)
                    connectionString = sqlServerSetup.IntegratedSecurityConnectionString;
                else
                    connectionString = sqlServerSetup.PooledConnectionString;

                ModifyConfigFiles(connectionString, dataProviderString, Convert.ToBoolean(m_state["encryptSqlServerConnectionStrings"]));
                SaveOldConnectionString();

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
                AppendStatusMessage(ex.Message);
                OnSetupFailed();
            }
            finally
            {
                if (sqlServerSetup != null)
                {
                    sqlServerSetup.OutputDataReceived -= SqlServerSetup_OutputDataReceived;
                    sqlServerSetup.ErrorDataReceived -= SqlServerSetup_ErrorDataReceived;
                }
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
                string adminUserName, adminPassword;
                string dataProviderString = null;

                oracleSetup = m_state["oracleSetup"] as OracleSetup;
                m_state["newOleDbConnectionString"] = oracleSetup.OleDbConnectionString;
                adminUserName = oracleSetup.AdminUserName;
                adminPassword = oracleSetup.AdminPassword;

                // Get user customized data provider string
                dataProviderString = oracleSetup.DataProviderString;

                if (string.IsNullOrWhiteSpace(dataProviderString))
                    dataProviderString = OracleSetup.DefaultDataProviderString;

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
                        }
                    }
                    else
                    {
                        this.CanGoBack = true;
                        ScreenManager sm = m_state["screenManager"] as ScreenManager;
                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            while (!(sm.CurrentScreen is SqlServerDatabaseSetupScreen))
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

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
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
                string connectionString = "Data Source=" + destination + "; Version=3; Foreign Keys=True";
                string dataProviderString = "AssemblyName={System.Data.SQLite, Version=1.0.79.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139}; ConnectionType=System.Data.SQLite.SQLiteConnection; AdapterType=System.Data.SQLite.SQLiteDataAdapter";
                bool existing = Convert.ToBoolean(m_state["existing"]);
                bool migrate = existing && Convert.ToBoolean(m_state["updateConfiguration"]);

                if (!existing || migrate)
                {
                    bool initialDataScript = !migrate && Convert.ToBoolean(m_state["initialDataScript"]);
                    bool sampleDataScript = initialDataScript && Convert.ToBoolean(m_state["sampleDataScript"]);

                    if (!initialDataScript)
                        filePath = Directory.GetCurrentDirectory() + "\\Database scripts\\Sqlite\\" + App.BaseSqliteConfig;
                    else if (!sampleDataScript)
                        filePath = Directory.GetCurrentDirectory() + "\\Database scripts\\Sqlite\\" + App.SqliteInitialData;
                    else
                        filePath = Directory.GetCurrentDirectory() + "\\Database scripts\\Sqlite\\" + App.SqliteSampleData;

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
                    }
                }
                else if (m_state.ContainsKey("createNewNode") && Convert.ToBoolean(m_state["createNewNode"]))
                {
                    CreateNewNode(connectionString, dataProviderString);
                }

                // Modify the openHistorian configuration file.
                ModifyConfigFiles(connectionString, dataProviderString, false);

                m_state["oldOleDbConnectionString"] = m_oldConnectionString;
                m_state["oldOleDbDataType"] = "Unspecified";
                m_state["newOleDbConnectionString"] = connectionString;

                OnSetupSucceeded();
            }
            catch (Exception ex)
            {
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
        /// <returns>returns true if database exists or user says no to database delete, false if database does not exist or user says yes to database delete.</returns>
        private bool CheckIfDatabaseExists(string connectionString, string dataProviderString, string databaseName)
        {
            AppendStatusMessage(string.Format("Checking if database {0} already exists.", databaseName));

            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
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

                    //if (m_state["databaseType"].ToString() == "sql server")
                    //    connection.ConnectionString = connectionString + ";pooling=false;"; // this was done to avoid connection pooling so SQL database can be deleted easily.
                    //else
                    //    connection.ConnectionString = connectionString;
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    IDbCommand command = connection.CreateCommand();

                    if (m_state["databaseType"].ToString() == "sql server")
                        command.CommandText = string.Format("SELECT COUNT(*) FROM sys.databases WHERE name = '{0}'", databaseName);
                    else if (m_state["databaseType"].ToString() == "oracle")
                        command.CommandText = string.Format("SELECT COUNT(*) FROM all_users WHERE USERNAME = '{0}'", databaseName.ToUpper());
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

                if (dbCount > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat("Database \"{0}\" already exists.\r\n", databaseName);
                    sb.AppendLine();
                    sb.AppendLine("    Click YES to delete existing database.");
                    sb.AppendLine("    Click NO to go back to change database name.");
                    sb.AppendLine();
                    sb.AppendLine("WARNING: If you delete the existing database ALL configuration in that database will be permanently deleted.");

                    bool dropDatabase = (MessageBox.Show(sb.ToString(), "Database Exists!", MessageBoxButton.YesNo) == MessageBoxResult.Yes);

                    if (dropDatabase)
                    {
                        if (m_state["databaseType"].ToString() == "sql server")
                        {
                            SqlServerSetup sqlServerSetup = m_state["sqlServerSetup"] as SqlServerSetup;
                            sqlServerSetup.DatabaseName = "master";
                            if (!sqlServerSetup.ExecuteStatement(string.Format("USE [master] ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE DROP DATABASE {0}", databaseName)))
                                MessageBox.Show(string.Format("Failed to delete database {0}", databaseName), "Delete Database Failed");
                            else
                                AppendStatusMessage(string.Format("Dropped database {0} successfully.", databaseName));

                            sqlServerSetup.DatabaseName = databaseName;
                        }
                        else if (m_state["databaseType"].ToString() == "oracle")
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
                        else
                        {
                            MySqlSetup mySqlSetup = m_state["mySqlSetup"] as MySqlSetup;
                            if (!mySqlSetup.ExecuteStatement(string.Format("DROP DATABASE {0}", databaseName)))
                                MessageBox.Show(string.Format("Failed to delete database {0}", databaseName), "Delete Database Failed");
                            else
                                AppendStatusMessage(string.Format("Dropped database {0} successfully.", databaseName));
                        }
                        return false;
                    }
                    else
                        return true;
                }
                return false;
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        // Called when the user has asked to set up an XML configuration.
        private void SetUpXmlConfiguration()
        {
            try
            {
                // Modify the openHistorian configuration file.
                ModifyConfigFiles(m_state["xmlFilePath"].ToString(), string.Empty, false);
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

            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
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

            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
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

                    nameParameter.ParameterName = paramChar + "name";
                    createdByParameter.ParameterName = paramChar + "createdBy";
                    updatedByParameter.ParameterName = paramChar + "updatedBy";

                    nameParameter.Value = m_state["adminUserName"].ToString();
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

                    nameParameter.ParameterName = paramChar + "name";
                    passwordParameter.ParameterName = paramChar + "password";
                    firstNameParameter.ParameterName = paramChar + "firstName";
                    lastNameParameter.ParameterName = paramChar + "lastName";
                    createdByParameter.ParameterName = paramChar + "createdBy";
                    updatedByParameter.ParameterName = paramChar + "updatedBy";

                    nameParameter.Value = m_state["adminUserName"].ToString();
                    passwordParameter.Value = FormsAuthentication.HashPasswordForStoringInConfigFile(@"O3990\P78f9E66b:a35_V©6M13©6~2&[" + m_state["adminPassword"].ToString(), "SHA1");
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
                newNameParameter.Value = m_state["adminUserName"].ToString();

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
                UpdateProgressBar(97);
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
                string databaseType = m_state["databaseType"].ToString();

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

            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
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
                if (child.Attributes != null)
                {
                    if (child.Attributes["name"].Value == "DataProviderString")
                    {
                        // Retrieve the old data provider string from the config file.
                        if (m_oldDataProviderString == null)
                            m_oldDataProviderString = child.Attributes["value"].Value;

                        child.Attributes["value"].Value = dataProviderString;
                    }
                    else if (child.Attributes["name"].Value == "ConnectionString")
                    {
                        if (m_oldConnectionString == null)
                        {
                            // Retrieve the old connection string from the config file.
                            m_oldConnectionString = child.Attributes["value"].Value;

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
                    attribute.Value = "True";
                    addElement.Attributes.Append(attribute);

                    attribute = configFile.CreateAttribute("description");
                    attribute.Value = "True if an encountered exception is logged to the database; otherwise False.";
                    addElement.Attributes.Append(attribute);

                    attribute = configFile.CreateAttribute("encrypted");
                    attribute.Value = "false";
                    addElement.Attributes.Append(attribute);

                    errorLoggerNode.AppendChild(addElement);
                }
            }

            // Make sure alarm services settings exist
            XmlNode alarmServicesNode = configFile.SelectSingleNode("configuration/categorizedSettings/alarmServicesAlarmService");
            if (serviceConfigFile && (object)alarmServicesNode == null)
            {
                alarmServicesNode = configFile.CreateElement("alarmServicesAlarmService");

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
                XmlNode connectionNode = section.ChildNodes.Cast<XmlNode>().SingleOrDefault(node => node.Attributes != null && node.Attributes["name"].Value == "ConnectionString");
                XmlNode dataProviderNode = section.ChildNodes.Cast<XmlNode>().SingleOrDefault(node => node.Attributes != null && node.Attributes["name"].Value == "DataProviderString");

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
            if (m_oldDataProviderString != null)
            {
                // Determine the type of connection string and convert it to OleDB.
                if (m_oldDataProviderString.Contains("MySqlConnection"))
                {
                    // Assume it's a MySQL ODBC connection string.
                    MySqlSetup oldConnectionStringSetup = new MySqlSetup();
                    oldConnectionStringSetup.ConnectionString = m_oldConnectionString;
                    m_state["oldOleDbConnectionString"] = oldConnectionStringSetup.OleDbConnectionString;
                    m_state["oldOleDbDataType"] = "MySQL";
                }
                else if (m_oldDataProviderString.Contains("OleDbConnection"))
                {
                    // Assume it's already an OleDB connection string.
                    m_state["oldOleDbConnectionString"] = m_oldConnectionString;
                    m_state["oldOleDbDataType"] = "Unspecified";
                }
                else
                {
                    // Assume it's a SQL Server ODBC connection string.
                    SqlServerSetup oldConnectionStringSetup = new SqlServerSetup();
                    oldConnectionStringSetup.ConnectionString = m_oldConnectionString;
                    m_state["oldOleDbConnectionString"] = oldConnectionStringSetup.OleDbConnectionString;
                    m_state["oldOleDbDataType"] = "SqlServer";
                }
            }
        }

        // Called when mysql.exe receives data on its standard output stream.
        private void MySqlSetup_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AppendStatusMessage(e.Data);
        }

        // Called when mysql.exe receives data on its standard error stream.
        private void MySqlSetup_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            AppendStatusMessage(e.Data);
        }

        // Called when sqlcmd.exe receives data on its standard output stream.
        private void SqlServerSetup_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AppendStatusMessage(e.Data);
        }

        // Called when sqlcmd.exe receives data on its standard error stream.
        private void SqlServerSetup_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            AppendStatusMessage(e.Data);
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