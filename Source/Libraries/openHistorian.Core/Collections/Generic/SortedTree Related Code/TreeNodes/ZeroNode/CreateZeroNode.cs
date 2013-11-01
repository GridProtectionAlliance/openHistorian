//******************************************************************************************************
//  CreateZeroNode.cs - Gbtc
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
//  4/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.Collections.Generic;

namespace openHistorian.Collections.Generic.TreeNodes
{
    /// <summary>
    /// Used to generically create a fixed size node.
    /// </summary>
    public class CreateZeroNode
        : CreateTreeNodeBase
    {
        // {E8054C6A-DFFC-4E9B-8074-475291C32862}
        public static readonly Guid TypeGuid = new Guid(0xe8054c6a, 0xdffc, 0x4e9b, 0x80, 0x74, 0x47, 0x52, 0x91, 0xc3, 0x28, 0x62);

        /// <summary>
        /// If this tree node type has a fixed key type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field must be assigned if <see cref="CreateTreeNodeBase.ValueTypeIfFixed"/> is assigned.
        /// </summary>
        public override Type KeyTypeIfFixed
        {
            get
            {
                return null;
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
                return null;
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
        /// <returns></returns>
        public override TreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level)
 
        {
            return new ZeroNode<TKey, TValue>(level);
        }
    }
}