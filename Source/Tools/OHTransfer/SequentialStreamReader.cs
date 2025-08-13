//******************************************************************************************************
//  SequentialStreamReader.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/13/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using GSF.Threading;
using openHistorian.Snap;

namespace OHTransfer;

/// <summary>
/// Represents a sequential reader stream for reading historian data from an archive list.
/// </summary>
/// <remarks>
/// We derive a specific implementation of this class so we can track last processed key in the stream.
/// </remarks>
internal class SequentialStreamReader(
    ArchiveList<HistorianKey, HistorianValue> archiveList, 
    SortedTreeEngineReaderOptions readerOptions = null, 
    SeekFilterBase<HistorianKey> keySeekFilter = null, 
    MatchFilterBase<HistorianKey, HistorianValue> keyMatchFilter = null, 
    WorkerThreadSynchronization workerThreadSynchronization = null) : 
    SequentialReaderStream<HistorianKey, HistorianValue>(
        archiveList, 
        readerOptions, 
        keySeekFilter, 
        keyMatchFilter, 
        workerThreadSynchronization)
{
    public HistorianKey LastKey = new();

    /// <inheritdoc />
    protected override bool ReadNext(HistorianKey key, HistorianValue value)
    {
        bool result = base.ReadNext(key, value);

        LastKey = key;

        return result;
    }
}