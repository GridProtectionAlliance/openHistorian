////******************************************************************************************************
////  StreamWriter.cs - Gbtc
////
////  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  1/26/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////*****************************************************************************************************

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Historian.PointTypes;

//namespace Historian.PointStream
//{
//    /// <summary>
//    /// Use this class to write time series data to a stream.
//    /// </summary>
//    public class StreamWriter
//    {
//        SinglePointMessage SingleMessage;

//        PooledMemoryStream m_stream;

//        SortedList<Guid, PointWriter> m_points;
//        SortedList<uint, PointWriter> m_points2;
//        PointWriter m_activePoint;
        
//        uint PreviousPoint = 0;


//        public StreamWriter()
//        {
//            SingleMessage = new SinglePointMessage();
//        }

//        public void AddPoint(PooledMemoryStream stream)
//        {
//            if (stream.ReadByte() != 2)
//                throw new Exception("Parsing Error");
//            PointWriter point = new PointWriter(stream);
//            AddPoint(point);
//        }
//        public void AddPoint(PointWriter point)
//        {
//            if (m_points.ContainsKey(point.PointID))
//            {
//                m_points[point.PointID]= point;
//            }
//            else
//            {
//                m_points.Add(point.PointID, point);
//            }
//        }
//        public void AddPoint(Guid pointID,Definition pointTypeDefinition)
//        {
//            AddPoint(new PointWriter(pointID, pointTypeDefinition));
//        }
     

//        public void SetActivePoint(Guid pointID)
//        {
//            m_activePoint = m_points[pointID];
//        }
//        public void SetActivePoint(uint localPointID)
//        {
//            m_activePoint = m_points2[localPointID];
//        }

//        public SinglePointMessage WriteMessage()
//        {
//            if (!m_activePoint.IsRemoteAware)
//            {
//                SendPointData();
//                m_activePoint.IsRemoteAware = true;
//            }
//            SingleMessage.SetWriter(m_activePoint);
//            return SingleMessage;
//        }
//        void SendPointData()
//        {

//        }

//    }
//}
