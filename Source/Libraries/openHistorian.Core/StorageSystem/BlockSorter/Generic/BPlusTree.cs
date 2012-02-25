using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.Core.StorageSystem.File;
using System.IO;

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
        where TKey : struct,IPrimaryKey
        where TValue : struct,IValue
    {
        #region [ Members ]

        #endregion

        #region [ Constructors ]

        //Opens an existing one
        public BPlusTree(BinaryStream stream)
        {
            TreeHeader(stream);
        }
        //Creates a new stream
        public BPlusTree(BinaryStream stream, int blockSize)
        {
            TreeHeader(stream, blockSize);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public void AddData(TKey key, TValue data)
        {
            uint indexAddress = RootIndexAddress;
            byte indexLevel = RootIndexLevel;
            long dataAddress = 1;
            long key1 = 1;
            AddItem(key1, dataAddress, ref indexAddress, ref indexLevel);
            if (indexAddress != RootIndexAddress || indexLevel != RootIndexLevel)
                SetRootIndex(indexAddress, indexLevel);
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
        public TValue GetData(TKey key)
        {
            long key1 = 0;
            long dataAddress = GetKey(RootIndexAddress, RootIndexLevel, key1);
            return default(TValue);
        }

        #endregion

    }
}
