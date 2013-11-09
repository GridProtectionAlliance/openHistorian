//******************************************************************************************************
//  RolloverArgs'2.cs - Gbtc
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.SortedTreeStore.Storage;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Engine.Writer
{
    /// <summary>
    /// Contains the set of arguments that are passed between stages
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class RolloverArgs<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
       
        /// <summary>
        /// Populates the default values of the arguments.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sequenceNumber"></param>
        public RolloverArgs(TreeStream<TKey, TValue> stream, long sequenceNumber)
        {
            File = null;
            CurrentStream = stream;
            SequenceNumber = sequenceNumber;
        }
        /// <summary>
        /// Populates the default values of the arguments.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sequenceNumber"></param>
        public RolloverArgs(SortedTreeTable<TKey, TValue> file, long sequenceNumber)
        {
            File = file;
            CurrentStream = null;
            SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Contains the archive file unless it comes from the prestaging table.
        /// May be null if only a stream was passed to this constructor
        /// </summary>
        public SortedTreeTable<TKey, TValue> File
        {
            get;
            private set;
        }

        /// <summary>
        /// Contains the stream from the prestaging table.
        /// May be null if only a file was passed to this constructor
        /// </summary>
        public TreeStream<TKey, TValue> CurrentStream
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public long SequenceNumber
        {
            get;
            private set;
        }
    }
}