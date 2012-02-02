//******************************************************************************************************
//  MetadataProviders.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/04/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/10/2009 - Pinal C. Patel
//       Override Initialize() to start OperationQueue.
//       Modified RefreshOne() take in the name of the provider to use for refreshing the metadata.
//
//******************************************************************************************************

using System;
using TVA.Adapters;

namespace openHistorian.Metadata
{
    /// <summary>
    /// A class that loads all <see cref="IMetadataProvider">Metadata Provider</see> adapters.
    /// </summary>
    /// <seealso cref="IMetadataProvider"/>
    public class MetadataProviders : AdapterLoader<IMetadataProvider>
    {
        #region [ Methods ]

        /// <summary>
        /// Initializes the <see cref="MetadataProviders"/>.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            OperationQueue.Start();
        }

        /// <summary>
        /// <see cref="IMetadataProvider.Refresh()"/>es the <see cref="IMetadataProvider.Metadata"/> using all loaded metadata provider <see cref="AdapterLoader{T}.Adapters"/>.
        /// </summary>
        public void RefreshAll()
        {
            OperationQueue.Add(null);
        }

        /// <summary>
        /// <see cref="IMetadataProvider.Refresh()"/>es the <see cref="IMetadataProvider.Metadata"/> using the specified <paramref name="provider"/> from the loaded metadata provider <see cref="AdapterLoader{T}.Adapters"/>.
        /// </summary>
        /// <param name="provider">Name of the <see cref="IMetadataProvider"/> to use for the <see cref="IMetadataProvider.Refresh()"/>.</param>
        public void RefreshOne(string provider)
        {
            OperationQueue.Add(provider);
        }

        /// <summary>
        /// Executes <see cref="IMetadataProvider.Refresh()"/> on the specified metadata provider <paramref name="adapter"/>.
        /// </summary>
        /// <param name="adapter">An <see cref="IMetadataProvider"/> object.</param>
        /// <param name="data"><see cref="System.Reflection.MemberInfo.Name"/> of the <paramref name="adapter"/>.</param>
        protected override void ExecuteAdapterOperation(IMetadataProvider adapter, object data)
        {
            if (adapter.Enabled && 
                (string.IsNullOrEmpty(Convert.ToString(data)) || 
                 string.Compare(data.ToString(), adapter.GetType().Name, true) == 0))
            {
                adapter.Refresh();
            }
        }

        #endregion        
    }
}
