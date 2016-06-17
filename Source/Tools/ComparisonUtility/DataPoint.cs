using System;
using System.Runtime.CompilerServices;
using GSF;

namespace ComparisonUtility
{
    public class DataPoint
    {
        public float ValueAsSingle
        {
            get
            {
                return BitMath.ConvertToSingle(Value);
            }
            set
            {
                Value = BitMath.ConvertToUInt64(value);
            }
        }

        public ulong Timestamp;
        public ulong PointID;
        public ulong Value;
        public ulong Flags;

        public void Clone(DataPoint destination)
        {
            destination.Timestamp = Timestamp;
            destination.PointID = PointID;
            destination.Value = Value;
            destination.Flags = Flags;
        }

        // Truncates timestamp to millisecond resolution
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundTimestamp(ulong timestamp, int frameRate) => (ulong)Ticks.RoundToSubsecondDistribution((long)timestamp, frameRate).Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTimestamps(ulong left, ulong right, int frameRate) => RoundTimestamp(left, frameRate).CompareTo(RoundTimestamp(right, frameRate));
    }
}