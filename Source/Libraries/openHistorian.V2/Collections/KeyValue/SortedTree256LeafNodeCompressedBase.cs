//******************************************************************************************************
//  SortedTree256LeafNodeCompressedBase.cs - Gbtc
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
//  11/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.KeyValue
{
    public abstract partial class SortedTree256LeafNodeCompressedBase : SortedTree256InternalNodeBase
    {
        long m_cachedNodeIndex;
        ulong m_lastKey1;
        ulong m_lastKey2;
        ulong m_lastValue1;
        ulong m_lastValue2;

        #region [ Constructors ]

        protected SortedTree256LeafNodeCompressedBase(IBinaryStream stream)
            : base(stream)
        {
            m_cachedNodeIndex = -1;
        }


        protected SortedTree256LeafNodeCompressedBase(IBinaryStream stream, int blockSize)
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

        protected override bool LeafNodeInsert(long nodeIndex, ulong key1, ulong key2, ulong value1, ulong value2)
        {
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

            bool insertAfter = true; //The default case. This will be reassigned if needing to be inserted before.
            bool skipScan = false; //If using the cached values reveals that the current key is after the end of the stream. the sequential scan can be skipped.
            //m_cachedNodeIndex = -1;
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
                    prevPosition = endOfStreamPosition;
                    curPosition = prevPosition;
                }
            }

            if (!skipScan)
            {
                while (curPosition < endOfStreamPosition)
                {
                    curKey1 = curKey1 ^ Stream.Read7BitUInt64();
                    curKey2 = curKey2 ^ Stream.Read7BitUInt64();
                    curValue1 = curValue1 ^ Stream.Read7BitUInt64();
                    curValue2 = curValue2 ^ Stream.Read7BitUInt64();
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

                int shiftDelta = Compression.Get7BitSize(curKey1 ^ key1);
                shiftDelta += Compression.Get7BitSize(curKey2 ^ key2);
                shiftDelta += Compression.Get7BitSize(curValue1 ^ value1);
                shiftDelta += Compression.Get7BitSize(curValue2 ^ value2);

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

                Stream.Position = prevPosition;

                Stream.Write7Bit(curKey1 ^ key1);
                Stream.Write7Bit(curKey2 ^ key2);
                Stream.Write7Bit(curValue1 ^ value1);
                Stream.Write7Bit(curValue2 ^ value2);

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

                    int shiftDelta = Compression.Get7BitSize(key1);
                    shiftDelta += Compression.Get7BitSize(key2);
                    shiftDelta += Compression.Get7BitSize(value1);
                    shiftDelta += Compression.Get7BitSize(value2);

                    shiftDelta += Compression.Get7BitSize(curKey1 ^ key1);
                    shiftDelta += Compression.Get7BitSize(curKey2 ^ key2);
                    shiftDelta += Compression.Get7BitSize(curValue1 ^ value1);
                    shiftDelta += Compression.Get7BitSize(curValue2 ^ value2);

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

                    Stream.Write7Bit(key1);
                    Stream.Write7Bit(key2);
                    Stream.Write7Bit(value1);
                    Stream.Write7Bit(value2);

                    Stream.Write7Bit(curKey1 ^ key1);
                    Stream.Write7Bit(curKey2 ^ key2);
                    Stream.Write7Bit(curValue1 ^ value1);
                    Stream.Write7Bit(curValue2 ^ value2);

                    header.ValidBytes += shiftDelta;
                    header.Save(Stream, BlockSize, nodeIndex);
                    return true;
                }
                else
                {
                    //if the insert is in the middle of the the stream
                    Stream.Position = prevPosition;
                    ulong prevKey1 = curKey1 ^ Stream.Read7BitUInt64();
                    ulong prevKey2 = curKey2 ^ Stream.Read7BitUInt64();
                    ulong prevValue1 = curValue1 ^ Stream.Read7BitUInt64();
                    ulong prevValue2 = curValue2 ^ Stream.Read7BitUInt64();

                    int shiftDelta = Compression.Get7BitSize(prevKey1 ^ key1);
                    shiftDelta += Compression.Get7BitSize(prevKey2 ^ key2);
                    shiftDelta += Compression.Get7BitSize(prevValue1 ^ value1);
                    shiftDelta += Compression.Get7BitSize(prevValue2 ^ value2);

                    shiftDelta += Compression.Get7BitSize(curKey1 ^ key1);
                    shiftDelta += Compression.Get7BitSize(curKey2 ^ key2);
                    shiftDelta += Compression.Get7BitSize(curValue1 ^ value1);
                    shiftDelta += Compression.Get7BitSize(curValue2 ^ value2);

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

                    Stream.Write7Bit(prevKey1 ^ key1);
                    Stream.Write7Bit(prevKey2 ^ key2);
                    Stream.Write7Bit(prevValue1 ^ value1);
                    Stream.Write7Bit(prevValue2 ^ value2);

                    Stream.Write7Bit(curKey1 ^ key1);
                    Stream.Write7Bit(curKey2 ^ key2);
                    Stream.Write7Bit(curValue1 ^ value1);
                    Stream.Write7Bit(curValue2 ^ value2);

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

            while (Stream.Position < lastPosition)
            {
                curKey1 = curKey1 ^ Stream.Read7BitUInt64();
                curKey2 = curKey2 ^ Stream.Read7BitUInt64();
                curValue1 = curValue1 ^ Stream.Read7BitUInt64();
                curValue2 = curValue2 ^ Stream.Read7BitUInt64();

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

        protected override ITreeScanner256 LeafNodeGetScanner()
        {
            return new TreeScanner(this);
        }

        #endregion

        #region [ Helper Methods ]

        void NewNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
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
            Stream.Write7Bit(key1);
            Stream.Write7Bit(key2);
            Stream.Write7Bit(value1);
            Stream.Write7Bit(value2);
            secondNodeHeader.ValidBytes = (int)(Stream.Position - secondNodeIndex * BlockSize);
            secondNodeHeader.Save(Stream, BlockSize, secondNodeIndex);

            NodeWasSplit(0, firstNodeIndex, key1, key2, secondNodeIndex);
        }

        void SplitNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
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

            while (curPosition < midPoint)
            {
                prevPosition = Stream.Position;
                curKey1 = curKey1 ^ Stream.Read7BitUInt64();
                curKey2 = curKey2 ^ Stream.Read7BitUInt64();
                curValue1 = curValue1 ^ Stream.Read7BitUInt64();
                curValue2 = curValue2 ^ Stream.Read7BitUInt64();
                curPosition = Stream.Position;
            }

            //Determine how many bytes it will take to store the new KVP since it will no longer be a delta
            int storageSize = Compression.Get7BitSize(curKey1);
            storageSize += Compression.Get7BitSize(curKey2);
            storageSize += Compression.Get7BitSize(curValue1);
            storageSize += Compression.Get7BitSize(curValue2);


            long secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = curPosition;
            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size + storageSize;

            int copyLength = (int)(endOfStreamPosition - curPosition);

            //do the copy
            Stream.Copy(sourceStartingAddress, targetStartingAddress, copyLength);

            Stream.Position = targetStartingAddress - storageSize;
            Stream.Write7Bit(curKey1);
            Stream.Write7Bit(curKey2);
            Stream.Write7Bit(curValue1);
            Stream.Write7Bit(curValue2);

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
