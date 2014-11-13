//******************************************************************************************************
//  CreateTreeNodeBase.cs - Gbtc
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
//  04/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using GSF.Snap.Definitions;

namespace GSF.Snap.Tree.TreeNodes
{
    /// <summary>
    /// A base class that allows for generically constructing any number of <see cref="SortedTreeNodeBase{TKey,TValue}"/> implementations.
    /// </summary>
    public abstract class CreateTreeNodeBase 
        : CreateDoubleValueBase
    {
        /// <summary>
        /// Creates a TreeNodeBase
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="level">the level of the tree node.</param>
        /// <returns></returns>
        public abstract SortedTreeNodeBase<TKey, TValue> Create<TKey, TValue>(byte level)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new();
    }
}