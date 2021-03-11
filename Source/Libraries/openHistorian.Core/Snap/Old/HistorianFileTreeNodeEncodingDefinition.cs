////******************************************************************************************************
////  HistorianFileTreeNodeEncodingDefinition.cs - Gbtc
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
////  07/26/2013 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF.Snap;
//using GSF.Snap.Definitions;
//using GSF.Snap.Tree;
//using openHistorian.Snap.Tree;

//namespace openHistorian.Snap.Definitions
//{
//    /// <summary>
//    /// Used to generically create a fixed size node.
//    /// </summary>
//    public class HistorianFileTreeNodeEncodingDefinition
//        : SortedTreeNodeBaseDefinition
//    {

//        // {AACA05B5-6B72-4512-859A-F4B2DF394BF7}
//        /// <summary>
//        /// A unique identifier for this compression method.
//        /// </summary>
//        public readonly static EncodingDefinition TypeGuid = new EncodingDefinition(new Guid(
//            0xaaca05b5, 0x6b72, 0x4512, 0x85, 0x9a, 0xf4, 0xb2, 0xdf, 0x39, 0x4b, 0xf7));

//        /// <summary>
//        /// Creates a TreeNodeBase
//        /// </summary>
//        /// <typeparam name="TKey"></typeparam>
//        /// <typeparam name="TValue"></typeparam>
//        /// <param name="level"></param>
//        /// <returns></returns>
//        public override SortedTreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level)
//        {
//            return (SortedTreeNodeBase<TKey, TValue>)(object)new HistorianFileTreeNodeEncoding(level);
//        }

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
//    }
//}