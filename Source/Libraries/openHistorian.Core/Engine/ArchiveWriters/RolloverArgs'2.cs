//******************************************************************************************************
//  PrestageWriter.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using openHistorian.Archive;
using openHistorian.Collections.Generic;

namespace openHistorian.Engine.ArchiveWriters
{
    public class RolloverArgs<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        /// <summary>
        /// Contains the archive file unless it comes from the prestaging table.
        /// </summary>
        public ArchiveTable<TKey, TValue> File
        {
            get;
            private set;
        }

        public TreeStream<TKey, TValue> CurrentStream
        {
            get;
            private set;
        }

        public long SequenceNumber
        {
            get;
            private set;
        }

        public RolloverArgs(ArchiveTable<TKey, TValue> file, TreeStream<TKey, TValue> stream, long sequenceNumber)
        {
            File = file;
            CurrentStream = stream;
            SequenceNumber = sequenceNumber;
        }
    }
}