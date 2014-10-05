//******************************************************************************************************
//  ServerDatabaseBase.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using GSF.Diagnostics;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    /// <summary>
    /// Represents the server side of a single database.
    /// </summary>
    public abstract class ServerDatabaseBase 
        : LogSourceBase
    {

        /// <summary>
        /// Creates a <see cref="ServerDatabaseBase"/>
        /// </summary>
        /// <param name="parent">the parent source.</param>
        protected ServerDatabaseBase(LogSource parent)
            : base(parent)
        {
        }

        protected override string GetSourceDetails()
        {
            var info = Info;
            return string.Format("Database: {0} Key: {1} Value: {2}", info.DatabaseName, info.KeyType.FullName, info.ValueType.FullName);
        }

        /// <summary>
        /// Gets basic information about the current Database.
        /// </summary>
        public abstract DatabaseInfo Info { get; }

        /// <summary>
        /// Creates a <see cref="ServerDatabase{TKey,TValue}.ClientDatabase"/>
        /// </summary>
        /// <returns></returns>
        public abstract ClientDatabaseBase CreateClientDatabase(Client client, Action<ClientDatabaseBase> onDispose);

        /// <summary>
        /// Gets the full status text for the server.
        /// </summary>
        /// <param name="status"></param>
        public abstract void GetFullStatus(StringBuilder status);

        /// <summary>
        /// Creates a new server database from the provided config.
        /// </summary>
        /// <param name="databaseConfig"></param>
        /// <param name="parent">the parent LogSource</param>
        /// <returns></returns>
        public static ServerDatabaseBase CreateDatabase(ServerDatabaseSettings databaseConfig, LogSource parent)
        {
            var keyType = Library.GetSortedTreeType(databaseConfig.KeyType);
            var valueType = Library.GetSortedTreeType(databaseConfig.ValueType);

            var type = typeof(ServerDatabaseBase);
            var method = type.GetMethod("CreateDatabase", BindingFlags.NonPublic | BindingFlags.Static);
            var reflectionMethod = method.MakeGenericMethod(keyType, valueType);
            return (ServerDatabaseBase)reflectionMethod.Invoke(null, new object[] { databaseConfig, parent });
        }

        //Called through reflection. Its the only way to call a generic function only knowing the Types
        [MethodImpl(MethodImplOptions.NoOptimization)] //Prevents removing this method as it may appear unused.
        static ServerDatabaseBase CreateDatabase<TKey, TValue>(ServerDatabaseConfig databaseConfig, LogSource parent)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return new ServerDatabase<TKey, TValue>(databaseConfig, parent);
        }


    }
}