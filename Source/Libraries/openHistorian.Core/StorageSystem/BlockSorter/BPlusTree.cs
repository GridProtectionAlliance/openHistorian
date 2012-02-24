using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.Core.StorageSystem.File;
using System.IO;

namespace openHistorian.Core.StorageSystem.BlockSorter
{
    class BPlusTree
    {
        #region [ Members ]

        TreeHeader m_header;

        #endregion

        #region [ Constructors ]

        private BPlusTree()
        {
        }
        void OpenInternal(BinaryStream stream)
        {
            m_header = new TreeHeader(stream);
        }
        void CreateInternal(BinaryStream stream, int blockSize)
        {
            m_header = new TreeHeader(stream, blockSize);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public void AddData(IBlockKey8 key, byte[] data)
        {
            uint indexAddress = m_header.RootIndexAddress;
            byte indexLevel = m_header.RootIndexLevel;
            long dataAddress = BPlusTreeDataBucket.Write(m_header, data);
            long key1;
            key.GetKey(out key1);
            BPlusTreeAdd.AddItem(m_header, key1, dataAddress, ref indexAddress, ref indexLevel);
            if (indexAddress != m_header.RootIndexAddress || indexLevel != m_header.RootIndexLevel)
                m_header.SetRootIndex(indexAddress,indexLevel);
        }
        //public void RemoveItem(IBlockKey8 key)
        //{
        //    throw new NotSupportedException();
        //}

        //public void SetData(IBlockKey8 key, byte[] data)
        //{
        //    throw new NotSupportedException();
        //}

        /// <summary>
        /// Returns the data for the following key. Null if the key does not exist.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns></returns>
        public byte[] GetData(IBlockKey8 key)
        {
            long key1;
            key.GetKey(out key1);
            long dataAddress = BPlusTreeGet.GetKey(m_header, m_header.RootIndexAddress, m_header.RootIndexLevel, key1);
            return BPlusTreeDataBucket.Read(m_header, dataAddress);
        }

        #endregion

        #region [ Static ]

        public static BPlusTree Open(BinaryStream stream)
        {
            BPlusTree bPlusTree = new BPlusTree();
            bPlusTree.OpenInternal(stream);
            return bPlusTree;
        }

        public static BPlusTree Create(BinaryStream stream, int blockSize)
        {
            BPlusTree bPlusTree = new BPlusTree();
            bPlusTree.CreateInternal(stream, blockSize);
            return bPlusTree;
        }

        #endregion

    }
}
