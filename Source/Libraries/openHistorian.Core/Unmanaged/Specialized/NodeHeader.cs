//******************************************************************************************************
//  NodeHeader.cs - Gbtc
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

namespace openHistorian.Core.Unmanaged.Specialized
{
    /// <summary>
    /// Assists in the read/write operations of the header of a node.
    /// </summary>
    struct NodeHeader
    {
        public const int Size = 11;
        public byte Level;
        public short ChildCount;
        public uint PreviousNode;
        public uint NextNode;

        public void Load(BinaryStream stream, int blockSize, uint nodeIndex)
        {
            stream.Position = blockSize * nodeIndex;
            Load(stream);
        }

        public void Load(BinaryStream stream)
        {
            Level = stream.ReadByte();
            ChildCount = stream.ReadInt16();
            PreviousNode = stream.ReadUInt32();
            NextNode = stream.ReadUInt32();
        }
        /// <summary>
        /// Saves the node data to the underlying stream. 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="nodeIndex"></param>
        public void Save(BinaryStream stream, int blockSize, uint nodeIndex)
        {
            stream.Position = blockSize * nodeIndex;
            Save(stream);
        }
        /// <summary>
        /// Saves the node data to the underlying stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <remarks>
        /// From the current position on the stream, the node header data is written to the stream.
        /// The position after calling this function is at the end of the header
        /// </remarks>
        public void Save(BinaryStream stream)
        {
            stream.Write(Level);
            stream.Write(ChildCount);
            stream.Write(PreviousNode);
            stream.Write(NextNode);
        }
    }
}
