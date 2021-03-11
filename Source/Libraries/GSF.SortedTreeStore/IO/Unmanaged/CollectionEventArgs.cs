//******************************************************************************************************
//  CollectionEventArgs.cs - Gbtc
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
//  8/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Speifies how critical the collection of memory blocks is.
    /// </summary>
    public enum MemoryPoolCollectionMode
    {
        /// <summary>
        /// This means no collection has to occur.
        /// </summary>
        None,
        /// <summary>
        /// This is the routine mode
        /// </summary>
        Normal,
        /// <summary>
        /// This means the engine is using more memory than desired
        /// </summary>
        Emergency,
        /// <summary>
        /// This means any memory that can be released should be released. 
        /// If no memory is released after this pass, 
        /// an out of memory exception will occur.
        /// </summary>
        Critical
    }

    /// <summary>
    /// This contains information about the collection that is requested from the system.
    /// </summary>
    public class CollectionEventArgs : EventArgs
    {
        private readonly Action<int> m_releasePage;

        /// <summary>
        /// When <see cref="CollectionMode"/> is <see cref="MemoryPoolCollectionMode.Emergency"/> or 
        /// <see cref="MemoryPoolCollectionMode.Critical"/> this field contains the number of pages
        /// that need to be released by all of the objects. This value will automatically decrement
        /// every time a page has been released.
        /// </summary>
        public int DesiredPageReleaseCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The mode for the collection
        /// </summary>
        public MemoryPoolCollectionMode CollectionMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new <see cref="CollectionEventArgs"/>.
        /// </summary>
        /// <param name="releasePage"></param>
        /// <param name="collectionMode"></param>
        /// <param name="desiredPageReleaseCount"></param>
        public CollectionEventArgs(Action<int> releasePage, MemoryPoolCollectionMode collectionMode, int desiredPageReleaseCount)
        {
            DesiredPageReleaseCount = desiredPageReleaseCount;
            m_releasePage = releasePage;
            CollectionMode = collectionMode;
        }

        /// <summary>
        /// Releases an unused page.
        /// </summary>
        /// <param name="index">the index of the page</param>
        public void ReleasePage(int index)
        {
            m_releasePage(index);
            DesiredPageReleaseCount--;
        }
    }
}