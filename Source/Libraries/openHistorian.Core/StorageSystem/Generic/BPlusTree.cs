using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.Core.StorageSystem.File;
using System.IO;

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
        where TKey : struct, ITreeType<TKey>
        where TValue : struct, ITreeType<TValue>
    {
        #region [ Members ]

        int KeySize;
        int ValueSize;
        BucketCache[] cache;

        #endregion

        #region [ Constructors ]

        BPlusTree()
        {
            cache=new BucketCache[5];
            TKey key = default(TKey);
            TValue value = default(TValue);
            //TKey key = new TKey();
            //TValue value = new TValue();
            KeySize = key.SizeOf;
            ValueSize = key.SizeOf;
            LeafStructureSize = key.SizeOf + value.SizeOf;
            InternalStructureSize = key.SizeOf + sizeof(uint);
        }
        //Opens an existing one
        public BPlusTree(BinaryStream stream)
            : this()
        {
            TreeHeader(stream);
        }
        //Creates a new stream
        public BPlusTree(BinaryStream stream, int blockSize)
            :this()
        {
            TreeHeader(stream, blockSize);
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        public void AddData(TKey key, TValue value)
        {
            AddItem(key, value, ref RootIndexAddress, ref RootIndexLevel);
        }
        public void ClearCache(byte level)
        {
            cache[level-1]=default(BucketCache);
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
            return GetKey(RootIndexAddress, RootIndexLevel, key);
        }

        #endregion

    }
}
