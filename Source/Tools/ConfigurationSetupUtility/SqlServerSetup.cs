//******************************************************************************************************
//  SqlServerSetup.cs - Gbtc
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
//  06/29/2010 - Stephen C. Wills
//       Generated original version of source code.
//  03/02/2011 - J. Ritchie Carroll
//       Added key value delimeters only between settings.
//       Added IntegratedSecurityConnectionString property.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GSF;
using GSF.Data;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of a SQL Server connection string as well as running the sqlcmd.exe process.
    /// </summary>
    public class SqlServerSetup
    {
        #region [ Members ]

        // Constants
        private const string HostNameKey = "Data Source";
        private const string DatabaseNameKey = "Initial Catalog";
        private const string UserNameKey = "User ID";
        private const string PasswordKey = "Password";
        private const string IntegratedSecurityKey = "Integrated Security";
        private const string TimeoutKey = "Connect Timeout";

        // Fields
        private Dictionary<string, string> m_settings;
        private string m_dataProviderString;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="SqlServerSetup"/> class.
        /// </summary>
        public SqlServerSetup()
        {
            m_settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the host name of the SQL Server database.
        /// </summary>
        public string HostName
        {
            get => this[HostNameKey];
            set => this[HostNameKey] = value;
        }

        /// <summary>
        /// Gets or sets the name of the SQL Server database.
        /// </summary>
        public string DatabaseName
        {
            get => this[DatabaseNameKey];
            set => this[DatabaseNameKey] = value;
        }

        /// <summary>
        /// Gets or sets the user name for the user who has access to the database.
        /// </summary>
        public string UserName
        {
            get => this[UserNameKey];
            set => this[UserNameKey] = value;
        }

        /// <summary>
        /// Gets or sets the password for the user who has access to the database.
        /// </summary>
        public string Password
        {
            get => this[PasswordKey];
            set => this[PasswordKey] = value;
        }

        /// <summary>
        /// Gets or sets the setting for integrated security.
        /// </summary>
        public string IntegratedSecurity
        {
            get => this[IntegratedSecurityKey];
            set => this[IntegratedSecurityKey] = value;
        }

        /// <summary>
        /// Gets or sets the amount of time to wait before timing out the initial connection.
        /// </summary>
        public string Timeout
        {
            get => this[TimeoutKey];
            set => this[TimeoutKey] = value;
        }

        /// <summary>
        /// Gets or sets the connection string used to access the database.
        /// </summary>
        public string ConnectionString
        {
            get => m_settings.JoinKeyValuePairs();
            set => m_settings = value.ParseKeyValuePairs();
        }

        /// <summary>
        /// Gets or sets the connection string for initiating connections without using connection pooling,
        /// useful in case the database needs to be deleted later.
        /// </summary>
        public string NonPooledConnectionString
        {
            get
            {
                const string PoolingKey = "Pooling";
                const string PoolingValue = "False";

                Dictionary<string, string> settings = new Dictionary<string, string>(m_settings, StringComparer.OrdinalIgnoreCase);
                settings[PoolingKey] = PoolingValue;
                return settings.JoinKeyValuePairs();
            }
        }

        /// <summary>
        /// Converts the current settings to an OleDB connection string.
        /// </summary>
        public string OleDbConnectionString
        {
            get
            {
                const string OleDbKey = "Provider";
                const string OleDbValue = "SQLOLEDB";

                Dictionary<string, string> settings = new Dictionary<string, string>(m_settings, StringComparer.OrdinalIgnoreCase);
                settings[OleDbKey] = OleDbValue;
                return settings.JoinKeyValuePairs();
            }
        }

        /// <summary>
        /// Gets or sets the data provider string used
        /// for establishing SQL Server connections.
        /// </summary>
        public string DataProviderString
        {
            get => m_dataProviderString;
            set => m_dataProviderString = value;
        }

        private string this[string key]
        {
            get
            {
                if (m_settings.ContainsKey(key))
                    return m_settings[key];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove(key);
                else
                    m_settings[key] = value;
            }
        }

        #endregion

        #region [ Methods ]

        public void CreateLogin(string loginName)
        {
            string databaseName = DatabaseName;
            DatabaseName = "master";
            ExecuteStatement(string.Format("IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'{0}') CREATE LOGIN [{0}] FROM WINDOWS WITH DEFAULT_DATABASE=[master]", loginName));
            DatabaseName = databaseName;
        }

        public void CreateLogin(string loginName, string password)
        {
            string databaseName = DatabaseName;
            DatabaseName = "master";
            ExecuteStatement(string.Format("IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'{0}') CREATE LOGIN [{0}] WITH PASSWORD=N'{1}', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF", loginName, password));
            DatabaseName = databaseName;
        }

        public void GrantDatabaseAccess(string userName, string loginName, string roleName)
        {
            string loginExists = $"EXISTS (SELECT * FROM sys.server_principals WHERE name = N'{loginName}')";
            string loginNotMapped = $"NOT EXISTS (SELECT * FROM sys.server_principals svr INNER JOIN sys.database_principals db ON svr.sid = db.sid WHERE svr.name = N'{loginName}')";
            string userDoesNotExist = $"DATABASE_PRINCIPAL_ID('{0}') IS NULL";
            ExecuteStatement($"IF {loginExists} AND {loginNotMapped} AND {userDoesNotExist} CREATE USER [{userName}] FOR LOGIN [{loginName}]");

            string roleExists = $"DATABASE_PRINCIPAL_ID('{roleName}') IS NOT NULL";
            string userExists = $"DATABASE_PRINCIPAL_ID('{userName}') IS NOT NULL";
            ExecuteStatement($"IF {roleExists} AND {userExists} EXEC sp_addrolemember N'{roleName}', N'{userName}'");
        }

        public void ExecuteStatement(string statement)
        {
            IDbConnection connection = null;

            try
            {
                OpenConnection(ref connection);
                connection.ExecuteNonQuery(statement);
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Executes a SQL script using the SQL Server database engine.
        /// </summary>
        /// <param name="scriptPath">The path to the SQL Server script to be executed.</param>
        public void ExecuteScript(string scriptPath)
        {
            IDbConnection connection = null;
            string databaseName = null;

            try
            {
                // Database may not exist yet -- remove the database name,
                // but make sure to remember it for later
                databaseName = DatabaseName;
                DatabaseName = null;

                // Open the connection to the database
                OpenConnection(ref connection);

                // Put the database name back -- the script may need it
                DatabaseName = databaseName;

                // Execute the script
                ExecuteScript(connection, scriptPath);
            }
            finally
            {
                DatabaseName = databaseName;

                if (connection != null)
                    connection.Dispose();
            }
        }

        /// <summary>
        /// Executes a database script using the given connection.
        /// </summary>
        /// <param name="connection">The connection to be used to execute the script.</param>
        /// <param name="fileName">The path to the script to be executed.</param>
        public void ExecuteScript(IDbConnection connection, string fileName)
        {
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

                    while ((object)line != null)
                    {
                        string trimLine = line.Trim();
                        string statement;

                        if (trimLine == "GO")
                        {
                            // Remove comments and execute the statement.
                            statement = statementBuilder.ToString();
                            command.CommandText = comment.Replace(statement, " ").Trim();
                            command.ExecuteNonQuery();
                            statementBuilder.Clear();
                        }
                        else
                        {
                            if (trimLine.StartsWith("CREATE DATABASE") || trimLine.StartsWith("ALTER DATABASE") || trimLine.StartsWith("USE"))
                                line = line.Replace("openHistorian", DatabaseName);

                            // Append this line to the statement
                            statementBuilder.Append(line);
                            statementBuilder.Append('\n');
                        }

                        // Read the next line from the file.
                        line = scriptReader.ReadLine();
                    }
                }
            }
            finally
            {
                if (scriptReader != null)
                    scriptReader.Dispose();
            }
        }

        public void OpenConnection(ref IDbConnection connection)
        {
            string dataProviderString = m_dataProviderString;
            Dictionary<string, string> settings = dataProviderString.ParseKeyValuePairs();
            string assemblyName = settings["AssemblyName"].ToNonNullString();
            string connectionTypeName = settings["ConnectionType"].ToNonNullString();
            string adapterTypeName = settings["AdapterType"].ToNonNullString();

            if (string.IsNullOrEmpty(connectionTypeName))
                throw new InvalidOperationException("Database connection type was not defined.");

            if (string.IsNullOrEmpty(adapterTypeName))
                throw new InvalidOperationException("Database adapter type was not defined.");

            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type connectionType = assembly.GetType(connectionTypeName);
            _ = assembly.GetType(adapterTypeName);

            connection = (IDbConnection)Activator.CreateInstance(connectionType);

            // Force use of non-pooled connection string such that database can later be deleted if needed
            connection.ConnectionString = NonPooledConnectionString;

            connection.Open();
        }

        public AdoDataConnection OpenConnection()
        {
            // Force use of non-pooled connection string such that database can later be deleted if needed
            return new AdoDataConnection(NonPooledConnectionString, m_dataProviderString);
        }

        #endregion
    }
}