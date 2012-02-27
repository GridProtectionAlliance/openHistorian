using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
    {
        struct BucketCache
        {
            public TKey UpperBound;
            public TKey LowerBound;
            public bool IsLowerValid;
            public bool IsUpperValid;
            public uint Bucket;
            public byte Level;
            public bool IsMatch(TKey currentKey)
            {
                if (Level == 0)
                    return false;
                if (!IsLowerValid)
                {
                    if (!IsUpperValid)
                        return false;
                    if (currentKey.CompareTo(UpperBound) < 0)
                        return true;
                }
                else if (!IsUpperValid)
                {
                    if (currentKey.CompareTo(LowerBound) >= 0)
                        return true;
                }
                else
                {
                    if (currentKey.CompareTo(LowerBound) >= 0)
                        if (currentKey.CompareTo(UpperBound) < 0)
                            return true;
                }
                return false;
            }
        }

        void CacheCurrentIndex(byte level, int childCount, long startAddress, int index, bool isForward, TKey key)
        {
            BucketCache cache = default(BucketCache);

            cache.Level = level;
            if (index == 0 && !isForward)
            {
                Stream.Position = startAddress + InternalStructureSize * index - 4;
                cache.IsLowerValid = this.cache[level].IsLowerValid;
                cache.LowerBound = this.cache[level].LowerBound;
                cache.Bucket = Stream.ReadUInt32();
                cache.UpperBound.LoadValue(Stream);
                cache.IsUpperValid = true;
            }
            else if (index == childCount || (index == childCount - 1 && isForward))
            {
                Stream.Position = startAddress + InternalStructureSize * (childCount - 1);
                cache.IsLowerValid = true;
                cache.LowerBound.LoadValue(Stream);
                cache.Bucket = Stream.ReadUInt32();
                cache.IsUpperValid = this.cache[level].IsUpperValid;
                cache.UpperBound = this.cache[level].UpperBound;

            }
            else if (isForward)
            {
                Stream.Position = startAddress + InternalStructureSize * index;
                cache.IsLowerValid = true;
                cache.LowerBound.LoadValue(Stream);
                cache.Bucket = Stream.ReadUInt32();
                cache.UpperBound.LoadValue(Stream);
                cache.IsUpperValid = true;
            }
            else
            {
                Stream.Position = startAddress + InternalStructureSize * (index - 1);
                cache.IsLowerValid = true;
                cache.LowerBound.LoadValue(Stream);
                cache.Bucket = Stream.ReadUInt32();
                cache.UpperBound.LoadValue(Stream);
                cache.IsUpperValid = true;
            }
            this.cache[level - 1] = cache;
        }

    }
}
