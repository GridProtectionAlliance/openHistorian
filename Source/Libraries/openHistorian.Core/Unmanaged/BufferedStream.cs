using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace openHistorian.V2.Unmanaged
{
    unsafe public class BufferedStream
    {
        static byte[] s_tmpBuffer = new byte[BufferPool.PageSize];

        struct Block
        {
            public byte* Location;
            public int Index;
        }

        SortedDictionary<int, Block> m_list;

        public Stream BaseStream;

        public BufferedStream(Stream stream)
        {
            BaseStream = stream;
            m_list = new SortedDictionary<int, Block>();
        }

        public void Read(long position, byte[] data, int start, int length)
        {
            int address = (int)(position >> BufferPool.PageShiftBits);
            int offset = (int)(position & BufferPool.PageMask);
            long streamPosition = position & ~BufferPool.PageMask;
            int availableLength = BufferPool.PageSize - offset;

            Block cachePage;

            //if the page is not in the entry, read it from the underlying stream.
            if (!m_list.TryGetValue(address, out cachePage))
            {
                IntPtr ptr;
                cachePage.Index = BufferPool.AllocatePage(out ptr);
                cachePage.Location = (byte*)ptr;

                BaseStream.Position = streamPosition;
                lock (s_tmpBuffer)
                {
                    BaseStream.Read(s_tmpBuffer, 0, s_tmpBuffer.Length);
                    Marshal.Copy(s_tmpBuffer, 0, ptr, s_tmpBuffer.Length);
                }
                m_list.Add(address, cachePage);
            }

            if (availableLength < length)
            {
                Marshal.Copy((IntPtr)cachePage.Location + offset, data, start, availableLength);
                Read(position + availableLength, data, start + availableLength, length - availableLength);
            }
            else
            {
                Marshal.Copy((IntPtr)cachePage.Location + offset, data, start, length);
            }
        }

        public void Write(long position, byte[] data, int start, int length)
        {
            int address = (int)(position >> BufferPool.PageShiftBits);
            int offset = (int)(position & BufferPool.PageMask);
            long streamPosition = position & ~BufferPool.PageMask;
            int availableLength = BufferPool.PageSize - offset;

            Block cachePage;

            //if the page is not in the entry, read it from the underlying stream.
            if (!m_list.TryGetValue(address, out cachePage))
            {
                IntPtr ptr;
                cachePage.Index = BufferPool.AllocatePage(out ptr);
                cachePage.Location = (byte*)ptr;

                BaseStream.Position = streamPosition;
                lock (s_tmpBuffer)
                {
                    BaseStream.Read(s_tmpBuffer, 0, s_tmpBuffer.Length);
                    Marshal.Copy(s_tmpBuffer, 0, ptr, s_tmpBuffer.Length);
                    //Populate with junk
                    for (int x = 0; x < BufferPool.PageSize; x += 8)
                    {
                        *(long*)ptr = (long)ptr * x;
                    }
                }
                m_list.Add(address, cachePage);
            }

            if (availableLength < length)
            {
                Marshal.Copy(data, start, (IntPtr)cachePage.Location + offset, availableLength);
                Write(position + availableLength, data, start + availableLength, length - availableLength);
            }
            else
            {
                Marshal.Copy(data, start, (IntPtr)cachePage.Location + offset, length);
            }
        }

        public void Flush()
        {
            foreach (var block in m_list)
            {
                BaseStream.Position = block.Key * (long)BufferPool.PageSize;
                Marshal.Copy((IntPtr) block.Value.Location,s_tmpBuffer, 0, s_tmpBuffer.Length);
                BaseStream.Write(s_tmpBuffer, 0, s_tmpBuffer.Length);
            }
        }


    }
}
