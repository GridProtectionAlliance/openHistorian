//******************************************************************************************************
//  MetadataProviders.cs - Gbtc
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
//  ----------------------------------------------------------------------------------------------------
//  08/04/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/10/2009 - Pinal C. Patel
//       Override Initialize() to start OperationQueue.
//       Modified RefreshOne() take in the name of the provider to use for refreshing the metadata.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using TVA.Adapters;

namespace TimeSeriesArchiver.MetadataProviders
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
