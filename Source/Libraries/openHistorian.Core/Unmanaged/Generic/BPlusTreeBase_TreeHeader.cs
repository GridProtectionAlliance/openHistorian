//******************************************************************************************************
//  BPlusTreeBase_TreeHeader.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  2/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;

namespace openHistorian.Core.Unmanaged.Generic
{
    abstract partial class BPlusTreeBase<TKey, TValue>
    {
        public static Guid s_fileType = new Guid("{7bfa9083-701e-4596-8273-8680a739271d}");
        protected BinaryStream m_internalNodeStream;
        protected BinaryStream m_leafNodeStream;
        
        protected int m_blockSize;
        uint m_rootIndexAddress;
        byte m_rootIndexLevel;
        uint m_nextUnallocatedBlock;

        void Load()
        {
            m_internalNodeStream.Position = 0;
            if (s_fileType != m_internalNodeStream.ReadGuid())
                throw new Exception("Header Corrupt");
            if (m_internalNodeStream.ReadByte() != 0)
                throw new Exception("Header Corrupt");
            m_nextUnallocatedBlock = m_internalNodeStream.ReadUInt32();
            m_blockSize = m_internalNodeStream.ReadInt32();
            m_rootIndexAddress = m_internalNodeStream.ReadUInt32();
            m_rootIndexLevel = m_internalNodeStream.ReadByte();
        }
        public void Save()
        {
            m_internalNodeStream.Position = 0;
            m_internalNodeStream.Write(s_fileType);
            m_internalNodeStream.Write((byte)0); //Version
            m_internalNodeStream.Write(m_nextUnallocatedBlock);
            m_internalNodeStream.Write(m_blockSize);
            m_internalNodeStream.Write(m_rootIndexAddress); //Root Index
            m_internalNodeStream.Write(m_rootIndexLevel); //Root Index
        }

        /// <summary>
        /// Returns the node index address for a freshly allocated block.
        /// The node address is block alligned.
        /// </summary>
        /// <returns></returns>
        uint AllocateNewNode()
        {
            uint newBlock = m_nextUnallocatedBlock;
            m_nextUnallocatedBlock++;
            return newBlock;
        }

    }
}
