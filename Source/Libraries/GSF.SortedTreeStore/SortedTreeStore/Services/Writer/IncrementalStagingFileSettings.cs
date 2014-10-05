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
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// The settings for an <see cref="IncrementalStagingFile{TKey,TValue}"/>
    /// </summary>
    public class IncrementalStagingFileSettings
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
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                m_initialSettings = value;
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
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                m_finalSettings = value;
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
                m_finalFileExtension = PathHelpers.FormatExtension(value);
            }
        }

        /// <summary>
        /// Clones the <see cref="ArchiveInitializerSettings"/>
        /// </summary>
        /// <returns></returns>
        public IncrementalStagingFileSettings Clone()
        {
            var other = (IncrementalStagingFileSettings)MemberwiseClone();
            other.m_initialSettings = m_initialSettings.Clone();
            other.m_finalSettings = m_finalSettings.Clone();
            return other;
        }


    }
    ///// <summary>
    ///// The settings for an <see cref="IncrementalStagingFile{TKey,TValue}"/>
    ///// </summary>
    //public class IncrementalStagingFileSettings
    //{
    //    /// <summary>
    //    /// Determines if the archive is a Memory Archive
    //    /// </summary>
    //    public bool IsMemoryArchive = true;
    //    /// <summary>
    //    /// Gets the encoding method to write final files in.
    //    /// </summary>
    //    public EncodingDefinition Encoding = SortedTree.FixedSizeNode;
    //    /// <summary>
    //    /// The save path to write final archive files in.
    //    /// </summary>
    //    public string SavePath = string.Empty;

    //    /// <summary>
    //    /// Sets the file extension that will be used when initially writing to the file. Should appear with a leading period only.
    //    /// </summary>
    //    public string PendingFileExtension = ".~d2i";

    //    /// <summary>
    //    /// Sets the file extension that the pending file will be renamed to upon completion. Should appear with a leading period only.
    //    /// </summary>
    //    public string CommittedFileExtension = ".d2i";
    //}
}