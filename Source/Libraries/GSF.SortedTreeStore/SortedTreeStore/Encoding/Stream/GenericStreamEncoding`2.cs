//******************************************************************************************************
//  GenericStreamEncoding`2.cs - Gbtc
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
//  2/24/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Encoding
{
    public class GenericStreamEncoding<TKey, TValue>
        : StreamEncodingBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        DoubleValueEncodingBase<TKey, TValue> m_encoding;
        TKey m_prevKey;
        TValue m_prevValue;

        public GenericStreamEncoding(EncodingDefinition encodingMethod)
        {
            m_encoding = Library.Encodings.GetEncodingMethod<TKey, TValue>(encodingMethod);
            m_prevKey = new TKey();
            m_prevValue = new TValue();
        }

        public override bool SupportsPointerSerialization
        {
            get
            {
                return false;
            }
        }

        public override int MaxCompressedSize
        {
            get
            {
                return m_encoding.MaxCompressionSize;
            }
        }

        public override EncodingDefinition EncodingMethod
        {
            get
            {
                return m_encoding.EncodingMethod;
            }
        }

        public override void WriteEndOfStream(BinaryStreamBase stream)
        {
            if (m_encoding.ContainsEndOfStreamSymbol)
                stream.Write(m_encoding.EndOfStreamSymbol);
            else
                stream.Write((byte)0);
        }

        public override void Encode(BinaryStreamBase stream, TKey currentKey, TValue currentValue)
        {
            if (!m_encoding.ContainsEndOfStreamSymbol)
            {
                stream.Write((byte)1);
            }
            m_encoding.Encode(stream, m_prevKey, m_prevValue, currentKey, currentValue);
            currentKey.CopyTo(m_prevKey);
            currentValue.CopyTo(m_prevValue);
        }

        public override unsafe int Encode(byte* stream, TKey currentKey, TValue currentValue)
        {
            throw new NotSupportedException();
        }

        public override unsafe bool TryDecode(BinaryStreamBase stream, TKey key, TValue value)
        {
            if (!m_encoding.ContainsEndOfStreamSymbol)
            {
                if (stream.ReadUInt8() == 0)
                    return false;
            }
            bool endOfStream;
            m_encoding.Decode(stream, m_prevKey, m_prevValue, key, value, out endOfStream);
            key.CopyTo(m_prevKey);
            value.CopyTo(m_prevValue);
            return !endOfStream;
        }

        public override void ResetEncoder()
        {
            m_prevKey.Clear();
            m_prevValue.Clear();
        }
    }
}
