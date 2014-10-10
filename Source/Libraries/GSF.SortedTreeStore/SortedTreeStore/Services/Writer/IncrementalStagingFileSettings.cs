//******************************************************************************************************
//  IncrementalStagingFileSettings.cs - Gbtc
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
//  02/16/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using GSF.IO;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// The settings for an <see cref="IncrementalStagingFile{TKey,TValue}"/>
    /// </summary>
    public class IncrementalStagingFileSettings
        : SettingsBase<IncrementalStagingFileSettings>
    {
        private ArchiveInitializerSettings m_initialSettings = new ArchiveInitializerSettings();
        private ArchiveInitializerSettings m_finalSettings = new ArchiveInitializerSettings();
        private string m_finalFileExtension = ".d2i";

        /// <summary>
        /// The settings for the archive initializer. This value cannot be null.
        /// </summary>
        public ArchiveInitializerSettings InitialSettings
        {
            get
            {
                return m_initialSettings;
            }
        }

        /// <summary>
        /// The settings for the archive initializer. This value cannot be null.
        /// </summary>
        public ArchiveInitializerSettings FinalSettings
        {
            get
            {
                return m_finalSettings;
            }
        }

        /// <summary>
        /// The extension to change the file to after writing all of the data.
        /// </summary>
        public string FinalFileExtension
        {
            get
            {
                return m_finalFileExtension;
            }
            set
            {
                TestForEditable();
                m_finalFileExtension = PathHelpers.FormatExtension(value);
            }
        }

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(m_finalFileExtension);
            m_initialSettings.Save(stream);
            m_finalSettings.Save(stream);
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    m_finalFileExtension = stream.ReadString();
                    m_initialSettings.Load(stream);
                    m_finalSettings.Load(stream);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);
            }
        }

        public override void Validate()
        {
            m_initialSettings.Validate();
            m_finalSettings.Validate();
        }
    }
}