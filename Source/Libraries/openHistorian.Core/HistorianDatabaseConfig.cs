//******************************************************************************************************
//  HistorianDatabaseConfig.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/05/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using GSF.SortedTreeStore.Encoding;
using openHistorian.Collections;
using openHistorian.SortedTreeStore.Types.CustomCompression.Ts;

namespace GSF.SortedTreeStore.Services.Configuration
{
    /// <summary>
    /// Creates a configuration for the database to utilize.
    /// </summary>
    public class HistorianDatabaseConfig
        : IToServerDatabaseSettings
    {
        private AdvancedServerDatabaseConfig<HistorianKey, HistorianValue> m_config;

        /// <summary>
        /// Gets a database config.
        /// </summary>
        public HistorianDatabaseConfig(string databaseName, string mainPath, bool supportsWriting)
        {
            m_config = new AdvancedServerDatabaseConfig<HistorianKey, HistorianValue>(databaseName, mainPath, supportsWriting);
            m_config.ArchiveEncodingMethod = CreateTsCombinedEncoding.TypeGuid;
            m_config.StreamingEncodingMethods.Add(CreateHistorianStreamEncoding.TypeGuid);
            m_config.StreamingEncodingMethods.Add(CreateHistorianFixedSizeCombinedEncoding.TypeGuid);
            m_config.IntermediateFileExtension = ".d2i";
            m_config.FinalFileExtension = ".d2";
        }

        /// <summary>
        /// The name associated with the database.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return m_config.DatabaseName;
            }
        }

        /// <summary>
        /// Gets all of the paths that are known by this historian.
        /// A path can be a file name or a folder.
        /// </summary>
        public List<string> ImportPaths
        {
            get
            {
                return m_config.ImportPaths;
            }
        }

        /// <summary>
        /// The list of directories where final files can be placed written. 
        /// If nothing is specified, the main directory is used.
        /// </summary>
        public List<string> FinalWritePaths
        {
            get
            {
                return m_config.FinalWritePaths;
            }
        }

        #region [ IToServerDatabaseSettings ]

        ServerDatabaseSettings IToServerDatabaseSettings.ToServerDatabaseSettings()
        {
            return ((IToServerDatabaseSettings)m_config).ToServerDatabaseSettings();
        }

        IToServerDatabaseSettings IToServerDatabaseSettings.Clone()
        {
            return ((IToServerDatabaseSettings)this).ToServerDatabaseSettings();
        }

        #endregion

    }
}