//******************************************************************************************************
//  BinaryStreamBase.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code.
//  11/30/2012 - Steven E. Chisholm
//       Converted to a base class 
//       
//
//******************************************************************************************************

//#define GetBaseMethodCallCount

using System;
using System.Text;

namespace GSF.IO
{
    public abstract unsafe class BinaryStreamBase : IDisposable
    {

#if GetBaseMethodCallCount
        static public long[] CallMethods = new long[100];
        public enum Method
            : int
        {
            Copy,
            InsertBytes,
            RemoveBytes,
            WriteSByte,
            WriteBool,
            WriteUInt16,
            WriteUInt32,
            WriteUInt64,
            WriteByte,
            WriteInt16,
            WriteInt32,
            WriteInt64,
            WriteSingle,
            WriteDouble,
            WriteDateTime,
            WriteDecimal,
            WriteGuid,
            Write7BitUInt,
            Write7BitULong
        }
#endif

        private readonly byte[] m_buffer = new byte[16];

        #region [ Abstract Code ]

        /// <summary>
        /// Gets/Sets the current position for the stream.
        /// <remarks>It is important to use this to Get/Set the position from the underlying stream since 
        /// this class buffers the results of the query.  Setting this field does not gaurentee that
        /// the underlying stream will get set. Call FlushToUnderlyingStream to acomplish this.</remarks>
        /// </summary>
        public abstract long Position
        {
            get;
            set;
        }

        public abstract void Write(byte[] value, int offset, int count);
        public abstract int Read(byte[] value, int offset, int count);
        public abstract void Dispose();

        #endregion

        public long PointerVersion
        {
            get;
            protected set;
        }

        /// <summary>
        /// Determines if the binary stream can be cloned.  
        /// Since a base stream may only be able to support a definate 
        /// number of concurrent IO Sessions, this should be analized
        /// before cloning the stream.
        /// </summary>
        public virtual bool SupportsAnotherClone
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Clones a binary stream if it is supported.  Check <see cref="BinaryStreamBase.SupportsAnotherClone"/> before calling this method.
        /// </summary>
        /// <returns></returns>
        public virtual BinaryStreamBase CloneStream()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public virtual void UpdateLocalBuffer(bool isWriting)
        {
        }

        /// <summary>
        /// Gets a pointer from the current position that can be used for writing up the the provided length.
        /// The current position is not advanced after calling this function.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length">the number of bytes valid for the writing.</param>
        /// <returns></returns>
        /// <remarks>This method will throw an exeption if the provided length cannot be provided.</remarks>
        public virtual byte* GetWritePointer(long position, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a pointer from the current position that can be used for writing up the the provided length.
        /// The current position is not advanced after calling this function.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length">the number of bytes valid for the writing.</param>
        /// <returns></returns>
        /// <remarks>This method will throw an exeption if the provided length cannot be provided.</remarks>
        public virtual byte* GetReadPointer(long position, int length, out bool supportsWriting)
        {
            throw new NotImplementedException();
        }

        public virtual byte* GetReadPointer(long position, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies a specified number of bytes to a new location
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        public virtual void Copy(long source, long destination, int length)
        {

#if GetBaseMethodCallCount
            CallMethods[(int)Method.Copy]++;
#endif

            byte[] data = new byte[length];
            long oldPos = Position;
            Position = source;
            Read(data, 0, length);
            Position = destination;
            Write(data, 0, length);
            Position = oldPos;
        }

        /// <summary>
        /// Sets the current position and lets the underlying buffer know if this block is about to be 
        /// written to.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isWritingHint">true if the page will be written to. false otherwise.</param>
        public virtual void SetPosition(long position, bool isWritingHint)
        {
            Position = position;
            UpdateLocalBuffer(isWritingHint);
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
        public virtual void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.InsertBytes]++;
#endif
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
        public virtual void RemoveBytes(int numberOfBytes, int lengthOfValidDataToShift)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.RemoveBytes]++;
#endif
            byte[] data = new byte[lengthOfValidDataToShift];
            long oldPos = Position;
            Position = oldPos + numberOfBytes;
            Read(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
            Write(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
        }

        public virtual void Write(sbyte value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteSByte]++;
#endif
            Write((byte)value);
        }

        public virtual void Write(bool value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteBool]++;
#endif
            if (value)
                Write((byte)1);
            else
                Write((byte)0);
        }

