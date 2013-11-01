//******************************************************************************************************
//  BinaryStreamWrapper.cs - Gbtc
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.IO;

namespace GSF.IO
{
    public class BinaryStreamWrapper : BinaryStreamBase
    {
        private readonly Stream m_stream;

        public BinaryStreamWrapper(Stream stream)
        {
            m_stream = stream;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
        }

        /// <summary>
        /// Gets/Sets the current position for the stream.
        /// <remarks>It is important to use this to Get/Set the position from the underlying stream since 
        /// this class buffers the results of the query.  Setting this field does not gaurentee that
        /// the underlying stream will get set. Call FlushToUnderlyingStream to acomplish this.</remarks>
        /// </summary>
        public override long Position
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

        public override void Write(byte value)
        {
            m_stream.WriteByte(value);
        }

        public override void Write(byte[] value, int offset, int count)
        {
            m_stream.Write(value, offset, count);
        }

        public override byte ReadByte()
        {
            int value = m_stream.ReadByte();
            if (value < 0)
                throw new EndOfStreamException();
            return (byte)value;
        }

        public override int Read(byte[] value, int offset, int count)
        {
            int len = m_stream.Read(value, offset, count);
            if (len != count)
                throw new EndOfStreamException();
            return len;
        }
    }
}