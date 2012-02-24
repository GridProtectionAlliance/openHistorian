using System;
using System.IO;

namespace Historian
{
    public unsafe class CompressBlockUIn32Static
    {
        
        public static void Initialize(out int bitPosition, out uint oldValue, out int reservation)
        {
            bitPosition = 8;
            oldValue = 0;
            reservation = -1;
        }

        public static int AddValue(uint value, byte[] buffer, int position, ref byte meta, ref uint oldValue, ref int bitPosition, ref int reservation)
        {
            if (bitPosition == 8)
            {
                if (reservation > 0)
                    buffer[reservation] = meta;
                meta = 0;
                reservation = position;
                position++;
                bitPosition = 0;
            }
            int pos = Compress(buffer, position, oldValue, value, ref meta, ref bitPosition);
            oldValue = value;
            return pos;
        }

        public static void Flush(byte[] buffer, ref byte meta, ref uint oldValue, ref int bitPosition, ref int reservation)
        {
            if (reservation > 0)
                buffer[reservation] = meta;
            reservation = -1;
            meta = 0;
            bitPosition = 8;
        }

        static int Compress(byte[] buffer, int position, uint previousValue, uint currentValue, ref byte meta, ref int bitPosition)
        {
            uint diff = previousValue ^ currentValue;
            return Write(diff,buffer,position, ref meta, ref bitPosition);
        }

        static int Write(uint value, byte[] buffer, int position, ref byte meta, ref int bitPosition)
        {
            if (value <= 0xFF)
            {
                WriteLength(1 - 1, ref meta, ref bitPosition);
                fixed (byte* lp = buffer)
                {
                    lp[position] = (byte)value;
                }
                return position + 1;
            }
            if (value <= 0xFFFF)
            {
                WriteLength(2 - 1, ref meta, ref bitPosition);
                fixed (byte* lp = buffer)
                {
                    *(ushort*)(lp + position) = (ushort)value;
                }
                return position + 2;
            }
            if (value <= 0xFFFFFF)
            {
                WriteLength(3 - 1, ref meta, ref bitPosition);
                fixed (byte* lp = buffer)
                {
                    *(uint*)(lp + position) = (uint)value;
                }
                return position + 3;
            }
            WriteLength(4-1,ref meta, ref bitPosition);
            fixed (byte* lp = buffer)
            {
                *(uint*)(lp + position) = (uint)value;
            }
            return position + 4;
        }

        static void WriteLength(byte b, ref byte meta, ref int bitPosition)
        {
            meta |= (byte)(b << bitPosition);
            bitPosition += 2;
        }

    }
}
