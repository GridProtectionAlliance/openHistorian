using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    unsafe public class MemoryStream : ISupportsBinaryStream
    {

        public event EventHandler StreamDisposed;
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
        long m_position;

        List<int> m_pageIndex;

        List<long> m_pagePointer;

        /// <summary>
        /// Releases all the resources used by the <see cref="MemoryStream"/> object.
        /// </summary>
        bool m_disposed;

        /// <summary>
        /// A debug counter that keep track of the number of time a lookup is performed.
        /// </summary>
        public long LookupCount = 0;

        /// <summary>
        /// Create a new <see cref="MemoryStream"/>
        /// </summary>
        public MemoryStream()
        {
            m_position = 0;
            m_pageIndex = new List<int>(100);
            m_pagePointer = new List<long>(100);
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="MemoryStream"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~MemoryStream()
        {
            Dispose(false);
        }


        /// <summary>
        /// Returns the page that corresponds to the absolute position.  
        /// This function will also autogrow the stream.
        /// </summary>
        /// <param name="position">The position to use to calculate the page to retrieve</param>
        /// <returns></returns>
        byte* GetPage(long position)
        {
            if (m_disposed)
                throw new ObjectDisposedException("MemoryStream");

            int page = (int)(position >> ShiftLength);
            //If there are not enough pages in the stream, add enough.
            while (page >= m_pageIndex.Count)
            {
                int pageIndex;
                IntPtr pagePointer;
                pageIndex = BufferPool.AllocatePage(out pagePointer);
                Memory.Clear((byte*)pagePointer, BufferPool.PageSize);
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


        int ISupportsBinaryStream.RemainingSupportedIoSessions
        {
            get
            {
                return int.MaxValue;
            }
        }

        void ISupportsBinaryStream.GetBlock(int ioSession, long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
        {
            LookupCount++;
            length = Length;
            firstPosition = position & ~(length - 1);
            firstPointer = (IntPtr)GetPage(position);
            supportsWriting = true;
        }

        void ISupportsBinaryStream.ReleaseIoSession(int ioSession)
        {

        }

        int ISupportsBinaryStream.GetNextIoSession()
        {
            return 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MemoryStream"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (StreamDisposed != null)
                        StreamDisposed.Invoke(this, EventArgs.Empty);

                    // This will be done regardless of whether the object is finalized or disposed.
                    foreach (int index in m_pageIndex)
                    {
                        BufferPool.ReleasePage(index);
                    }
                    m_pageIndex = null;
                    m_pagePointer = null;

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }
    }
}
