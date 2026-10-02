//******************************************************************************************************
//  SortedTreeTableSnapshotInfo`2.cs - Gbtc
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
//  05/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.IO.FileStructure;

namespace GSF.Snap.Storage;

/// <summary>
/// Acquires a read transaction on the current archive partition. This will allow all user created
/// transactions to have snapshot isolation of the entire data set.
/// </summary>
public class SortedTreeTableSnapshotInfo<TKey, TValue>
    where TKey : SnapTypeBase<TKey>, new()
    where TValue : SnapTypeBase<TValue>, new()
{
    #region [ Members ]

    private readonly TransactionalFileStructure m_fileStructure;
    private readonly Lazy<ulong[]> m_pointIDs;
    private readonly Lazy<ArchiveTimeBucketBloomIndex> m_bloom;
    private readonly ReadSnapshot m_currentTransaction;
    private readonly SubFileName m_fileName;

    #endregion

    #region [ Constructors ]

    internal SortedTreeTableSnapshotInfo(TransactionalFileStructure fileStructure, SubFileName fileName)
    {
        m_fileName = fileName;
        m_fileStructure = fileStructure;
        m_currentTransaction = m_fileStructure.Snapshot;

        m_pointIDs = new Lazy<ulong[]>(() =>
        {
            Guid keyType = new TKey().GenericTypeGuid;
            Guid valueType = new TValue().GenericTypeGuid;
            
            return m_fileName == SubFileName.Create(SortedTreeFile.PrimaryArchiveType, keyType, valueType) ? 
                ArchivePointIDIndex.Load(m_fileStructure.FileName, m_currentTransaction.Header.ArchiveId, m_currentTransaction.Header.SnapshotSequenceNumber, keyType, valueType) : 
                null;
        });

        m_bloom = new Lazy<ArchiveTimeBucketBloomIndex>(() =>
        {
            Guid keyType = new TKey().GenericTypeGuid;
            Guid valueType = new TValue().GenericTypeGuid;

            return m_fileName == SubFileName.Create(SortedTreeFile.PrimaryArchiveType, keyType, valueType) ? 
                ArchiveTimeBucketBloomIndex.Load(m_fileStructure.FileName, m_currentTransaction.Header.ArchiveId, m_currentTransaction.Header.SnapshotSequenceNumber, keyType, valueType) : 
                null;
        });
    }

    #endregion

    #region [ Properties ]

    internal bool HasTimeBucketBloomIndex => m_bloom.Value is not null;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Opens an instance of the archive file to allow for concurrent reading of a snapshot.
    /// </summary>
    public SortedTreeTableReadSnapshot<TKey, TValue> CreateReadSnapshot()
    {
        return new SortedTreeTableReadSnapshot<TKey, TValue>(m_currentTransaction, m_fileName);
    }

    internal bool MayContainPointID(ulong id, ulong lower, ulong upper)
    {
        return m_bloom.Value is null || m_bloom.Value.MayContain(id, lower, upper);
    }

    internal bool MayContainPointID(ulong id)
    {
        return m_pointIDs.Value is null || Array.BinarySearch(m_pointIDs.Value, id) >= 0;
    }

    #endregion
}