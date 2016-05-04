//******************************************************************************************************
//  PostgresSetup.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  09/27/2011 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security;
using GSF;

namespace ConfigurationSetupUtility
{
    /// <summary>
    /// This class is used to aid in the manipulation of a PostgreSQL DB connection string
    /// as well as executing scripts and storing the data required for PostgreSQL DB setup.
    /// </summary>
    public class PostgresSetup
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Data provider string used to load the assembly for making PostgreSQL DB connections.
        /// </summary>
        public const string DataProviderString =
            "AssemblyName={Npgsql, Version=0.0.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7}; " +
            "ConnectionType=Npgsql.NpgsqlConnection; AdapterType=Npgsql.NpgsqlDataAdapter";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The name of the host running the PostgreSQL database engine.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The port on which the PostgreSQL database engine is listening for connections.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// The name of the database to be created.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The user name of the administrative user.
        /// </summary>
        public string AdminUserName { get; set; }

        /// <summary>
        /// The password of the administrative user.
        /// </summary>
        public SecureString AdminPassword { get; set; }

        /// <summary>
        /// The user name of the openPG schema user.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// The password of the openPG schema user.
        /// </summary>
        public SecureString RolePassword { get; set; }

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
                Dictionary<string, string> settings = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(HostName))
                    settings["Server"] = HostName;

                if (!string.IsNullOrEmpty(Port))
                    settings["Port"] = Port;

                if (!string.IsNullOrEmpty(DatabaseName))
                    settings["Database"] = DatabaseName.ToLower();

                if (!string.IsNullOrEmpty(RoleName))
                    settings["User Id"] = RoleName.ToLower();

                if ((object)RolePassword != null && RolePassword.Length > 0)
                    settings["Password"] = RolePassword.ToUnsecureString();

                return settings.JoinKeyValuePairs();
            }
            set
            {
                Dictionary<string, string> settings;

                string hostName;
                string port;
                string databaseName;
                string userName;
                string password;

                HostName = null;
                Port = null;
                RolePassword = null;

                settings = value.ParseKeyValuePairs();

                if (settings.TryGetValue("Server", out hostName))
                    HostName = hostName;

                if (settings.TryGetValue("Port", out port))
                    Port = port;

                if (settings.TryGetValue("User Id", out userName))
                    RoleName = userName;

                if (settings.TryGetValue("Password", out password))
                    RolePassword = password.ToSecureString();

                if (!settings.TryGetValue("User Id", out userName))
                    RoleName = null;
                else if (!userName.Equals(RoleName, StringComparison.OrdinalIgnoreCase))
                    RoleName = userName;

                if (!settings.TryGetValue("Database", out databaseName))
                    DatabaseName = null;
                else if (!databaseName.Equals(DatabaseName, StringComparison.OrdinalIgnoreCase))
                    DatabaseName = databaseName;
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
                Dictionary<string, string> settings = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(HostName))
                    settings["Server"] = HostName;

                if (!string.IsNullOrEmpty(Port))
                    settings["Port"] = Port;

                if (!string.IsNullOrEmpty(DatabaseName))
                    settings["Database"] = DatabaseName.ToLower();

                if (!string.IsNullOrEmpty(AdminUserName))
                    settings["User Id"] = AdminUserName.ToLower();

                if ((object)AdminPassword != null && AdminPassword.Length > 0)
                    settings["Password"] = AdminPassword.ToUnsecureString();

                return settings.JoinKeyValuePairs();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens a connection as the schema user.
        /// </summary>
        /// <param name="connection">The connection object to be created and opened.</param>
        public void OpenConnection(ref IDbConnection connection)
        {
            OpenConnection(ref connection, ConnectionString + "; Pooling=false");
        }

        /// <summary>
        /// Opens a connection as the admin user.
        /// </summary>
        /// <param name="connection">The connection object to be created and opened.</param>
        public void OpenAdminConnection(ref IDbConnection connection)
        {
            OpenConnection(ref connection, AdminConnectionString + "; Pooling=false");
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
                if ((object)connection != null)
                    connection.Dispose();
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
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = File.ReadAllText(fileName);
                command.ExecuteNonQuery();
            }
        }

        public void CreateLogin(string loginName)
        {
            string query =
                $"DO " +
                $"$body$ " +
                $"BEGIN " +
                $"    IF NOT EXISTS " +
                $"    ( " +
                $"        SELECT * " +
                $"        FROM pg_catalog.pg_user " +
                $"        WHERE usename = '{loginName}' " +
                $"    ) " +
                $"    THEN " +
                $"        CREATE ROLE {loginName} WITH LOGIN; " +
                $"    END IF;" +
                $"END " +
                $"$body$;";

            ExecuteStatement(query);
        }

        public void CreateLogin(string loginName, string password)
        {
            string query =
                $"DO " +
                $"$body$ " +
                $"BEGIN " +
                $"    IF NOT EXISTS " +
                $"    ( " +
                $"        SELECT * " +
                $"        FROM pg_catalog.pg_user " +
                $"        WHERE usename = '{loginName}' " +
                $"    ) " +
                $"    THEN " +
                $"        CREATE ROLE {loginName} WITH LOGIN PASSWORD '{password}'; " +
                $"    END IF;" +
                $"END " +
                $"$body$;";

            ExecuteStatement(query);
        }

        public void GrantDatabaseAccess(string loginName)
        {
            ExecuteStatement($"REVOKE CONNECT ON DATABASE {DatabaseName.ToLower()} FROM PUBLIC");
            ExecuteStatement($"GRANT CONNECT ON DATABASE {DatabaseName.ToLower()} TO {loginName}");
            ExecuteStatement($"GRANT ALL ON ALL TABLES IN SCHEMA public TO {loginName}");
            ExecuteStatement($"GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO {loginName}");
            ExecuteStatement($"GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO {loginName}");
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
            adapterType = assembly.GetType(adapterTypeName);

            connection = (IDbConnection)Activator.CreateInstance(connectionType);
            connection.ConnectionString = connectionString;
            connection.Open();
        }

        #endregion
    }
}
