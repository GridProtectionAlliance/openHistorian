//******************************************************************************************************
//  SortedTree256EncodedLeafNodeBase.cs - Gbtc
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
using openHistorian.IO;
using openHistorian.IO.Unmanaged;

namespace openHistorian.Collections.KeyValue
{
    internal abstract partial class SortedTree256EncodedLeafNodeBase : SortedTree256Base
    {
        long m_cachedNodeIndex;
        ulong m_lastKey1;
        ulong m_lastKey2;
        ulong m_lastValue1;
        ulong m_lastValue2;

        #region [ Constructors ]

        protected SortedTree256EncodedLeafNodeBase(BinaryStreamBase stream1, BinaryStreamBase stream2)
            : base(stream1,stream2)
        {
            m_cachedNodeIndex = -1;
        }


        protected SortedTree256EncodedLeafNodeBase(BinaryStreamBase stream1, BinaryStreamBase stream2, int blockSize)
            : base(stream1,stream2, blockSize)
        {
            m_cachedNodeIndex = -1;
        }

        #endregion

        #region [ Methods ]

        #region [ Abstract Methods ]

        protected abstract unsafe int EncodeRecord(byte* buffer, ulong key1, ulong key2, ulong value1, ulong value2, ulong prevKey1, ulong prevKey2, ulong prevValue1, ulong prevValue2);

        protected abstract void DecodeNextRecord(ref ulong curKey1, ref ulong curKey2, ref ulong curValue1, ref ulong curValue2);

        #endregion

        #region [ Override Methods ]

        protected override void LeafNodeCreateEmptyNode(long newNodeIndex)
        {
            StreamLeaf.Position = BlockSize * newNodeIndex;
            NodeHeader.Save(StreamLeaf, NodeHeader.Size, 0, 0);
        }

        protected override unsafe bool LeafNodeInsert(long nodeIndex, ITreeScanner256 treeScanner, ref ulong key1, ref ulong key2, ref ulong value1, ref ulong value2, ref bool isValid, ref ulong maxKey, ref ulong minKey)
        {
            if (key1 < minKey)
                minKey = key1;
            if (key1 > maxKey)
                maxKey = key1;

            byte* buffer = stackalloc byte[64];
            byte* buffer2 = stackalloc byte[64];
            var header = new NodeHeader(StreamLeaf, BlockSize, nodeIndex);
            long firstPosition = nodeIndex * BlockSize + NodeHeader.Size;
            long endOfStreamPosition = nodeIndex * BlockSize + header.ValidBytes;
            int bytesRemaining = BlockSize - header.ValidBytes;
            long curPosition = firstPosition;
            long prevPosition = firstPosition;
            StreamLeaf.Position = firstPosition;

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

            bool insertAfter = true; //The default case. This will be reassigned if needing to be inserted before.
            bool skipScan = false; //If using the cached values reveals that the current key is after the end of the stream. the sequential scan can be skipped.
            //m_cachedNodeIndex = -1;
            if (m_cachedNodeIndex == nodeIndex)
            {
                int compareKeysResults = (CompareKeys(key1, key2, m_lastKey1, m_lastKey2));
                if (compareKeysResults == 0) //if keys match, result is found.
                {
                    isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
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
                    prevKey1 = curKey1;
                    prevKey2 = curKey2;
                    prevValue1 = curValue1;
                    prevValue2 = curValue2;

                    DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2);
                    curPosition = StreamLeaf.Position;

                    int compareKeysResults = CompareKeys(key1, key2, curKey1, curKey2);
                    if (compareKeysResults == 0) //if keys match, result is found.
                    {
                        isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                        return false;
                    }
                    if (compareKeysResults < 0) //if the key is greater than the test index, change the lower bounds
                    {
                        insertAfter = false;
                        break;
                    }
                    prevPosition = StreamLeaf.Position;
                }
            }

