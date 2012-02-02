//******************************************************************************************************
//  ReplicationProviders.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/05/2009 - Pinal C. Patel
//       Generated original version of source code.
//
//******************************************************************************************************

using TVA.Adapters;

namespace openHistorian.Replication
{
    /// <summary>
    /// A class that loads all of the <see cref="IReplicationProvider">replication providers</see>.
    /// </summary>
    public class ReplicationProviders : AdapterLoader<IReplicationProvider>
    {
    }
}
