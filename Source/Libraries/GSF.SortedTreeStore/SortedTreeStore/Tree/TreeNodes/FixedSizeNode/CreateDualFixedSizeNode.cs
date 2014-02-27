//******************************************************************************************************
//  CreateDualFixedSizeNode.cs - Gbtc
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

namespace GSF.SortedTreeStore.Tree.TreeNodes.FixedSizeNode
{
    /// <summary>
    /// Used to generically create a fixed size node.
    /// </summary>
    public class CreateDualFixedSizeNode
        : CreateTreeNodeBase
    {
        // {1DEA326D-A63A-4F73-B51C-7B3125C6DA55}
        /// <summary>
        /// The guid that represents the encoding method of this class
        /// </summary>
        public static readonly EncodingDefinition TypeGuid = new EncodingDefinition(
            new Guid(0x1dea326d, 0xa63a, 0x4f73, 0xb5, 0x1c, 0x7b, 0x31, 0x25, 0xc6, 0xda, 0x55),
            new Guid(0x1dea326d, 0xa63a, 0x4f73, 0xb5, 0x1c, 0x7b, 0x31, 0x25, 0xc6, 0xda, 0x55));

        /// <summary>
        /// The key type supported by the encoded method. Can be null if the encoding is not type specific.
        /// </summary>
        public override Type KeyTypeIfNotGeneric
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The value type supported by the encoded method. Can be null if the encoding is not type specific.
        /// </summary>
        public override Type ValueTypeIfNotGeneric
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The encoding method that defines this class.
        /// </summary>
        public override EncodingDefinition Method
        {
            get
            {
                return TypeGuid;
            }
        }

        /// <summary>
        /// Creates a <see cref="SortedTreeNodeBase{TKey,TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="level">the node level of the tree</param>
        /// <returns></returns>
        public override SortedTreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level)
        {
            return new FixedSizeNode<TKey, TValue>(level);
        }


    }
}