////******************************************************************************************************
////  HistorianFixedSizeCombinedEncodingDefinition.cs - Gbtc
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
////  02/21/2014 - Steven E. Chisholm
////       Generated original version of source code. 
////     
////******************************************************************************************************

//using System;
//using GSF.Snap;
//using GSF.Snap.Definitions;
//using GSF.Snap.Encoding;
//using openHistorian.Snap.Encoding;

//namespace openHistorian.Snap.Definitions
//{
//    /// <summary>
//    /// A constructor class for this specific type of encoding.
//    /// </summary>
//    public class HistorianFixedSizeCombinedEncodingDefinition
//        : CombinedEncodingBaseDefinition
//    {
//        // {1DEA326D-A63A-4F73-B51C-7B3125C6DA55}
//        /// <summary>
//        /// The guid that represents the encoding method of this class
//        /// </summary>
//        public static readonly EncodingDefinition TypeGuid = new EncodingDefinition(
//            new Guid(0x1dea326d, 0xa63a, 0x4f73, 0xb5, 0x1c, 0x7b, 0x31, 0x25, 0xc6, 0xda, 0x55));

//        /// <summary>
//        /// The key type supported by the encoded method. Can be null if the encoding is not type specific.
//        /// </summary>
//        public override Type KeyTypeIfNotGeneric
//        {
//            get
//            {
//                return typeof(HistorianKey);
//            }
//        }

//        /// <summary>
//        /// The value type supported by the encoded method. Can be null if the encoding is not type specific.
//        /// </summary>
//        public override Type ValueTypeIfNotGeneric
//        {
//            get
//            {
//                return typeof(HistorianValue);
//            }
//        }

//        /// <summary>
//        /// The encoding method that defines this class.
//        /// </summary>
//        public override EncodingDefinition Method
//        {
//            get
//            {
//                return TypeGuid;
//            }
//        }

//        /// <summary>
//        /// Constructs a new class based on this encoding method. 
//        /// </summary>
//        /// <typeparam name="TKey">The key for this encoding method</typeparam>
//        /// <typeparam name="TValue">The value for this encoding method</typeparam>
//        /// <returns>The encoding method</returns>
//        public override CombinedEncodingBase<TKey, TValue> Create<TKey, TValue>()
//        {
//            return (CombinedEncodingBase<TKey, TValue>)(object)new HistorianFixedSizeCombinedEncoding();
//        }
//    }
//}