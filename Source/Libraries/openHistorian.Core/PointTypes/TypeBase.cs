using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.PointTypes
{
    public abstract class TypeQueueBase
    {
        static TypeQueueBase[] s_Types;
        static NotSupportedException notSupportedType;
        static TypeQueueBase()
        {
            notSupportedType = new NotSupportedException("The type requested is not a recgonized type of this class");
            s_Types = new TypeQueueBase[1];
            s_Types[0]= new EmptyQueue();
        }
        public static TypeQueueBase CreatePointType(PointDataTypes Type, PointDataTypes[] NestedTypes=null)
        {
            return s_Types[(byte)Type].Clone(NestedTypes);
        }
        public static TypeQueueBase CreatePointType(PooledMemoryStream pointDefinition)
        {
            return null;
            //return s_Types[pointDefinition.ReadByte()].Clone(pointDefinition);
        }
        
        public abstract byte TypeCode{ get; }
        public abstract void Skip();
        public abstract int ItemCount { get; }
        public abstract TypeQueueBase Clone(PooledMemoryStream pointDefinition);
        public abstract TypeQueueBase Clone(PointDataTypes[] NestedTypes);

        public void Write(byte value)
        {
            throw notSupportedType;
        }
        public void Write(short value)
        {
            throw notSupportedType;
        }
        public void Write(int value)
        {
            throw notSupportedType;
        }
        public void Write(long value)
        {
            throw notSupportedType;
        }
        public void Write(ushort value)
        {
            throw notSupportedType;
        }
        public void Write(uint value)
        {
            throw notSupportedType;
        }
        public void Write(ulong value)
        {
            throw notSupportedType;
        }
        public void Write(decimal value)
        {
            throw notSupportedType;
        }
        public void Write(Guid value)
        {
            throw notSupportedType;
        }
        public void Write(DateTime value)
        {
            throw notSupportedType;
        }
        public void Write(float value)
        {
            throw notSupportedType;
        }
        public void Write(double value)
        {
            throw notSupportedType;
        }
        public void Write(bool value)
        {
            throw notSupportedType;
        }
        public void Write(string value)
        {
            throw notSupportedType;
        }
        public void Write(byte[] value, int offset, int count)
        {
            throw notSupportedType;
        }
        public void Write(byte[] value)
        {
            Write(value, 0, value.Length);
        }
        public void Write(sbyte value)
        {
            throw notSupportedType;
        }
     
        public byte ReadByte()
        {
            throw notSupportedType;
        }
        public short ReadShort()
        {
            throw notSupportedType;
        }
        public int ReadInt32()
        {
            throw notSupportedType;
        }
        public long ReadInt64()
        {
            throw notSupportedType;
        }
        public ushort ReadUInt16()
        {
            throw notSupportedType;
        }
        public uint ReadUInt32()
        {
            throw notSupportedType;
        }
        public ulong ReadUInt64()
        {
            throw notSupportedType;
        }
        public decimal ReadDecimal()
        {
            throw notSupportedType;
        }
        public Guid ReadGuid()
        {
            throw notSupportedType;
        }
        public DateTime ReadDateTime()
        {
            throw notSupportedType;
        }
        public float ReadSingle()
        {
            throw notSupportedType;
        }
        public double ReadDouble()
        {
            throw notSupportedType;
        }
        public bool ReadBoolean()
        {
            throw notSupportedType;
        }
        public string ReadString()
        {
            throw notSupportedType;
        }
        public int Read(byte[] value, int offset, int count)
        {
            throw notSupportedType;
        }
        public sbyte ReadSByte()
        {
            throw notSupportedType;
        }
    }
}
