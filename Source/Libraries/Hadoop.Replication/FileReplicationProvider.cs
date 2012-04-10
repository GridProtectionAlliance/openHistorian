//*******************************************************************************************************
//  FileReplicationProvider.cs - Gbtc
//
//  Tennessee Valley Authority, 2009
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  11/05/2009 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Linq;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Files;
using TVA;
using TVA.Historian.Replication;
using TVA.IO;

namespace NERC.PCS.Replication
{
    /// <summary>
    /// Represents a provider of replication for the <see cref="TVA.Historian.IArchive"/> to a file system folder/share using file-base synchronization.
    /// </summary>
    public class FileReplicationProvider : ReplicationProviderBase
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReplicationProvider"/> class.
        /// </summary>
        public FileReplicationProvider()
            : base()
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Replicates the <see cref="TVA.Historian.IArchive"/>.
        /// </summary>
        protected override void ReplicateArchive()
        {
            // Connect to remote share if specified.
            string archiveLocation = ArchiveLocation;
            string replicaLocation = ReplicaLocation;
            if (replicaLocation.StartsWith(@"\\") && replicaLocation.Contains(':') && replicaLocation.Contains('@'))
            {
                // Format: \\[<domain>\]<username>:<password>@<network share>
                string share = @"\\" + replicaLocation.Substring(replicaLocation.IndexOf('@') + 1);
                string login = replicaLocation.Substring(2, replicaLocation.IndexOf(':') - 2);
                string password = replicaLocation.Substring(replicaLocation.IndexOf(':') + 1, replicaLocation.IndexOf('@') - replicaLocation.IndexOf(':') - 1);

                replicaLocation = share;
                string[] loginParts = login.Split('\\');
                if (loginParts.Length == 2)
                    FilePath.ConnectToNetworkShare(replicaLocation, loginParts[0], password, loginParts[1]);
                else
                    FilePath.ConnectToNetworkShare(replicaLocation, login, password, Environment.UserDomainName);
            }

            FileSyncProvider syncSource = null;
            FileSyncProvider syncDestination = null;
            try
            {
                // Setup file synchronization filter.
                FileSyncScopeFilter synchFilter = new FileSyncScopeFilter();
                synchFilter.FileNameIncludes.Add("*_to_*.d");

                // Setup file synchronization providers.
                syncSource = new FileSyncProvider(archiveLocation, synchFilter, FileSyncOptions.CompareFileStreams);
                syncDestination = new FileSyncProvider(replicaLocation, synchFilter, FileSyncOptions.CompareFileStreams);
                syncDestination.ApplyingChange += SyncDestination_ApplyingChange;
                syncDestination.AppliedChange += SyncDestination_AppliedChange;

                // Setup and start file synchronization agent.
                SyncOrchestrator syncAgent = new SyncOrchestrator();
                syncAgent.LocalProvider = syncSource;
                syncAgent.RemoteProvider = syncDestination;
                syncAgent.Direction = SyncDirectionOrder.Upload;
                syncAgent.Synchronize();
            }
            finally
            {
                // Release resource used by synchronization providers.
                if (syncSource != null)
                    syncSource.Dispose();

                if (syncDestination != null)
                    syncDestination.Dispose();
            }
        }

        private void SyncDestination_ApplyingChange(object sender, ApplyingChangeEventArgs e)
        {
            // Skip synchronization for deletes and renames.
            if (e.ChangeType == ChangeType.Delete || e.ChangeType == ChangeType.Rename)
                e.SkipChange = true;
        }

        private void SyncDestination_AppliedChange(object sender, AppliedChangeEventArgs e)
        {
            // Provide an update for the replication progress.
            OnReplicationProgress(new ProcessProgress<int>("ReplicateArchive", e.NewFilePath, 1, 1));
        }

        #endregion
    }
}
