using System;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.Unmanaged
{
    /// <summary>
    /// Creates a fixed size array of unmanaged bytes.
    /// </summary>
    public unsafe class UnmanagedArray : IDisposable
    {
        int m_size;
        int m_blockSize;
        int m_blockMask;
        int m_blockShiftBits;
        byte*[] m_intPtr;
        int[] m_index;
        bool m_disposed = false;

        public UnmanagedArray(int size)
        {
            m_size = size;
            m_blockSize = BufferPool.PageSize;
            m_blockMask = BufferPool.PageMask;
            m_blockShiftBits = BufferPool.PageShiftBits;

            int blocks = size / m_blockSize;
            if (blocks * m_blockSize < size)
                blocks++;

            m_intPtr = new byte*[blocks];
            m_index = new int[blocks];

            for (int x = 0; x < blocks; x++)
            {
                IntPtr ptr;
                m_index[x] = BufferPool.AllocatePage(out ptr);
                m_intPtr[x] = (byte*)ptr.ToPointer();
            }
        }
        ~UnmanagedArray()
        {
            Dispose();
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= m_size)
                    throw new Exception("out of bounds");
                return m_intPtr[index >> m_blockShiftBits][index & m_blockMask];
            }
            set
            {
                if (index < 0 || index >= m_size)
                    throw new Exception("out of bounds");
                m_intPtr[index >> m_blockShiftBits][index & m_blockMask] = value;
            }
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                foreach (int blockIndex in m_index)
                {
                    BufferPool.ReleasePage(blockIndex);
                }
                m_disposed = true;
                m_intPtr = null;
                m_index = null;
                GC.SuppressFinalize(this);

            }

        }
    }
}
