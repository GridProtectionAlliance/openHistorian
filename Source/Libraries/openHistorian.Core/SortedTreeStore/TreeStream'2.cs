//******************************************************************************************************
//  TreeStream'2.cs - Gbtc
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
//  9/15/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.SortedTreeStore
{
    /// <summary>
    /// Represents a stream of KeyValues
    /// </summary>
    /// <typeparam name="TKey">The key associated with the point</typeparam>
    /// <typeparam name="TValue">The value associated with the point</typeparam>
    public abstract class TreeStream<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        /// <summary>
        /// Constructs the TreeStream using new instances of CurrentKey and CurrentValue
        /// </summary>
        protected TreeStream()
            : this(new TKey(), new TValue())
        {
        }

        /// <summary>
        /// Constructs the TreeStream using the provided instances of <see cref="key"/> and <see cref="value"/> 
        /// for <see cref="CurrentKey"/> and <see cref="CurrentValue"/>.
        /// </summary>
        /// <param name="key">the instance to use for <see cref="CurrentKey"/></param>
        /// <param name="value">the instance to use for <see cref="CurrentValue"/></param>
        protected TreeStream(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");
            CurrentKey = key;
            CurrentValue = value;
        }

        /// <summary>
        /// Gets if the <see cref="CurrentKey"/> and <see cref="CurrentValue"/> fields are valid.
        /// </summary>
        public bool IsValid
        {
            get;
            protected set;
        }

        /// <summary>
        /// The instance that will contain the current key if <see cref="IsValid"/> is true.
        /// </summary>
        /// <remarks>
        /// This instance will continue to be reused. Therefore if maintaining the value is important, be sure
        /// to clone this class before calling <see cref="Read"/>.
        /// </remarks>
        public TKey CurrentKey
        {
            get;
            private set;
        }

        /// <summary>
        /// The instance that will contain the current value if <see cref="IsValid"/> is true.
        /// </summary>
        /// <remarks>
        /// This instance will continue to be reused. Therefore if maintaining the value is important, be sure
        /// to clone this class before calling <see cref="Read"/>.
        /// </remarks>
        public TValue CurrentValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public abstract bool Read();

        /// <summary>
        /// Cancels the reading of the stream. This does not need to be called if <see cref="Read"/> returns
        /// the end of the stream.
        /// </summary>
        public virtual void Cancel()
        {
        }

        /// <summary>
        /// Used to maintain the relationship that a stream's <see cref="CurrentKey"/> and <see cref="CurrentValue"/>
        /// are always the same instance. Can also help having to copy the Key and Value parameters if in a nested stream reading case.
        /// </summary>
        /// <param name="key">the instance to use for <see cref="CurrentKey"/></param>
        /// <param name="value">the instance to use for <see cref="CurrentValue"/></param>
        /// <remarks>
        /// Be weary of calling this function too often as assignment of classes in .NET requires a function call.
        /// </remarks>
        protected void SetKeyValueReferences(TKey key, TValue value)
        {
            CurrentKey = key;
            CurrentValue = value;
        }
    }
}