        public virtual void Write(ushort value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteUInt16]++;
#endif
            Write((short)value);
        }


        public virtual void Write(uint value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteUInt32]++;
#endif
            Write((int)value);
        }

        public virtual void Write(ulong value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteUInt64]++;
#endif
            Write((long)value);
        }

        public virtual void Write(byte value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteByte]++;
#endif
            m_buffer[0] = value;
            Write(m_buffer, 0, 1);
        }

        public virtual void Write(short value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteInt16]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(short*)lp = value;
            }
            Write(m_buffer, 0, 2);
        }

        public virtual void Write(int value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteInt32]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(int*)lp = value;
            }
            Write(m_buffer, 0, 4);
        }

        public virtual void Write(float value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteSingle]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(float*)lp = value;
            }
            Write(m_buffer, 0, 4);
        }

        public virtual void Write(long value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteInt64]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(long*)lp = value;
            }
            Write(m_buffer, 0, 8);
        }

        public virtual void Write(double value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteDouble]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(double*)lp = value;
            }
            Write(m_buffer, 0, 8);
        }

        public virtual void Write(DateTime value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteDateTime]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(long*)lp = value.Ticks;
            }
            Write(m_buffer, 0, 8);
        }

        public virtual void Write(decimal value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteDecimal]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(decimal*)lp = value;
            }
            Write(m_buffer, 0, 16);
        }

        public virtual void Write(Guid value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.WriteGuid]++;
#endif
            fixed (byte* lp = m_buffer)
            {
                *(Guid*)lp = value;
            }
            Write(m_buffer, 0, 16);
        }

        public virtual void WriteUInt24(uint value)
        {
            Write((ushort)value);
            Write((byte)(value >> 16));
        }

        public virtual void WriteUInt40(ulong value)
        {
            Write((uint)value);
            Write((byte)(value >> 32));
        }

        public virtual void WriteUInt48(ulong value)
        {
            Write((uint)value);
            Write((ushort)(value >> 32));
        }

        public virtual void WriteUInt56(ulong value)
        {
            Write((uint)value);
            WriteUInt24((uint)(value >> 32));
        }

        public virtual void WriteUInt(ulong value, int bytes)
        {
            switch (bytes)
            {
                case 0:
                    return;
                case 1:
                    Write((byte)value);
                    return;
                case 2:
                    Write((ushort)value);
                    return;
                case 3:
                    WriteUInt24((uint)value);
                    return;
                case 4:
                    Write((uint)value);
                    return;
                case 5:
                    WriteUInt40(value);
                    return;
                case 6:
                    WriteUInt48(value);
                    return;
                case 7:
                    WriteUInt56(value);
                    return;
                case 8:
                    Write(value);
                    return;
            }
            throw new ArgumentOutOfRangeException("bytes", "must be between 0 and 8 inclusive.");
        }

        public virtual void Write7Bit(uint value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.Write7BitUInt]++;
