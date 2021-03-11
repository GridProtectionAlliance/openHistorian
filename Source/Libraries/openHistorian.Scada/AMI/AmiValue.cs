using System;
using GSF.IO;
using GSF.Snap;

namespace openHistorian.Scada.AMI
{
    public class AmiValue
        : SnapTypeBase<AmiValue>
    {

        public ulong CollectedTime;
        public int TableId;
        public byte DataLength;
        public byte[] Data = new byte[40];

        public override Guid GenericTypeGuid =>
            // {12E31741-8F7F-4027-B10C-68B16387CE3B}
            new Guid(0x12e31741, 0x8f7f, 0x4027, 0xb1, 0x0c, 0x68, 0xb1, 0x63, 0x87, 0xce, 0x3b);

        public override int Size => 53;

        public override void CopyTo(AmiValue destination)
        {
            destination.CollectedTime = CollectedTime;
            destination.TableId = TableId;
            destination.DataLength = DataLength;
            Data.CopyTo(destination.Data, 0);
        }

        public override int CompareTo(AmiValue other)
        {
            int rv;
            rv = CollectedTime.CompareTo(other.CollectedTime); if (rv != 0) return rv;
            rv = TableId.CompareTo(other.TableId); if (rv != 0) return rv;
            rv = DataLength.CompareTo(other.DataLength); if (rv != 0) return rv;
            for (int x = 0; x < DataLength; x++)
            {
                if (Data[x] != other.Data[x])
                    return Data[x].CompareTo(other.Data[x]);
            }
            return 0;
        }

        public override void SetMin()
        {
            CollectedTime = 0;
            TableId = int.MinValue;
            DataLength = 0;
            Array.Clear(Data, 0, Data.Length);
        }

        public override void SetMax()
        {
            CollectedTime = ulong.MaxValue;
            TableId = int.MaxValue;
            DataLength = (byte)Data.Length;
            for (int x = 0; x < Data.Length; x++)
            {
                Data[x] = 255;
            }
        }

        public override void Clear()
        {
            SetMin();
            TableId = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            CollectedTime = stream.ReadUInt64();
            TableId = stream.ReadInt32();
            DataLength = stream.ReadUInt8();
            stream.Read(Data, 0, Data.Length);
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(CollectedTime);
            stream.Write(TableId);
            stream.Write(DataLength);
            stream.Write(Data, 0, Data.Length);
        }
    }
}
