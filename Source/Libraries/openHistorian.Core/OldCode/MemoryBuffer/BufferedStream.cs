using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.MemoryBuffer
{
    /// <summary>
    /// This provides a stream that will wrap an existing stream to provide a very large buffer space that is in common across the entire instance of this application. 
    /// </summary>
    public class BufferedStream : Stream
    {
        /// <summary>
        /// The in-memory buffer that this class will use to buffer the stream.
        /// </summary>
        IMemoryBuffer m_buffer;
        /// <summary>
        /// The stream that is being buffered
        /// </summary>
        Stream m_stream;

        long m_DataSetID;
        
        MemoryUnit m_currentPage;
     
        MemoryUnit currentPage
        {
            get
            {
                return m_currentPage;
            }
            set
            {
                if (m_currentPage != null)
                {
                    m_currentPage.RemovePressure();
                }
                m_currentPage = value;
                if (value != null) 
                    m_currentPage.AddPressure();
            }
        }
        
        long m_position;
        
        public BufferedStream(IMemoryBuffer buffer, Stream stream)
        {
            m_buffer = buffer;
            m_stream = stream;
            m_DataSetID = buffer.GetNextDataSetID();
        }

        public override bool CanRead
        {
            get { return m_stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_stream.CanWrite; }
        }

        public override void Flush()
        {
           m_stream.Flush();
        }

        public override long Length
        {
            get { return m_stream.Length; }
        }

        public override long Position
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            internalRead(Position, buffer, offset, count);
            Position += count;
            return count;
        }

        void internalRead(long position, byte[] buffer, int offset, int count)
        {
            uint pageID = getPageID(position);
            int localPosition = getLocalPosition(position);
            
            //Lookup the page if it is in the cache.
            if (currentPage == null)
            {
                currentPage = m_buffer.LookupPage(m_DataSetID, pageID);
            }
            else if (currentPage.BlockIndex != pageID)
            {
                currentPage = m_buffer.LookupPage(m_DataSetID, pageID);
            }
            //If it was not found in the local cache, read it from the base stream and add it to the cache.
            if (currentPage == null)
            {
                currentPage = m_buffer.GetFreePage();
                m_stream.Position = position - localPosition;
                m_stream.Read(currentPage.DataSpace, 0, currentPage.DataSpace.Length);
                m_buffer.AddToBuffer(m_DataSetID, pageID, currentPage);
            }
            //begin writing the data to the stream
            if (count + localPosition <= m_buffer.BlockSize)
            {
                Array.Copy(currentPage.DataSpace, localPosition, buffer, offset, count);
            }
            else
            {
                int length = m_buffer.BlockSize-localPosition;
                //Copy the remaining data on this page
                Array.Copy(currentPage.DataSpace, localPosition, buffer, offset, length);
                internalRead(position + length, buffer, offset + length, count - length);
            }
        }
        
        uint getPageID(long position)
        {
            return (uint)(position >> m_buffer.BlockBits);
        }
        int getLocalPosition(long position)
        {
            return (int)(position & m_buffer.BlockMask);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #region [ Reading Methods ]
        public unsafe T ReadStructure<T>(long position) where T : struct
        {
            fixed (byte* lp = currentPage.DataSpace)
            {
                //return *(T*)lp;
                
                Type ObjType = typeof(T);
                if (ObjType.IsValueType & ObjType.IsLayoutSequential & ObjType.IsExplicitLayout)
                    throw new Exception("Type is not a structure that is explicitly layed out");
                return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(new IntPtr(lp), typeof(T));
            }
        }
        public unsafe Guid ReadGUID(long position)
        {
            fixed (byte* lp = currentPage.DataSpace)
            {
                return *(Guid*)(lp);
            }
        }

        #endregion


    }
}
