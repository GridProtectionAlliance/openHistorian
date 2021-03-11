//******************************************************************************************************
//  SeekFilterBase`1.cs - Gbtc
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
//  11/09/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.Snap.Filters
{
    /// <summary>
    /// Represents a filter that is based on a series of ranges of the key value
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class SeekFilterBase<TKey>
    {
        /// <summary>
        /// The filter guid 
        /// </summary>
        public abstract Guid FilterType { get; }

        /// <summary>
        /// the start of the frame to search [Inclusive]
        /// </summary>
        public TKey StartOfFrame { get; protected set; }
        /// <summary>
        /// the end of the frame to search [Inclusive]
        /// </summary>
        public TKey EndOfFrame { get; protected set; }

        /// <summary>
        /// the start of the entire range to search [Inclusive]
        /// </summary>
        public TKey StartOfRange { get; protected set; }
        /// <summary>
        /// the end of the entire range to search [Inclusive]
        /// </summary>
        public TKey EndOfRange { get; protected set; }
        
        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public abstract void Save(BinaryStreamBase stream);
        
        //Seekable portion of the filter

        /// <summary>
        /// Resets the iterative nature of the filter. 
        /// </summary>
        /// <remarks>
        /// Since a time filter is a set of date ranges, this will reset the frame so a
        /// call to <see cref="NextWindow"/> will return the first window of the sequence.
        /// </remarks>
        public abstract void Reset();

        /// <summary>
        /// Gets the next search window.
        /// </summary>
        /// <returns>true if window exists, false if finished.</returns>
        public abstract bool NextWindow();

       

    }
}
