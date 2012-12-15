//******************************************************************************************************
//  CalculationMethod.cs - Gbtc
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

namespace openVisN
{
    public class CalculationMethod
    {
        public static CalculationMethod Empty { get; private set; }
        static CalculationMethod()
        {
            Empty = new CalculationMethod();
        }

        protected MetadataBase[] Dependencies;

        protected CalculationMethod(params MetadataBase[] dependencies)
        {
            Dependencies = dependencies;
        }
        public virtual void Calculate(QueryResults query)
        {

        }
        public void AddDependentPoints(HashSet<Guid> dependencies)
        {
            foreach (var point in Dependencies)
            {
                dependencies.Add(point.UniqueId);
                point.Calculations.AddDependentPoints(dependencies);
            }
        }
    }

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

        public override void Calculate(QueryResults resuls)
        {
            var pointListVM = resuls.GetPointList(Dependencies[0].UniqueId);
            var pointListVA = resuls.GetPointList(Dependencies[1].UniqueId);
            var pointListIM = resuls.GetPointList(Dependencies[2].UniqueId);
            var pointListIA = resuls.GetPointList(Dependencies[3].UniqueId);

            pointListVA.Calculate();
            pointListVM.Calculate();
            pointListIA.Calculate();
            pointListIM.Calculate();

            var pointListW = resuls.GetPointList(Watt.UniqueId);
            var pointListPF = resuls.GetPointList(PowerFactor.UniqueId);
            var pointListVAmp = resuls.GetPointList(VoltAmpre.UniqueId);
            var pointListVAR = resuls.GetPointList(VoltAmpreReactive.UniqueId);

            int posVM = 0;
            int posVA = 0;
            int posIM = 0;
            int posIA = 0;

            while (posVM < pointListVM.Count && posVA < pointListVA.Count && posIM < pointListIM.Count &&
                   posIA < pointListIA.Count)
            {
                var ptVM = pointListVM.GetDouble(posVM);
                var ptVA = pointListVA.GetDouble(posVA);
                var ptIM = pointListIM.GetDouble(posIM);
                var ptIA = pointListIA.GetDouble(posIA);

                var time = ptVM.Key;

                if (ptVM.Key == ptVA.Key && ptVM.Key == ptIM.Key && ptVM.Key == ptIA.Key)
                {
                    posVM++;
                    posVA++;
                    posIM++;
                    posIA++;

                    double angleDiffRadians = (ptVA.Value - ptIA.Value) * TwoPieOver360;
                    double mva = ptVM.Value * ptIM.Value;
                    double pf = Math.Cos(angleDiffRadians);
                    double mw = (mva * pf);
                    double mvar = mva * Math.Sin(angleDiffRadians);

                    pointListW.AddPoint(time, Watt.ToNative(mw));
                    pointListVAR.AddPoint(time, VoltAmpreReactive.ToNative(mvar));
                    pointListVAmp.AddPoint(time, VoltAmpre.ToNative(mva));
                    pointListPF.AddPoint(time, PowerFactor.ToNative(pf));
                }
                else
                {
                    ulong maxTime = Math.Max(ptVM.Key, ptVA.Key);
                    maxTime = Math.Max(maxTime, ptIM.Key);
                    maxTime = Math.Max(maxTime, ptIA.Key);

                    if (ptVM.Key < maxTime)
                        posVM++;
                    if (ptVA.Key < maxTime)
                        posVA++;
                    if (ptIM.Key < maxTime)
                        posIM++;
                    if (ptIA.Key < maxTime)
                        posIA++;
                }
            }
        }
    }
}
