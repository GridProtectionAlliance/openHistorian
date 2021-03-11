//******************************************************************************************************
//  WriteProcessorSettings.cs - Gbtc
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
//  10/03/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Data;
using GSF.IO;
using System.IO;
using GSF.Immutable;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// The settings for the Write Processor
    /// </summary>
    public class WriteProcessorSettings
        : SettingsBase<WriteProcessorSettings>
    {
        private bool m_isEnabled;
        private readonly PrebufferWriterSettings m_prebufferWriter;
        private readonly FirstStageWriterSettings m_firstStageWriter;
        private readonly ImmutableList<CombineFilesSettings> m_stagingRollovers;

        /// <summary>
        /// The default write processor settings
        /// </summary>
        public WriteProcessorSettings()
        {
            m_isEnabled = false;
            m_prebufferWriter = new PrebufferWriterSettings();
            m_firstStageWriter = new FirstStageWriterSettings();
            m_stagingRollovers = new ImmutableList<CombineFilesSettings>(x =>
            {
                if (x is null)
                    throw new ArgumentNullException("value", "cannot be null");
                return x;
            });
        }

        /// <summary>
        /// The settings for the prebuffer.
        /// </summary>
        public PrebufferWriterSettings PrebufferWriter => m_prebufferWriter;

        /// <summary>
        /// The settings for the first stage writer.
        /// </summary>
        public FirstStageWriterSettings FirstStageWriter => m_firstStageWriter;

        /// <summary>
        /// Contains all of the staging rollovers.
        /// </summary>
        public ImmutableList<CombineFilesSettings> StagingRollovers => m_stagingRollovers;

        /// <summary>
        /// Gets/Sets if writing will be enabled
        /// </summary>
        public bool IsEnabled
        {
            get => m_isEnabled;
            set
            {
                TestForEditable();
                m_isEnabled = false;
            }
        }

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(m_isEnabled);
            m_prebufferWriter.Save(stream);
            m_firstStageWriter.Save(stream);
            stream.Write(m_stagingRollovers.Count);
            foreach (CombineFilesSettings stage in m_stagingRollovers)
            {
                stage.Save(stream);
            }
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    m_isEnabled = stream.ReadBoolean();
                    m_prebufferWriter.Load(stream);
                    m_firstStageWriter.Load(stream);
                    int cnt = stream.ReadInt32();
                    m_stagingRollovers.Clear();
                    while (cnt > 0)
                    {
                        cnt--;
                        CombineFilesSettings cfs = new CombineFilesSettings();
                        cfs.Load(stream);
                        m_stagingRollovers.Add(cfs);
                    }
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);
            }
        }

        public override void Validate()
        {
            if (IsEnabled)
            {
                m_prebufferWriter.Validate();
                m_firstStageWriter.Validate();
                foreach (CombineFilesSettings stage in m_stagingRollovers)
                {
                    stage.Validate();
                }
            }
        }
    }
}