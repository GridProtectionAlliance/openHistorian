//******************************************************************************************************
//  WriteProcessorSettings.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/21/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.Data;
using GSF.IO;

namespace GSF.Snap.Tree
{
    /// <summary>
    /// A static class for some basic functions of the sortedtree.
    /// </summary>
    public static class SortedTree
    {
        /// <summary>
        /// Reads the header data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="treeNodeType"></param>
        /// <param name="blockSize"></param>
        internal static void ReadHeader(BinaryStreamBase stream, out EncodingDefinition treeNodeType, out int blockSize)
        {
            stream.Position = 0;
            byte version = stream.ReadUInt8();
            if (version == 109)
            {
                stream.Position = 0;
                stream.ReadGuid();
                treeNodeType = new EncodingDefinition(stream.ReadGuid());
                blockSize = stream.ReadInt32();
            }
            else if (version == 1)
            {
                blockSize = stream.ReadInt32();
                treeNodeType = new EncodingDefinition(stream);
            }
            else
            {
                throw new VersionNotFoundException();
            }
        }
    }
}
