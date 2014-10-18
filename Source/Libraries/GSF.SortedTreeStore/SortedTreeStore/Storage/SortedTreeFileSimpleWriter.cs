//******************************************************************************************************
//  SortedTreeFileSimpleWriter.cs - Gbtc
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
//  10/16/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.IO.FileStructure;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.Specialized;

namespace GSF.SortedTreeStore.Storage
{
    /// <summary>
    /// Will write a file.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static class SortedTreeFileSimpleWriter<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        /// <summary>
        /// Creates a new arhive file with the supplied data.
        /// </summary>
        /// <param name="pendingFileName"></param>
        /// <param name="completeFileName"></param>
        /// <param name="blockSize"></param>
        /// <param name="treeNodeType"></param>
        /// <param name="treeStream"></param>
        /// <param name="flags"></param>
        public static void Create(string pendingFileName, string completeFileName, int blockSize, EncodingDefinition treeNodeType, TreeStream<TKey, TValue> treeStream, params Guid[] flags)
        {
            using (var writer = new SimplifiedFileWriter(pendingFileName, completeFileName, blockSize, flags))
            {
                using (var file = writer.CreateFile(GetFileName()))
                using (var bs = new BinaryStream(file))
                {
                    SequentialSortedTreeWriter<TKey, TValue>.Create(bs, blockSize - 32, treeNodeType, treeStream);
                }
                writer.Commit();
            }
        }

        /// <summary>
        /// Helper method. Creates the <see cref="SubFileName"/> for the default table.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        private static SubFileName GetFileName()
        {
            Guid keyType = new TKey().GenericTypeGuid;
            Guid valueType = new TValue().GenericTypeGuid;
            return SubFileName.Create(SortedTreeFile.PrimaryArchiveType, keyType, valueType);
        }


    }
}
