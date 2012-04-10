//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.InteropServices;
//using System.IO;

//namespace openHistorian.Core.PointTypes
//{
//    [StructLayout(LayoutKind.Explicit)]
//    public struct SinglePrecisionFloatingPoint : IComparable<SinglePrecisionFloatingPoint>
//    {
//        [FieldOffset(0)]
//        public DateTime Time;
//        [FieldOffset(8)]
//        public uint Quality;
//        [FieldOffset(12)]
//        public float Value;

//        public SinglePrecisionFloatingPoint(DateTime time, uint quality, float value)
//        {
//            Time = time;
//            Quality = quality;
//            Value = value;
//        }
//        public int CompareTo(SinglePrecisionFloatingPoint other)
//        {
//            return Time.CompareTo(other.Time);
//        }
//        public void Save(DataReadWrite writer)
//        {
//            writer.Write(Time.Ticks);
//            writer.Write(Quality);
//            writer.Write(Value);
//        }
//    }
//}
