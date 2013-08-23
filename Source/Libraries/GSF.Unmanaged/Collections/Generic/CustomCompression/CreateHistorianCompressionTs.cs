//******************************************************************************************************
//  CreateHistorianCompressionTs.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  7/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.Collections.Generic.CustomCompression;

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// Used to generically create a fixed size node.
    /// </summary>
    public class CreateHistorianCompressionTs
        : CreateTreeNodeBase
    {

        // {AACA05B5-6B72-4512-859A-F4B2DF394BF7}
        public readonly static Guid TypeGuid = new Guid(0xaaca05b5, 0x6b72, 0x4512, 0x85, 0x9a, 0xf4, 0xb2, 0xdf, 0x39, 0x4b, 0xf7);

        /// <summary>
        /// If this tree node type has a fixed key type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field must be assigned if <see cref="CreateTreeNodeBase.ValueTypeIfFixed"/> is assigned.
        /// </summary>
        public override Type KeyTypeIfFixed
        {
            get
            {
                return typeof(HistorianKey);
            }
        }

        /// <summary>
        /// If this tree node type has a fixed value type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field cannot be assigned if <see cref="CreateTreeNodeBase.KeyTypeIfFixed"/> is null.
        /// </summary>
        public override Type ValueTypeIfFixed
        {
            get
            {
                return typeof(HistorianValue);
            }
        }

        /// <summary>
        /// A guid that is specific to the underlying storage structure.
        /// </summary>
        /// <remarks>
        /// A Guid,Type,Type will uniquely define how to encode/decode a node. Therefore, mulitple types can be the same Guid.
        /// </remarks>
        public override Guid GetTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        /// <summary>
        /// Creates a TreeNodeBase
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="level"></param>
        /// <param name="keyMethod"></param>
        /// <param name="valueMethod"></param>
        /// <returns></returns>
        public override TreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level, CreateKeyMethodBase<TKey> keyMethod, CreateValueMethodBase<TValue> valueMethod)
        {
            return (TreeNodeBase<TKey, TValue>)(object)new HistorianCompressionTs(level, keyMethod.As<HistorianKey>().Create(), valueMethod.As<HistorianValue>().Create());
        }
    }
}