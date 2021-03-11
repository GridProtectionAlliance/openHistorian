//******************************************************************************************************
//  AllSignalGroups.cs - Gbtc
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
    public class SignalGroupBook
    {
        public string GroupName;
        public double NominalVoltage;
        public long Im;
        public long Ia;
        public long Vm;
        public long Va;
        public long Dfdt;
        public long Freq;
        public long Status;

        public SignalGroupBook()
        {
            GroupName = Guid.NewGuid().ToString();
            NominalVoltage = 1;
            Im = -1;
            Ia = -1;
            Vm = -1;
            Va = -1;
            Dfdt = -1;
            Freq = -1;
            Status = -1;
        }

        public SignalGroupBook(string line)
        {
            string[] parts = line.Split('\t');
            GroupName = parts[1].Trim();
            NominalVoltage = double.Parse(parts[3]);
            Im = long.Parse(parts[9]);
            Ia = long.Parse(parts[10]);
            Vm = long.Parse(parts[11]);
            Va = long.Parse(parts[12]);
            Dfdt = long.Parse(parts[13]);
            Freq = long.Parse(parts[14]);
            Status = long.Parse(parts[15]);
        }

        public SignalGroupBook(DataRow row)
        {
            GroupName = IsNull(row, "GroupName", Guid.NewGuid().ToString());
            NominalVoltage = IsNull(row, "NominalVoltage", 1.0);
            Im = IsNull(row, "CurrentMagnitude", -1L);
            Ia = IsNull(row, "CurrentAngle", -1L);
            Vm = IsNull(row, "VoltageMagnitude", -1L);
            Va = IsNull(row, "VoltageAngle", -1L);
            Dfdt = IsNull(row, "DFDT", -1L);
            Freq = IsNull(row, "Frequency", -1L);
            Status = IsNull(row, "Status", -1L);
        }

        public SignalGroup CreateGroup(Dictionary<ulong, MetadataBase> points, MetadataBase signalReference)
        {
            SinglePhasorTerminal signal = new SinglePhasorTerminal();
            signal.SignalGroupName = GroupName;
            AssignIfFound(Im, ref signal.CurrentMagnitude, points);
            AssignIfFound(Ia, ref signal.CurrentAngle, points);
            AssignIfFound(Vm, ref signal.VoltageMagnitude, points);
            AssignIfFound(Va, ref signal.VoltageAngle, points);
            AssignIfFound(Dfdt, ref signal.Dfdt, points);
            AssignIfFound(Freq, ref signal.Frequency, points);
            AssignIfFound(Status, ref signal.Status, points);
            signal.ExtraData = this;

            signal.CreateCalculatedSignals(signalReference);
            return signal;
        }

        private void AssignIfFound(long id, ref MetadataBase category, Dictionary<ulong, MetadataBase> points)
        {
            if (id >= 0 && points.ContainsKey((ulong)id))
                category = points[(ulong)id];
        }

        static T IsNull<T>(DataRow row, string columnName, T defaultValue)
        {
            if (row.IsNull(columnName))
                return defaultValue;

            return (T)row[columnName];
        }
    }

    public class AllSignalGroups
    {
        public static List<SignalGroupBook> DefaultSignalGroups = new List<SignalGroupBook>();
        public List<SignalGroupBook> SignalGroups;

        public AllSignalGroups()
        {
            SignalGroups = DefaultSignalGroups.ToList();
        }

        public AllSignalGroups(string config)
        {
            SignalGroups = new List<SignalGroupBook>();

            bool firstLine = true;
            foreach (string line in File.ReadAllLines(config))
            {
                if (firstLine)
                {
                    firstLine = false;
                }
                else
                {
                    SignalGroups.Add(new SignalGroupBook(line));
                }
            }
        }
    }
}