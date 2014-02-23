//******************************************************************************************************
//  SortedTreeNodeCompressionType.cs - Gbtc
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
//  2/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.SortedTreeStore.Tree
{
    /// <summary>
    /// An immutable class that represents the compression method used 
    /// for a <see cref="SortedTreeNodeBase{TKey,TValue}"/>
    /// </summary>
    public class SortedTreeNodeCompressionType
    {
        /// <summary>
        /// Gets if the compression method compresses the key and value as a unit.
        /// </summary>
        public bool IsCombinedKeyValue { get; private set; }

        Guid m_keyCompressionMethod;
        Guid m_valueCompressionMethod;
        Guid m_keyValueCompressionMethod;

        public SortedTreeNodeCompressionType(Guid keyValueCompression)
        {
            m_keyCompressionMethod = Guid.Empty;
            m_valueCompressionMethod = Guid.Empty;
            m_keyValueCompressionMethod = keyValueCompression;
            IsCombinedKeyValue = true;
        }

        public SortedTreeNodeCompressionType(Guid keyCompression, Guid valueCompression)
        {
            m_keyCompressionMethod = keyCompression;
            m_valueCompressionMethod = valueCompression;
            m_keyValueCompressionMethod = Guid.Empty;
            IsCombinedKeyValue = false;
        }

        public void Load(BinaryStreamBase stream)
        {
            m_keyCompressionMethod = stream.ReadGuid();
            m_valueCompressionMethod = stream.ReadGuid();
            m_keyValueCompressionMethod = stream.ReadGuid();
            IsCombinedKeyValue = (m_keyValueCompressionMethod != Guid.Empty);
        }

        public void Save(BinaryStreamBase stream)
        {
            stream.Write(m_keyCompressionMethod);
            stream.Write(m_valueCompressionMethod);
            stream.Write(m_keyValueCompressionMethod);
        }

    }
}
