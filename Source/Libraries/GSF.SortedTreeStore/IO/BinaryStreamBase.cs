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

using System;
using System.IO;
using System.Text;

namespace GSF.IO
{
    /// <summary>
    /// Since object that inherit from MarshalByRefObject cannot be inlined, I decided to make
    /// a wrapper class for the pointer information so that it can be inlined.
    /// </summary>
    public class PointerVersionBox
    {
        public long Version { get; private set; }

        public PointerVersionBox(out Action increment)
        {
            Version = 0;
            increment = Increment;
        }
        void Increment()
        {
            Version++;
        }
    }

    /// <summary>
    /// An abstract class for reading/writing to a little endian stream.
    /// </summary>
    public abstract unsafe class BinaryStreamBase
        : Stream
    {

        protected BinaryStreamBase()
        {
            PointerVersionBox = new PointerVersionBox(out IncrementPointer);
        }

        private readonly byte[] m_buffer = new byte[16];
        protected Action IncrementPointer;

        public PointerVersionBox PointerVersionBox { get; private set; }

        /// <summary>
        /// Gets the pointer version number assuming that this binary stream has an unmanaged buffer backing this stream. 
        /// If the pointer version is the same, than any pointer acquired is still valid.
        /// </summary>
        public long PointerVersion
        {
            get
            {
                return PointerVersionBox.Version;
            }
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
            byte[] data = new byte[length];
            long oldPos = Position;
            Position = source;
            ReadAll(data, 0, length);
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
            byte[] data = new byte[lengthOfValidDataToShift];
            long oldPos = Position;
            ReadAll(data, 0, lengthOfValidDataToShift);
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
            byte[] data = new byte[lengthOfValidDataToShift];
            long oldPos = Position;
            Position = oldPos + numberOfBytes;
            ReadAll(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
            Write(data, 0, lengthOfValidDataToShift);
            Position = oldPos;
        }

        #region [ Write ]

        public override void WriteByte(byte value)
        {
            Write(value);
        }

        public virtual void Write(sbyte value)
        {
            Write((byte)value);
        }

        public virtual void Write(bool value)
        {
            if (value)
                Write((byte)1);
            else
                Write((byte)0);
        }

        public virtual void Write(ushort value)
        {
            Write((short)value);
        }

        public virtual void Write(uint value)
        {
            Write((int)value);
        }

        public virtual void Write(ulong value)
        {
            Write((long)value);
        }

        public virtual void Write(byte value)
        {
            m_buffer[0] = value;
            Write(m_buffer, 0, 1);
        }

        public virtual void Write(short value)
        {
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            Write(m_buffer, 0, 2);
        }

        public virtual void Write(int value)
        {
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            m_buffer[2] = (byte)(value >> 16);
            m_buffer[3] = (byte)(value >> 24);
            Write(m_buffer, 0, 4);
        }

        public virtual void Write(float value)
        {
            Write(*(int*)(&value));
        }

        public virtual void Write(long value)
        {
            m_buffer[0] = (byte)value;
            m_buffer[1] = (byte)(value >> 8);
            m_buffer[2] = (byte)(value >> 16);
            m_buffer[3] = (byte)(value >> 24);
            m_buffer[4] = (byte)(value >> 32);
            m_buffer[5] = (byte)(value >> 40);
            m_buffer[6] = (byte)(value >> 48);
            m_buffer[7] = (byte)(value >> 56);
            Write(m_buffer, 0, 8);
        }

        public virtual void Write(double value)
        {
            Write(*(long*)&value);
        }

        public virtual void Write(DateTime value)
        {
            Write(value.Ticks);
        }

        public virtual void Write(decimal value)
        {
            int* ptr = (int*)&value;
            Write(ptr[0]); //flags
            Write(ptr[1]); //high
            Write(ptr[2]); //low
            Write(ptr[3]); //mid
        }

        public virtual void Write(Guid value)
        {
            byte* ptr = (byte*)&value;
            Write(*(int*)(ptr + 0));
            Write(*(short*)(ptr + 4));
            Write(*(short*)(ptr + 6));
            Write(ptr[8]);
            Write(ptr[9]);
            Write(ptr[10]);
            Write(ptr[11]);
            Write(ptr[12]);
            Write(ptr[13]);
            Write(ptr[14]);
            Write(ptr[15]);
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
            Write((ushort)(value >> 32));
            Write((byte)(value >> 48));
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
            Encoding7Bit.Write(Write, value);
        }

        public virtual void Write7Bit(ulong value)
        {
            Encoding7Bit.Write(Write, value);
        }

        public virtual void Write(string value)
        {
            WriteWithLength(Encoding.ASCII.GetBytes(value));
        }

        public virtual void Write(string value, Encoding encoding)
        {
            WriteWithLength(encoding.GetBytes(value));
        }

        public virtual void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public virtual void WriteWithLength(byte[] value)
        {
            Write7Bit((uint)value.Length);
            Write(value, 0, value.Length);
        }

        public virtual void Write(byte* buffer, int length)
        {
            for (int x = 0; x < length; x++)
            {
                Write(buffer[x]);
            }
        }

        #endregion

        #region [ Read ]

        //public override int ReadByte()
        //{
        //    throw new NotSupportedException();
        //}

        public virtual sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }

        public virtual bool ReadBoolean()
        {
            return ReadUInt8() != 0;
        }

        public virtual ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }

        public virtual uint ReadUInt24()
        {
            uint value = ReadUInt16();
            return value | ((uint)ReadUInt8() << 16);
        }

        public virtual uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }

        public virtual ulong ReadUInt40()
        {
            ulong value = ReadUInt32();
            return value | ((ulong)ReadUInt8() << 32);
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
                    return ReadUInt8();
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

        public virtual byte ReadUInt8()
        {
            ReadAll(m_buffer, 0, 1);
            return m_buffer[0];
        }

        public virtual short ReadInt16()
        {
            ReadAll(m_buffer, 0, 2);
            return (short)((int)m_buffer[0]
                | (int)m_buffer[1] << 8);
        }

        public virtual int ReadInt32()
        {
            ReadAll(m_buffer, 0, 4);
            return (int)m_buffer[0]
                | (int)m_buffer[1] << 8
                | (int)m_buffer[2] << 16
                | (int)m_buffer[3] << 24;
        }

        public virtual float ReadSingle()
        {
            int value = ReadInt32();
            return *(float*)&value;
        }

        public virtual long ReadInt64()
        {
            ReadAll(m_buffer, 0, 8);
            return (long)m_buffer[0]
                   | (long)m_buffer[1] << 8
                   | (long)m_buffer[2] << 16
                   | (long)m_buffer[3] << 24
                   | (long)m_buffer[4] << 32
                   | (long)m_buffer[5] << 40
                   | (long)m_buffer[6] << 48
                   | (long)m_buffer[7] << 56;
        }

        public virtual double ReadDouble()
        {
            long value = ReadInt64();
            return *(double*)&value;
        }

        public virtual DateTime ReadDateTime()
        {
            return new DateTime(ReadInt64());
        }

        public virtual decimal ReadDecimal()
        {
            decimal rv;
            int* ptr = (int*)&rv;
            ptr[0] = ReadInt32();
            ptr[1] = ReadInt32();
            ptr[2] = ReadInt32();
            ptr[3] = ReadInt32();
            return rv;
        }

        public virtual Guid ReadGuid()
        {
            ReadAll(m_buffer, 0, 16);
            return new Guid(m_buffer);
        }

        public virtual uint Read7BitUInt32()
        {
            return Encoding7Bit.ReadUInt32(ReadUInt8);
        }

        public virtual ulong Read7BitUInt64()
        {
            return Encoding7Bit.ReadUInt64(ReadUInt8);
        }

        public virtual byte[] ReadBytes(int count)
        {
            byte[] value = new byte[count];
            ReadAll(value, 0, count);
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

        public virtual string ReadString(Encoding encoding)
        {
            return encoding.GetString(ReadBytes());
        }

        public virtual void ReadAll(byte* buffer, int length)
        {
            for (int x = 0; x < length; x++)
            {
                buffer[x] = ReadUInt8();
            }
        }

        /// <summary>
        /// Reads all of the provided bytes. Will not return prematurely, 
        /// but continue to execute a <see cref="Read"/> command until the entire
        /// <see cref="length"/> has been read.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <exception cref="EndOfStreamException">occurs if the end of the stream has been reached.</exception>
        public virtual void ReadAll(byte[] buffer, int position, int length)
        {
        ReadAgain:
            int bytesRead = Read(buffer, position, length);
            if (length == bytesRead)
                return;
            if (bytesRead == 0)
                throw new EndOfStreamException();
            length -= bytesRead;
            position += bytesRead;
            goto ReadAgain;
        }

        #endregion

    }
}