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
        protected TreeStream()
        {
            CurrentKey = new TKey();
            CurrentValue = new TValue();
        }

        /// <summary>
        /// Gets if the <see cref="CurrentKey"/> and <see cref="CurrentValue"/> fields are valid.
        /// </summary>]
        [Obsolete("Not used anymore")]
        public bool IsValid
        {
            get;
            protected set;
        }

        [Obsolete("Not used anymore")]
        public TKey CurrentKey
        {
            get;
            private set;
        }

        [Obsolete("Not used anymore")]
        public TValue CurrentValue
        {
            get;
            private set;
        }

        [Obsolete("Not used anymore")]
        public bool Read()
        {
            if (Read(CurrentKey, CurrentValue))
            {
                IsValid = true;
            }
            IsValid = false;
            EOS = true;
            return false;
        }

        /// <summary>
        /// Boolean indicating that the end of the stream has been reached.
        /// </summary>
        public bool EOS { get; protected set; }

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public abstract bool Read(TKey key, TValue value);

        /// <summary>
        /// Cancels the reading of the stream. This does not need to be called if <see cref="Read"/> returns
        /// the end of the stream.
        /// </summary>
        public virtual void Cancel()
        {
            EOS = true;
        }

    }
}