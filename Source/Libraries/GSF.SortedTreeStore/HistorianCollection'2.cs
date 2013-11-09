//******************************************************************************************************
//  HistorianCollection.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  12/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************


using GSF.SortedTreeStore.Engine;

namespace openHistorian
{
    /// <summary>
    /// Contains a set of named HistorianDatabaseBase.
    /// </summary>
    /// <typeparam name="TKey">They key type of the historian database. Must inherit HistorianKeyBase.</typeparam>
    /// <typeparam name="TValue">The value type of the historian database.</typeparam>
    public abstract class HistorianCollection<TKey, TValue>
        where TKey : EngineKeyBase<TKey>, new()
        where TValue : class, new()
    {
        /// <summary>
        /// Accesses <see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="SortedTreeEngineBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public abstract SortedTreeEngineBase<TKey, TValue> this[string databaseName]
        {
            get;
        }
    }
}