//******************************************************************************************************
//  ServerDatabaseSettings.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using GSF.IO;
using GSF.Immutable;
using GSF.Snap.Services.Writer;

namespace GSF.Snap.Services
{
    /// <summary>
    /// The settings for a <see cref="SnapServerDatabase{TKey,TValue}"/>
    /// </summary>
    public class ServerDatabaseSettings
        : SettingsBase<ServerDatabaseSettings>, IToServerDatabaseSettings
    {
        private Guid m_keyType;
        private Guid m_valueType;
        private string m_databaseName;
        private readonly ImmutableList<EncodingDefinition> m_streamingEncodingMethods;
        private readonly ArchiveListSettings m_archiveList;
        private readonly WriteProcessorSettings m_writeProcessor;
        private readonly RolloverLogSettings m_rolloverLog;
        private bool m_supportsWriting;

        /// <summary>
        /// Creates a new <see cref="ServerDatabaseSettings"/>
        /// </summary>
        public ServerDatabaseSettings()
        {
            m_databaseName = string.Empty;
            m_archiveList = new ArchiveListSettings();
            m_writeProcessor = new WriteProcessorSettings();
            m_rolloverLog = new RolloverLogSettings();
            m_keyType = Guid.Empty;
            m_valueType = Guid.Empty;
            m_streamingEncodingMethods = new ImmutableList<EncodingDefinition>(x =>
            {
                if (x is null)
                    throw new ArgumentNullException("value");
                return x;
            });
        }

        /// <summary>
        /// Gets the type of the key componenet
        /// </summary>
        public Guid KeyType
        {
            get => m_keyType;
            set
            {
                TestForEditable();
                m_keyType = value;
            }
        }

        /// <summary>
        /// Gets the type of the value componenent.
        /// </summary>
        public Guid ValueType
        {
            get => m_valueType;
            set
            {
                TestForEditable();
                m_valueType = value;
            }
        }

        /// <summary>
        /// The name associated with the database.
        /// </summary>
        public string DatabaseName
        {
            get => m_databaseName;
            set
            {
                TestForEditable();
                m_databaseName = value;
            }
        }

        /// <summary>
        /// Gets the supported streaming methods.
        /// </summary>
        public ImmutableList<EncodingDefinition> StreamingEncodingMethods => m_streamingEncodingMethods;

        /// <summary>
        /// The settings for the ArchiveList.
        /// </summary>
        public ArchiveListSettings ArchiveList => m_archiveList;

        /// <summary>
        /// Settings for the writer. Null if the server does not support writing.
        /// </summary>
        public WriteProcessorSettings WriteProcessor => m_writeProcessor;

        /// <summary>
        /// The settings for the rollover log.
        /// </summary>
        public RolloverLogSettings RolloverLog => m_rolloverLog;

        /// <summary>
        /// Gets if writing or file combination will be enabled.
        /// </summary>
        public bool SupportsWriting
        {
            get => m_supportsWriting;
            set
            {
                TestForEditable();
                m_supportsWriting = value;
            }
        }

        ServerDatabaseSettings IToServerDatabaseSettings.ToServerDatabaseSettings()
        {
            return this;
        }

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(m_keyType);
            stream.Write(m_valueType);
            stream.Write(m_databaseName);
            stream.Write(m_streamingEncodingMethods.Count);
            foreach (EncodingDefinition path in m_streamingEncodingMethods)
            {
                path.Save(stream);
            }
            m_archiveList.Save(stream);
            m_writeProcessor.Save(stream);
            m_rolloverLog.Save(stream);
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    m_keyType = stream.ReadGuid();
                    m_valueType = stream.ReadGuid();
                    m_databaseName = stream.ReadString();
                    int cnt = stream.ReadInt32();
                    m_streamingEncodingMethods.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        m_streamingEncodingMethods.Add(new EncodingDefinition(stream));
                    }
                    m_archiveList.Load(stream);
                    m_writeProcessor.Load(stream);
                    m_rolloverLog.Load(stream);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);

            }
        }

        public override void Validate()
        {
            m_archiveList.Validate();
            m_rolloverLog.Validate();
            if (m_supportsWriting)
                m_writeProcessor.Validate();
            if (StreamingEncodingMethods.Count == 0)
                throw new Exception("Must specify a streaming method");
        }
    }

}
