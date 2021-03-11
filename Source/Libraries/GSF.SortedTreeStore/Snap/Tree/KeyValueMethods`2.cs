//******************************************************************************************************
//  KeyValueMethods`2.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/09/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// Allows combined operations on key/value methods. If a substantial amount of copying is occuring,
    /// overriding this method to support the specific copy can make things faster.
    /// </summary>
    public abstract class KeyValueMethods
    {
        internal KeyValueMethods()
        {
            
        }

        /// <summary>
        /// The type of the key
        /// </summary>
        public abstract Type KeyType { get; }

        /// <summary>
        /// The type of the value
        /// </summary>
        public abstract Type ValueType { get; }
    }

    /// <summary>
    /// Allows combined operations on key/value methods. If a substantial amount of copying is occuring,
    /// overriding this method to support the specific copy can make things faster.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyValueMethods<TKey, TValue>
        : KeyValueMethods
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {
        /// <summary>
        /// Copies the source values to the destination.
        /// </summary>
        /// <param name="srcKey"></param>
        /// <param name="srcValue"></param>
        /// <param name="destKey"></param>
        /// <param name="dstValue"></param>
        public virtual void Copy(TKey srcKey, TValue srcValue, TKey destKey, TValue dstValue)
        {
            srcKey.CopyTo(destKey);
            srcValue.CopyTo(dstValue);
        }

        /// <summary>
        /// The type of the key
        /// </summary>
        public override Type KeyType => typeof(TKey);

        /// <summary>
        /// The type of the value
        /// </summary>
        public override Type ValueType => typeof(TValue);
    }
}
