////******************************************************************************************************
////  SignalDefinition.cs - Gbtc
////
////  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://opensource.org/licenses/MIT
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  12/14/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////
////******************************************************************************************************

//using System;
//using System.Collections.Generic;

//namespace openVisN
//{


//    public enum SignalType : int
//    {
//        VoltageMagnitude,
//        VoltageMagnitudePerUnit,
//        VoltageAngle,
//        VoltageAngleReference,
//        CurrentMagnitude,
//        CurrentAngle,
//        Dfdt,
//        Frequency,
//        Status,
//        DigitalWords,
//        Watt,
//        VarAmpre
//    }

//    public class SignalDefinition
//    {
//        SignalType m_signalType;
//        protected ulong[] PointIds;
//        public SignalDefinition(SignalType signalType, MetadataBase pointIds)
//            : this(signalType, new ulong[] { pointIds.HistorianId })
//        {
//        }

//        protected SignalDefinition(SignalType signalType, ulong[] pointIds)
//        {
//            m_signalType = signalType;
//            PointIds = pointIds;
//        }

//        public SignalType SignaType
//        {
//            get
//            {
//                return m_signalType;

//            }
//        }
//        public IEnumerable<ulong> GetRequiredPoints()
//        {
//            return PointIds;
//        }

//        public virtual List<KeyValuePair<ulong, ulong>> CalculatePoint(QueryResults resuls)
//        {
//            return resuls.GetPointList(PointIds[0]);
//        }

//    }

//    public class SignalProduct : SignalDefinition
//    {
//        public SignalProduct(SignalType signalType, MetadataBase pointId1, MetadataBase pointId2)
//            : base(signalType, new ulong[] { pointId1.HistorianId, pointId2.HistorianId })
//        {
//        }

//        public unsafe override List<KeyValuePair<ulong, ulong>> CalculatePoint(QueryResults resuls)
//        {
//            var pointList1 = resuls.GetPointList(PointIds[0]);
//            var pointList2 = resuls.GetPointList(PointIds[1]);

//            var result = new List<KeyValuePair<ulong, ulong>>(pointList1.Count);

//            int pos1 = 0;
//            int pos2 = 0;

//            while (pos1 < pointList1.Count && pos2 < pointList2.Count)
//            {
//                var pt1 = pointList1[pos1];
//                var pt2 = pointList2[pos2];
//                if (pt1.Key == pt2.Key)
//                {
//                    pos1++;
//                    pos2++;
//                    uint value1 = (uint)pt1.Value;
//                    uint value2 = (uint)pt2.Value;

//                    float value = *(float*)&value1 * *(float*)&value2;
//                    result.Add(new KeyValuePair<ulong, ulong>(pt1.Key, *(uint*)&value));
//                }
//                else if (pt1.Key < pt2.Key)
//                {
//                    pos1++;
//                }
//                else
//                {
//                    pos2++;
//                }
//            }
//            return result;
//        }
//    }

//    public class SignalWatt : SignalDefinition
//    {
//        const double TwoPieOver360 = 2.0 * Math.PI / 360.0;

//        public SignalWatt(SignalType signalType, MetadataBase voltageMagnitudePointId, MetadataBase voltageAnglePointId, MetadataBase currentMagnitudePointId, MetadataBase currentAnglePointId)
//            : base(signalType, new ulong[] { voltageMagnitudePointId.HistorianId, voltageAnglePointId.HistorianId, currentMagnitudePointId.HistorianId, currentAnglePointId.HistorianId })
//        {
//        }

//        public unsafe override List<KeyValuePair<ulong, ulong>> CalculatePoint(QueryResults resuls)
//        {
//            var pointListVM = resuls.GetPointList(PointIds[0]);
//            var pointListVA = resuls.GetPointList(PointIds[1]);
//            var pointListIM = resuls.GetPointList(PointIds[2]);
//            var pointListIA = resuls.GetPointList(PointIds[3]);

//            var result = new List<KeyValuePair<ulong, ulong>>(pointListVM.Count);

//            int posVM = 0;
//            int posVA = 0;
//            int posIM = 0;
//            int posIA = 0;

//            while (posVM < pointListVM.Count && posVA < pointListVA.Count && posIM < pointListIM.Count && posIA < pointListIA.Count)
//            {
//                var ptVM = pointListVM[posVM];
//                var ptVA = pointListVA[posVA];
//                var ptIM = pointListIM[posIM];
//                var ptIA = pointListIA[posIA];
//                if (ptVM.Key == ptVA.Key && ptVM.Key == ptIM.Key && ptVM.Key == ptIA.Key)
//                {
//                    posVM++;
//                    posVA++;
//                    posIM++;
//                    posIA++;
//                    uint vm = (uint)ptVM.Value;
//                    uint va = (uint)ptVA.Value;
//                    uint im = (uint)ptIM.Value;
//                    uint ia = (uint)ptIA.Value;

//                    double angleDiff = (*(float*)&va - *(float*)&ia) * TwoPieOver360;
//                    double mva = *(float*)&vm * *(float*)&im;
//                    double pf = Math.Cos(angleDiff);
//                    float mw = (float)(mva * pf);

//                    result.Add(new KeyValuePair<ulong, ulong>(ptVM.Key, *(uint*)&mw));
//                }
//                else
//                {
//                    ulong maxTime = Math.Max(ptVM.Key, ptVA.Key);
//                    maxTime = Math.Max(maxTime, ptIM.Key);
//                    maxTime = Math.Max(maxTime, ptIA.Key);

//                    if (ptVM.Key < maxTime)
//                        posVM++;
//                    if (ptVA.Key < maxTime)
//                        posVA++;
//                    if (ptIM.Key < maxTime)
//                        posIM++;
//                    if (ptIA.Key < maxTime)
//                        posIA++;
//                }
//            }
//            return result;
//        }
//    }

//}

