//******************************************************************************************************
//  SparseIndexWriter`1.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/18/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.Snap.Types;

namespace GSF.Snap.Tree.Specialized
{
    /// <summary>
    /// Contains information on how to parse the index nodes of the SortedTree
    /// </summary>
    public sealed class SparseIndexWriter<TKey>
        : TreeStream<TKey, SnapUInt32>
        where TKey : SnapTypeBase<TKey>, new()
    {
        #region [ Members ]

        private long m_count;
        private long m_readingCount;
        private readonly BinaryStreamPointerBase m_stream;
        private bool m_isReading;
        private readonly SnapUInt32 m_value = new SnapUInt32();

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new sparse index. 
        /// </summary>
        public SparseIndexWriter()
        {
            m_stream = new BinaryStream(true);
        }

        #endregion

        /// <summary>
        /// Gets the number of nodes in the sparse index.
        /// </summary>
        public long Count => m_count;

    #region [ Methods ]

        /// <summary>
        /// Adds the following node pointer to the sparse index.
        /// </summary>
        /// <param name="leftPointer">The pointer to the left element, Only used to prime the list.</param>
        /// <param name="nodeKey">the first key in the <see cref="pointer"/>. Only uses the key portion of the TKeyValue</param>
        /// <param name="pointer">the index of the later node</param>
        /// <remarks>This class will add the new node data to the parent node, 
        /// or create a new root if the current root is split.</remarks>
        public void Add(uint leftPointer, TKey nodeKey, uint pointer)
        {
            if (m_isReading)
                throw new Exception("This sparse index writer has already be set in reading mode.");
            if (m_count == 0)
            {
                TKey tmpKey = new TKey();
                tmpKey.SetMin();
                tmpKey.Write(m_stream);
                m_value.Value = leftPointer;
                m_value.Write(m_stream);
                m_count++;
            }
            nodeKey.Write(m_stream);
            m_value.Value = pointer;
            m_value.Write(m_stream);
            m_count++;
        }

        public void SwitchToReading()
        {
            if (m_isReading)
                throw new Exception("Duplicate call.");
            m_isReading = true;
            m_stream.Position = 0;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            m_stream.Dispose();
            base.Dispose(disposing);
        }

        public override bool IsAlwaysSequential => true;

        public override bool NeverContainsDuplicates => true;

        protected override bool ReadNext(TKey key, SnapUInt32 value)
        {
            if (!m_isReading)
                throw new Exception("Must call SwitchToReading() first.");
            if (m_readingCount < m_count)
            {
                m_readingCount++;
                key.Read(m_stream);
                value.Read(m_stream);
                return true;
            }
            return false;
        }
    }
}