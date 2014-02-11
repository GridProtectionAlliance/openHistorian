using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GSF.IO
{
    /// <summary>
    /// Represents an ultra high speed way to write data to a stream. 
    /// StreamWriter's methods can be slow at times.
    /// </summary>
    public class UltraStreamWriter
    {
        const int Size = 1024;
        const int FlushSize = 1024 - 40;
        char[] m_buffer;
        int m_position;
        StreamWriter m_stream;
        string nl = Environment.NewLine;
        public UltraStreamWriter(StreamWriter stream)
        {
            m_buffer = new char[Size];
            m_stream = stream;
        }

        public void Write(char value)
        {
            if (m_position < FlushSize)
                Flush();
            m_buffer[m_position] = value;
            m_position++;
        }
        
        public void Write(float value)
        {
            if (m_position < FlushSize)
                Flush();
            m_position += value.WriteToChars(m_buffer, m_position);
        }

        public void WriteLine()
        {
            if (m_position < FlushSize)
                Flush();
            if (nl.Length == 2)
            {
                m_buffer[m_position] = nl[0];
                m_buffer[m_position + 1] = nl[1];
                m_position += 2;
            }
            else
            {
                m_buffer[m_position] = nl[0];
                m_position += 1;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Flush()
        {
            if (m_position > 0)
                m_stream.Write(m_buffer, 0, m_position);
            m_position = 0;
        }

    }
}
