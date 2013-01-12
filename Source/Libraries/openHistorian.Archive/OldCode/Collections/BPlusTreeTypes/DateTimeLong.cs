////******************************************************************************************************
////  DateTimeLong.cs - Gbtc
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
//using System.Runtime.InteropServices;
//using openHistorian.IO;

//namespace openHistorian.Collections.BPlusTreeTypes
//{
//    [StructLayout(LayoutKind.Explicit)]
//    public struct DateTimeLong : IBPlusTreeType<DateTimeLong>
//    {
//        [FieldOffset(0)]
//        public DateTime Time;
//        [FieldOffset(8)]
//        public long Key;

//        public int SizeOf
//        {
//            get
//            {
//                return 16;
//            }
//        }

//        public void LoadValue(IBinaryStream stream)
//        {
//            Time = stream.ReadDateTime();
//            Key = stream.ReadInt64();
//        }

//        public void SaveValue(IBinaryStream stream)
//        {
//            stream.Write(Time.Ticks);
//            stream.Write(Key);
//        }

//        public int CompareToStream(IBinaryStream stream)
//        {
//            DateTime time = stream.ReadDateTime();
//            long key = stream.ReadInt64();

//            if (Time.Ticks == time.Ticks && Key == key)
//                return 0;
//            if (Time.Ticks > time.Ticks)
//                return 1;
//            if (Time.Ticks < time.Ticks)
//                return -1;
//            if (Key > key)
//                return 1;
//            return -1;

//        }
//        public int CompareTo(DateTimeLong key)
//        {
//            if (Time.Ticks == key.Time.Ticks && Key == key.Key)
//                return 0;
//            if (Time.Ticks > key.Time.Ticks)
//                return 1;
//            if (Time.Ticks < key.Time.Ticks)
//                return -1;
//            if (Key > key.Key)
//                return 1;
//            return -1;
//        }
//    }
//}
