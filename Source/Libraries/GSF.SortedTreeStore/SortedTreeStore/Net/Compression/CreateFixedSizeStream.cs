//******************************************************************************************************
//  CreateFixedSizeStream.cs - Gbtc
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
using GSF.SortedTreeStore.Net.Initialization;

namespace GSF.SortedTreeStore.Net.Compression
{
    class CreateFixedSizeStream
        : CreateKeyValueStreamCompressionBase
    {

        // {8949C57A-ABF3-4CB8-9015-AA732D6A4670}
        public readonly static Guid TypeGuid = new Guid(0x8949c57a, 0xabf3, 0x4cb8, 0x90, 0x15, 0xaa, 0x73, 0x2d, 0x6a, 0x46, 0x70);


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
            return new FixedSizeStream<TKey, TValue>();
        }
    }
}
