//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Historian.StorageSystem.File;
//using System.IO;

//namespace Historian.StorageSystem.BlockSorter
//{
//    class Node
//    {
//        #region [ Members ]

//        const int NodeHeader = 3;
//        const int InternalAddressStructure = 20;
//        const int LeafAddressStructure = 24;

//        BinaryStream m_stream;
//        long m_NextUnallocatedByte;
//        long m_RootPosition;
//        int m_BlockSize;
//        int m_maxElementsPerLeafNode;
//        int m_maxElementsPerInternalNode;

//        #endregion

//        #region [ Constructors ]

//        private Node()
//        {
//        }
//        void OpenInternal(BinaryStream stream)
//        {
//            m_stream = stream;
//            m_stream.Position = 0;
//            if (s_FileType != stream.ReadGuid())
//                throw new Exception("Header Corrupt");
//            if (stream.ReadByte() != 0)
//                throw new Exception("Header Corrupt");
//            m_NextUnallocatedByte = stream.ReadInt64();
//            m_BlockSize = stream.ReadInt32();
//            m_RootPosition = stream.ReadUInt32() * m_BlockSize;
//            m_maxElementsPerLeafNode = (m_BlockSize - NodeHeader) / LeafAddressStructure;
//            m_maxElementsPerInternalNode = (m_BlockSize - NodeHeader - 4) / InternalAddressStructure;
//        }
//        void CreateInternal(BinaryStream stream, int blockSize)
//        {
//            stream.Position = 0;
//            stream.Write(s_FileType);
//            stream.Write((byte)0);
//            stream.Write(blockSize * 2L); //Next Unallocated Byte
//            stream.Write(blockSize);
//            stream.Write(1); //Root Index
//            stream.Position = blockSize;
//            stream.Write(true);//IsLeaf
//            stream.Write((short)0);
//            OpenInternal(stream);
//        }

//        #endregion

//        #region [ Properties ]

//        #endregion

//        #region [ Methods ]

//        public void RemoveItem(IBlockKey key)
//        {
//            throw new NotSupportedException();
//        }

//        public void SetData(IBlockKey key, byte[] data)
//        {
//            throw new NotSupportedException();
//        }

//        public void AddData(IBlockKey key, byte[] data)
//        {
//            long dataToWrite = WriteDataBucket(data);
//            long key1, key2;
//            key.GetKey(out key1, out key2);
//            SplitDetails split = AddItem(m_RootPosition, key, data);
//            if (split.IsSplit)
//            {
//                uint position = CreateEmptyTreeNode(false, 1);
//                m_stream.Position = position * (long)m_BlockSize + 3;
//                m_stream.Write((uint)(m_RootPosition / m_BlockSize));
//                m_stream.Write(split.Key1, split.Key2);
//                m_stream.Write(split.NodeAddress);
//                m_RootPosition = position * (long)m_BlockSize;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="key"></param>
//        /// <param name="data"></param>
//        SplitDetails AddItem(long position, IBlockKey key, byte[] data)
//        {
//            m_stream.Position = position;
//            bool isLeaf = m_stream.ReadBoolean();
//            ushort indexCount = m_stream.ReadUInt16();
//            long key1, key2;
//            key.GetKey(out key1, out key2);

//            int index;
//            bool isExactMatch;
//            long nextAddress;

//            if (isLeaf)
//            {
//                IndexOfKeyLeafNode(position, key1, key2, indexCount, out isExactMatch, out index, out nextAddress);
//                if (isExactMatch)
//                {
//                    throw new Exception("The index already exists in the database");
//                }
//                else
//                {
                    
//                    if (indexCount < m_maxElementsPerLeafNode)
//                    {
//                        m_stream.Position = position + index * LeafAddressStructure + NodeHeader;
//                        int bytesToMove = indexCount * LeafAddressStructure - index * LeafAddressStructure;
//                        m_stream.InsertBytes(LeafAddressStructure, bytesToMove);
//                        m_stream.Write(key1, key2);

