//******************************************************************************************************
//  IReplicationProvider.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  -----------------------------------------------------------------------------------------------------
//  11/02/2009 - Pinal C. Patel
//       Generated original version of source code.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;
using TVA.Adapters;

namespace openHistorian.V1.Replication
{
    /// <summary>
    /// Defines a provider of replication mechanism for the <see cref="IArchive"/>.
    /// </summary>
    public interface IReplicationProvider : IAdapter
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IArchive"/> is started.
        /// </summary>
        event EventHandler ReplicationStart;

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IArchive"/> is complete.
        /// </summary>
        event EventHandler ReplicationComplete;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered during the replication process of <see cref="IArchive"/>.
        /// </summary>
        event EventHandler<EventArgs<Exception>> ReplicationException;

        /// <summary>
        /// Occurs when the <see cref="IArchive"/> is being replicated.
        /// </summary>
        event EventHandler<EventArgs<ProcessProgress<int>>> ReplicationProgress;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the primary location of the <see cref="IArchive"/>.
        /// </summary>
        string ArchiveLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mirrored location of the <see cref="IArchive"/>.
        /// </summary>
        string ReplicaLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interval in milliseconds at which the <see cref="IArchive"/> is to be replicated.
        /// </summary>
        int ReplicationInterval
        {
            get;
            set;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Replicates the <see cref="IArchive"/>.
        /// </summary>
        /// <returns>true if the replication is successful; otherwise false.</returns>
        bool Replicate();

        #endregion
    }
}
