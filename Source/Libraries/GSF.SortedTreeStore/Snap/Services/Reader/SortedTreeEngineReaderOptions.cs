//******************************************************************************************************
//  SortedTreeEngineReaderOptions.cs - Gbtc
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Data;
using GSF.IO;

namespace GSF.Snap.Services.Reader
{
    /// <summary>
    /// Contains the options to use for executing an individual read request.
    /// </summary>
    public class SortedTreeEngineReaderOptions
    {
        /// <summary>
        /// Default options. Same as default constructor
        /// </summary>
        public static SortedTreeEngineReaderOptions Default
        {
            get;
            private set;
        }

        static SortedTreeEngineReaderOptions()
        {
            Default = new SortedTreeEngineReaderOptions();
        }

        /// <summary>
        /// Creates <see cref="SortedTreeEngineReaderOptions"/>.
        /// </summary>
        /// <param name="timeout">the time before a query will end prematurely</param>
        /// <param name="maxReturnedCount">the maximum number of Key/Values to send to the client before ending prematurely</param>
        /// <param name="maxScanCount">the maximum number of points for the database to read before ending prematurely</param>
        /// <param name="maxSeekCount">the maximum seeks that will occur before ending prematurely</param>
        public SortedTreeEngineReaderOptions(TimeSpan timeout = default, long maxReturnedCount = 0, long maxScanCount = 0, long maxSeekCount = 0)
        {
            Timeout = timeout;
            MaxReturnedCount = maxReturnedCount;
            MaxScanCount = maxScanCount;
            MaxSeekCount = maxSeekCount;
        }

        /// <summary>
        /// Creates a new <see cref="SortedTreeEngineReaderOptions"/> from a stream
        /// </summary>
        /// <param name="stream">the stream to read from</param>
        public SortedTreeEngineReaderOptions(BinaryStreamBase stream)
        {
            byte version = stream.ReadUInt8();
            switch (version)
            {
                case 0:
                    Timeout = new TimeSpan(stream.ReadInt64());
                    MaxReturnedCount = stream.ReadInt64();
                    MaxScanCount = stream.ReadInt64();
                    MaxSeekCount = stream.ReadInt64();
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
        }

        /// <summary>
        /// Writes this data to the <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">the stream to write data to</param>
        public void Save(BinaryStreamBase stream)
        {
            stream.Write((byte)0);
            stream.Write(Timeout.Ticks);
            stream.Write(MaxReturnedCount);
            stream.Write(MaxScanCount);
            stream.Write(MaxSeekCount);
        }

        /// <summary>
        /// The time before the query times out.
        /// </summary>
        public TimeSpan Timeout
        {
            get;
            private set;
        }

        /// <summary>
        /// The maximum number of points to return. 0 means no limit.
        /// </summary>
        public long MaxReturnedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The maximum number of points to scan to get the results set. 
        /// This includes any point that was filtered
        /// </summary>
        public long MaxScanCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The maximum number of seeks permitted
        /// </summary>
        public long MaxSeekCount
        {
            get;
            private set;
        }
    }
}