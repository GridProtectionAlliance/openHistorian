using System;
using GSF.IO;
using GSF.UnmanagedMemory;

namespace openHistorian.Collections.Generic.ZeroNode
{
    public unsafe class ZeroNodeScanner<TKey, TValue>
        : EncodedNodeScannerBase<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        private readonly int m_shimSize;
        private readonly byte[] m_buffer;

        public ZeroNodeScanner(byte level, int blockSize, BinaryStreamBase stream, Func<TKey, byte, uint> lookupKey, TreeKeyMethodsBase<TKey> keyMethods, TreeValueMethodsBase<TValue> valueMethods)
            : base(level, blockSize, stream, lookupKey, keyMethods, valueMethods)
        {
            m_shimSize = KeyValueSize >> 3 + (((KeyValueSize & 7) == 0) ? 0 : 1);
            m_buffer = new byte[MaximumStorageSize];
        }

        protected override unsafe int DecodeRecord(byte* stream, TKey prevKey, TValue prevValue, TKey currentKey, TValue currentValue)
        {
            fixed (byte* tmp = m_buffer)
            {
                Memory.Clear(tmp, KeyValueSize);
                int nextReadPosition = m_shimSize;
                for (int x = 0; x < KeyValueSize; x++)
                {
                    if ((stream[x >> 3] & (1 << (x & 7))) > 0)
                    {
                        tmp[x] = stream[nextReadPosition];
                        nextReadPosition++;
                    }
                }

                KeyMethods.Read(tmp, currentKey);
                ValueMethods.Read(tmp + KeySize, currentValue);
                return nextReadPosition;
            }
        }

        private int MaximumStorageSize
        {
            get
            {
                return KeyValueSize + m_shimSize;
            }
        }

        protected override unsafe byte Version
        {
            get
            {
                return 2;
            }
        }
    }
}