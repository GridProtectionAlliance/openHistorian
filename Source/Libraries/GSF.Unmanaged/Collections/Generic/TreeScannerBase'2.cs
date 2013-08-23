//******************************************************************************************************
//  TreeScannerBase.cs - Gbtc
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
//  3/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace openHistorian.Collections.Generic
{
    public abstract unsafe class TreeScannerBase<TKey, TValue>
        : TreeScannerCoreBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private const int OffsetOfVersion = 0;
        private const int OffsetOfNodeLevel = OffsetOfVersion + sizeof(byte);
        private const int OffsetOfRecordCount = OffsetOfNodeLevel + sizeof(byte);
        private const int OffsetOfValidBytes = OffsetOfRecordCount + sizeof(ushort);
        private const int OffsetOfLeftSibling = OffsetOfValidBytes + sizeof(ushort);
        private const int OffsetOfRightSibling = OffsetOfLeftSibling + IndexSize;
        private const int OffsetOfLowerBounds = OffsetOfRightSibling + IndexSize;
        private const int IndexSize = sizeof(uint);

        private readonly Func<TKey, byte, uint> m_lookupKey;
        private readonly TKey m_tempKey;
        protected TKey LowerKey;
        protected TKey UpperKey;
        protected TreeKeyMethodsBase<TKey> KeyMethods;
        protected TreeValueMethodsBase<TValue> ValueMethods;

        protected uint NodeIndex
        {
            get;
            private set;
        }

        protected ushort RecordCount
        {
            get;
            private set;
        }

        protected uint LeftSiblingNodeIndex
        {
            get;
            private set;
        }

        protected uint RightSiblingNodeIndex
        {
            get;
            private set;
        }

        private readonly byte m_version;
        private readonly byte m_level;
        private readonly int m_blockSize;
        protected readonly int KeyValueSize;
        protected readonly BinaryStreamBase Stream;
        protected byte* Pointer;
        protected long PointerVersion;
        protected int KeyIndexOfCurrentKey;
        protected int HeaderSize;
        protected int OffsetOfUpperBounds;
        protected int KeySize;

        protected TreeScannerBase(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey,
                                  TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods)
        {
            m_tempKey = new TKey();
            m_version = Version;
            m_lookupKey = lookupKey;
            m_level = level;
            LowerKey = new TKey();
            UpperKey = new TKey();

            //m_currentNode = new Node(stream, blockSize);

            KeyMethods = keyMethods;
            ValueMethods = valueMethods;

            KeySize = KeyMethods.Size;
            KeyValueSize = (KeyMethods.Size + ValueMethods.Size);

            OffsetOfUpperBounds = OffsetOfLowerBounds + KeySize;
            HeaderSize = OffsetOfLowerBounds + 2 * KeySize;
            m_blockSize = blockSize;
            Stream = stream;
            PointerVersion = -1;
        }

        protected abstract byte Version
        {
            get;
        }

        protected abstract void ReadNext();
        protected abstract void FindKey(TKey key);

        public override bool Read()
        {
            if (KeyIndexOfCurrentKey < RecordCount && Stream.PointerVersion == PointerVersion)
            {
                ReadNext();
                return true;
            }
            return Read2();
        }

        protected bool Read2()
        {
            //return false;
            //If there are no more records in the current node.
            if (KeyIndexOfCurrentKey >= RecordCount)
            {
                //If the last leaf node, return false
                if (RightSiblingNodeIndex == uint.MaxValue)
                {
                    KeyMethods.Clear(CurrentKey);
                    return false;
                }

                LoadNode(RightSiblingNodeIndex);
            }
            if (Stream.PointerVersion != PointerVersion)
            {
                RefreshPointer();
            }

            ReadNext();
            return true;
        }

        public virtual void SeekToStart()
        {
            KeyMethods.SetMin(m_tempKey);
            SeekToKey(m_tempKey);
        }

        public override void SeekToKey(TKey key)
        {
            LoadNode(FindLeafNodeAddress(key));
            FindKey(key);
        }

        private void LoadNode(uint value)
        {
            NodeIndex = value;

            RefreshPointer();

            byte* ptr = Pointer - HeaderSize;
            if (ptr[OffsetOfVersion] != m_version)
                throw new Exception("Unknown node Version.");
            if (ptr[OffsetOfNodeLevel] != m_level)
                throw new Exception("This node is not supposed to access the underlying node level.");
            RecordCount = *(ushort*)(ptr + OffsetOfRecordCount);
            LeftSiblingNodeIndex = *(uint*)(ptr + OffsetOfLeftSibling);
            RightSiblingNodeIndex = *(uint*)(ptr + OffsetOfRightSibling);
            KeyMethods.Read(ptr + OffsetOfLowerBounds, LowerKey);
            KeyMethods.Read(ptr + OffsetOfUpperBounds, UpperKey);
            KeyIndexOfCurrentKey = 0;
            Reset();
        }

        private void RefreshPointer()
        {
            Pointer = Stream.GetReadPointer(NodeIndex * m_blockSize, m_blockSize) + HeaderSize;
            PointerVersion = Stream.PointerVersion;
        }

        protected uint FindLeafNodeAddress(TKey key)
        {
            return m_lookupKey(key, m_level);
        }

        protected virtual void Reset()
        {
        }
    }
}