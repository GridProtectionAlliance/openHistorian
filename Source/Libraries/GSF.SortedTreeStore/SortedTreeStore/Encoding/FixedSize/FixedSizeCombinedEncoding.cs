//******************************************************************************************************
//  FixedSizeCombinedEncoding`1.cs - Gbtc
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
//  2/21/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Encoding
{
    public class CreateFixedSizeCombinedEncoding
        : CreateCombinedValuesBase
    {
        // {1DEA326D-A63A-4F73-B51C-7B3125C6DA55}
        /// <summary>
        /// The guid that represents the encoding method of this class
        /// </summary>
        public static readonly Guid TypeGuid = new Guid(0x1dea326d, 0xa63a, 0x4f73, 0xb5, 0x1c, 0x7b, 0x31, 0x25, 0xc6, 0xda, 0x55);

        public override Type KeyTypeIfNotGeneric
        {
            get
            {
                return null;
            }
        }

        public override Type ValueTypeIfNotGeneric
        {
            get
            {
                return null;
            }
        }

        public override Guid Method
        {
            get
            {
                return TypeGuid;
            }
        }

        public override DoubleValueEncodingBase<TKey, TValue> Create<TKey, TValue>()
        {
            return new FixedSizeCombinedEncoding<TKey, TValue>();
        }
    }

    public class FixedSizeCombinedEncoding<TKey, TValue>
        : DoubleValueEncodingBase<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {

        SortedTreeTypeMethods<TKey> m_keyMethods;
        SortedTreeTypeMethods<TValue> m_valueMethods;

        public FixedSizeCombinedEncoding()
        {
            m_keyMethods = new TKey().CreateValueMethods();
            m_valueMethods = new TValue().CreateValueMethods();
        }

        public override bool UsesPreviousKey
        {
            get
            {
                return false;
            }
        }

        public override bool UsesPreviousValue
        {
            get
            {
                return false;
            }
        }

        public override int MaxCompressionSize
        {
            get
            {
                return m_keyMethods.Size + m_valueMethods.Size;
            }
        }

        public override void Compress(BinaryStreamBase stream, TKey prevKey, TValue prevValue, TKey key, TValue value)
        {
            m_keyMethods.Write(stream, key);
            m_valueMethods.Write(stream, value);
        }

        public override void Decompress(BinaryStreamBase stream, TKey prevKey, TValue prevValue, TKey key, TValue value)
        {
            m_keyMethods.Read(stream, key);
            m_valueMethods.Read(stream, value);
        }

        public override DoubleValueEncodingBase<TKey, TValue> Clone()
        {
            return new FixedSizeCombinedEncoding<TKey, TValue>();
        }
    }
}
