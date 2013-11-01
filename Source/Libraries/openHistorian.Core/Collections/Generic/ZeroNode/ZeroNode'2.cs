//******************************************************************************************************
//  ZeroNode`2.cs - Gbtc
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
//  4/16/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using GSF.UnmanagedMemory;

namespace openHistorian.Collections.Generic.ZeroNode
{
    public unsafe class ZeroNode<TKey, TValue>
        : EncodedNodeBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private readonly int m_shimSize;

        public ZeroNode(byte level, TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods)
            : base(level, keyMethods, valueMethods, 2)
        {
            m_shimSize = KeyValueSize >> 3 + (((KeyValueSize & 7) == 0) ? 0 : 1);
        }

        protected override unsafe int EncodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            return Write(stream, currentKey, currentValue);
        }

        protected override unsafe int DecodeRecord(byte* stream, byte* buffer, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            //byte* tmp = stackalloc byte[MaximumStorageSize];
            return Read(stream, buffer, currentKey, currentValue);
        }

        protected override unsafe int MaximumStorageSize
        {
            get
            {
                return KeyValueSize + m_shimSize;
            }
        }

        protected override int MaxOverheadWithCombineNodes
        {
            get
            {
                return MaximumStorageSize * 2;
            }
        }

        public int Write(byte* stream, TKey key, TValue value)
        {
            KeyMethods.Write(stream + m_shimSize, key);
            ValueMethods.Write(stream + m_shimSize + KeySize, value);

            for (int x = 0; x < m_shimSize; x++)
            {
                stream[x] = 0;
            }

            int nextWritePosition = m_shimSize;
            for (int x = 0; x < KeyValueSize; x++)
            {
                if (stream[x + m_shimSize] != 0)
                {
                    stream[x >> 3] ^= (byte)(1 << (x & 7));
                    stream[nextWritePosition] = stream[x + m_shimSize];
                    nextWritePosition++;
                }
            }
            return nextWritePosition;
        }

        public int Read(byte* stream, byte* temp, TKey key, TValue value)
        {
            Memory.Clear(temp, KeyValueSize);
            int nextReadPosition = m_shimSize;
            for (int x = 0; x < KeyValueSize; x++)
            {
                if ((stream[x >> 3] & (1 << (x & 7))) > 0)
                {
                    temp[x] = stream[nextReadPosition];
                    nextReadPosition++;
                }
            }

            KeyMethods.Read(temp, key);
            ValueMethods.Read(temp + KeySize, value);
            return nextReadPosition;
        }

        public override unsafe TreeScannerBase<TKey, TValue> CreateTreeScanner()
        {
            return new ZeroNodeScanner<TKey, TValue>(Level, BlockSize, Stream, SparseIndex.Get, KeyMethods.Create(), ValueMethods.Create());
        }
    }
}