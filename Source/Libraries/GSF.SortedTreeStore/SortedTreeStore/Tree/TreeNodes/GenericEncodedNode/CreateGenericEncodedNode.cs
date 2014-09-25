//******************************************************************************************************
//  CreateGenericEncodedNode`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  5/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************


using System;
using GSF.SortedTreeStore.Encoding;

namespace GSF.SortedTreeStore.Tree.TreeNodes
{
    /// <summary>
    /// Used to create an initializer to return to the SortedTree.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class CreateGenericEncodedNode<TKey, TValue>
        : CreateTreeNodeBase
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        DoubleValueEncodingBase<TKey, TValue> m_encoding;

        public CreateGenericEncodedNode(DoubleValueEncodingBase<TKey, TValue> encoding)
        {
            m_encoding = encoding;
        }

        public override EncodingDefinition Method
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Type KeyTypeIfNotGeneric
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Type ValueTypeIfNotGeneric
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override SortedTreeNodeBase<TKey1, TValue1> Create<TKey1, TValue1>(byte level)
        {
            return (SortedTreeNodeBase<TKey1, TValue1>)(object)new GenericEncodedNode<TKey, TValue>(m_encoding.Clone(), level);
        }
    }
}