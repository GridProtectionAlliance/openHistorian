//******************************************************************************************************
//  SubscriptionFramework.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN.Library
{
    public class SignalBook
    {
        public Guid SignalId;
        public long PointId;
        public long DeviceId;
        public string Description;
        public string DeviceName;
        public SignalBook(string line)
        {
            var parts = line.Split('\t');
            SignalId = Guid.Parse(parts[0]);
            PointId = long.Parse(parts[1]);
            DeviceId = long.Parse(parts[2]);
            Description = parts[3].Trim();
            DeviceName = parts[4].Trim();
        }

        public MetadataBase MakeSignal()
        {
            if (PointId < 0)
                return new MetadataSingle(SignalId, null, DeviceName, Description);
            else
                return new MetadataSingle(SignalId, (ulong)PointId, DeviceName, Description);
        }
    }

    public class AllSignals
    {
        public List<SignalBook> Signals;

        public AllSignals()
            : this(@"C:\Unison\GPA\ArchiveFiles\SignalMetadata.txt")
        {
        }
        public AllSignals(string config)
        {
            Signals = new List<SignalBook>();

            bool firstLine = true;
            foreach (var line in File.ReadAllLines(config))
            {
                if (firstLine)
                {
                    firstLine = false;
                }
                else
                {
                    Signals.Add(new SignalBook(line));
                }
            }
        }
    }
}
