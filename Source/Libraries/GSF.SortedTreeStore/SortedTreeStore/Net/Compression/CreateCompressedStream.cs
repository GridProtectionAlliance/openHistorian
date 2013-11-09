//******************************************************************************************************
//  CreateCompressedStream.cs - Gbtc
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
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Net.Initialization;

namespace GSF.SortedTreeStore.Net.Compression
{
    class CreateCompressedStream
        : CreateKeyValueStreamCompressionBase
    {

        // {E7E8A378-340E-47DF-B29F-E65361AB32AB}
        public readonly static Guid TypeGuid = new Guid(0xe7e8a378, 0x340e, 0x47df, 0xb2, 0x9f, 0xe6, 0x53, 0x61, 0xab, 0x32, 0xab);



        public override Type KeyTypeIfFixed
        {
            get
            {
                return null;
            }
        }

        public override Type ValueTypeIfFixed
        {
            get
            {
                return null;
            }
        }

        public override Guid GetTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        public override KeyValueStreamCompressionBase<TKey, TValue> Create<TKey, TValue>()
        {
            return new CompressedStream<TKey, TValue>();
        }
    }
}
