//******************************************************************************************************
//  SqlServerSetup.cs - Gbtc
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
//  06/29/2010 - Stephen C. Wills
//       Generated original version of source code.
//  03/02/2011 - J. Ritchie Carroll
//       Added key value delimeters only between settings.
//       Added IntegratedSecurityConnectionString property.
//
//******************************************************************************************************

using GSF;
using GSF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of a SQL Server connection string as well as running the sqlcmd.exe process.
    /// </summary>
    public class SqlServerSetup
    {

        #region [ Members ]

        // Fields

        private Dictionary<string, string> m_settings;
        private string m_dataProviderString;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="MySqlSetup"/> class.
        /// </summary>
        public SqlServerSetup()
        {
            m_settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            m_settings["pooling"] = "false";
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the host name of the MySQL database.
        /// </summary>
        public string HostName
        {
            get
            {
                return m_settings["Data Source"];
            }
            set
            {
                m_settings["Data Source"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the MySQL database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                if (m_settings.ContainsKey("Initial Catalog"))
                    return m_settings["Initial Catalog"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Initial Catalog");
                else
                    m_settings["Initial Catalog"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user name for the user whom has access to the database.
        /// </summary>
        public string UserName
        {
            get
            {
                if (m_settings.ContainsKey("User ID"))
                    return m_settings["User ID"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("User ID");
                else
                    m_settings["User ID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the user whom has access to the database.
        /// </summary>
        public string Password
        {
            get
            {
                if (m_settings.ContainsKey("Password"))
                    return m_settings["Password"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Password");
                else
                    m_settings["Password"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the setting for integrated security.
        /// </summary>
        public string IntegratedSecurity
        {
            get
            {
                if (m_settings.ContainsKey("Integrated Security"))
                    return m_settings["Integrated Security"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Integrated Security");
                else
                    m_settings["Integrated Security"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string used to access the database.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                foreach (string key in m_settings.Keys)
                {
                    if (builder.Length > 0)
                        builder.Append("; ");

                    builder.Append(key);
                    builder.Append('=');
                    builder.Append(m_settings[key]);
                }

                return builder.ToString();
            }
            set
            {
                m_settings = value.ParseKeyValuePairs();
            }
        }

        /// <summary>
        /// Gets the connection string without "pooling=false" setting in it (this will be used to store into the config files).
        /// </summary>
        public string PooledConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                foreach (string key in m_settings.Keys)
                {
                    if (string.Compare(key, "pooling", true) != 0)
                    {
                        if (builder.Length > 0)
                            builder.Append("; ");

                        builder.Append(key);
                        builder.Append('=');
                        builder.Append(m_settings[key]);
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Converts the current settings to an OleDB connection string.
        /// </summary>
        public string OleDbConnectionString
        {
            get
            {
                return "Provider=SQLOLEDB; " + ConnectionString;
            }
        }

        /// <summary>
        /// Gets or sets the data provider string used
        /// for establishing SQL Server connections.
        /// </summary>
        public string DataProviderString
        {
            get
            {
                return m_dataProviderString;
            }
            set
            {
                m_dataProviderString = value;
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

        public void GrantDatabaseAccess(string loginName)
        {
            ExecuteStatement(string.Format("IF EXISTS (SELECT * FROM sys.server_principals WHERE name = N'{0}') " +
                "AND NOT EXISTS (SELECT * FROM sys.server_principals svr INNER JOIN sys.database_principals db ON svr.sid = db.sid WHERE svr.name = N'{0}') " +
                "AND DATABASE_PRINCIPAL_ID('{0}') IS NULL CREATE USER [{0}] FOR LOGIN [{0}]", loginName));

            ExecuteStatement("IF DATABASE_PRINCIPAL_ID('openHistorianAdminRole') IS NULL CREATE ROLE [openHistorianAdminRole] AUTHORIZATION [dbo]");
            ExecuteStatement(string.Format("IF DATABASE_PRINCIPAL_ID('{0}') IS NOT NULL AND DATABASE_PRINCIPAL_ID('openHistorianAdminRole') IS NOT NULL EXEC sp_addrolemember N'openHistorianAdminRole', N'{0}'", loginName));
            ExecuteStatement(string.Format("IF DATABASE_PRINCIPAL_ID('openHistorianAdminRole') IS NOT NULL EXEC sp_addrolemember N'db_datareader', N'openHistorianAdminRole'"));
            ExecuteStatement(string.Format("IF DATABASE_PRINCIPAL_ID('openHistorianAdminRole') IS NOT NULL EXEC sp_addrolemember N'db_datawriter', N'openHistorianAdminRole'"));
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
                if ((object)connection != null)
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

                if ((object)connection != null)
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
                if ((object)scriptReader != null)
                    scriptReader.Dispose();
            }
        }

        public void OpenConnection(ref IDbConnection connection)
        {
            Dictionary<string, string> settings;
            string assemblyName, connectionTypeName, adapterTypeName;
            Assembly assembly;
            Type connectionType, adapterType;
            string dataProviderString;

            dataProviderString = m_dataProviderString;
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
            adapterType = assembly.GetType(adapterTypeName);

            connection = (IDbConnection)Activator.CreateInstance(connectionType);

            // Force use of non-pooled connection string such that database can later be deleted if needed
            connection.ConnectionString = PooledConnectionString + "; pooling=false";

            connection.Open();
        }

        #endregion
    }
}