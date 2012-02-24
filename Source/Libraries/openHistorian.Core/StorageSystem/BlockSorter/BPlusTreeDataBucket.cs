//******************************************************************************************************
//  BPlusTreeDataBucket.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  2/22/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace Historian.StorageSystem.BlockSorter
{
    static class BPlusTreeDataBucket
    {
        /// <summary>
        /// Reads the data from the byte array block that starts at the given address.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="address">The absolute position of the start of the byte array block.</param>
        /// <returns></returns>
        internal static byte[] Read(TreeHeader header, long address)
        {
            if (address < 0)
                return null;
            long oldPosition = header.Stream.Position;
            header.Stream.Position = address;
            int length = (int)header.Stream.Read7BitUInt32();
            var data = new byte[length];
            header.Stream.Read(data, 0, length);
            header.Stream.Position = oldPosition;
            return data;
        }

        /// <summary>
        /// Writes the following data to the stream and returns the address of the data block.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static long Write(TreeHeader header, byte[] data)
        {
            long oldPosition = header.Stream.Position;
            int requiredSize = Compression.Get7BitSize((uint)data.Length) + data.Length;
            long starting = header.AllocateSpace(requiredSize);
            header.Stream.Position = starting;
            header.Stream.Write7Bit((uint)data.Length);
            header.Stream.Write(data, 0, data.Length);
            header.Stream.Position = oldPosition;
            return starting;
        }
    }
}
