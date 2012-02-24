////******************************************************************************************************
////  PointWriter.cs - Gbtc
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
//using openHistorian.Core.PointTypes;

//namespace openHistorian.Core.PointStream
//{
//    public class PointWriter
//    {
//        public bool IsRemoteAware;
//        public Guid PointID;
//        public uint LocalPointID;
//        public TypeQueueBase Time;
//        public TypeQueueBase MetaData;
//        public TypeQueueBase Data;
//        public int Position;
//        public int PositionEndOfTime;
//        public int PositionEndOfMetaData;
//        public int PositionEndOfData;
        
//        public PointWriter(Guid PointID, Definition pointDefinition)
//        {
//            this.PointID = PointID;
//            IsRemoteAware = false;

//            Time = TypeQueueBase.CreatePointType(pointDefinition.Time, pointDefinition.TimeNestedTypes);
//            MetaData = TypeQueueBase.CreatePointType(pointDefinition.MetaData, pointDefinition.MetaDataNestedTypes);
//            Time = TypeQueueBase.CreatePointType(pointDefinition.Data, pointDefinition.DataNestedTypes);
         
//            Position = 0;
//            PositionEndOfTime = Time.ItemCount;
//            PositionEndOfMetaData = PositionEndOfTime + MetaData.ItemCount;
//            PositionEndOfData = PositionEndOfMetaData + Data.ItemCount;
//        }
//        public PointWriter(PooledMemoryStream pointDefinition)
//        {
//            IsRemoteAware = false;
//            PointID = pointDefinition.ReadGuid();

//            Time=TypeQueueBase.CreatePointType(pointDefinition);
//            MetaData = TypeQueueBase.CreatePointType(pointDefinition);
//            Data = TypeQueueBase.CreatePointType(pointDefinition);

//            Position = 0;
//            PositionEndOfTime = Time.ItemCount;
//            PositionEndOfMetaData = PositionEndOfTime + MetaData.ItemCount;
//            PositionEndOfData = PositionEndOfMetaData + Data.ItemCount;
//        }

//        TypeQueueBase GetNextPosition()
//        {
//            if (Position >= PositionEndOfData)
//                Position = 0;
//            if (Position < PositionEndOfTime)
//            {
//                Position++;
//                return Time;
//            }
//            if (Position < PositionEndOfMetaData)
//            {
//                Position++;
//                return MetaData;
//            }
//            if (Position < PositionEndOfData)
//            {
//                Position++;
//                return Data;
//            }
//            throw new Exception("This point contains no data.");
//        }


//        public void Write(byte value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(short value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(int value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(long value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(ushort value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(uint value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(ulong value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(decimal value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(Guid value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(DateTime value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(float value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(double value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(bool value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(string value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(byte[] value, int offset, int count)
//        {
//            GetNextPosition().Write(value,offset,count);
//        }
//        public void Write(byte[] value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(sbyte value)
//        {
//            GetNextPosition().Write(value);
//        }
//    }
//}
