//******************************************************************************************************
//  OleDbMetadataProvider.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  07/20/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/17/2009 - Pinal C. Patel
//       Renamed ConnectString to ConnectionString.
//  02/03/2010 - Pinal C. Patel
//       Disabled the encryption of ConnectionString when persisted to the config file.
//
//******************************************************************************************************

using System;
using System.Data.OleDb;
using openHistorian.Archives.V1;
using TVA.Adapters;
using TVA.Configuration;
using TVA.Data;

namespace openHistorian.Metadata
{
    /// <summary>
    /// Represents a provider of data to a <see cref="MetadataFile"/> from any OLE DB data store.
    /// </summary>
    /// <seealso cref="MetadataUpdater"/>
    public class OleDbMetadataProvider : MetadataProviderBase
    {
        #region [ Members ]

        // Fields
        private string m_connectionString;
        private string m_selectString;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbMetadataProvider"/> class.
        /// </summary>
        public OleDbMetadataProvider()
            : base()
        {
            m_connectionString = string.Empty;
            m_selectString = string.Empty;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the connection string for connecting to the OLE DB data store of metadata.
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
        /// Gets or sets the SELECT statement for retrieving metadata from the OLE DB data store.
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
        /// Saves <see cref="OleDbMetadataProvider"/> settings to the config file if the <see cref="Adapter.PersistSettings"/> property is set to true.
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
                settings["SelectString", true].Update(m_selectString);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved <see cref="OleDbMetadataProvider"/> settings from the config file if the <see cref="Adapter.PersistSettings"/> property is set to true.
        /// </summary>
        public override void LoadSettings()
        {
            base.LoadSettings();
            if (PersistSettings)
            {
                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("ConnectionString", m_connectionString, "Connection string for connecting to the OLE DB data store of metadata.");
                settings.Add("SelectString", m_selectString, "SELECT statement for retrieving metadata from the OLE DB data store.");
                ConnectionString = settings["ConnectionString"].ValueAs(m_connectionString);
                SelectString = settings["SelectString"].ValueAs(m_selectString);
            }
        }

        /// <summary>
        /// Refreshes the <see cref="MetadataProviderBase.Metadata"/> from an OLE DB data store.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="ConnectionString"/> or <see cref="SelectString"/> is set to a null or empty string.</exception>
        protected override void RefreshMetadata()
        {
            if (string.IsNullOrEmpty(m_connectionString))
                throw new ArgumentNullException("ConnectionString");

            if (string.IsNullOrEmpty(m_selectString))
                throw new ArgumentNullException("SelectString");

            OleDbConnection connection = new OleDbConnection(m_connectionString);

            try
            {
                // Open OleDb connection.
                connection.Open();

                // Update existing metadata.
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