#endif
            Compression.Write7Bit(Write, value);
        }

        public virtual void Write7Bit(ulong value)
        {
#if GetBaseMethodCallCount
            CallMethods[(int)Method.Write7BitULong]++;
#endif
            Compression.Write7Bit(Write, value);
        }

        public virtual void Write(string value)
        {
            WriteWithLength(Encoding.ASCII.GetBytes(value));
        }

        public virtual void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public virtual void WriteWithLength(byte[] value)
        {
            Write7Bit((uint)value.Length);
            Write(value);
        }

        public virtual void Write(byte* buffer, int length)
        {
            int pos = 0;
            while (pos + 8 <= length)
            {
                Write(*(long*)(buffer + pos));
                pos += 8;
            }
            if (pos + 4 <= length)
            {
                Write(*(int*)(buffer + pos));
                pos += 4;
            }
            if (pos + 2 <= length)
            {
                Write(*(short*)(buffer + pos));
                pos += 2;
            }
            if (pos + 1 <= length)
            {
                Write(*(buffer + pos));
            }
        }

        public virtual sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public virtual bool ReadBoolean()
        {
            return ReadByte() != 0;
        }

        public virtual ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }

        public virtual uint ReadUInt24()
        {
            uint value = ReadUInt16();
            return value | ((uint)ReadByte() << 16);
        }

        public virtual uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }

        public virtual ulong ReadUInt40()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadByte() << 32);
        }

        public virtual ulong ReadUInt48()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadUInt16() << 32);
        }

        public virtual ulong ReadUInt56()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadUInt24() << 32);
        }

        public virtual ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }

        public virtual ulong ReadUInt(int bytes)
        {
            switch (bytes)
            {
                case 0:
                    return 0;
                case 1:
                    return ReadByte();
                case 2:
                    return ReadUInt16();
                case 3:
                    return ReadUInt24();
                case 4:
                    return ReadUInt32();
                case 5:
                    return ReadUInt40();
                case 6:
                    return ReadUInt48();
                case 7:
                    return ReadUInt56();
                case 8:
                    return ReadUInt64();
            }
            throw new ArgumentOutOfRangeException("bytes", "must be between 0 and 8 inclusive.");
        }

        public virtual byte ReadByte()
        {
            Read(m_buffer, 0, 1);
            return m_buffer[0];
        }

        public virtual short ReadInt16()
        {
            Read(m_buffer, 0, 2);
            fixed (byte* lp = m_buffer)
            {
                return *(short*)lp;
            }
        }

        public virtual int ReadInt32()
        {
            Read(m_buffer, 0, 4);
            fixed (byte* lp = m_buffer)
            {
                return *(int*)lp;
            }
        }

        public virtual float ReadSingle()
        {
            Read(m_buffer, 0, 4);
            fixed (byte* lp = m_buffer)
            {
                return *(float*)lp;
            }
        }

        public virtual long ReadInt64()
        {
            Read(m_buffer, 0, 8);
            fixed (byte* lp = m_buffer)
            {
                return *(long*)lp;
            }
        }

        public virtual double ReadDouble()
        {
            Read(m_buffer, 0, 8);
            fixed (byte* lp = m_buffer)
            {
                return *(double*)lp;
            }
        }

        public virtual DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64());
        }

        public virtual decimal ReadDecimal()
        {
            Read(m_buffer, 0, 16);
            fixed (byte* lp = m_buffer)
            {
                return *(decimal*)lp;
            }
        }

        public virtual Guid ReadGuid()
        {
            Read(m_buffer, 0, 16);
            return new Guid(m_buffer);
        }

        public virtual uint Read7BitUInt32()
        {
            return Compression.Read7BitUInt32(ReadByte);
        }

        public virtual ulong Read7BitUInt64()
        {
            return Compression.Read7BitUInt64(ReadByte);
        }

        public virtual byte[] ReadBytes(int count)
        {
            byte[] value = new byte[count];
            Read(value, 0, count);
            return value;
        }

        public virtual byte[] ReadBytes()
        {
            return ReadBytes((int)Read7BitUInt32());
        }

        public virtual string ReadString()
        {
            return Encoding.ASCII.GetString(ReadBytes());
        }

        public virtual void Read(byte* buffer, int length)
        {
            int pos = 0;
            while (pos + 8 <= length)
            {
                *(long*)(buffer + pos) = ReadInt64();
                pos += 8;
            }
            if (pos + 4 <= length)
            {
                *(int*)(buffer + pos) = ReadInt32();
                pos += 4;
            }
            if (pos + 2 <= length)
            {
                *(short*)(buffer + pos) = ReadInt16();
                pos += 2;
            }
            if (pos + 1 <= length)
            {
                *(buffer + pos) = ReadByte();
            }
        }
    }
}