//******************************************************************************************************
//  ServerDatabaseSettings.cs - Gbtc
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.SortedTreeStore.Services.Writer;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// The settings for a <see cref="ServerDatabase{TKey,TValue}"/>
    /// </summary>
    public class ServerDatabaseSettings
        : IToServerDatabaseSettings
    {
        /// <summary>
        /// Creates a new <see cref="ServerDatabaseSettings"/>
        /// </summary>
        public ServerDatabaseSettings()
        {
            DatabaseName = string.Empty;
            ArchiveList = new ArchiveListSettings();
            WriteProcessor = new WriteProcessorSettings();
            RolloverLog = new RolloverLogSettings();
            KeyType = Guid.Empty;
            ValueType = Guid.Empty;
        }

        /// <summary>
        /// Gets the type of the key componenet
        /// </summary>
        public Guid KeyType { get; set; }

        /// <summary>
        /// Gets the type of the value componenent.
        /// </summary>
        public Guid ValueType { get; set; }

        /// <summary>
        /// The name associated with the database.
        /// </summary>
        public string DatabaseName;

        /// <summary>
        /// Gets the supported streaming methods.
        /// </summary>
        public List<EncodingDefinition> StreamingEncodingMethods = new List<EncodingDefinition>();

        /// <summary>
        /// The settings for the ArchiveList.
        /// </summary>
        public ArchiveListSettings ArchiveList;

        /// <summary>
        /// Settings for the writer. Null if the server does not support writing.
        /// </summary>
        public WriteProcessorSettings WriteProcessor;

        /// <summary>
        /// The settings for the rollover log.
        /// </summary>
        public RolloverLogSettings RolloverLog;

   

       


        ServerDatabaseSettings IToServerDatabaseSettings.ToServerDatabaseSettings()
        {
            return this;
        }

        IToServerDatabaseSettings IToServerDatabaseSettings.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a clone of this class.
        /// </summary>
        /// <returns></returns>
        public ServerDatabaseSettings Clone()
        {
            var obj = (ServerDatabaseSettings)MemberwiseClone();
            obj.ArchiveList = ArchiveList.Clone();
            obj.WriteProcessor = WriteProcessor.Clone();
            return obj;
        }
    }

}