            if (insertAfter)
            {
                /////////////////////////////////////////////////////////////////////////////
                //
                //This is where the optimizations will take place 
                //to increase the speed of inserting sequential data.
                if (header.RightSiblingNodeIndex == 0)
                {
                    bool headerChanged = false;
                    while (true)
                    {
                        int shiftDelta2 = EncodeRecord(buffer, key1, key2, value1, value2, curKey1, curKey2, curValue1, curValue2);
                        if (bytesRemaining < shiftDelta2)
                        {
                            if (headerChanged)
                                header.Save(StreamLeaf, BlockSize, nodeIndex);

                            NewNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                            isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                            return true;
                        }
                        bytesRemaining -= shiftDelta2;
                        headerChanged = true;

                        StreamLeaf.Position = prevPosition;
                        WriteToStream(buffer, shiftDelta2);
                        header.ValidBytes += shiftDelta2;

                        curKey1 = key1;
                        curKey2 = key2;
                        curValue1 = value1;
                        curValue2 = value2;
                        prevPosition += shiftDelta2; 
                        
                        isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                        if (!isValid && IsLessThanOrEqualTo(key1, key2, curKey1, curKey2))
                            break;

                        minKey = Math.Min(key1, minKey);
                        maxKey = Math.Max(key1, maxKey);
                    }

                    m_cachedNodeIndex = nodeIndex;
                    m_lastKey1 = key1;
                    m_lastKey2 = key2;
                    m_lastValue1 = value1;
                    m_lastValue2 = value2;

                    header.Save(StreamLeaf, BlockSize, nodeIndex);
                    return true;
                }
                //
                /////////////////////////////////////////////////////////////////////////////

                //Insert afters only occur at the end of the stream
                int shiftDelta = EncodeRecord(buffer, key1, key2, value1, value2, curKey1, curKey2, curValue1, curValue2);
                if (bytesRemaining < shiftDelta)
                {
                    if (header.RightSiblingNodeIndex == 0)
                    {
                        NewNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                        return true;
                    }
                    else
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                        return true;
                    }
                }
                
                m_cachedNodeIndex = nodeIndex;
                m_lastKey1 = key1;
                m_lastKey2 = key2;
                m_lastValue1 = value1;
                m_lastValue2 = value2;

                StreamLeaf.Position = prevPosition;
                WriteToStream(buffer, shiftDelta);

                header.ValidBytes += shiftDelta;
                header.Save(StreamLeaf, BlockSize, nodeIndex);
                isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                return true;
            }
            else
            {
                //the insert will need to occur before the current point.
                if (prevPosition == firstPosition)
                {
                    //if the insert is at the beginning of the stream

                    int shiftDelta1 = EncodeRecord(buffer, key1, key2, value1, value2, 0, 0, 0, 0);

                    int shiftDelta2 = EncodeRecord(buffer2, curKey1, curKey2, curValue1, curValue2, key1, key2, value1, value2);

                    int shiftDelta = shiftDelta1 + shiftDelta2;
                    shiftDelta -= (int)(curPosition - prevPosition);

                    if (bytesRemaining < shiftDelta)
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                        return true;
                    }

                    StreamLeaf.Position = firstPosition;
                    if (shiftDelta < 0)
                        StreamLeaf.RemoveBytes(-shiftDelta, (int)(endOfStreamPosition - prevPosition));
                    else
                        StreamLeaf.InsertBytes(shiftDelta, (int)(endOfStreamPosition - prevPosition));

                    WriteToStream(buffer, shiftDelta1);
                    WriteToStream(buffer2, shiftDelta2);

                    header.ValidBytes += shiftDelta;
                    header.Save(StreamLeaf, BlockSize, nodeIndex);
                    isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                    return true;
                }
                else
                {
                    //if the insert is in the middle of the the stream
                    StreamLeaf.Position = curPosition;

                    int shiftDelta1 = EncodeRecord(buffer, key1, key2, value1, value2, prevKey1, prevKey2, prevValue1, prevValue2);

                    int shiftDelta2 = EncodeRecord(buffer2, curKey1, curKey2, curValue1, curValue2, key1, key2, value1, value2);

                    int shiftDelta = shiftDelta1 + shiftDelta2;

                    shiftDelta -= (int)(curPosition - prevPosition);

                    if (bytesRemaining < shiftDelta)
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                        return true;
                    }

                    StreamLeaf.Position = prevPosition;
                    if (shiftDelta < 0)
                        StreamLeaf.RemoveBytes(-shiftDelta, (int)(endOfStreamPosition - prevPosition));
                    else
                        StreamLeaf.InsertBytes(shiftDelta, (int)(endOfStreamPosition - prevPosition));

                    WriteToStream(buffer, shiftDelta1);
                    WriteToStream(buffer2, shiftDelta2);

