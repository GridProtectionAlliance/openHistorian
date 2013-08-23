//******************************************************************************************************
//  EncodedNodeScannerBase.cs - Gbtc
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
//  5/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace openHistorian.Collections.Generic
{
    public abstract class EncodedNodeScannerBase<TKey, TValue>
        : TreeScannerBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private int m_nextOffset;
        private int m_currentIndex;
        private bool m_skipNextRead;

        protected EncodedNodeScannerBase(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods)
        {
        }

        protected abstract unsafe int DecodeRecord(byte* stream, TKey key, TValue value);
        
        protected abstract void ResetEncoder();

        protected override unsafe void ReadNext()
        {
            if (m_skipNextRead)
                m_skipNextRead = false;
            else
                Read();
            KeyIndexOfCurrentKey++;
        }

        protected override unsafe void FindKey(TKey key)
        {
            Reset();
            while (Read() && KeyMethods.IsLessThan(CurrentKey, key))
                ;
            KeyIndexOfCurrentKey = (ushort)m_currentIndex;
            m_skipNextRead = true;
        }

        protected override void Reset()
        {
            m_nextOffset = HeaderSize;
            m_currentIndex = -1;
            ResetEncoder();
        }

        private unsafe bool Read()
        {
            if (m_currentIndex == RecordCount)
            {
                throw new Exception("Read past the end of the stream");
            }
            m_currentIndex++;
            if (m_currentIndex == RecordCount)
                return false;

            m_nextOffset += DecodeRecord(Pointer + m_nextOffset - HeaderSize, CurrentKey, CurrentValue);
            return true;
        }
    }
}