using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.IO;

namespace openHistorian.V2.Streaming
{
    interface IOutboundBuffer
    {
        int SegmentSize { get; set; }
        void Flush(bool flushToStream = true, long position = -1);

        long Position { get; set; }
        void Copy(long source, long destination, int length);
        void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift);
        void RemoveBytes(int numberOfBytes, int lengthOfValidDataToShift);
        void Write(sbyte value);
        void Write(bool value);
        void Write(ushort value);
        void Write(uint value);
        void Write(ulong value);
        void Write(byte value);
        void Write(short value);
        void Write(int value);
        void Write(float value);
        void Write(long value);
        void Write(double value);
        void Write(DateTime value);
        void Write(decimal value);
        void Write(Guid value);
        void Write7Bit(uint value);
        void Write7Bit(ulong value);
        void Write(byte[] value, int offset, int count);
        void Write(string value);
        sbyte ReadSByte();
        bool ReadBoolean();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
        byte ReadByte();
        short ReadInt16();
        int ReadInt32();
        float ReadSingle();
        long ReadInt64();
        double ReadDouble();
        DateTime ReadDateTime();
        decimal ReadDecimal();
        Guid ReadGuid();
        uint Read7BitUInt32();
        ulong Read7BitUInt64();
        int Read(byte[] value, int offset, int count);
    }
}
