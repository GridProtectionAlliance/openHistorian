//******************************************************************************************************
//  UltraStreamWriter.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/11/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GSF.IO
{
    /// <summary>
    /// Represents an ultra high speed way to write data to a stream. 
    /// StreamWriter's methods can be slow at times.
    /// </summary>
    public class UltraStreamWriter
    {
        private const int Size = 1024;
        private const int FlushSize = 1024 - 40;
        private readonly char[] m_buffer;
        private int m_position;
        private readonly StreamWriter m_stream;
        private readonly string m_nl = Environment.NewLine;

        /// <summary>
        /// Creates a <see cref="UltraStreamWriter"/> around <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The stream to wrap</param>
        public UltraStreamWriter(StreamWriter stream)
        {
            m_buffer = new char[Size];
            m_stream = stream;
        }

        /// <summary>
        /// Writes the provided character to the stream
        /// </summary>
        /// <param name="value">the character to write.</param>
        public void Write(char value)
        {
            if (m_position < FlushSize)
                Flush();
            m_buffer[m_position] = value;
            m_position++;
        }
        
        /// <summary>
        /// Writes the provided float to the stream
        /// </summary>
        /// <param name="value">the value to write.</param>
        public void Write(float value)
        {
            if (m_position < FlushSize)
                Flush();
            m_position += value.WriteToChars(m_buffer, m_position);
        }

        /// <summary>
        /// Writes a NewLine to the stream
        /// </summary>
        public void WriteLine()
        {
            if (m_position < FlushSize)
                Flush();
            if (m_nl.Length == 2)
            {
                m_buffer[m_position] = m_nl[0];
                m_buffer[m_position + 1] = m_nl[1];
                m_position += 2;
            }
            else
            {
                m_buffer[m_position] = m_nl[0];
                m_position += 1;
            }
        }

        /// <summary>
        /// Flushes to the underlying stream
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Flush()
        {
            if (m_position > 0)
                m_stream.Write(m_buffer, 0, m_position);
            m_position = 0;
        }

    }
}
