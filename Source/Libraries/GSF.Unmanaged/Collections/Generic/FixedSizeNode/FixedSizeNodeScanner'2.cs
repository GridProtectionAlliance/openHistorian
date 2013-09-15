//******************************************************************************************************
//  FixedSizeNode.cs - Gbtc
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
//  4/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace openHistorian.Collections.Generic
{
    public class FixedSizeNodeScanner<TKey, TValue>
        : TreeScannerBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        public FixedSizeNodeScanner(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods, 1)
        {
        }

        protected override unsafe void ReadNext()
        {
            byte* ptr = Pointer + KeyIndexOfCurrentKey * KeyValueSize;
            KeyIndexOfCurrentKey++;
            KeyMethods.Read(ptr, CurrentKey);
            ValueMethods.Read(ptr + KeySize, CurrentValue);
        }

        protected override unsafe void FindKey(TKey key)
        {
            int offset = KeyMethods.BinarySearch(Pointer, key, RecordCount, KeyValueSize);
            if (offset < 0)
                offset = ~offset;
            KeyIndexOfCurrentKey = (ushort)offset;
        }
    }
}