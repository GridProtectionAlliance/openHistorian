//******************************************************************************************************
//  SortedTree256LeafNodeCodedBase.cs - Gbtc
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
//  11/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.KeyValue
{
    public abstract partial class SortedTree256LeafNodeCodedBase : SortedTree256InternalNodeBase
    {
        long m_cachedNodeIndex;
        ulong m_lastKey1;
        ulong m_lastKey2;
        ulong m_lastValue1;
        ulong m_lastValue2;
        ulong m_lastPrevValue2;

        #region [ Constructors ]

        protected SortedTree256LeafNodeCodedBase(IBinaryStream stream)
            : base(stream)
        {
            m_cachedNodeIndex = -1;
        }


        protected SortedTree256LeafNodeCodedBase(IBinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
            m_cachedNodeIndex = -1;
        }

        #endregion

        #region [ Methods ]

        #region [ Override Methods ]

        protected override void LeafNodeCreateEmptyNode(long newNodeIndex)
        {
            Stream.Position = BlockSize * newNodeIndex;
            NodeHeader.Save(Stream, NodeHeader.Size, 0, 0);
        }

        unsafe protected override bool LeafNodeInsert(long nodeIndex, ulong key1, ulong key2, ulong value1, ulong value2)
        {
            byte* buffer = stackalloc byte[64];
            byte* buffer2 = stackalloc byte[64];
            var header = new NodeHeader(Stream, BlockSize, nodeIndex);
            long firstPosition = nodeIndex * BlockSize + NodeHeader.Size;
            long endOfStreamPosition = nodeIndex * BlockSize + header.ValidBytes;
            int bytesRemaining = BlockSize - header.ValidBytes;
            long curPosition = firstPosition;
            long prevPosition = firstPosition;
            Stream.Position = firstPosition;

            //Find the best location to insert
            //This is done before checking if a split is required to prevent splitting 
            //if a duplicate key is found
            ulong curKey1 = 0;
            ulong curKey2 = 0;
            ulong curValue1 = 0;
            ulong curValue2 = 0;
            ulong prevKey1 = 0;
            ulong prevKey2 = 0;
            ulong prevValue1 = 0;
            ulong prevValue2 = 0;
            ulong oldPrevValue2 = 0;

            bool insertAfter = true; //The default case. This will be reassigned if needing to be inserted before.
            bool skipScan = false; //If using the cached values reveals that the current key is after the end of the stream. the sequential scan can be skipped.
            m_cachedNodeIndex = -1;
            if (m_cachedNodeIndex == nodeIndex)
            {
                int compareKeysResults = (CompareKeys(key1, key2, m_lastKey1, m_lastKey2));
                if (compareKeysResults == 0) //if keys match, result is found.
                {
                    return false;
                }
                if (compareKeysResults > 0) //if the key is greater than the test index, the insert will occur at the end of the stream.
                {
                    skipScan = true;
                    curKey1 = m_lastKey1;
                    curKey2 = m_lastKey2;
                    curValue1 = m_lastValue1;
                    curValue2 = m_lastValue2;
                    prevValue2 = m_lastPrevValue2;
                    prevPosition = endOfStreamPosition;
                    curPosition = prevPosition;
                }
            }

            if (!skipScan)
            {
                while (curPosition < endOfStreamPosition)
                {
                    prevKey1 = curKey1;
                    prevKey2 = curKey2;
                    prevValue1 = curValue1;
                    oldPrevValue2 = prevValue2;
                    prevValue2 = curValue2;

                    DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2, ref prevValue2);
                    curPosition = Stream.Position;

                    int compareKeysResults = CompareKeys(key1, key2, curKey1, curKey2);
                    if (compareKeysResults == 0) //if keys match, result is found.
                    {
                        return false;
                    }
                    if (compareKeysResults < 0) //if the key is greater than the test index, change the lower bounds
                    {
                        insertAfter = false;
                        break;
                    }
                    prevPosition = Stream.Position;
                }
            }

            if (insertAfter)
            {
                //Insert afters only occur at the end of the stream
                int shiftDelta = EncodeRecord(buffer, key1, key2, value1, value2, curKey1, curKey2, curValue1, curValue2, prevValue2);
                if (bytesRemaining < shiftDelta)
                {
                    if (header.RightSiblingNodeIndex == 0)
                    {
                        NewNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        return true;
                    }
                    else
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        return true;
                    }
                }

                m_cachedNodeIndex = nodeIndex;
                m_lastKey1 = key1;
                m_lastKey2 = key2;
                m_lastValue1 = value1;
                m_lastValue2 = value2;
                m_lastPrevValue2 = curValue2;

                Stream.Position = prevPosition;
                WriteToStream(buffer, shiftDelta);

                header.ValidBytes += shiftDelta;
                header.Save(Stream, BlockSize, nodeIndex);
                return true;
            }
            else
            {
                //the insert will need to occur before the current point.
                if (prevPosition == firstPosition)
                {
                    //if the insert is at the beginning of the stream

                    int shiftDelta1 = EncodeRecord(buffer, key1, key2, value1, value2, 0, 0, 0, 0, 0);

                    int shiftDelta2 = EncodeRecord(buffer2, curKey1, curKey2, curValue1, curValue2, key1, key2, value1, value2, 0);

                    int shiftDelta = shiftDelta1 + shiftDelta2;
                    shiftDelta -= (int)(curPosition - prevPosition);

                    if (bytesRemaining < shiftDelta)
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        return true;
                    }

                    Stream.Position = firstPosition;
                    if (shiftDelta < 0)
                        Stream.RemoveBytes(-shiftDelta, (int)(endOfStreamPosition - prevPosition));
                    else
                        Stream.InsertBytes(shiftDelta, (int)(endOfStreamPosition - prevPosition));

                    WriteToStream(buffer, shiftDelta1);
                    WriteToStream(buffer2, shiftDelta2);

                    header.ValidBytes += shiftDelta;
                    header.Save(Stream, BlockSize, nodeIndex);
                    return true;
                }
                else
                {
                    //if the insert is in the middle of the the stream
                    Stream.Position = curPosition;

                    int shiftDelta1 = EncodeRecord(buffer, key1, key2, value1, value2, prevKey1, prevKey2, prevValue1, prevValue2, oldPrevValue2);

                    int shiftDelta2 = EncodeRecord(buffer2, curKey1, curKey2, curValue1, curValue2, key1, key2, value1, value2, prevValue2);

                    int shiftDelta = shiftDelta1 + shiftDelta2;

                    shiftDelta -= (int)(curPosition - prevPosition);

                    if (bytesRemaining < shiftDelta)
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        return true;
                    }

                    Stream.Position = prevPosition;
                    if (shiftDelta < 0)
                        Stream.RemoveBytes(-shiftDelta, (int)(endOfStreamPosition - prevPosition));
                    else
                        Stream.InsertBytes(shiftDelta, (int)(endOfStreamPosition - prevPosition));

                    WriteToStream(buffer, shiftDelta1);
                    WriteToStream(buffer2, shiftDelta2);

                    header.ValidBytes += shiftDelta;
                    header.Save(Stream, BlockSize, nodeIndex);
                    return true;
                }

            }
        }

        /// <summary>
        /// Outputs the value associated with the provided key in the given node.
        /// </summary>
        /// <param name="nodeIndex">the node to search</param>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns>true if the key was found, false if the key was not found.</returns>
        protected override bool LeafNodeGetValue(long nodeIndex, ulong key1, ulong key2, out ulong value1, out ulong value2)
        {
            var header = new NodeHeader(Stream, BlockSize, nodeIndex);
            Stream.Position = nodeIndex * BlockSize + NodeHeader.Size;
            long lastPosition = nodeIndex * BlockSize + header.ValidBytes;

            ulong curKey1 = 0;
            ulong curKey2 = 0;
            ulong curValue1 = 0;
            ulong curValue2 = 0;
            ulong prevValue2 = 0;

            while (Stream.Position < lastPosition)
            {
                DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2, ref prevValue2);

                int compareKeysResults = CompareKeys(key1, key2, curKey1, curKey2);
                if (compareKeysResults == 0) //if keys match, result is found.
                {
                    value1 = curValue1;
                    value2 = curValue2;
                    return true;
                }
                if (compareKeysResults < 0) //if passed the key, no results exists
                {
                    value1 = 0;
                    value2 = 0;
                    return false;
                }
            }
            value1 = 0;
            value2 = 0;
            return false;
        }

        unsafe int EncodeRecord(byte* buffer, ulong key1, ulong key2, ulong value1, ulong value2, ulong prevKey1, ulong prevKey2, ulong prevValue1, ulong prevValue2, ulong oldPrevValue2)
        {
            byte code = 0;
            int size = 1; //1 is the code prefix.

            //Code key1, and key2
            if (prevKey1 != key1)
            {
                code |= 3 << 6;
                Compression.Write7Bit(buffer, ref size, key1 - prevKey1);
                Compression.Write7Bit(buffer, ref size, key2);
            }
            else if (key2 == prevKey2 + 1)
            {
                code |= 0 << 6;
            }
            else if (key2 == prevKey2 + 2)
            {
                code |= 1 << 6;
            }
            else
            {
                code |= 2 << 6;
                Compression.Write7Bit(buffer, ref size, key2 - prevKey2);
            }

            //Code the quality (value1)
            if (value1 == 0)
            {
                code |= 0 << 4;
            }
            else if (value1 == prevValue1)
            {
                code |= 1 << 4;
            }
            else if (value1 < (value1 ^ prevValue1))
            {
                code |= 2 << 4;
                Compression.Write7Bit(buffer, ref size, value1);
            }
            else
            {
                code |= 3 << 4;
                Compression.Write7Bit(buffer, ref size, value1 ^ prevValue1);
            }

            Compression.Write7Bit(buffer, ref size, value2 ^ prevValue2);
            buffer[0] = code;
            return size;

            //Code the value (value2)
            code |= 11;
            Compression.Write7Bit(buffer, ref size, value2);
            buffer[0] = code;
            return size;
        }

        void DecodeNextRecord(ref ulong curKey1, ref ulong curKey2, ref ulong curValue1, ref ulong curValue2, ref ulong prevValue2)
        {
            ulong tmpValue;
            byte code = Stream.ReadByte();

            switch (code >> 6)
            {
                case 0:
                    curKey2++;
                    break;
                case 1:
                    curKey2 += 2;
                    break;
                case 2:
                    curKey2 += Stream.Read7BitUInt64();
                    break;
                case 3:
                    curKey1 += Stream.Read7BitUInt64();
                    curKey2 = Stream.Read7BitUInt64();
                    break;
            }
           
            //The quality bit
            switch ((code >> 4) & 3)
            {
                case 0:
                    curValue1 = 0;
                    break;
                case 1:
                    break;
                case 2:
                    curValue1 = Stream.Read7BitUInt64();
                    break;
                case 3:
                    curValue1 = curValue1 ^ Stream.Read7BitUInt64();
                    break;
            }

            curValue2 = curValue2 ^ Stream.Read7BitUInt64();
            return;
            //The actual value
            switch (code & 15)
            {
                case 0:
                    break;
                case 1:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadByte();
                    break;
                case 2:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt16();
                    break;
                case 3:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt24();
                    break;
                case 4:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt32();
                    break;
                case 5:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt40();
                    break;
                case 6:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt48();
                    break;
                case 7:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt56();
                    break;
                case 8:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 ^ Stream.ReadUInt64();
                    break;
                case 9:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 - Stream.Read7BitUInt64();
                    break;
                case 10:
                    prevValue2 = curValue2;
                    curValue2 = curValue2 + Stream.Read7BitUInt64();
                    break;
                case 11:
                    prevValue2 = curValue2;
                    curValue2 = Stream.Read7BitUInt64();
                    break;
                case 12:
                    prevValue2 = curValue2;
                    curValue2 = 0;
                    break;
                case 13:
                    //assuming the previous difference occured again, backtrack this about (7-bit)
                    tmpValue = curValue2;
                    curValue2 = curValue2 + (curValue2 - prevValue2) - Stream.Read7BitUInt64();
                    prevValue2 = tmpValue;
                    break;
                case 14:
                    //assuming the previous difference occured twice, backtrack this about (7-bit)
                    tmpValue = curValue2;
                    curValue2 = curValue2 + (curValue2 - prevValue2) + (curValue2 - prevValue2) - Stream.Read7BitUInt64();
                    prevValue2 = tmpValue;
                    break;
                case 15:
                    //assuming the previous difference occured three times, backtrack this about (7-bit)
                    tmpValue = curValue2;
                    curValue2 = curValue2 + (curValue2 - prevValue2) + (curValue2 - prevValue2) + (curValue2 - prevValue2) - Stream.Read7BitUInt64();
                    prevValue2 = tmpValue;
                    break;
            }
        }

        protected override ITreeScanner256 LeafNodeGetScanner()
        {
            return new TreeScanner(this);
        }

        #endregion

        #region [ Helper Methods ]

        unsafe void NewNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
            byte* buffer = stackalloc byte[64];

            m_cachedNodeIndex = -1;
            NodeHeader firstNodeHeader = new NodeHeader(Stream, BlockSize, firstNodeIndex);
            NodeHeader secondNodeHeader = default(NodeHeader);

            //Debug code.
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
                throw new Exception();

            long secondNodeIndex = GetNextNewNodeIndex();

            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(Stream, BlockSize, firstNodeIndex);

            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;

            Stream.Position = secondNodeIndex * BlockSize + NodeHeader.Size;

            int length = EncodeRecord(buffer, key1, key2, value1, value2, 0, 0, 0, 0, 0);
            WriteToStream(buffer, length);
            secondNodeHeader.ValidBytes = (int)(Stream.Position - secondNodeIndex * BlockSize);
            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

            NodeWasSplit(0, firstNodeIndex, key1, key2, secondNodeIndex);
        }

        unsafe void WriteToStream(byte* buffer, int length)
        {
            int pos = 0;
            while (pos + 8 <= length)
            {
                Stream.Write(*(long*)(buffer + pos));
                pos += 8;
            }
            if (pos + 4 <= length)
            {
                Stream.Write(*(int*)(buffer + pos));
                pos += 4;
            }
            if (pos + 2 <= length)
            {
                Stream.Write(*(short*)(buffer + pos));
                pos += 2;
            }
            if (pos + 1 <= length)
            {
                Stream.Write(*(buffer + pos));
            }
        }

        unsafe void SplitNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
            byte* buffer = stackalloc byte[64];

            m_cachedNodeIndex = -1;
            NodeHeader firstNodeHeader = new NodeHeader(Stream, BlockSize, firstNodeIndex);
            NodeHeader secondNodeHeader = default(NodeHeader);

            long midPoint = firstNodeIndex * BlockSize + (BlockSize >> 1);
            long firstPosition = firstNodeIndex * BlockSize + NodeHeader.Size;
            long endOfStreamPosition = firstNodeIndex * BlockSize + firstNodeHeader.ValidBytes;
            long curPosition = firstPosition;
            long prevPosition = firstPosition;
            Stream.Position = firstPosition;

            //navigate to the approximate midpoint.
            ulong curKey1 = 0;
            ulong curKey2 = 0;
            ulong curValue1 = 0;
            ulong curValue2 = 0;
            ulong prevValue2 = 0;

            while (curPosition < midPoint)
            {
                prevPosition = Stream.Position;
                DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2, ref prevValue2);
                curPosition = Stream.Position;
            }

            //Determine how many bytes it will take to store the new KVP since it will no longer be a delta
            int storageSize = EncodeRecord(buffer, curKey1, curKey2, curValue1, curValue2, 0, 0, 0, 0, 0);

            long secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = curPosition;
            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size + storageSize;

            int copyLength = (int)(endOfStreamPosition - curPosition);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, copyLength);

            Stream.Position = targetStartingAddress - storageSize;
            WriteToStream(buffer, storageSize);

            //update the node that was the old right sibling
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
            {
                NodeHeader oldRightSibling = new NodeHeader(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
                oldRightSibling.Save(Stream, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
            }

            //update the second header
            secondNodeHeader.ValidBytes = copyLength + storageSize + NodeHeader.Size;
            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;
            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

            //update the first header
            firstNodeHeader.ValidBytes = (int)(prevPosition - firstNodeIndex * BlockSize);
            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(Stream, BlockSize, firstNodeIndex);

            NodeWasSplit(0, firstNodeIndex, curKey1, curKey2, secondNodeIndex);
            if (CompareKeys(key1, key2, curKey1, curKey2) > 0)
            {
                LeafNodeInsert(secondNodeIndex, key1, key2, value1, value2);
            }
            else
            {
                LeafNodeInsert(firstNodeIndex, key1, key2, value1, value2);
            }
        }

        #endregion

        #endregion


    }
}
