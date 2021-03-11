//******************************************************************************************************
//  FirstStageWriterSettings.cs - Gbtc
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
//  09/18/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Data;
using System.IO;
using GSF.IO;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// The settings for the <see cref="FirstStageWriter{TKey,TValue}"/>
    /// </summary>
    public class FirstStageWriterSettings
        : SettingsBase<FirstStageWriterSettings>
    {
        private int m_rolloverInterval = 10000;
        private int m_rolloverSizeMb = 80;
        private int m_maximumAllowedMb = 100;
        private readonly SimplifiedArchiveInitializerSettings m_finalSettings = new SimplifiedArchiveInitializerSettings();
        private EncodingDefinition m_encodingMethod = EncodingDefinition.FixedSizeCombinedEncoding;
        
        /// <summary>
        /// The number of milliseconds before data is flushed to the disk. 
        /// </summary>
        /// <remarks>
        /// Must be between 1,000 ms and 60,000 ms.
        /// </remarks>
        public int RolloverInterval
        {
            get => m_rolloverInterval;
            set
            {
                TestForEditable();
                if (value < 1000)
                {
                    m_rolloverInterval = 1000;
                }
                else if (value > 60000)
                {
                    m_rolloverInterval = 60000;
                }
                else
                {
                    m_rolloverInterval = value;
                }
            }
        }

        /// <summary>
        /// The size at which a rollover will be signaled
        /// </summary>
        /// <remarks>
        /// Must be at least 1MB. Upper Limit should be Memory Constrained, but not larger than 1024MB.
        /// </remarks>
        public int RolloverSizeMb
        {
            get => m_rolloverSizeMb;
            set
            {
                TestForEditable();
                if (value < 1)
                {
                    m_rolloverSizeMb = 1;
                }
                else if (value > 1024)
                {
                    m_rolloverSizeMb = 1024;
                }
                else
                {
                    m_rolloverSizeMb = value;
                }
            }
        }

        /// <summary>
        /// The size after which the incoming write queue will pause
        /// to wait for rollovers to complete.
        /// </summary>
        /// <remarks>
        /// It is recommended to make this value larger than <see cref="RolloverSizeMb"/>.
        /// If this value is smaller than <see cref="RolloverSizeMb"/> then <see cref="RolloverSizeMb"/> will be used.
        /// Must be at least 1MB. Upper Limit should be Memory Constrained, but not larger than 1024MB.
        /// </remarks>
        public int MaximumAllowedMb
        {
            get => m_maximumAllowedMb;
            set
            {
                TestForEditable();
                if (value < 1)
                {
                    m_maximumAllowedMb = 1;
                }
                else if (value > 1024)
                {
                    m_maximumAllowedMb = 1024;
                }
                else
                {
                    m_maximumAllowedMb = value;
                }
            }
        }

        /// <summary>
        /// The encoding method that will be used to write files.
        /// </summary>
        public EncodingDefinition EncodingMethod
        {
            get => m_encodingMethod;
            set
            {
                TestForEditable();
                if (value is null)
                    throw new ArgumentNullException("value");
                m_encodingMethod = value;
            }
        }

        /// <summary>
        /// The settings that will be used for the rolled over files.
        /// </summary>
        public SimplifiedArchiveInitializerSettings FinalSettings => m_finalSettings;

        public override void Save(Stream stream)
        {
            stream.Write((byte)1);
            stream.Write(m_rolloverInterval);
            stream.Write(m_rolloverSizeMb);
            stream.Write(m_maximumAllowedMb);
            m_finalSettings.Save(stream);
            m_encodingMethod.Save(stream);
        }

        public override void Load(Stream stream)
        {
            TestForEditable();
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    m_rolloverInterval = stream.ReadInt32();
                    m_rolloverSizeMb = stream.ReadInt32();
                    m_maximumAllowedMb = stream.ReadInt32();
                    m_finalSettings.Load(stream);
                    m_encodingMethod = new EncodingDefinition(stream);
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version Code: " + version);
            }
        }

        public override void Validate()
        {
            m_finalSettings.Validate();
        }
    }
}