                    header.ValidBytes += shiftDelta;
                    header.Save(StreamLeaf, BlockSize, nodeIndex);
                    isValid = treeScanner.GetNextKey(out key1, out key2, out value1, out value2);
                    return true;
                }

            }
        }

        unsafe protected override bool LeafNodeInsert(long nodeIndex, ulong key1, ulong key2, ulong value1, ulong value2)
        {
            byte* buffer = stackalloc byte[64];
            byte* buffer2 = stackalloc byte[64];
            var header = new NodeHeader(StreamLeaf, BlockSize, nodeIndex);
            long firstPosition = nodeIndex * BlockSize + NodeHeader.Size;
            long endOfStreamPosition = nodeIndex * BlockSize + header.ValidBytes;
            int bytesRemaining = BlockSize - header.ValidBytes;
            long curPosition = firstPosition;
            long prevPosition = firstPosition;
            StreamLeaf.Position = firstPosition;

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
                    prevKey1 = curKey1;
                    prevKey2 = curKey2;
                    prevValue1 = curValue1;
                    prevValue2 = curValue2;

                    DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2);
                    curPosition = StreamLeaf.Position;

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
                    prevPosition = StreamLeaf.Position;
                }
            }

            if (insertAfter)
            {
                //Insert afters only occur at the end of the stream
                int shiftDelta = EncodeRecord(buffer, key1, key2, value1, value2, curKey1, curKey2, curValue1, curValue2);
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

                StreamLeaf.Position = prevPosition;
                WriteToStream(buffer, shiftDelta);

                header.ValidBytes += shiftDelta;
                header.Save(StreamLeaf, BlockSize, nodeIndex);
                return true;
            }
            else
            {
                //the insert will need to occur before the current point.
                if (prevPosition == firstPosition)
                {
                    //if the insert is at the beginning of the stream

                    int shiftDelta1 = EncodeRecord(buffer, key1, key2, value1, value2, 0, 0, 0, 0);

                    int shiftDelta2 = EncodeRecord(buffer2, curKey1, curKey2, curValue1, curValue2, key1, key2, value1, value2);

                    int shiftDelta = shiftDelta1 + shiftDelta2;
                    shiftDelta -= (int)(curPosition - prevPosition);

                    if (bytesRemaining < shiftDelta)
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        return true;
                    }

                    StreamLeaf.Position = firstPosition;
                    if (shiftDelta < 0)
                        StreamLeaf.RemoveBytes(-shiftDelta, (int)(endOfStreamPosition - prevPosition));
                    else
                        StreamLeaf.InsertBytes(shiftDelta, (int)(endOfStreamPosition - prevPosition));

                    WriteToStream(buffer, shiftDelta1);
                    WriteToStream(buffer2, shiftDelta2);

                    header.ValidBytes += shiftDelta;
                    header.Save(StreamLeaf, BlockSize, nodeIndex);
                    return true;
                }
                else
                {
                    //if the insert is in the middle of the the stream
                    StreamLeaf.Position = curPosition;

                    int shiftDelta1 = EncodeRecord(buffer, key1, key2, value1, value2, prevKey1, prevKey2, prevValue1, prevValue2);

                    int shiftDelta2 = EncodeRecord(buffer2, curKey1, curKey2, curValue1, curValue2, key1, key2, value1, value2);

                    int shiftDelta = shiftDelta1 + shiftDelta2;

                    shiftDelta -= (int)(curPosition - prevPosition);

                    if (bytesRemaining < shiftDelta)
                    {
                        SplitNodeThenInsert(key1, key2, value1, value2, nodeIndex);
                        return true;
                    }

                    StreamLeaf.Position = prevPosition;
                    if (shiftDelta < 0)
                        StreamLeaf.RemoveBytes(-shiftDelta, (int)(endOfStreamPosition - prevPosition));
                    else
                        StreamLeaf.InsertBytes(shiftDelta, (int)(endOfStreamPosition - prevPosition));

                    WriteToStream(buffer, shiftDelta1);
                    WriteToStream(buffer2, shiftDelta2);

                    header.ValidBytes += shiftDelta;
                    header.Save(StreamLeaf, BlockSize, nodeIndex);
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
            var header = new NodeHeader(StreamLeaf, BlockSize, nodeIndex);
            StreamLeaf.Position = nodeIndex * BlockSize + NodeHeader.Size;
            long lastPosition = nodeIndex * BlockSize + header.ValidBytes;

            ulong curKey1 = 0;
            ulong curKey2 = 0;
            ulong curValue1 = 0;
            ulong curValue2 = 0;

            while (StreamLeaf.Position < lastPosition)
            {
                DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2);

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

        unsafe void NewNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
            byte* buffer = stackalloc byte[64];

            m_cachedNodeIndex = -1;
            NodeHeader firstNodeHeader = new NodeHeader(StreamLeaf, BlockSize, firstNodeIndex);
            NodeHeader secondNodeHeader = default(NodeHeader);

            //Debug code.
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
                throw new Exception();

            long secondNodeIndex = GetNextNewNodeIndex();

            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(StreamLeaf, BlockSize, firstNodeIndex);

            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;

            StreamLeaf.Position = secondNodeIndex * BlockSize + NodeHeader.Size;

            int length = EncodeRecord(buffer, key1, key2, value1, value2, 0, 0, 0, 0);
            WriteToStream(buffer, length);
            secondNodeHeader.ValidBytes = (int)(StreamLeaf.Position - secondNodeIndex * BlockSize);
            secondNodeHeader.Save(StreamLeaf, BlockSize, secondNodeIndex);

            //Prevents multiple seeks by moving up here
            NodeWasSplit(0, firstNodeIndex, key1, key2, secondNodeIndex);

        }
        
        unsafe void WriteToStream(byte* buffer, int length)
        {
            int pos = 0;
            while (pos + 8 <= length)
            {
                StreamLeaf.Write(*(long*)(buffer + pos));
                pos += 8;
            }
            if (pos + 4 <= length)
            {
                StreamLeaf.Write(*(int*)(buffer + pos));
                pos += 4;
            }
            if (pos + 2 <= length)
            {
                StreamLeaf.Write(*(short*)(buffer + pos));
                pos += 2;
            }
            if (pos + 1 <= length)
            {
                StreamLeaf.Write(*(buffer + pos));
            }
        }

        unsafe void SplitNodeThenInsert(ulong key1, ulong key2, ulong value1, ulong value2, long firstNodeIndex)
        {
            byte* buffer = stackalloc byte[64];

            m_cachedNodeIndex = -1;
            NodeHeader firstNodeHeader = new NodeHeader(StreamLeaf, BlockSize, firstNodeIndex);
            NodeHeader secondNodeHeader = default(NodeHeader);

            long midPoint = firstNodeIndex * BlockSize + (BlockSize >> 1);
            long firstPosition = firstNodeIndex * BlockSize + NodeHeader.Size;
            long endOfStreamPosition = firstNodeIndex * BlockSize + firstNodeHeader.ValidBytes;
            long curPosition = firstPosition;
            long prevPosition = firstPosition;
            StreamLeaf.Position = firstPosition;

            //navigate to the approximate midpoint.
            ulong curKey1 = 0;
            ulong curKey2 = 0;
            ulong curValue1 = 0;
            ulong curValue2 = 0;

            while (curPosition < midPoint)
            {
                prevPosition = StreamLeaf.Position;
                DecodeNextRecord(ref curKey1, ref curKey2, ref curValue1, ref curValue2);
                curPosition = StreamLeaf.Position;
            }

            //Determine how many bytes it will take to store the new KVP since it will no longer be a delta
            int storageSize = EncodeRecord(buffer, curKey1, curKey2, curValue1, curValue2, 0, 0, 0, 0);

            long secondNodeIndex = GetNextNewNodeIndex();
            long sourceStartingAddress = curPosition;
            long targetStartingAddress = secondNodeIndex * BlockSize + NodeHeader.Size + storageSize;

            int copyLength = (int)(endOfStreamPosition - curPosition);

            //do the copy
            StreamLeaf.Copy(sourceStartingAddress, targetStartingAddress, copyLength);

            StreamLeaf.Position = targetStartingAddress - storageSize;
            WriteToStream(buffer, storageSize);

            //update the node that was the old right sibling
            if (firstNodeHeader.RightSiblingNodeIndex != 0)
            {
                NodeHeader oldRightSibling = new NodeHeader(StreamLeaf, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
                oldRightSibling.LeftSiblingNodeIndex = secondNodeIndex;
                oldRightSibling.Save(StreamLeaf, BlockSize, firstNodeHeader.RightSiblingNodeIndex);
            }

            //update the second header
            secondNodeHeader.ValidBytes = copyLength + storageSize + NodeHeader.Size;
            secondNodeHeader.LeftSiblingNodeIndex = firstNodeIndex;
            secondNodeHeader.RightSiblingNodeIndex = firstNodeHeader.RightSiblingNodeIndex;
            secondNodeHeader.Save(StreamLeaf, BlockSize, secondNodeIndex);

            //update the first header
            firstNodeHeader.ValidBytes = (int)(prevPosition - firstNodeIndex * BlockSize);
            firstNodeHeader.RightSiblingNodeIndex = secondNodeIndex;
            firstNodeHeader.Save(StreamLeaf, BlockSize, firstNodeIndex);

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