//                        m_stream.Write();
//                        indexCount++;
//                        m_stream.Position = position + 1;
//                        m_stream.Write(indexCount);
//                        return default(SplitDetails);
//                    }
//                    else
//                    {
//                        SplitDetails split = SplitFullLeafNode(position, indexCount);

//                        //Determine where to add the current key.
//                        if (key1 > split.Key1 || (key1 == split.Key1 && key2 >= split.Key2))
//                        {
//                            //The proper bucket is the second one.
//                            AddItem(split.NodeAddress * (long)m_BlockSize, key, data);
//                        }
//                        else
//                        {
//                            //The proper bucket is the current one.
//                            AddItem(position, key, data);
//                        }
//                        return split;
//                    }
//                }
//            }
//            else //If an internal node or root node.
//            {
//                IndexOfKeyInternalNode(position, key1, key2, indexCount, out isExactMatch, out index, out nextAddress);
//                SplitDetails split = AddItem(nextAddress, key, data);
//                if (split.IsSplit)
//                {

//                    throw new NotSupportedException();
//                }
//                return default(SplitDetails);
//            }
//        }

//        SplitDetails SplitFullLeafNode(long position, ushort indexCount)
//        {
//            ushort startingIndexToCopy = (ushort)(indexCount >> 1); // divide by 2.
//            ushort entriesToCopy = (ushort)(indexCount - startingIndexToCopy);
//            long sourceAddress = position + NodeHeader + LeafAddressStructure * startingIndexToCopy; //navigate to this index

//            //populate the return values
//            SplitDetails split;
//            split.IsSplit = true;
//            split.NodeAddress = CreateEmptyTreeNode(true, entriesToCopy);
//            m_stream.Position = sourceAddress;
//            m_stream.ReadInt128(out split.Key1, out split.Key2);

//            //Copy half of the nodes to the other leaf.
//            m_stream.Copy(sourceAddress, split.NodeAddress * (long)m_BlockSize + 3, entriesToCopy * LeafAddressStructure);

//            //overwrite the indexCount
//            m_stream.Position = position + 1;
//            m_stream.Write((short)startingIndexToCopy);
//            return split;
//        }
       
//        SplitDetails SplitFullInternalNode(long position, ushort indexCount)
//        {
//            throw new NotSupportedException();
//            ushort startingIndexToCopy = (ushort)(indexCount >> 1); // divide by 2.
//            ushort entriesToCopy = (ushort)(indexCount - startingIndexToCopy);
//            long sourceAddress = position + NodeHeader + LeafAddressStructure * startingIndexToCopy; //navigate to this index

//            //populate the return values
//            SplitDetails split;
//            split.IsSplit = true;
//            split.NodeAddress = CreateEmptyTreeNode(true, entriesToCopy);
//            m_stream.Position = sourceAddress;
//            m_stream.ReadInt128(out split.Key1, out split.Key2);

//            //Copy half of the nodes to the other leaf.
//            m_stream.Copy(sourceAddress, split.NodeAddress * (long)m_BlockSize + 3, entriesToCopy * LeafAddressStructure);

//            //overwrite the indexCount
//            m_stream.Position = position + 1;
//            m_stream.Write((short)startingIndexToCopy);
//            return split;
//        }

//        uint CreateEmptyTreeNode(bool isLeaf, ushort nodeCount)
//        {
//            long origionalPosition = m_stream.Position;
//            //round the next block to the nearest boundry.
//            long byteFragment = m_NextUnallocatedByte % m_BlockSize;
//            if (byteFragment != 0)
//                m_NextUnallocatedByte += m_BlockSize - byteFragment;

//            long startingPosition = m_NextUnallocatedByte;
//            m_stream.Position = startingPosition;
//            m_NextUnallocatedByte += m_BlockSize;
//            m_stream.Write(isLeaf);
//            m_stream.Write(nodeCount);
//            m_stream.Position = origionalPosition;
//            return (uint)(startingPosition / m_BlockSize);
//        }

