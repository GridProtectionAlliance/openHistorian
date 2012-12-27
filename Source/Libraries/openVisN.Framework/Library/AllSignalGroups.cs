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

using System.Collections.Generic;
using System.IO;

namespace openVisN.Library
{
    public class SignalGroupBook
    {
        public long GroupId;
        public string GroupName;
        public string Type;
        public double NominalVoltage;
        public double Priority;
        public double Longitude;
        public double Latitude;
        public long SectionId;
        public string Manufacture;
        public long Im;
        public long Ia;
        public long Vm;
        public long Va;
        public long Dfdt;
        public long Freq;
        public long Status;
        public long D1;
        public long D2;
        public long D3;
        public long D4;
        public SignalGroupBook(string line)
        {
            var parts = line.Split('\t');
            GroupId = long.Parse(parts[0]);
            GroupName = parts[1].Trim();
            Type = parts[2].Trim();
            NominalVoltage = double.Parse(parts[3]);
            Priority = double.Parse(parts[4]);
            Longitude = double.Parse(parts[5]);
            Latitude = double.Parse(parts[6]);
            SectionId = long.Parse(parts[7]);
            Manufacture = parts[8].Trim();
            Im = long.Parse(parts[9]);
            Ia = long.Parse(parts[10]);
            Vm = long.Parse(parts[11]);
            Va = long.Parse(parts[12]);
            Dfdt = long.Parse(parts[13]);
            Freq = long.Parse(parts[14]);
            Status = long.Parse(parts[15]);
            D1 = long.Parse(parts[16]);
            D2 = long.Parse(parts[17]);
            D3 = long.Parse(parts[18]);
            D4 = long.Parse(parts[19]);
        }

        public SignalGroup CreateGroup(Dictionary<ulong, MetadataBase> points, MetadataBase signalReference)
        {
            var signal = new SinglePhasorTerminal();
            signal.SignalGroupName = GroupName;
            AssignIfFound(Im, ref signal.CurrentMagnitude, points);
            AssignIfFound(Ia, ref signal.CurrentAngle, points);
            AssignIfFound(Vm, ref signal.VoltageMagnitude, points);
            AssignIfFound(Va, ref signal.VoltageAngle, points);
            AssignIfFound(Dfdt, ref signal.Dfdt, points);
            AssignIfFound(Freq, ref signal.Frequency, points);
            AssignIfFound(Status, ref signal.Status, points);
            //AssignIfFound(D1, ref signal.Digital1, points);
            //AssignIfFound(D2, ref signal.CurrentMagnitude, points);
            //AssignIfFound(D3, ref signal.CurrentMagnitude, points);
            //AssignIfFound(D4, ref signal.CurrentMagnitude, points);
            signal.ExtraData = this;

            signal.CreateCalculatedSignals(signalReference);
            return signal;
        }

        void AssignIfFound(long id, ref MetadataBase category, Dictionary<ulong, MetadataBase> points)
        {
            if (id >= 0 && points.ContainsKey((ulong)id))
                category = points[(ulong)id];
        }
    }

    public class AllSignalGroups
    {
        public List<SignalGroupBook> SignalGroups;

        public AllSignalGroups()
            : this(@"C:\Unison\GPA\ArchiveFiles\SignalGroups.txt")
        {
        }
        public AllSignalGroups(string config)
        {
            SignalGroups = new List<SignalGroupBook>();

            bool firstLine = true;
            foreach (var line in File.ReadAllLines(config))
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
