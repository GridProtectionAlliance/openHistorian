//******************************************************************************************************
//  CircularBuffer.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;

namespace openHistorian.Communications
{
    /// <summary>
    /// Provides a fixed size circular byte buffer
    /// </summary>
    public class CircularBuffer
    {
        byte[] m_buffer;
        int m_capacity;
        int m_head;
        int m_tail;
        int m_freeSpace;

        public CircularBuffer(int capacity)
        {
            m_capacity = capacity;
            m_head = 0;
            m_tail = 0;
            m_freeSpace = capacity;
            m_buffer = new byte[capacity];
        }

        /// <summary>
        /// Gets the number of bytes that are available 
        /// for reading from the <see cref="CircularBuffer"/>.
        /// </summary>
        public int DataAvailable
        {
            get
            {
                return Capacity - FreeSpace;
            }
        }
        /// <summary>
        /// Gets the number of bytes that can be written to this <see cref="CircularBuffer"/>.
        /// </summary>
        public int FreeSpace
        {
            get
            {
                return m_freeSpace;
            }
        }
        /// <summary>
        /// Gets the maximum number of bytes that can be stored on this buffer.
        /// </summary>
        public int Capacity
        {
            get
            {
                return m_capacity;
            }
        }

        /// <summary>
        /// Reads the data from the circular buffer.
        /// </summary>
        /// <param name="buffer">where to write the data</param>
        /// <param name="offset">the starting position in the <see cref="buffer"/>.</param>
        /// <param name="count">the number of bytes to read</param>
        /// <returns>the number of bytes read.</returns>
        /// <remarks>
        /// Will always read the number of bytes specified by <see cref="count"/>
        /// unless the read empties the buffer. In this case it will return 
        /// all of the bytes in the buffer.
        /// </remarks>
        public int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, DataAvailable);
            m_freeSpace += count;

            int remainAtEnd = m_capacity - m_head;
            if (count <= remainAtEnd)
            {
                Array.Copy(m_buffer, m_head, buffer, offset, count);
                m_head += count;
            }
            else
            {
                Array.Copy(m_buffer, m_head, buffer, offset, remainAtEnd); //ReadToEnd
                Array.Copy(m_buffer, 0, buffer, offset + remainAtEnd, count - remainAtEnd); //ReadAtBeginning
                m_head = count - remainAtEnd;
            }
            if (m_head == Capacity)
                m_head = 0;
            if (m_freeSpace == Capacity)
            {
                m_head = 0;
                m_tail = 0;
            }
            return count;
        }
       
        /// <summary>
        /// Writes the provided byte array to the circular buffer.
        /// </summary>
        /// <param name="buffer">where to write the data</param>
        /// <param name="offset">the starting position in the <see cref="buffer"/>.</param>
        /// <param name="count">the number of bytes to write</param>
        /// <exception cref="InternalBufferOverflowException">occurs when trying to write
        /// more data than the amount of free space in the buffer</exception>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (count <= 0)
                return;
            if (count > FreeSpace)
                throw new InternalBufferOverflowException("Buffer is full");
            int remainAtEnd = m_capacity - m_tail;
            m_freeSpace -= count;

            if (count <= remainAtEnd)
            {
                Array.Copy(buffer, offset, m_buffer, m_tail, count);
                m_tail += count;
            }
            else
            {
                Array.Copy(buffer, offset, m_buffer, m_tail, remainAtEnd);
                Array.Copy(buffer, offset + remainAtEnd, m_buffer, 0, count - remainAtEnd);
                m_tail = count - remainAtEnd;
            }
            if (m_tail == m_capacity)
                m_tail = 0;

        }

    }
}
