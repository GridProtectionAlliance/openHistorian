using System;
using GSF.IO;
using GSF.Snap.Types;

namespace openHistorian.Scada.AMI
{
    public class AmiKey
            : TimestampPointIDBase<AmiKey>
    {
        public ulong CollectedTime;

        public int TableId;

        public ulong JobRunTime
        {
            get => Timestamp;
            set => Timestamp = value;
        }

        public ulong DeviceCode
        {
            get => PointID;
            set => PointID = value;
        }

        public DateTime JobRunTimeAsDate
        {
            get => new DateTime((long)Timestamp);
            set => Timestamp = (ulong)value.Ticks;
        }

        public DateTime CollectedTimeAsDate
        {
            get => new DateTime((long)CollectedTime);
            set => CollectedTime = (ulong)value.Ticks;
        }

        public override Guid GenericTypeGuid =>
            // {CA57E35C-BCBD-4E95-89F4-419A023FF09E}
            new Guid(0xca57e35c, 0xbcbd, 0x4e95, 0x89, 0xf4, 0x41, 0x9a, 0x02, 0x3f, 0xf0, 0x9e);

        public override int Size => 28;

        /// <summary>
        /// Sets all of the values in this class to their minimum value
        /// </summary>
        public override void SetMin()
        {
            Timestamp = ulong.MinValue;
            PointID = ulong.MinValue;
            TableId = int.MinValue;
            CollectedTime = ulong.MinValue;
        }

        /// <summary>
        /// Sets all of the values in this class to their maximum value
        /// </summary>
        public override void SetMax()
        {
            Timestamp = ulong.MaxValue;
            PointID = ulong.MaxValue;
            TableId = int.MaxValue;
            CollectedTime = ulong.MaxValue;
        }

        /// <summary>
        /// Sets the key to the default values.
        /// </summary>
        public override void Clear()
        {
            //SetMin();
            Timestamp = 0;
            PointID = 0;
            TableId = 0;
            CollectedTime = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Timestamp = stream.ReadUInt64();
            PointID = stream.ReadUInt64();
            TableId = stream.ReadInt32();
            CollectedTime = stream.ReadUInt64();
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Timestamp);
            stream.Write(PointID);
            stream.Write(TableId);
            stream.Write(CollectedTime);
        }

        public override void CopyTo(AmiKey destination)
        {
            destination.Timestamp = Timestamp;
            destination.PointID = PointID;
            destination.TableId = TableId;
            destination.CollectedTime = CollectedTime;
        }

        /// <summary>
        /// Compares the current instance to <see cref="other"/>.
        /// </summary>
        /// <param name="other">the key to compare to</param>
        /// <returns></returns>
        public override int CompareTo(AmiKey other)
        {
            if (Timestamp < other.Timestamp)
                return -1;
            if (Timestamp > other.Timestamp)
                return 1;
            if (PointID < other.PointID)
                return -1;
            if (PointID > other.PointID)
                return 1;
            if (TableId < other.TableId)
                return -1;
            if (TableId > other.TableId)
                return 1;
            if (CollectedTime < other.CollectedTime)
                return -1;
            if (CollectedTime > other.CollectedTime)
                return 1;
            return 0;
        }


        #region [ Optional Overrides ]

        // Read(byte*)
        // Write(byte*)
        // IsLessThan(T)
        // IsEqualTo(T)
        // IsGreaterThan(T)
        // IsLessThanOrEqualTo(T)
        // IsBetween(T,T)

        public override unsafe void Read(byte* stream)
        {
            Timestamp = *(ulong*)stream;
            PointID = *(ulong*)(stream + 8);
            TableId = *(int*)(stream + 16);
            CollectedTime = *(ulong*)(stream + 20);
        }
        public override unsafe void Write(byte* stream)
        {
            *(ulong*)stream = Timestamp;
            *(ulong*)(stream + 8) = PointID;
            *(int*)(stream + 16) = TableId;
            *(ulong*)(stream + 20) = CollectedTime;
        }
        public override bool IsLessThan(AmiKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp < right.Timestamp;

            if (PointID != right.PointID)
                return PointID < right.PointID;

            if (TableId != right.TableId)
                return TableId < right.TableId;

            return CollectedTime < right.CollectedTime;
        }
        public override bool IsEqualTo(AmiKey right)
        {
            return Timestamp == right.Timestamp && PointID == right.PointID &&
                TableId == right.TableId && CollectedTime == right.CollectedTime;
        }
        public override bool IsGreaterThan(AmiKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            if (PointID != right.PointID)
                return PointID > right.PointID;

            if (TableId != right.TableId)
                return TableId > right.TableId;

            return CollectedTime > right.CollectedTime;
        }
        public override bool IsGreaterThanOrEqualTo(AmiKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            if (PointID != right.PointID)
                return PointID > right.PointID;

            if (TableId != right.TableId)
                return TableId > right.TableId;

            return CollectedTime >= right.CollectedTime;
        }

       

        //public override bool IsBetween(HistorianKey lowerBounds, HistorianKey upperBounds)
        //{
        //    
        //}

        //public override SortedTreeTypeMethods<HistorianKey> CreateValueMethods()
        //{
        //    
        //}

        #endregion
    }
}
