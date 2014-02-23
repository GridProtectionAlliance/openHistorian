//******************************************************************************************************
//  CreatePointCollectionBase.cs - Gbtc
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
//  2/7/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Collection
{
    /// <summary>
    /// A base class that allows for generically constructing any number of <see cref="PointCollectionBase{TKey,TValue}"/> implementations.
    /// </summary>
    public abstract class CreatePointCollectionBase
    {
        /// <summary>
        /// Verifies that the abstraction implementations are valid.
        /// </summary>
        protected CreatePointCollectionBase(Type keyTypeIfFixed, Type valueTypeIfFixed)
        {
            KeyTypeIfFixed = keyTypeIfFixed;
            ValueTypeIfFixed = valueTypeIfFixed;
            if (ValueTypeIfFixed == null && KeyTypeIfFixed != null)
                throw new Exception("Cannot fix the value type but not the key type.");
        }

        /// <summary>
        /// If this tree node type has a fixed key type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field must be assigned if <see cref="ValueTypeIfFixed"/> is assigned.
        /// </summary>
        public Type KeyTypeIfFixed { get; private set; }

        /// <summary>
        /// If this tree node type has a fixed value type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field cannot be assigned if <see cref="KeyTypeIfFixed"/> is null.
        /// </summary>
        public Type ValueTypeIfFixed { get; private set; }

        /// <summary>
        /// Creates a TreeNodeBase
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public abstract PointCollectionBase<TKey, TValue> Create<TKey, TValue>(int capacity)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new();
    }
}