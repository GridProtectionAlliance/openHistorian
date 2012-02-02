//******************************************************************************************************
//  IReplicationProvider.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  11/02/2009 - Pinal C. Patel
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using openHistorian.Archives;
using TVA;
using TVA.Adapters;

namespace openHistorian.Replication
{
    /// <summary>
    /// Defines a provider of replication mechanism for the <see cref="IDataArchive"/>.
    /// </summary>
    public interface IReplicationProvider : IAdapter
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IDataArchive"/> is started.
        /// </summary>
        event EventHandler ReplicationStart;

        /// <summary>
        /// Occurs when the process of replicating the <see cref="IDataArchive"/> is complete.
        /// </summary>
        event EventHandler ReplicationComplete;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered during the replication process of <see cref="IDataArchive"/>.
        /// </summary>
        event EventHandler<EventArgs<Exception>> ReplicationException;

        /// <summary>
        /// Occurs when the <see cref="IDataArchive"/> is being replicated.
        /// </summary>
        event EventHandler<EventArgs<ProcessProgress<int>>> ReplicationProgress;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the primary location of the <see cref="IDataArchive"/>.
        /// </summary>
        string ArchiveLocation { get; set; }

        /// <summary>
        /// Gets or sets the mirrored location of the <see cref="IDataArchive"/>.
        /// </summary>
        string ReplicaLocation { get; set; }

        /// <summary>
        /// Gets or sets the interval in milliseconds at which the <see cref="IDataArchive"/> is to be replicated.
        /// </summary>
        int ReplicationInterval { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Replicates the <see cref="IDataArchive"/>.
        /// </summary>
        /// <returns>true if the replication is successful; otherwise false.</returns>
        bool Replicate();

        #endregion
    }
}
