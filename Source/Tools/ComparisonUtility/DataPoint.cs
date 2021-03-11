using System.Runtime.CompilerServices;
using GSF;

namespace ComparisonUtility
{
    public class DataPoint
    {
        public ulong Timestamp;
        public ulong PointID;
        public ulong Value;
        public ulong Flags;

        public float ValueAsSingle
        {
            get => BitConvert.ToSingle(Value);
            set => Value = BitConvert.ToUInt64(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() => Timestamp == 0 && PointID == 0 && Value == 0 && Flags == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Timestamp = 0;
            PointID = 0;
            Value = 0;
            Flags = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clone(DataPoint dataPoint)
        {
            dataPoint.Timestamp = Timestamp;
            dataPoint.PointID = PointID;
            dataPoint.Value = Value;
            dataPoint.Flags = Flags;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataPoint Clone()
        {
            DataPoint dataPoint = new DataPoint();
            Clone(dataPoint);
            return dataPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundTimestamp(ulong timestamp, int frameRate) => (ulong)Ticks.RoundToSubsecondDistribution((long)timestamp, frameRate).Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTimestamps(ulong left, ulong right, int frameRate) => RoundTimestamp(left, frameRate).CompareTo(RoundTimestamp(right, frameRate));
    }
}