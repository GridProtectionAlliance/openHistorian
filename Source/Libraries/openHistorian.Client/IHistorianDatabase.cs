//******************************************************************************************************
//  IHistorianDatabase.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace openHistorian
{

    /// <summary>
    /// Represents a single historian database.
    /// </summary>
    public interface IHistorianDatabase : IDisposable
    {
        /// <summary>
        /// Determines if this database is currently online.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// The most recent transaction that is available to be queried.
        /// </summary>
        long LastCommittedTransactionId { get; }
        /// <summary>
        /// The most recent transaction id that has been committed to a perminent storage system.
        /// </summary>
        long LastDiskCommittedTransactionId { get; }
        /// <summary>
        /// The transaction of the most recently inserted data.
        /// </summary>
        long CurrentTransactionId { get; }
        
        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        IHistorianDataReader OpenDataReader();

        /// <summary>
        /// Takes the historian database offline
        /// </summary>
        /// <param name="waitTimeSeconds">the maximum number of seconds to wait before terminating all client connections.</param>
        void TakeOffline(float waitTimeSeconds = 0);
        /// <summary>
        /// Brings this database online.
        /// </summary>
        void BringOnline();
        /// <summary>
        /// Shuts down this database.
        /// </summary>
        /// <param name="waitTimeSeconds">the maximum number of seconds to wait before terminating all client connections.</param>
        void Shutdown(float waitTimeSeconds = 0);
        
        void Write(IPointStream points);
        void Write(ulong key1, ulong key2, ulong value1, ulong value2);
        long WriteBulk(IPointStream points);

        bool IsCommitted(long transactionId);
        bool IsDiskCommitted(long transactionId);

        bool WaitForCommitted(long transactionId);
        bool WaitForDiskCommitted(long transactionId);

        void Commit();
        void CommitToDisk();
    }
}
