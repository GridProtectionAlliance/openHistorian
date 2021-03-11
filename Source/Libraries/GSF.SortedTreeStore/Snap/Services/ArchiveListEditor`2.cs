//******************************************************************************************************
//  ArchiveListEditor`2.cs - Gbtc
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.Snap.Storage;

namespace GSF.Snap.Services
{
    /// <summary>
    /// Provides a way to edit an <see cref="ArchiveList{TKey,TValue}"/> since all edits must be atomic.
    /// WARNING: Instancing this class on an <see cref="ArchiveList{TKey,TValue}"/> will lock the class
    /// until <see cref="ArchiveListEditor.Dispose"/> is called. Therefore, keep locks to a minimum and always
    /// use a Using block.
    /// </summary>
    public abstract class ArchiveListEditor<TKey, TValue>
        : ArchiveListEditor
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {

        /// <summary>
        /// Adds an archive file to the list with the given state information.
        /// </summary>
        /// <param name="sortedTree">archive table to add</param>
        public abstract void Add(SortedTreeTable<TKey, TValue> sortedTree);

    }
}
