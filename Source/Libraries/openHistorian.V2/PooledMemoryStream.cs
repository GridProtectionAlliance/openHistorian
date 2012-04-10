using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.StorageSystem;

namespace openHistorian.V2
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in common. 
    /// Keeping the memory blocks to a small size improves the dynamic memory allocation requirements,
    /// making it superior to System.IO.MemorySteam.
    /// It also provides binary read and write operations.
    /// </summary>
    public class PooledMemoryStream : ISupportsBinaryStream2
    {
        /// <summary>
        /// The number of bits in the page size.
        /// </summary>
        const int ShiftLength = 12;
        /// <summary>
        /// The mask that can be used to Logical AND the position to get the relative position within the page.
        /// </summary>
        const int OffsetMask = Length - 1;
        /// <summary>
        /// The size of each page.
        /// </summary>
        const int Length = 4096;

        /// <summary>
        /// The byte position in the stream
        /// </summary>
        private long m_position;
        /// <summary>
        /// The list of all pages in the stream
        /// </summary>
        private List<byte[]> m_stream;

        /// <summary>
        /// Create a new <see cref="PooledMemoryStream"/>
        /// </summary>
        public PooledMemoryStream()
        {
            m_position = 0;
            m_stream = new List<byte[]>();
        }

        /// <summary>
        /// Returns the page that corresponds to the absolute position.  
        /// This function will also autogrow the stream.
        /// </summary>
        /// <param name="position">The position to use to calculate the page to retrieve</param>
        /// <returns></returns>
        byte[] GetPage(long position)
        {
            int page = (int)(position >> ShiftLength);
            //If there are not enough pages in the stream, add enough.
            while (page >= m_stream.Count)
            {
                m_stream.Add(new byte[Length]);
            }
            return m_stream[page];
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
            byte[] block = GetPage(position);

            if (availableLength >= count)
            {
                Array.Copy(data, offset, block, destOffset, count);
            }
            else
            {
                Array.Copy(data, offset, block, destOffset, availableLength);
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
            byte[] block = GetPage(position);

            if (availableLength >= count)
            {
                Array.Copy(block, destOffset, data, offset, count);
            }
            else
            {
                Array.Copy(block, destOffset, data, offset, availableLength);
                Read(position + availableLength, data, offset + availableLength, count - availableLength);
            }
            return count;
        }

        /// <summary>
        /// Implementation of ISupportsBinaryStream to speed up writing to the stream.
        /// </summary>
        /// <returns></returns>
        void ISupportsBinaryStream2.GetCurrentBlock(long position, bool isWriting, out byte[] buffer, out int firstIndex, out int lastIndex, out int currentIndex)
        {
            firstIndex = 0;
            lastIndex = Length - 1;
            currentIndex = CalculateOffset(position);
            buffer = GetPage(position);
        }

    }
}
