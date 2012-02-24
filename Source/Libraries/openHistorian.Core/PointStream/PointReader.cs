//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using openHistorian.Core.PointTypes;

//namespace openHistorian.Core.PointStream
//{
//    public class PointReader
//    {
//        public Guid PointID;
//        public uint LocalPointID;
//        public TypeQueueBase Time;
//        public TypeQueueBase MetaData;
//        public TypeQueueBase Data;
//        public int Position;
//        public int PositionEndOfTime;
//        public int PositionEndOfMetaData;
//        public int PositionEndOfData;
//        public PointReader(PooledMemoryStream pointDefinition)
//        {
//            LocalPointID = pointDefinition.ReadUInt32();
//            PointID = pointDefinition.ReadGuid();
//            Time=TypeQueueBase.CreatePointType(pointDefinition);
//            MetaData = TypeQueueBase.CreatePointType(pointDefinition);
//            Data = TypeQueueBase.CreatePointType(pointDefinition);

//            Position = 0;
//            PositionEndOfTime = Time.ItemCount;
//            PositionEndOfMetaData = PositionEndOfTime + MetaData.ItemCount;
//            PositionEndOfData = PositionEndOfMetaData + Data.ItemCount;
//        }

//        TypeQueueBase GetNextPosition()
//        {
//            if (Position >= PositionEndOfData)
//                Position = 0;
//            if (Position < PositionEndOfTime)
//            {
//                Position++;
//                return Time;
//            }
//            if (Position < PositionEndOfMetaData)
//            {
//                Position++;
//                return MetaData;
//            }
//            if (Position < PositionEndOfData)
//            {
//                Position++;
//                return Data;
//            }
//            throw new Exception("This point contains no data.");
//        }

//        public void Write(byte value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(short value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(int value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(long value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(ushort value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(uint value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(ulong value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(decimal value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(Guid value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(DateTime value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(float value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(double value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(bool value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(string value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(byte[] value, int offset, int count)
//        {
//            GetNextPosition().Write(value,offset,count);
//        }
//        public void Write(byte[] value)
//        {
//            GetNextPosition().Write(value);
//        }
//        public void Write(sbyte value)
//        {
//            GetNextPosition().Write(value);
//        }

//        public byte ReadByte()
//        {
//            return GetNextPosition().ReadByte();
//        }
//        public short ReadShort()
//        {
//            return GetNextPosition().ReadShort();
//        }
//        public int ReadInt32()
//        {
//            return GetNextPosition().ReadInt32();
//        }
//        public long ReadInt64()
//        {
//            return GetNextPosition().ReadInt64();
//        }
//        public ushort ReadUInt16()
//        {
//            return GetNextPosition().ReadUInt16();
//        }
//        public uint ReadUInt32()
//        {
//            return GetNextPosition().ReadUInt32();
//        }
//        public ulong ReadUInt64()
//        {
//            return GetNextPosition().ReadUInt64();
//        }
//        public decimal ReadDecimal()
//        {
//            return GetNextPosition().ReadDecimal();
//        }
//        public Guid ReadGuid()
//        {
//            return GetNextPosition().ReadGuid();
//        }
//        public DateTime ReadDateTime()
//        {
//            return GetNextPosition().ReadDateTime();
//        }
//        public float ReadSingle()
//        {
//            return GetNextPosition().ReadSingle();
//        }
//        public double ReadDouble()
//        {
//            return GetNextPosition().ReadDouble();
//        }
//        public bool ReadBoolean()
//        {
//            return GetNextPosition().ReadBoolean();
//        }
//        public string ReadString()
//        {
//            return GetNextPosition().ReadString();
//        }
//        public int Read(byte[] value, int offset, int count)
//        {
//            return GetNextPosition().Read(value,offset,count);
//        }
//        public sbyte ReadSByte()
//        {
//            return GetNextPosition().ReadSByte();
//        }
//    }
//}
