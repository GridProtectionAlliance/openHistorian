////******************************************************************************************************
////  SortedTree256CompressionNone.cs - Gbtc
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
//using GSF.IO;
//using openHistorian.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Collections
//{
//    public struct SortedTree256CompressionNone
//        //: ISortedTreeCompressionMethods<KeyValue256>
//    {
//        // {C4BB5945-A6DA-4634-A8A9-F74C5FB4A052}
//        static Guid s_fileType = new Guid(0xc4bb5945, 0xa6da, 0x4634, 0xa8, 0xa9, 0xf7, 0x4c, 0x5f, 0xb4, 0xa0, 0x52);

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
//                return true;

//            }
//        }
//        public unsafe int EncodeRecord(byte* buffer, KeyValue256 currentKey, KeyValue256 previousKey)
//        {
//            throw new NotImplementedException();
//        }

//        public void DecodeNextRecord(BinaryStreamBase stream, KeyValue256 currentKey)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

