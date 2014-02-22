//******************************************************************************************************
//  EngineKeyBase'1.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine
{
    /// <summary>
    /// Base implementation of a historian key. 
    /// These are the required functions that are 
    /// necessary for the historian engine to operate
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EngineKeyBase<TKey>
        : IComparable<TKey>, IEquatable<TKey>, ISortedTreeKey<TKey>
        where TKey : class,new()
    {
        /// <summary>
        /// The timestamp stored as native ticks. 
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// The id number of the point.
        /// </summary>
        public ulong PointID;
       
        /// <summary>
        /// Compares the current instance to <see cref="other"/>.
        /// </summary>
        /// <param name="other">the key to compare to</param>
        /// <returns></returns>
        public abstract int CompareTo(TKey other);
    
        /// <summary>
        /// Is the current instance equal to <see cref="other"/>
        /// </summary>
        /// <param name="other">the key to compare to</param>
        /// <returns></returns>
        public bool IsEqualTo(TKey other)
        {
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TKey other)
        {
            return IsEqualTo(other);
        }

        public abstract SortedTreeKeyMethodsBase<TKey> CreateKeyMethods();
        public abstract void RegisterCustomKeyImplementations();
    }
}