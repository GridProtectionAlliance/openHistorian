//******************************************************************************************************
//  SignalGroup.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.Collections.Generic;
using openVisN.Calculations;
using openVisN.Library;

namespace openVisN
{

    public abstract class SignalGroup
    {
        public string SignalGroupName { get; set; }
        public abstract List<MetadataBase> GetAllSignals();

        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }
        public override int GetHashCode()
        {
            return SignalGroupName.GetHashCode();
        }

    }

    public class SinglePhasorTerminal : SignalGroup
    {
        public SignalGroupBook ExtraData;
        public MetadataBase VoltageMagnitude;
        public MetadataBase VoltageAngle;
        public MetadataBase CurrentMagnitude;
        public MetadataBase CurrentAngle;
        public MetadataBase Dfdt;
        public MetadataBase Frequency;
        public MetadataBase Status;

        public MetadataBase Watt;
        public MetadataBase PowerFactor;
        public MetadataBase VoltAmpre;
        public MetadataBase VoltAmpreHour;

        public void CreateCalculatedSignals()
        {
            var calc = new SinglePhasorPowerSignals(VoltageMagnitude, VoltageAngle, CurrentMagnitude, CurrentAngle);
            calc.GetPoints(out Watt, out PowerFactor, out VoltAmpre, out VoltAmpreHour);
        }


        public override List<MetadataBase> GetAllSignals()
        {
            var lst = new List<MetadataBase>()
                {
                    VoltageMagnitude,
                    VoltageAngle,
                    CurrentMagnitude,
                    CurrentAngle,
                    Dfdt,
                    Frequency,
                    Status,

                    Watt,
                    PowerFactor,
                    VoltAmpre,
                    VoltAmpreHour
                };
            return lst;

        }
    }
}
