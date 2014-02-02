//******************************************************************************************************
//  SortedTreeValueMethodsUInt32.cs - Gbtc
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
//  4/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Types
{

  

    public class SortedTreeValueMethodsUInt32
        : SortedTreeValueMethodsBase<SortedTreeUInt32>
    {
        // {03F4BD3A-D9CF-4358-B175-A9D38BE6715A}
        public static Guid TypeGuid = new Guid(0x03f4bd3a, 0xd9cf, 0x4358, 0xb1, 0x75, 0xa9, 0xd3, 0x8b, 0xe6, 0x71, 0x5a);

        public override void ReadCompressed(BinaryStreamBase stream, SortedTreeUInt32 currentKey, SortedTreeUInt32 previousKey)
        {
            currentKey.Value = stream.Read7BitUInt32() ^ previousKey.Value;
        }
        public override void WriteCompressed(BinaryStreamBase stream, SortedTreeUInt32 currentKey, SortedTreeUInt32 previousKey)
        {
            stream.Write7Bit(previousKey.Value ^ currentKey.Value);
        }

        protected override int GetSize()
        {
            return 4;
        }

        public override unsafe void Write(byte* stream, SortedTreeUInt32 data)
        {
            *(uint*)stream = data.Value;
        }

        public override void Clear(SortedTreeUInt32 data)
        {
            data.Value = 0;
        }

        public override unsafe void Read(byte* stream, SortedTreeUInt32 data)
        {
            data.Value = *(uint*)stream;
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                return TypeGuid;
            }
        }

        public override unsafe void Copy(SortedTreeUInt32 source, SortedTreeUInt32 destination)
        {
            destination.Value = source.Value;
        }
    }
}