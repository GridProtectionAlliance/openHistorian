using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace openHistorian.IO
{
    public class BinaryStreamWrapper : BinaryStreamBase
    {
        Stream m_stream;
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