//        /// <summary>
//        /// Returns the data for the following key. Null if the key does not exist.
//        /// </summary>
//        /// <param name="key">The key to look up.</param>
//        /// <returns></returns>
//        public byte[] GetData(IBlockKey key)
//        {
//            return GetKey(m_RootPosition, key);
//        }
//        /// <summary>
//        /// Returns the byte array for the data requested with the following key.  
//        /// </summary>
//        /// <param name="position">the offset for the start of the node.</param>
//        /// <param name="key">the key to match.</param>
//        /// <returns>null if the key could not be found</returns>
//        /// <remarks>this function is designed to be recursive and will continue calling itself until the data is reached.</remarks>
//        byte[] GetKey(long position, IBlockKey key)
//        {
//            m_stream.Position = position;
//            bool isLeaf = m_stream.ReadBoolean();
//            ushort indexCount = m_stream.ReadUInt16();
//            long key1, key2;
//            key.GetKey(out key1, out key2);

//            int index;
//            bool isExactMatch;
//            long nextAddress;

//            if (isLeaf)
//            {
//                IndexOfKeyLeafNode(position, key1, key2, indexCount, out isExactMatch, out index, out nextAddress);
//                if (isExactMatch)
//                {
//                    //If there are not any more nested keys, the link given is the final address.
//                    return ReadDataBucket(nextAddress);
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            else //If an internal node or root node.
//            {
//                IndexOfKeyInternalNode(position, key1, key2, indexCount, out isExactMatch, out index, out nextAddress);
//                return GetKey(nextAddress, key);
//            }
//        }

