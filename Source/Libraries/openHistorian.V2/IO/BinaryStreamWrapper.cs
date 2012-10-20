using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace openHistorian.V2.IO
{
    public class BinaryStreamWrapper : IBinaryStream
    {
        Stream m_stream;
        BinaryReader m_br;
        BinaryWriter m_bw;
        public BinaryStreamWrapper(Stream stream)
        {
            m_stream = stream;
            m_br = new BinaryReader(stream);
            m_bw = new BinaryWriter(stream);
            SupportsAnotherClone = false;

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {

        }

        /// <summary>
        /// Determines if the binary stream can be cloned.  
        /// Since a base stream may only be able to support a definate 
        /// number of concurrent IO Sessions, this should be analized
        /// before cloning the stream.
        /// </summary>
        public bool SupportsAnotherClone { get; private set; }

        /// <summary>
        /// Gets/Sets the current position for the stream.
        /// <remarks>It is important to use this to Get/Set the position from the underlying stream since 
        /// this class buffers the results of the query.  Setting this field does not gaurentee that
        /// the underlying stream will get set. Call FlushToUnderlyingStream to acomplish this.</remarks>
        /// </summary>
        public long Position
        {
            get
            {
                return m_stream.Position;
            }
            set
            {
                m_stream.Position = value;
            }
        }

        /// <summary>
        /// Clones a binary stream if it is supported.  Check <see cref="IBinaryStream.SupportsAnotherClone"/> before calling this method.
        /// </summary>
        /// <returns></returns>
        public IBinaryStream CloneStream()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public void UpdateLocalBuffer(bool isWriting)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Copies a specified number of bytes to a new location
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        public void Copy(long source, long destination, int length)
        {
            byte[] data = new byte[length];
            long oldPos = Position;
            Position = source;
            Read(data, 0, length);
            Position = destination;
            Write(data, 0, length);
            Position = oldPos;
        }

        /// <summary>
        /// Inserts a certain number of bytes into the stream, shifting valid data to the right.  The stream's position remains unchanged. 
        /// (ie. pointing to the beginning of the newly inserted bytes).
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes to insert</param>
        /// <param name="lengthOfValidDataToShift">The number of bytes that will need to be shifted to perform this insert</param>
        /// <remarks>Internally this fuction merely acomplishes an Array.Copy(stream,position,stream,position+numberOfBytes,lengthOfValidDataToShift)
        /// However, it's much more complicated than this. So this is a pretty useful function.
        /// The newly created space is uninitialized. 
        /// </remarks>
        public void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
            byte[] data = new byte[lengthOfValidDataToShift];
            long oldPos = Position;
            Read(data, 0, lengthOfValidDataToShift);
            Position = oldPos + numberOfBytes;
            Write(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
        }

        /// <summary>
        /// Removes a certain number of bytes from the stream, shifting valid data after this location to the left.  The stream's position remains unchanged. 
        /// (ie. pointing to where the data used to exist).
        /// </summary>
        /// <param name="numberOfBytes">The distance to shift.  Positive means shifting to the right (ie. inserting data)
        /// Negative means shift to the left (ie. deleteing data)</param>
        /// <param name="lengthOfValidDataToShift">The number of bytes that will need to be shifted to perform the remove. 
        /// This only includes the data that is valid after the shift is complete, and not the data that will be removed.</param>
        /// <remarks>Internally this fuction merely acomplishes an Array.Copy(stream,position+numberOfBytes,stream,position,lengthOfValidDataToShift)
        /// However, it's much more complicated than this. So this is a pretty useful function.
        /// The space at the end of the copy is uninitialized. 
        /// </remarks>
        public void RemoveBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
            byte[] data = new byte[lengthOfValidDataToShift];
            long oldPos = Position;
            Position = oldPos + numberOfBytes;
            Read(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
            Write(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
        }

        public void Write(sbyte value)
        {
            m_bw.Write(value);
        }

        public void Write(bool value)
        {
            m_bw.Write(value);
        }

        public void Write(ushort value)
        {
            m_bw.Write(value);
        }

        public void Write(uint value)
        {
            m_bw.Write(value);
        }

        public void Write(ulong value)
        {
            m_bw.Write(value);
        }

        public void Write(byte value)
        {
            m_bw.Write(value);
        }

        public void Write(short value)
        {
            m_bw.Write(value);
        }

        public void Write(int value)
        {
            m_bw.Write(value);
        }

        public void Write(float value)
        {
            m_bw.Write(value);
        }

        public void Write(long value)
        {
            m_bw.Write(value);
        }

        public void Write(double value)
        {
            m_bw.Write(value);
        }

        public void Write(DateTime value)
        {
            m_bw.Write(value.Ticks);
        }

        public void Write(decimal value)
        {
            m_bw.Write(value);
        }

        public void Write(Guid value)
        {
            m_bw.Write(value.ToByteArray());
        }

        public void Write7Bit(uint value)
        {
            byte[] data = new byte[Compression.Get7BitSize(value)];
            int x = 0;
            Compression.Write7Bit(data, ref x, value);
            Write(data,0,data.Length);
        }

        public void Write7Bit(ulong value)
        {
            byte[] data = new byte[Compression.Get7BitSize(value)];
            int x = 0;
            Compression.Write7Bit(data, ref x, value);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] value, int offset, int count)
        {
            m_bw.Write(value, offset, count);
        }

        public sbyte ReadSByte()
        {
            return m_br.ReadSByte();
        }

        public bool ReadBoolean()
        {
            return m_br.ReadBoolean();
        }

        public ushort ReadUInt16()
        {
            return m_br.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return m_br.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return m_br.ReadUInt64();
        }

        public byte ReadByte()
        {
            return m_br.ReadByte();
        }

        public short ReadInt16()
        {
            return m_br.ReadInt16();
        }

        public int ReadInt32()
        {
            return m_br.ReadInt32();
        }

        public float ReadSingle()
        {
            return m_br.ReadSingle();
        }

        public long ReadInt64()
        {
            return m_br.ReadInt64();
        }

        public double ReadDouble()
        {
            return m_br.ReadDouble();
        }

        public DateTime ReadDateTime()
        {
            return new DateTime(m_br.ReadInt64());
        }

        public decimal ReadDecimal()
        {
            return m_br.ReadDecimal();
        }

        public Guid ReadGuid()
        {
            return new Guid(m_br.ReadBytes(16));
        }

        public uint Read7BitUInt32()
        {
            byte[] data = new byte[5];
            Read(data, 0, 5);
            int x = 0;
            uint value = 0;
            Compression.Read7BitUInt32(data, ref x, out value);
            Position -= (5 - x);
            return value;
        }

        public ulong Read7BitUInt64()
        {
            byte[] data = new byte[10];
            Read(data, 0, 10);
            int x = 0;
            ulong value = 0;
            Compression.Read7BitUInt64(data, ref x, out value);
            Position -= (10 - x);
            return value;
        }

        public int Read(byte[] value, int offset, int count)
        {
            return m_br.Read(value, offset, count);
        }
    }
}
