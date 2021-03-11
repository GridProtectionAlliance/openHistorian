//******************************************************************************************************
//  PrebufferRolloverArgs`2.cs - Gbtc
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
//  01/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// A set of variables that are generated in the prebuffer stage that are provided to the onRollover 
    /// <see cref="Action"/> passed to the constructor of <see cref="PrebufferWriter{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The key</typeparam>
    /// <typeparam name="TValue">The value</typeparam>
    public class PrebufferRolloverArgs<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {

        /// <summary>
        /// The stream of points that need to be rolled over. 
        /// </summary>
        public readonly TreeStream<TKey, TValue> Stream;

        /// <summary>
        /// The transaction id assoicated with the points in this buffer. 
        /// This is the id of the last point in this buffer.
        /// </summary>
        public readonly long TransactionId;

        /// <summary>
        /// Creates a set of args
        /// </summary>
        /// <param name="stream">the stream to specify</param>
        /// <param name="transactionId">the number to specify</param>
        public PrebufferRolloverArgs(TreeStream<TKey, TValue> stream, long transactionId)
        {
            Stream = stream;
            TransactionId = transactionId;
        }
    }
}