//******************************************************************************************************
//  MySqlSetup.cs - Gbtc
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
//  02/23/2011 - Mehulbhai Thakkar
//       Added "Allow User Variables" setting so that session variables can be created without errors.
//  03/02/2011 - J. Ritchie Carroll
//       Added key value delimeters only between settings.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using GSF.Data;
using GSF;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of a MySQL connection string as well as running the mysql.exe process.
    /// </summary>
    public class MySqlSetup
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
        public MySqlSetup()
        {
            m_settings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            m_settings["Allow User Variables"] = "true";    // This setting allows creation of user defined session variables.
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the host name of the MySQL database.
        /// </summary>
        public string HostName
        {
            get => m_settings["Server"];
            set => m_settings["Server"] = value;
        }

        /// <summary>
        /// Gets or sets the name of the MySQL database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                if (m_settings.ContainsKey("Database"))
                    return m_settings["Database"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Database");
                else
                    m_settings["Database"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user name for the user whom has access to the database.
        /// </summary>
        public string UserName
        {
            get
            {
                if (m_settings.ContainsKey("Uid"))
                    return m_settings["Uid"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Uid");
                else
                    m_settings["Uid"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the user whom has access to the database.
        /// </summary>
        public string Password
        {
            get
            {
                if (m_settings.ContainsKey("Pwd"))
                    return m_settings["Pwd"];
                else
                    return null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_settings.Remove("Pwd");
                else
                    m_settings["Pwd"] = value;
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
            set => m_settings = value.ParseKeyValuePairs();
        }

        /// <summary>
        /// Converts the current settings to an OleDB connection string.
        /// </summary>
        public string OleDbConnectionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("Provider=MySQLProv");
                builder.Append("; location=");
                builder.Append(HostName.Replace("localhost", "MACHINE"));
                builder.Append("; Data Source=");
                builder.Append(DatabaseName);

                if (!string.IsNullOrEmpty(UserName))
                {
                    builder.Append("; User Id=");
                    builder.Append(UserName);
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    builder.Append("; Password=");
                    builder.Append(Password);
                }

                return builder.ToString();
            }
        }

        public string DataProviderString
        {
            get => m_dataProviderString;
            set => m_dataProviderString = value;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Execute a SQL statement using the mysql.exe process.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
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
                connection?.Dispose();
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
            TextReader scriptReader = null;

            try
            {
                string line;
                string delimiter;

                scriptReader = File.OpenText(fileName);
                line = scriptReader.ReadLine();
                delimiter = ";";

                using (IDbCommand command = connection.CreateCommand())
                {
                    StringBuilder statementBuilder = new StringBuilder();
                    Regex comment = new Regex(@"/\*.*\*/|--.*\n", RegexOptions.Multiline);

                    while ((object)line != null)
                    {
                        string statement;

                        if (line.StartsWith("CREATE DATABASE") || line.StartsWith("USE"))
                            line = line.Replace("openHistorian", DatabaseName);

                        if (line.StartsWith("DELIMITER "))
                        {
                            delimiter = line.Split(' ')[1].Trim();
                        }
                        else
                        {
                            statementBuilder.Append(line);
                            statementBuilder.Append('\n');
                            statement = statementBuilder.ToString();
                            statement = comment.Replace(statement, " ").Trim();

                            if (statement.EndsWith(delimiter))
                            {
                                // Remove trailing delimiter.
                                statement = statement.Remove(statement.Length - delimiter.Length);

                                // Remove comments and execute the statement.
                                command.CommandText = statement;
                                command.ExecuteNonQuery();
                                statementBuilder.Clear();
                            }
                        }

                        // Read the next line from the file.
                        line = scriptReader.ReadLine();
                    }
                }
            }
            finally
            {
                scriptReader?.Dispose();
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
            _ = assembly.GetType(adapterTypeName);

            connection = (IDbConnection)Activator.CreateInstance(connectionType);
            connection.ConnectionString = ConnectionString;
            connection.Open();
        }

        #endregion
    }
}