//******************************************************************************************************
//  SinglePhasorPowerSignals.cs - Gbtc
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openVisN.Query;

namespace openVisN.Calculations
{
    public class SinglePhasorPowerSignals : CalculationMethod
    {
        const double TwoPieOver360 = 2.0 * Math.PI / 360.0;
        MetadataBase Watt;
        MetadataBase PowerFactor;
        MetadataBase VoltAmpre;
        MetadataBase VoltAmpreReactive;

        public SinglePhasorPowerSignals(MetadataBase voltageMagnitudePointId, MetadataBase voltageAnglePointId,
                                       MetadataBase currentMagnitudePointId, MetadataBase currentAnglePointId)
            : base(voltageMagnitudePointId, voltageAnglePointId, currentMagnitudePointId, currentAnglePointId)
        {
            Watt = new MetadataDouble(Guid.NewGuid(), 0, "", "", this);
            PowerFactor = new MetadataDouble(Guid.NewGuid(), 0, "", "", this);
            VoltAmpre = new MetadataDouble(Guid.NewGuid(), 0, "", "", this);
            VoltAmpreReactive = new MetadataDouble(Guid.NewGuid(), 0, "", "", this);
        }

        public void GetPoints(out MetadataBase watt, out MetadataBase powerFactor, out MetadataBase voltAmpre, out MetadataBase voltAmpreReactive)
        {
            watt = Watt;
            powerFactor = PowerFactor;
            voltAmpre = VoltAmpre;
            voltAmpreReactive = VoltAmpreReactive;
        }

        public override void Calculate(QueryResultsCalculation resuls)
        {
            var pointListVM = resuls.GetSignal(Dependencies[0].UniqueId);
            var pointListVA = resuls.GetSignal(Dependencies[1].UniqueId);
            var pointListIM = resuls.GetSignal(Dependencies[2].UniqueId);
            var pointListIA = resuls.GetSignal(Dependencies[3].UniqueId);

            pointListVA.Calculate(resuls);
            pointListVM.Calculate(resuls);
            pointListIA.Calculate(resuls);
            pointListIM.Calculate(resuls);

            var pointListW = resuls.TryGetSignal(Watt.UniqueId);
            var pointListPF = resuls.TryGetSignal(PowerFactor.UniqueId);
            var pointListVAmp = resuls.TryGetSignal(VoltAmpre.UniqueId);
            var pointListVAR = resuls.TryGetSignal(VoltAmpreReactive.UniqueId);

            if (pointListW != null)
                pointListW.HasBeenCalculated();
            if (pointListPF != null)
                pointListPF.HasBeenCalculated();
            if (pointListVAmp != null)
                pointListVAmp.HasBeenCalculated();
            if (pointListVAR != null)
                pointListVAR.HasBeenCalculated();

            int posVM = 0;
            int posVA = 0;
            int posIM = 0;
            int posIA = 0;

            while (posVM < pointListVM.Data.Count && posVA < pointListVA.Data.Count && posIM < pointListIM.Data.Count &&
                   posIA < pointListIA.Data.Count)
            {
                ulong timeVM, timeVA, timeIM, timeIA;
                double vm, va, im, ia;

                pointListVM.Data.GetData(posVM, out timeVM, out vm);
                pointListVA.Data.GetData(posVA, out timeVA, out va);
                pointListIM.Data.GetData(posIM, out timeIM, out im);
                pointListIA.Data.GetData(posIA, out timeIA, out ia);

                var time = timeVM;

                if (timeVM == timeVA && timeVM == timeIM && timeVM == timeIA)
                {
                    posVM++;
                    posVA++;
                    posIM++;
                    posIA++;

                    double angleDiffRadians = (va - ia) * TwoPieOver360;
                    double mva = vm * im;
                    double pf = Math.Cos(angleDiffRadians);
                    double mw = (mva * pf);
                    double mvar = mva * Math.Sin(angleDiffRadians);

                    if (pointListW != null)
                        pointListW.Data.AddData(time, mw);
                    if (pointListVAR != null)
                        pointListVAR.Data.AddData(time, mvar);
                    if (pointListVAmp != null)
                        pointListVAmp.Data.AddData(time, mva);
                    if (pointListPF != null)
                        pointListPF.Data.AddData(time, pf);
                }
                else
                {
                    ulong maxTime = Math.Max(timeVM, timeVA);
                    maxTime = Math.Max(maxTime, timeIM);
                    maxTime = Math.Max(maxTime, timeIA);

                    if (timeVM < maxTime)
                        posVM++;
                    if (timeVA < maxTime)
                        posVA++;
                    if (timeIM < maxTime)
                        posIM++;
                    if (timeIA < maxTime)
                        posIA++;
                }
            }
        }
    }
}
