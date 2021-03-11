//******************************************************************************************************
//  SubscriptionFramework.cs - Gbtc
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace openVisN.Library
{
    public class SignalBook
    {
        public Guid SignalId;
        public long PointId;
        public string Description;
        public string DeviceName;

        public SignalBook()
        {
            SignalId = Guid.NewGuid();
            PointId = -1;
            Description = string.Empty;
            DeviceName = Guid.NewGuid().ToString();
        }

        public SignalBook(string line)
        {
            string[] parts = line.Split('\t');
            SignalId = Guid.Parse(parts[0]);
            PointId = long.Parse(parts[1]);
            Description = parts[3].Trim();
            DeviceName = parts[4].Trim();
        }

        public SignalBook(DataRow row)
        {
            SignalId = IsNull(row, "SignalID", Guid.NewGuid());
            PointId = IsNull(row, "PointID", -1L);
            Description = IsNull(row, "Description", string.Empty);
            DeviceName = IsNull(row, "DeviceName", Guid.NewGuid().ToString());
        }

        public MetadataBase MakeSignal()
        {
            if (PointId < 0)
                return new MetadataSingle(SignalId, null, DeviceName, Description);
            else
                return new MetadataSingle(SignalId, (ulong)PointId, DeviceName, Description);
        }

        static T IsNull<T>(DataRow row, string columnName, T defaultValue)
        {
            if (row.IsNull(columnName))
                return defaultValue;

            return (T)row[columnName];
        }

    }

    public class AllSignals
    {
        public static List<SignalBook> DefaultSignals = new List<SignalBook>();

        public List<SignalBook> Signals;

        public AllSignals()
        {
            Signals = DefaultSignals.ToList();
        }

        public AllSignals(string config)
        {
            Signals = new List<SignalBook>();

            bool firstLine = true;
            foreach (string line in File.ReadAllLines(config))
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