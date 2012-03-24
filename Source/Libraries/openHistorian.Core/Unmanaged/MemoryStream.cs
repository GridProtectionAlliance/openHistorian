using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using openHistorian.Core.StorageSystem;

namespace openHistorian.Core.Unmanaged
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    unsafe public class MemoryStream : ISupportsBinaryStream
    {
        /// <summary>
        /// The number of bits in the page size.
        /// </summary>
        const int ShiftLength = BufferPool.PageShiftBits;
        /// <summary>
        /// The mask that can be used to Logical AND the position to get the relative position within the page.
        /// </summary>
        const int OffsetMask = BufferPool.PageMask;
        /// <summary>
        /// The size of each page.
        /// </summary>
        const int Length = BufferPool.PageSize;

        /// <summary>
        /// The byte position in the stream
        /// </summary>
        private long m_position;

        private List<int> m_pageIndex;
        private List<long> m_pagePointer;

        /// <summary>
        /// Create a new <see cref="PooledMemoryStream"/>
        /// </summary>
        public MemoryStream()
        {
            m_position = 0;
            m_pageIndex = new List<int>(100);
            m_pagePointer = new List<long>(100);
        }

        /// <summary>
        /// Returns the page that corresponds to the absolute position.  
        /// This function will also autogrow the stream.
        /// </summary>
        /// <param name="position">The position to use to calculate the page to retrieve</param>
        /// <returns></returns>
        byte* GetPage(long position)
        {
            int page = (int)(position >> ShiftLength);
            //If there are not enough pages in the stream, add enough.
            while (page >= m_pageIndex.Count)
            {
                int pageIndex;
                IntPtr pagePointer;
                pageIndex = BufferPool.AllocatePage(out pagePointer);
                Memory.Clear((byte*)pagePointer,BufferPool.PageSize);
                m_pageIndex.Add(pageIndex);
                m_pagePointer.Add(pagePointer.ToInt64());
            }
            return (byte*)m_pagePointer[page];
        }

        /// <summary>
        /// This calculates the number of bytes remain at the end of the current page.
        /// </summary>
        /// <param name="position">The position to use to calculate the remaining bytes.</param>
        /// <returns></returns>
        int RemainingLenght(long position)
        {
            return Length - CalculateOffset(position);
        }

        /// <summary>
        /// Returns the relative offset within the page where the start of the position exists.
        /// </summary>
        /// <param name="position">The position to use to calculate the offset.</param>
        /// <returns></returns>
        int CalculateOffset(long position)
        {
            return (int)(position & OffsetMask);
        }

        /// <summary>
        /// Gets/Sets the cursor position within the stream
        /// </summary>
        public long Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public long FileSize
        {
            get
            {
                return m_pageIndex.Count * BufferPool.PageSize;
            }
        }

        /// <summary>
        /// Writes the following data to the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="data">The data to write</param>
        /// <param name="offset">The position to start the write</param>
        /// <param name="count">The number of bytes to write</param>
        public void Write(byte[] data, int offset, int count)
        {
            Write(Position, data, offset, count);
            Position += count;
        }

        /// <summary>
        /// Writes the following data to the stream at the provided postion. The internal position will remain uneffected.
        /// </summary>
        /// <param name="position">The position to reference</param>
        /// <param name="data">The data to write</param>
        /// <param name="offset">The position to start the write</param>
        /// <param name="count">The number of bytes to write</param>
        public void Write(long position, byte[] data, int offset, int count)
        {
            int availableLength = RemainingLenght(position);
            int destOffset = CalculateOffset(position);
            byte* block = GetPage(position);

            if (availableLength >= count)
            {

                Marshal.Copy(data, offset, (IntPtr)(block + destOffset), count);
                //Array.Copy(data, offset, block, destOffset, count);
            }
            else
            {
                Marshal.Copy(data, offset, (IntPtr)(block + destOffset), availableLength);
                //Array.Copy(data, offset, block, destOffset, availableLength);
                Write(position + availableLength, data, offset + availableLength, count - availableLength);
            }
        }

        /// <summary>
        /// Reads data from the stream at the current position, advancing the position.
        /// </summary>
        /// <param name="data">The data to read</param>
        /// <param name="offset">The position to start the read</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read from the stream. This will always be equal to count.</returns>
        public int Read(byte[] data, int offset, int count)
        {
            int bytesRead = Read(Position, data, offset, count);
            Position += count;
            return bytesRead;
        }

        /// <summary>
        /// Reads data from the stream at the provided position, the stream's current position is not effected.
        /// </summary>
        /// <param name="position">The position from the stream to read from.</param>
        /// <param name="data">The data to read</param>
        /// <param name="offset">The position to start the read</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read from the stream. This will always be equal to count.</returns>
        public int Read(long position, byte[] data, int offset, int count)
        {
            int availableLength = RemainingLenght(position);
            int destOffset = CalculateOffset(position);
            byte* block = GetPage(position);

            if (availableLength >= count)
            {
                Marshal.Copy((IntPtr)(block + destOffset), data, offset, count);
            }
            else
            {
                Marshal.Copy((IntPtr)(block + destOffset), data, offset, availableLength);
                Read(position + availableLength, data, offset + availableLength, count - availableLength);
            }
            return count;
        }

        /// <summary>
        /// Implementation of ISupportsBinaryStream to speed up writing to the stream.
        /// </summary>
        /// <returns></returns>
        void ISupportsBinaryStream.GetCurrentBlock(long position, bool isWriting, out long bufferPointer, out int firstIndex, out int lastIndex, out int currentIndex)
        {
            firstIndex = 0;
            lastIndex = Length - 1;
            currentIndex = CalculateOffset(position);
            bufferPointer = (long)GetPage(position);
        }

    }
}
