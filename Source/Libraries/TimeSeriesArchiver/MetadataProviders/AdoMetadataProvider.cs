//******************************************************************************************************
//  AdoMetadataProvider.cs - Gbtc
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
//  -----------------------------------------------------------------------------------------------------
//  09/15/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//  09/17/2009 - Pinal C. Patel
//       Renamed ConnectString to ConnectionString.
//  12/11/2009 - Pinal C. Patel
//       Disabled the encryption of DataProviderString when persisted to the config file.
//  02/03/2010 - Pinal C. Patel
//       Disabled the encryption of ConnectionString when persisted to the config file.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using TVA;
using TVA.Configuration;
using TVA.Data;

namespace TimeSeriesArchiver.MetadataProviders
{
    /// <summary>
    /// Represents a provider of data to a <see cref="TimeSeriesArchiver.Files.MetadataFile"/> from any ADO.NET based data store.
    /// </summary>
    /// <seealso cref="MetadataUpdater"/>
    public class AdoMetadataProvider : MetadataProviderBase
    {
        #region [ Members ]

        // Fields
        private string m_connectionString;
        private string m_dataProviderString;
        private string m_selectString;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoMetadataProvider"/> class.
        /// </summary>
        public AdoMetadataProvider()
            : base()
        {
            m_connectionString = string.Empty;
            m_dataProviderString = string.Empty;
            m_selectString = string.Empty;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the connection string for connecting to the ADO.NET based data store of metadata.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return m_connectionString;
            }
            set
            {
                m_connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the ADO.NET data provider assembly type creation string.
        /// </summary>
        /// <remarks>
        /// Expected keys: AssemblyName;ConnectionType<br/>
        /// Examples:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Database Connection Type</term>
        ///         <description>Example ADO.NET Data Provider String</description>
        ///     </listheader>
        ///     <item>
        ///         <term>SQL Server</term>
        ///         <description>AssemblyName={System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089};ConnectionType=System.Data.SqlClient.SqlConnection</description>
        ///     </item>
        ///     <item>
        ///         <term>MySQL</term>
        ///         <description>AssemblyName={MySql.Data, Version=5.2.7.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d};ConnectionType=MySql.Data.MySqlClient.MySqlConnection</description>
        ///     </item>
        ///     <item>
        ///         <term>Oracle</term>
        ///         <description>AssemblyName={System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089};ConnectionType=System.Data.OracleClient.OracleConnection</description>
        ///     </item>
        ///     <item>
        ///         <term>OleDb</term>
        ///         <description>AssemblyName={System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089};ConnectionType=System.Data.OleDb.OleDbConnection</description>
        ///     </item>
        ///     <item>
        ///         <term>ODBC</term>
        ///         <description>AssemblyName={System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089};ConnectionType=System.Data.Odbc.OdbcConnection</description>
        ///     </item>
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// Gets or sets the SELECT statement for retrieving metadata from the ADO.NET based data store.
        /// </summary>
        public string SelectString
        {
            get
            {
                return m_selectString;
            }
            set
            {
                m_selectString = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Saves <see cref="AdoMetadataProvider"/> settings to the config file if the <see cref="MetadataProviderBase.PersistSettings"/> property is set to true.
        /// </summary>
        public override void SaveSettings()
        {
            base.SaveSettings();
            if (PersistSettings)
            {
                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings["ConnectionString", true].Update(m_connectionString);
                settings["DataProviderString", true].Update(m_dataProviderString);
                settings["SelectString", true].Update(m_selectString);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved <see cref="AdoMetadataProvider"/> settings from the config file if the <see cref="MetadataProviderBase.PersistSettings"/> property is set to true.
        /// </summary>
        public override void LoadSettings()
        {
            base.LoadSettings();
            if (PersistSettings)
            {
                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("ConnectionString", m_connectionString, "Connection string for connecting to the ADO.NET based data store of metadata.");
                settings.Add("DataProviderString", m_dataProviderString, "The ADO.NET data provider assembly type creation string used to create a connection to the data store of metadata.");
                settings.Add("SelectString", m_selectString, "SELECT statement for retrieving metadata from the ADO.NET based data store.");
                ConnectionString = settings["ConnectionString"].ValueAs(m_connectionString);
                DataProviderString = settings["DataProviderString"].ValueAs(m_dataProviderString);
                SelectString = settings["SelectString"].ValueAs(m_selectString);
            }
        }

        /// <summary>
        /// Refreshes the <see cref="MetadataProviderBase.Metadata"/> from an ADO.NET based data store.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="ConnectionString"/> or <see cref="SelectString"/> is set to a null or empty string.</exception>
        protected override void RefreshMetadata()
        {
            if (string.IsNullOrEmpty(m_connectionString))
                throw new ArgumentNullException("ConnectionString");

            if (string.IsNullOrEmpty(m_selectString))
                throw new ArgumentNullException("SelectString");
            
            // Attempt to load configuration from an ADO.NET database connection
            IDbConnection connection = null;
            Dictionary<string, string> settings;
            string assemblyName, connectionTypeName, adapterTypeName;
            Assembly assembly;
            Type connectionType, adapterType;

            try
            {
                settings = m_dataProviderString.ParseKeyValuePairs();
                assemblyName = settings["AssemblyName"].ToNonNullString();
                connectionTypeName = settings["ConnectionType"].ToNonNullString();
                adapterTypeName = settings["AdapterType"].ToNonNullString();

                if (string.IsNullOrEmpty(connectionTypeName))
                    throw new InvalidOperationException("Database connection type was not defined");

                if (string.IsNullOrEmpty(adapterTypeName))
                    throw new InvalidOperationException("Database adapter type was not defined");

                assembly = Assembly.Load(new AssemblyName(assemblyName));
                connectionType = assembly.GetType(connectionTypeName);
                adapterType = assembly.GetType(adapterTypeName);

                // Open ADO.NET provider connection
                connection = (IDbConnection)Activator.CreateInstance(connectionType);
                connection.ConnectionString = m_connectionString;
                connection.Open();

                // Update existing metadata
                MetadataUpdater metadataUpdater = new MetadataUpdater(Metadata);
                metadataUpdater.UpdateMetadata(connection.ExecuteReader(m_selectString));
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        #endregion
    }
}
