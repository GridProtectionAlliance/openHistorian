//******************************************************************************************************
//  SignalScaling.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Data.Query;

namespace openVisN.Calculations
{
    internal class SignalScaling : CalculationMethod
    {
        private readonly double m_scalingFactor;
        private readonly MetadataBase m_newSignal;

        public SignalScaling(double scalingFactor, MetadataBase signalId)
            : base(signalId)
        {
            m_scalingFactor = scalingFactor;
            m_newSignal = new MetadataDouble(Guid.NewGuid(), null, "", "", this);
        }

        public void GetPoints(out MetadataBase newSignal)
        {
            newSignal = m_newSignal;
        }

        public override void Calculate(IDictionary<Guid, SignalDataBase> signals)
        {
            Dependencies[0].Calculate(signals);

            SignalDataBase origionalSignal = signals[Dependencies[0].UniqueId];

            SignalDataBase newSignal = TryGetSignal(m_newSignal, signals);

            if (newSignal is null || newSignal.IsComplete)
                return;

            int pos = 0;

            while (pos < origionalSignal.Count)
            {
                origionalSignal.GetData(pos, out ulong time, out double vm);
                pos++;

                newSignal.AddData(time, vm * m_scalingFactor);
            }
            newSignal.Completed();
        }

        private SignalDataBase TryGetSignal(MetadataBase signal, IDictionary<Guid, SignalDataBase> results)
        {
            if (results.TryGetValue(signal.UniqueId, out SignalDataBase data))
                return data;
            return null;
        }
    }
}