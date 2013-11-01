//******************************************************************************************************
//  CreateTreeNodeBase.cs - Gbtc
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
//  4/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// A base class that allows for generically constructing any number of <see cref="T:openHistorian.Collections.Generic.TreeNodeBase`2"/> implementations.
    /// </summary>
    public abstract class CreateTreeNodeBase
    {
        /// <summary>
        /// Verifies that the abstraction implementations are valid.
        /// </summary>
        protected CreateTreeNodeBase()
        {
            if (ValueTypeIfFixed == null && KeyTypeIfFixed != null)
                throw new Exception("Cannot fix the value type but not the key type.");
        }

        /// <summary>
        /// If this tree node type has a fixed key type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field must be assigned if <see cref="ValueTypeIfFixed"/> is assigned.
        /// </summary>
        public abstract Type KeyTypeIfFixed
        {
            get;
        }

        /// <summary>
        /// If this tree node type has a fixed value type, it is specified here. If this property returns null,
        /// this tree node type is not type constrained. This field cannot be assigned if <see cref="KeyTypeIfFixed"/> is null.
        /// </summary>
        public abstract Type ValueTypeIfFixed
        {
            get;
        }

        /// <summary>
        /// A guid that is specific to the underlying storage structure.
        /// </summary>
        /// <remarks>
        /// A Guid,Type,Type will uniquely define how to encode/decode a node. Therefore, mulitple types can be the same Guid.
        /// </remarks>
        public abstract Guid GetTypeGuid
        {
            get;
        }

        /// <summary>
        /// Creates a TreeNodeBase
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="level"></param>
        /// <returns></returns>
        public abstract TreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new();
    }
}