//        /// <summary>
//        /// Seeks the current B+Tree node for the position where this key belongs.
//        /// </summary>
//        /// <param name="position">The position of the start of the node.</param>
//        /// <param name="key1">The key value</param>
//        /// <param name="key2">The key value</param>
//        /// <param name="indexCount">The number of indexes in this node.</param>
//        /// <param name="isExactMatch">An output parameter describing if this is an exact match or not. If this is false, the index position will point to the 
//        /// location where the new data should be inserted.</param>
//        /// <param name="index">The index position of the match.  If isExactMatch is false, A 0 is before the beginning and an index equal to indexCount means after the end.
//        /// In the case of an empty array (ie: indexCount=0) index will return 0 meaning after the end.</param>
//        /// <param name="nextAddress">Returns the address for the next node or data. Value is -1 if not an exact match.</param>
//        /// <remarks>After returning, the stream position will be in a arbitrary location. 
//        /// It is recommended to calculate the desired position from the index information returned
//        /// from this function.</remarks>
//        void IndexOfKeyLeafNode(long position, long key1, long key2, ushort indexCount, out bool isExactMatch, out int index, out long nextAddress)
//        {
//            long tmpKey1, tmpKey2;
//            m_stream.Position = position + NodeHeader;
//            for (int x = 0; x < indexCount; x++)
//            {
//                m_stream.ReadInt128(out tmpKey1, out tmpKey2);
//                if (tmpKey1 > key1 || (tmpKey1 == key1 && tmpKey2 > key2))
//                {
//                    isExactMatch = false;
//                    index = x;
//                    nextAddress = -1;
//                    return;
//                }
//                else if (tmpKey1 == key1 && tmpKey2 == key2)
//                {
//                    index = x;
//                    isExactMatch = true;
//                    nextAddress = m_stream.ReadInt64();
//                    return;
//                }
//                else
//                {
//                    //Skip the address
//                    m_stream.Position += sizeof(long);
//                }
//            }
//            isExactMatch = false;
//            index = indexCount;
//            nextAddress = -1;
//        }
//        /// <summary>
//        /// Seeks the current B+Tree node for the position where this key belongs.
//        /// </summary>
//        /// <param name="position">The position of the start of the node.</param>
//        /// <param name="key1">The key value</param>
//        /// <param name="key2">The key value</param>
//        /// <param name="indexCount">The number of indexes in this node.</param>
//        /// <param name="isExactMatch">An output parameter describing if this is an exact match or not. If this is false, the index position will point to the 
//        /// location where the new data should be inserted.</param>
//        /// <param name="index">The index position of the match.  If isExactMatch is false, A 0 is before the beginning and 
//        /// an index equal to indexCount means after the end. </param>
//        /// <param name="nextAddress">Returns the address for the appropriate child node. 
//        /// By definition, this value will never be invalid since internal nodes always have children.</param>
//        /// <remarks>After returning, the stream position will be in a arbitrary location. 
//        /// It is recommended to calculate the desired position from the index information returned
//        /// from this function.</remarks>
//        void IndexOfKeyInternalNode(long position, long key1, long key2, ushort indexCount, out bool isExactMatch, out int index, out long nextAddress)
//        {
//            long tmpKey1, tmpKey2;
//            m_stream.Position = position + NodeHeader + 4;
//            for (int x = 0; x < indexCount; x++)
//            {
//                m_stream.ReadInt128(out tmpKey1, out tmpKey2);
//                //if the key for the current index is greater than the search key then exit
//                if (tmpKey1 > key1 || (tmpKey1 == key1 && tmpKey2 > key2))
//                {
//                    isExactMatch = false;
//                    index = x;
//                    m_stream.Position -= sizeof(long) + sizeof(long) + 4;
//                    nextAddress = m_stream.ReadUInt32() * (long)m_BlockSize;
//                    return;
//                }
//                else if (tmpKey1 == key1 && tmpKey2 == key2)
//                {
//                    index = x;
//                    isExactMatch = true;
//                    nextAddress = m_stream.ReadUInt32() * (long)m_BlockSize;
//                    return;
//                }
//                else
//                {
//                    //Skip the meta data and address if the key does not match.
//                    m_stream.Position += sizeof(uint);
//                }
//            }
//            m_stream.Position -= sizeof(uint);
//            isExactMatch = false;
//            index = indexCount;
//            nextAddress = m_stream.ReadUInt32() * (long)m_BlockSize;
//        }

//        #region [ Read/Write DataBucket ]

//        /// <summary>
//        /// Reads the data from the byte array block that starts at the given address.
//        /// </summary>
//        /// <param name="address">The absolute position of the start of the byte array block.</param>
//        /// <returns></returns>
//        byte[] ReadDataBucket(long address)
//        {
//            long oldPosition = m_stream.Position;
//            m_stream.Position = address;
//            int length = (int)m_stream.Read7BitUInt32();
//            var data = new byte[length];
//            m_stream.Read(data, 0, length);
//            m_stream.Position = oldPosition;
//            return data;
//        }

//        /// <summary>
//        /// Writes the following data to the stream and returns the address of the data block.
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        long WriteDataBucket(byte[] data)
//        {
//            long oldPosition = m_stream.Position;
//            long starting = m_NextUnallocatedByte;
//            m_NextUnallocatedByte += data.Length + 4;
//            m_stream.Position = starting;
//            m_stream.Write7Bit((uint)data.Length);
//            m_stream.Write(data, 0, data.Length);
//            m_stream.Position = oldPosition;
//            return starting;
//        }

//        #endregion

//        #endregion

//        #region [ Operators ]

//        #endregion

//        #region [ Static ]

//        static Guid s_FileType = new Guid("{7bfa9083-701e-4596-8273-8680a739271d}");

//        public static Node Open(BinaryStream stream)
//        {
//            Node node = new Node();
//            node.OpenInternal(stream);
//            return node;
//        }

//        public static Node Create(BinaryStream stream, int blockSize)
//        {
//            Node node = new Node();
//            node.CreateInternal(stream, blockSize);
//            return node;
//        }

//        #endregion

//    }
//}
