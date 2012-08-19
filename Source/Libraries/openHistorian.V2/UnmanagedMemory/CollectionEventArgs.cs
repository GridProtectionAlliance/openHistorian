//******************************************************************************************************
//  CollectionEventArgs.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  8/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.UnmanagedMemory
{
    public enum BufferPoolCollectionMode
    {
        None,
        Normal,
        Emergency,
        Critical
    }
    
    /// <summary>
    /// This contains information about the collection that is requested from the system.
    /// </summary>
    public class CollectionEventArgs : EventArgs
    {
        Func<int, bool> m_releasePage;

        /// <summary>
        /// When <see cref="CollectionMode"/> is <see cref="BufferPoolCollectionMode.Emergency"/> or 
        /// <see cref="BufferPoolCollectionMode.Critical"/> this field contains the number of pages
        /// that need to be released by all of the objects. This value will automatically decrement
        /// every time a page has been released.
        /// </summary>
        public int DesiredPageReleaseCount { get; private set; }

        /// <summary>
        /// The mode for the collection
        /// </summary>
        public BufferPoolCollectionMode CollectionMode { get; private set; }

        /// <summary>
        /// Creates a new <see cref="CollectionEventArgs"/>.
        /// </summary>
        /// <param name="releasePage"></param>
        /// <param name="collectionMode"></param>
        /// <param name="desiredPageReleaseCount"></param>
        public CollectionEventArgs(Func<int, bool> releasePage, BufferPoolCollectionMode collectionMode, int desiredPageReleaseCount)
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
            if (m_releasePage(index))
            {
                DesiredPageReleaseCount--;
            }
        }

    }
}
