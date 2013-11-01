//******************************************************************************************************
//  ZeroNodeScanner`2.cs - Gbtc
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

using System;
using GSF.IO;
using GSF.UnmanagedMemory;

namespace openHistorian.Collections.Generic.TreeNodes
{
    public unsafe class ZeroNodeScanner<TKey, TValue>
        : EncodedNodeScannerBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private readonly int m_shimSize;
        private readonly byte[] m_buffer;

        public ZeroNodeScanner(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods, 2)
        {
            m_shimSize = KeyValueSize >> 3 + (((KeyValueSize & 7) == 0) ? 0 : 1);
            m_buffer = new byte[MaximumStorageSize];
        }

        protected override unsafe int DecodeRecord(byte* stream, TKey key, TValue value)
        {
            fixed (byte* tmp = m_buffer)
            {
                Memory.Clear(tmp, KeyValueSize);
                int nextReadPosition = m_shimSize;
                for (int x = 0; x < KeyValueSize; x++)
                {
                    if ((stream[x >> 3] & (1 << (x & 7))) > 0)
                    {
                        tmp[x] = stream[nextReadPosition];
                        nextReadPosition++;
                    }
                }

                KeyMethods.Read(tmp, key);
                ValueMethods.Read(tmp + KeySize, value);
                return nextReadPosition;
            }
        }

        protected override void ResetEncoder()
        {
            //No cached values to reset.
        }

        private int MaximumStorageSize
        {
            get
            {
                return KeyValueSize + m_shimSize;
            }
        }

    }
}