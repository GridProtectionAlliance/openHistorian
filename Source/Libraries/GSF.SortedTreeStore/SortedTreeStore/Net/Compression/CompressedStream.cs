//******************************************************************************************************
//  CompressedStream.cs - Gbtc
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
//  8/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Net.Initialization;

namespace GSF.SortedTreeStore.Net.Compression
{
    public class CompressedStream<TKey, TValue>
        : KeyValueStreamCompressionBase<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {
        TKey prevKey;
        TValue prevValue;

        public CompressedStream()
        {
            prevKey = new TKey();
            prevValue = new TValue();
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
                return -1;
            }
        }

        public override Guid CompressionType
        {
            get
            {
                return CreateCompressedStream.TypeGuid;
            }
        }

        public override void WriteEndOfStream(BinaryStreamBase stream)
        {
            stream.Write(false);
        }

        public override void Encode(BinaryStreamBase stream, TKey currentKey, TValue currentValue)
        {
            stream.Write(true);
            KeyMethods.WriteCompressed(stream, currentKey, prevKey);
            ValueMethods.WriteCompressed(stream, currentValue, prevValue);

            KeyMethods.Copy(currentKey, prevKey);
            ValueMethods.Copy(currentValue, prevValue);
        }

        public override unsafe int Encode(byte* stream, TKey currentKey, TValue currentValue)
        {
            throw new NotImplementedException();
        }

        public override unsafe bool TryDecode(BinaryStreamBase stream, TKey key, TValue value)
        {
            if (!stream.ReadBoolean())
                return false;

            KeyMethods.ReadCompressed(stream, key, prevKey);
            ValueMethods.ReadCompressed(stream, value, prevValue);

            KeyMethods.Copy(key, prevKey);
            ValueMethods.Copy(value, prevValue);

            return true;
        }

        public override void ResetEncoder()
        {
            KeyMethods.Clear(prevKey);
            ValueMethods.Clear(prevValue);
        }
    }
}
