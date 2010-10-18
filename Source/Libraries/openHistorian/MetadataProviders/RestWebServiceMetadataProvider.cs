//******************************************************************************************************
//  RestWebServiceMetadataProvider.cs - Gbtc
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
//  08/11/2009 - Pinal C. Patel
//       Generated original version of source code.
//  08/18/2009 - Pinal C. Patel
//       Added cleanup code for response stream of the REST web service.
//  08/21/2009 - Pinal C. Patel
//       Moved RestDataFormat to Services namespace as SerializationFormat.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Net;
using TVA.Configuration;
using TVA.Web.Services;

namespace openHistorian.MetadataProviders
{   
    /// <summary>
    /// Represents a provider of data to a <see cref="openHistorian.Files.MetadataFile"/> from a REST (Representational State Transfer) web service.
    /// </summary>
    /// <seealso cref="MetadataUpdater"/>
    public class RestWebServiceMetadataProvider : MetadataProviderBase
    {
        #region [ Members ]

        // Fields
        private string m_serviceUri;
        private SerializationFormat m_serviceDataFormat;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RestWebServiceMetadataProvider"/> class.
        /// </summary>
        public RestWebServiceMetadataProvider()
            : base()
        {
            m_serviceUri = string.Empty;
            m_serviceDataFormat = SerializationFormat.PoxRest;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the URI where the REST web service is hosted.
        /// </summary>
        public string ServiceUri
        {
            get
            {
                return m_serviceUri;
            }
            set
            {
                m_serviceUri = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SerializationFormat"/> in which the REST web service exposes the data.
        /// </summary>
        public SerializationFormat ServiceDataFormat
        {
            get
            {
                return m_serviceDataFormat;
            }
            set
            {
                m_serviceDataFormat = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Saves <see cref="RestWebServiceMetadataProvider"/> settings to the config file if the <see cref="MetadataProviderBase.PersistSettings"/> property is set to true.
        /// </summary>
        public override void SaveSettings()
        {
            base.SaveSettings();
            if (PersistSettings)
            {
                // Save settings under the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings["ServiceUri", true].Update(m_serviceUri);
                settings["ServiceDataFormat", true].Update(m_serviceDataFormat);
                config.Save();
            }
        }

        /// <summary>
        /// Loads saved <see cref="RestWebServiceMetadataProvider"/> settings from the config file if the <see cref="MetadataProviderBase.PersistSettings"/> property is set to true.
        /// </summary>
        public override void LoadSettings()
        {
            base.LoadSettings();
            if (PersistSettings)
            {
                // Load settings from the specified category.
                ConfigurationFile config = ConfigurationFile.Current;
                CategorizedSettingsElementCollection settings = config.Settings[SettingsCategory];
                settings.Add("ServiceUri", m_serviceUri, "URI where the REST web service is hosted.");
                settings.Add("ServiceDataFormat", m_serviceDataFormat, "Format (Json; PoxAsmx; PoxRest) in which the REST web service exposes the data.");
                ServiceUri = settings["ServiceUri"].ValueAs(m_serviceUri);
                ServiceDataFormat = settings["ServiceDataFormat"].ValueAs(m_serviceDataFormat);
            }
        }

        /// <summary>
        /// Refreshes the <see cref="MetadataProviderBase.Metadata"/> from a REST web service.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="ServiceUri"/> is set to a null or empty string.</exception>
        protected override void RefreshMetadata()
        {
            if (string.IsNullOrEmpty(m_serviceUri))
                throw new ArgumentNullException("ServiceUri");

            WebResponse response = null;
            Stream responseStream = null;
            try
            {
                // Retrieve new metadata.
                response = WebRequest.Create(m_serviceUri).GetResponse();
                responseStream = response.GetResponseStream();

                // Update existing metadata.
                MetadataUpdater metadataUpdater = new MetadataUpdater(Metadata);
                metadataUpdater.UpdateMetadata(responseStream, m_serviceDataFormat);
            }
            finally
            {
                if (response != null)
                    response.Close();

                if (responseStream != null)
                    responseStream.Dispose();
            }
        }

        #endregion
    }
}
