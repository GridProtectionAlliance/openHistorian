////******************************************************************************************************
////  SortedTree256CompressionDeltaEncoded.cs - Gbtc
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
////  3/22/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF;
//using GSF.IO;
//using openHistorian.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Collections
//{
//    public struct SortedTree256CompressionDeltaEncoded
//        //: ISortedTreeCompressionMethods<KeyValue256>
//    {
//        // {0EF85145-F110-4F6F-937A-F90801CE5F2D}
//        static Guid s_fileType = new Guid(0x0ef85145, 0xf110, 0x4f6f, 0x93, 0x7a, 0xf9, 0x08, 0x01, 0xce, 0x5f, 0x2d);

//        public Guid FileType
//        {
//            get
//            {
//                return s_fileType;
//            }
//        }

//        public bool IsFixedSize
//        {
//            get
//            {
//                return false;

//            }
//        }
//        public unsafe int EncodeRecord(byte* buffer, KeyValue256 currentKey, KeyValue256 previousKey)
//        {
//            int size = 0;
//            Compression.Write7Bit(buffer, ref size, currentKey.Key1 ^ previousKey.Key1);
//            Compression.Write7Bit(buffer, ref size, currentKey.Key2 ^ previousKey.Key2);
//            Compression.Write7Bit(buffer, ref size, currentKey.Value1 ^ previousKey.Value1);
//            Compression.Write7Bit(buffer, ref size, currentKey.Value2 ^ previousKey.Value2);
//            return size;
//        }

//        public void DecodeNextRecord(BinaryStreamBase stream, KeyValue256 currentKey)
//        {
//            currentKey.Key1 ^= stream.Read7BitUInt64();
//            currentKey.Key2 ^= stream.Read7BitUInt64();
//            currentKey.Value1 ^= stream.Read7BitUInt64();
//            currentKey.Value2 ^= stream.Read7BitUInt64();
//        }
//    }
//}

