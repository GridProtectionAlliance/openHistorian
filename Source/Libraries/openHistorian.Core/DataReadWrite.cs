using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace openHistorian.Core
{
    public unsafe class DataReadWrite
    {
        public int Position;
        public byte[] Buffer;
        Stream m_baseStream;
        public DataReadWrite(Stream baseStream)
        {
            m_baseStream = baseStream;
            Buffer = new byte[4096];
        }
        public void Flush()
        {
            m_baseStream.Write(Buffer, 0, Position);
            Position = 0;
        }
        public int RemainingLenght
        {
            get
            {
                return Buffer.Length - Position;
            }
        }
  
        public void Write(uint value)
        {
            if (RemainingLenght < 4)
                Flush();
            fixed (byte* lp = Buffer)
            {
                uint* lpp = (uint*)(lp + Position);
                *lpp = value;
            }
            Position += 4;
        }
        public void Write(DateTime value)
        {
            if (RemainingLenght < 8)
                Flush();
            fixed (byte* lp = Buffer)
            {
                DateTime* lpp = (DateTime*)(lp + Position);
                *lpp = value;
            }
            Position += 8;
        }
        public void Write(float value)
        {
            if (RemainingLenght < 4)
                Flush();
            fixed (byte* lp = Buffer)
            {
                float* lpp = (float*)(lp + Position);
                *lpp = value;
            }
            Position += 4;
        }
        public void Write(long value)
        {
            if (RemainingLenght < 8)
                Flush();
            fixed (byte* lp = Buffer)
            {
                long* lpp = (long*)(lp + Position);
                *lpp = value;
            }
            Position += 8;
        }
        public void Write(Guid value)
        {
            if (RemainingLenght < 16)
                Flush();
            fixed (byte* lp = Buffer)
            {
                Guid* lpp = (Guid*)(lp + Position);
                *lpp = value;
            }
            Position += 16;
        }
        
    }
}
