//******************************************************************************************************
//  DataReaderOptions.cs - Gbtc
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
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Data;
using GSF.IO;

namespace openHistorian
{
    /// <summary>
    /// Contains the options to use for executing an individual read request.
    /// </summary>
    public class DataReaderOptions
    {
        public static DataReaderOptions Default { get; private set; }
        static DataReaderOptions()
        {
            Default = new DataReaderOptions();
        }

        public DataReaderOptions(TimeSpan timeout = default(TimeSpan), long maxReturnedCount = 0, long maxScanCount = 0, long maxSeekCount = 0)
        {
            Timeout = timeout;
            MaxReturnedCount = maxReturnedCount;
            MaxScanCount = maxScanCount;
            MaxSeekCount = maxSeekCount;
        }

        public DataReaderOptions(BinaryStreamBase stream)
        {
            var version = stream.ReadByte();
            switch (version)
            {
                case 0:
                    Timeout=new TimeSpan(stream.ReadInt64());
                    MaxReturnedCount = stream.ReadInt64();
                    MaxScanCount = stream.ReadInt64();
                    MaxSeekCount = stream.ReadInt64();
                    break;
                default:
                    throw new VersionNotFoundException("Unknown Version");
            }
        }

        public void Save(BinaryStreamBase stream)
        {
            stream.Write((byte)0);
            stream.Write(Timeout.Ticks); 
            stream.Write(MaxReturnedCount); 
            stream.Write(MaxScanCount); 
            stream.Write(MaxSeekCount); 
        }

        public TimeSpan Timeout { get; private set; }
        public long MaxReturnedCount { get; private set; }
        public long MaxScanCount { get; private set; }
        public long MaxSeekCount { get; private set; }
    }
}
