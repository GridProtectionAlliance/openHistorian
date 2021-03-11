////******************************************************************************************************
////  CreateHistorianCompressedStream.cs - Gbtc
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
////  08/10/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************


//using System;
//using GSF.Snap;
//using GSF.Snap.Definitions;
//using GSF.Snap.Encoding;
//using openHistorian.Snap.Stream;

//namespace openHistorian.Snap.Definitions
//{
//    public class HistorianStreamEncodingDefinition
//        : StreamEncodingBaseDefinition
//    {
//        // {0418B3A7-F631-47AF-BBFA-8B9BC0378328}
//        public static readonly EncodingDefinition TypeGuid =
//            new EncodingDefinition(new Guid(0x0418b3a7, 0xf631, 0x47af, 0xbb, 0xfa, 0x8b, 0x9b, 0xc0, 0x37, 0x83, 0x28));

//        public override Type KeyTypeIfNotGeneric
//        {
//            get
//            {
//                return typeof(HistorianKey);
//            }
//        }

//        public override Type ValueTypeIfNotGeneric
//        {
//            get
//            {
//                return typeof(HistorianValue);
//            }
//        }

//        public override EncodingDefinition Method
//        {
//            get
//            {
//                return TypeGuid;
//            }
//        }

//        public override StreamEncodingBase<TKey, TValue> Create<TKey, TValue>()
//        {
//            return (StreamEncodingBase<TKey, TValue>)(object)new HistorianStreamEncoding();
//        }
//    }
//}
