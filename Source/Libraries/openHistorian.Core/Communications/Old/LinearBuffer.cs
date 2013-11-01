////******************************************************************************************************
////  LinearBuffer.cs - Gbtc
////
////  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
////
////  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
////  the NOTICE file distributed with this work for additional information regarding copyright ownership.
////  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
////  not use this file except in compliance with the License. You may obtain a copy of the License at:
////
////      http://www.opensource.org/licenses/eclipse-1.0.php
////
////  Unless agreed to in writing, the subject software distributed under the License is distributed on an
////  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
////  License for the specific language governing permissions and limitations.
////
////  Code Modification History:
////  ----------------------------------------------------------------------------------------------------
////  12/8/2012 - Steven E. Chisholm
////       Generated original version of source code. 
////       
////
////******************************************************************************************************

//using System;
//using System.IO;

//namespace GSF.Communications
//{
//    /// <summary>
//    /// Provides a fixed size Linear Byte Buffer.
//    /// </summary>
//    /// <remarks>
//    /// A <see cref="LinearBuffer"/> should be used when the entire buffer will always
//    /// be read at the same time. Use a <see cref="CircularBuffer"/>. if this is not
//    /// the case.
//    /// </remarks>
//    public class LinearBuffer
//    {
//        private readonly byte[] m_buffer;
//        private int m_position;

//        public LinearBuffer(int capacity)
//        {
//            m_buffer = new byte[capacity];
//            m_position = 0;
//        }

//        /// <summary>
//        /// Gets the number of bytes that are available 
//        /// for reading from the <see cref="CircularBuffer"/>.
//        /// </summary>
//        public int DataAvailable
//        {
//            get
//            {
//                return m_position;
//            }
//        }

//        /// <summary>
//        /// Gets the number of bytes that can be written to this <see cref="CircularBuffer"/>.
//        /// </summary>
//        public int FreeSpace
//        {
//            get
//            {
//                return Capacity - m_position;
//            }
//        }

//        /// <summary>
//        /// Gets the maximum number of bytes that can be stored on this buffer.
//        /// </summary>
//        public int Capacity
//        {
//            get
//            {
//                return m_buffer.Length;
//            }
//        }

//        /// <summary>
//        /// Gets the internal buffer that can be used to flush the data from the stream.
//        /// Do not use this method to write to the buffer.
//        /// </summary>
//        public byte[] InternalBuffer
//        {
//            get
//            {
//                return m_buffer;
//            }
//        }

//        /// <summary>
//        /// Writes the provided byte array to the buffer.
//        /// </summary>
//        /// <param name="buffer">where to write the data</param>
//        /// <param name="offset">the starting position in the <see cref="buffer"/>.</param>
//        /// <param name="count">the number of bytes to write</param>
//        /// <exception cref="InternalBufferOverflowException">occurs when trying to write
//        /// more data than the amount of free space in the buffer</exception>
//        public void Write(byte[] buffer, int offset, int count)
//        {
//            if (count <= 0)
//                return;
//            if (count > FreeSpace)
//                throw new InternalBufferOverflowException("length is too long");
//            Array.Copy(buffer, offset, m_buffer, m_position, count);
//            m_position += count;
//        }

//        /// <summary>
//        /// Clears all of the data in the buffer
//        /// </summary>
//        public void Clear()
//        {
//            m_position = 0;
//        }
//    }
//}