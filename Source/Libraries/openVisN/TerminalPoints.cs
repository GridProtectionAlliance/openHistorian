//******************************************************************************************************
//  TerminalPoints.cs - Gbtc
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN
{
    public class TerminalInfo
    {
        public int TerminalId;
        public string TerminalName;
        public ulong VoltageMagnitude;
        public ulong VoltageAngle;
        public ulong CurrentMagnitude;
        public ulong CurrentAngle;
        public ulong Dfdt;
        public ulong Frequency;
        public ulong Status;

        public TerminalSignals GetAllSignals()
        {
            return new TerminalSignals(this);
        }
    }

    public class TerminalSignals
    {
        public string TerminalName;

        public int TerminalId;

        public SortedList<SignalType, SignalDefinition> Signals;

        public TerminalSignals(TerminalInfo terminalInfo)
        {
            TerminalName = terminalInfo.TerminalName;
            TerminalId = terminalInfo.TerminalId;
            Signals = new SortedList<SignalType, SignalDefinition>();
            Signals.Add(SignalType.VoltageMagnitude, new SignalDefinition(SignalType.VoltageMagnitude, terminalInfo.VoltageMagnitude));
            Signals.Add(SignalType.VoltageAngle, new SignalDefinition(SignalType.VoltageAngle, terminalInfo.VoltageAngle));
            Signals.Add(SignalType.CurrentMagnitude, new SignalDefinition(SignalType.CurrentMagnitude, terminalInfo.CurrentMagnitude));
            Signals.Add(SignalType.CurrentAngle, new SignalDefinition(SignalType.CurrentAngle, terminalInfo.CurrentAngle));
            Signals.Add(SignalType.Dfdt, new SignalDefinition(SignalType.Dfdt, terminalInfo.Dfdt));
            Signals.Add(SignalType.Frequency, new SignalDefinition(SignalType.Frequency, terminalInfo.Frequency));
            Signals.Add(SignalType.Status, new SignalDefinition(SignalType.Status, terminalInfo.Status));
            Signals.Add(SignalType.VarAmpre, new SignalProduct(SignalType.VarAmpre, terminalInfo.VoltageMagnitude, terminalInfo.CurrentMagnitude));
            Signals.Add(SignalType.Watt, new SignalWatt(SignalType.Watt, terminalInfo.VoltageMagnitude, terminalInfo.VoltageAngle, terminalInfo.CurrentMagnitude, terminalInfo.CurrentAngle));
        }

    }
}
