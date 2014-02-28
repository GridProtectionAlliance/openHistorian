using System;
using System.Collections;
using GSF.IO;
using GSF.SortedTreeStore.Types;
using openHistorian.SortedTreeStore.Types.CustomCompression.Ts;

namespace openHistorian.Scada.AMI
{
    public class AmiKey
        : TimestampPointIDBase<AmiKey>
    {
        /// <summary>
        /// Conviently type cast the Timestamp as <see cref="DateTime"/>.
        /// </summary>
        public DateTime TimestampAsDate
        {
            get
            {
                return new DateTime((long)Timestamp);
            }
            set
            {
                Timestamp = (ulong)value.Ticks;
            }
        }

        public String DeviceId
        {
            get
            {
                return PointID.ToString();
            }
            set
            {
                PointID = ulong.Parse(value);
            }
        }

        public override Guid GenericTypeGuid
        {
            get
            {
                // {E311CA21-9DA4-4F6A-8E7B-37D9594604BE}
                return new Guid(0xe311ca21, 0x9da4, 0x4f6a, 0x8e, 0x7b, 0x37, 0xd9, 0x59, 0x46, 0x04, 0xbe);
            }
        }

        public override int Size
        {
            get
            {
                return 16;
            }
        }

        /// <summary>
        /// Sets all of the values in this class to their minimum value
        /// </summary>
        public override void SetMin()
        {
            Timestamp = 0;
            PointID = 0;
        }

        /// <summary>
        /// Sets all of the values in this class to their maximum value
        /// </summary>
        public override void SetMax()
        {
            Timestamp = ulong.MaxValue;
            PointID = ulong.MaxValue;
        }

        /// <summary>
        /// Sets the key to the default values.
        /// </summary>
        public override void Clear()
        {
            Timestamp = 0;
            PointID = 0;
        }

        public override void Read(BinaryStreamBase stream)
        {
            Timestamp = stream.ReadUInt64();
            PointID = stream.ReadUInt64();
        }

        public override void Write(BinaryStreamBase stream)
        {
            stream.Write(Timestamp);
            stream.Write(PointID);
        }

        public override void CopyTo(AmiKey destination)
        {
            destination.Timestamp = Timestamp;
            destination.PointID = PointID;
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
        }
        public override unsafe void Write(byte* stream)
        {
            *(ulong*)stream = Timestamp;
            *(ulong*)(stream + 8) = PointID;
        }
        public override bool IsLessThan(AmiKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp < right.Timestamp;

            //Implide left.Timestamp == right.Timestamp
            return PointID < right.PointID;
        }
        public override bool IsEqualTo(AmiKey right)
        {
            return Timestamp == right.Timestamp && PointID == right.PointID;
        }
        public override bool IsGreaterThan(AmiKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            //Implide left.Timestamp == right.Timestamp
            return PointID > right.PointID;
        }
        public override bool IsGreaterThanOrEqualTo(AmiKey right)
        {
            if (Timestamp != right.Timestamp)
                return Timestamp > right.Timestamp;

            //Implide left.Timestamp == right.Timestamp
            return PointID >= right.PointID;

        }

        public override IEnumerable GetEncodingMethods()
        {
            var list = new ArrayList();
            list.Add(new CreateAmiCombinedEncoding());
            return list;
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
