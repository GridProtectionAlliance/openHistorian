//******************************************************************************************************
//  OracleSetup.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  09/23/2011 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GSF;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of an Oracle DB connection string
    /// as well as executing scripts and storing the data required for Oracle DB setup.
    /// </summary>
    public class OracleSetup
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Default value for the data provider string used to
        /// load the assembly for making Oracle DB connections.
        /// </summary>
        public const string DefaultDataProviderString =
            "AssemblyName={Oracle.DataAccess, Version=2.112.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342};" +
            "ConnectionType=Oracle.DataAccess.Client.OracleConnection;AdapterType=Oracle.DataAccess.Client.OracleDataAdapter";

        // Fields
        private Dictionary<string, string> m_settings;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="OracleSetup"/> class.
        /// </summary>
        public OracleSetup()
        {
            m_settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            DataProviderString = DefaultDataProviderString;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The name of the transparent network substrate
        /// which defines the connection to the server.
        /// </summary>
        public string TnsName { get; set; }

        /// <summary>
        /// The user name of the administrative user.
        /// </summary>
        public string AdminUserName { get; set; }

        /// <summary>
        /// The password of the administrative user.
        /// </summary>
        public string AdminPassword { get; set; }

        /// <summary>
        /// The user name of the openHistorian schema user.
        /// </summary>
        public string SchemaUserName { get; set; }

        /// <summary>
        /// The password of the openHistorian schema user.
        /// </summary>
        public string SchemaPassword { get; set; }

        /// <summary>
        /// The data provider string used to load the
        /// assembly for making Oracle DB connections.
        /// </summary>
        public string DataProviderString { get; set; }

        /// <summary>
        /// Indicates whether a new schema user will
        /// be created by the Configuration Setup Utility.
        /// </summary>
        public bool CreateNewSchema { get; set; }

        /// <summary>
        /// Indicates whether the connection string setting
        /// in the configuration file should be encrypted.
        /// </summary>
        public bool EncryptConnectionString { get; set; }

        /// <summary>
        /// The connection string which defines the connection
        /// by which to connect to the database.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                m_settings.Remove("Data Source");
                m_settings.Remove("User Id");
                m_settings.Remove("Uid");
                m_settings.Remove("Password");
                m_settings.Remove("Pwd");

                if (!string.IsNullOrEmpty(TnsName))
                    m_settings["Data Source"] = TnsName;

                if (!string.IsNullOrEmpty(SchemaUserName))
                    m_settings["User Id"] = SchemaUserName;

                if (!string.IsNullOrEmpty(SchemaPassword))
                    m_settings["Password"] = SchemaPassword;

                return m_settings.JoinKeyValuePairs();
            }
            set
            {
                TnsName = null;
                SchemaUserName = null;
                SchemaPassword = null;

                m_settings = value.ParseKeyValuePairs();

                if (m_settings.TryGetValue("Data Source", out string tnsName))
                    TnsName = tnsName;

                if (m_settings.TryGetValue("User Id", out string userName) || m_settings.TryGetValue("Uid", out userName))
                    SchemaUserName = userName;

                if (m_settings.TryGetValue("Password", out string password) || m_settings.TryGetValue("Pwd", out password))
                    SchemaPassword = password;
            }
        }

        /// <summary>
        /// The connection string that defines the connection
        /// by which to connect to the database with admin privileges.
        /// </summary>
        public string AdminConnectionString
        {
            get
            {
                Dictionary<string, string> settings = new Dictionary<string, string>(m_settings);

                settings.Remove("Data Source");
                settings.Remove("User Id");
                settings.Remove("Uid");
                settings.Remove("Password");
                settings.Remove("Pwd");

                if (!string.IsNullOrEmpty(TnsName))
                    settings["Data Source"] = TnsName;

                if (!string.IsNullOrEmpty(AdminUserName))
                {
                    string[] user = AdminUserName.Split(new string[] { " AS ", " as ", " As ", " aS " }, StringSplitOptions.None);

                    settings["User Id"] = user[0];

                    if (user.Length > 1)
                        settings["DBA Privilege"] = user[1];
                }

                if (!string.IsNullOrEmpty(AdminPassword))
                    settings["Password"] = AdminPassword;

                return settings.JoinKeyValuePairs();
            }
        }

        /// <summary>
        /// The connection string which defines the OLEDB
        /// connection by which to connect to the database.
        /// </summary>
        public string OleDbConnectionString => "Provider=OraOLEDB.Oracle; " + ConnectionString;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens a connection as the schema user.
        /// </summary>
        /// <param name="connection">The connection object to be created and opened.</param>
        public void OpenConnection(ref IDbConnection connection)
        {
            OpenConnection(ref connection, ConnectionString);
        }

        /// <summary>
        /// Opens a connection as the admin user.
        /// </summary>
        /// <param name="connection">The connection object to be created and opened.</param>
        public void OpenAdminConnection(ref IDbConnection connection)
        {
            OpenConnection(ref connection, AdminConnectionString);
        }

        /// <summary>
        /// Executes the given statement as the admin user.
        /// </summary>
        /// <param name="statement">The statement to be executed.</param>
        public void ExecuteStatement(string statement)
        {
            IDbConnection connection = null;

            try
            {
                OpenAdminConnection(ref connection);
                ExecuteStatement(connection, statement);
            }
            finally
            {
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Executes the given statement using the given connection.
        /// </summary>
        /// <param name="connection">The connection used to execute the statement.</param>
        /// <param name="statement">The statement to be executed.</param>
        public void ExecuteStatement(IDbConnection connection, string statement)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = statement;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a database script as the admin user.
        /// </summary>
        /// <param name="fileName">The path to the script to be executed.</param>
        public void ExecuteScript(string fileName)
        {
            IDbConnection connection = null;

            try
            {
                OpenAdminConnection(ref connection);
                ExecuteScript(connection, fileName);
            }
            finally
            {
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Executes a database script using the given connection.
        /// </summary>
        /// <param name="connection">The connection to be used to execute the script.</param>
        /// <param name="fileName">The path to the script to be executed.</param>
        public void ExecuteScript(IDbConnection connection, string fileName)
        {
            IDbTransaction transaction = null;
            TextReader scriptReader = null;

            try
            {
                string line;

                scriptReader = File.OpenText(fileName);
                line = scriptReader.ReadLine();

                using (IDbCommand command = connection.CreateCommand())
                {
                    StringBuilder statementBuilder = new StringBuilder();
                    Regex comment = new Regex(@"/\*.*\*/|--.*\n", RegexOptions.Multiline);
                    string indexTablespaceName = $"{SchemaUserName.TruncateRight(24)}_INDEX";

                    transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    while ((object)line != null)
                    {
                        string trimLine = line.Trim();
                        string statement;
                        bool isPlsqlBlock;

                        statementBuilder.Append(line);
                        statementBuilder.Append('\n');
                        statement = statementBuilder.ToString();
                        statement = comment.Replace(statement, " ").Trim();

                        // Determine whether the statement is a PL/SQL block.
                        // If the statement is a PL/SQL block, the delimiter
                        // is a forward slash. Otherwise, it is a semicolon.
                        isPlsqlBlock = s_plsqlIdentifiers.Any(ident => statement.IndexOf(ident, StringComparison.CurrentCultureIgnoreCase) >= 0);

                        // If the statement is a PL/SQL block and the current line is a forward slash,
                        // or if the statement is not a PL/SQL block and the statement in a semicolon,
                        // then execute and flush the statement so that the next statement can be executed.
                        if (isPlsqlBlock && trimLine == "/" || !isPlsqlBlock && statement.EndsWith(";"))
                        {
                            // Remove trailing delimiter and newlines.
                            statement = statement.Remove(statement.Length - 1);

                            // Fix name of tablespace for index
                            statement = statement.Replace("openHistorian_INDEX", indexTablespaceName);

                            // Remove comments and execute the statement.
                            command.CommandText = statement;
                            command.ExecuteNonQuery();
                            statementBuilder.Clear();
                        }

                        // Read the next line from the file.
                        line = scriptReader.ReadLine();
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction?.Rollback();

                throw;
            }
            finally
            {
                scriptReader?.Dispose();
            }
        }

        // Opens a database connection with the given connection string.
        private void OpenConnection(ref IDbConnection connection, string connectionString)
        {
            Dictionary<string, string> settings;
            string assemblyName, connectionTypeName, adapterTypeName;
            Assembly assembly;
            Type connectionType, adapterType;
            string dataProviderString;

            dataProviderString = DataProviderString;
            settings = dataProviderString.ParseKeyValuePairs();
            assemblyName = settings["AssemblyName"].ToNonNullString();
            connectionTypeName = settings["ConnectionType"].ToNonNullString();
            adapterTypeName = settings["AdapterType"].ToNonNullString();

            if (string.IsNullOrEmpty(connectionTypeName))
                throw new InvalidOperationException("Database connection type was not defined.");

            if (string.IsNullOrEmpty(adapterTypeName))
                throw new InvalidOperationException("Database adapter type was not defined.");

            assembly = Assembly.Load(new AssemblyName(assemblyName));
            connectionType = assembly.GetType(connectionTypeName);
            _ = assembly.GetType(adapterTypeName);

            connection = (IDbConnection)Activator.CreateInstance(connectionType);
            connection.ConnectionString = connectionString;
            connection.Open();
        }

        #endregion

        #region [ Static ]

        // Static Fields

        // Defines a list of keywords used to identify PL/SQL blocks.
        private static readonly string[] s_plsqlIdentifiers =
        {
            "CREATE FUNCTION", "CREATE OR REPLACE FUNCTION",
            "CREATE PROCEDURE", "CREATE OR REPLACE PROCEDURE",
            "CREATE PACKAGE", "CREATE OR REPLACE PACKAGE",
            "DECLARE", "BEGIN"
        };

        #endregion
    }
}