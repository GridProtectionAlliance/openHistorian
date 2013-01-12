////******************************************************************************************************
////  IntegerFloat.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
////  5/1/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using openHistorian.IO;

//namespace openHistorian.Collections.BPlusTreeTypes
//{
//    public struct IntegerFloat : IBPlusTreeType<IntegerFloat>
//    {
       
//        public IntegerFloat(int value1, float value2)
//        {
//            Value1=value1;
//            Value2 = value2;
//        }

//        public int Value1;
//        public float Value2;

//        public int SizeOf
//        {
//            get
//            {
//                return 8;
//            }
//        }

//        public void LoadValue(IBinaryStream stream)
//        {
//            Value1 = stream.ReadInt32();
//            Value2 = stream.ReadSingle();
//        }

//        public void SaveValue(IBinaryStream stream)
//        {
//            stream.Write(Value1);
//            stream.Write(Value2);
//        }
//        public int CompareToStream(IBinaryStream stream)
//        {
//            throw new NotImplementedException();
//        }
//        public int CompareTo(IntegerFloat key)
//        {
//            throw new NotImplementedException();
//        }

//        public override string ToString()
//        {
//            return Value1.ToString() + " " + Value2.ToString();
//        }
//    }

//}
