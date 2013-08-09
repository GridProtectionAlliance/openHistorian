using System;
using openHistorian.Archive;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian
{
    public class PointStreamSequential
        : TreeStream<HistorianKey, HistorianValue>
    {
        private ulong m_start;
        private int m_count;
        private readonly Func<ulong, ulong> m_key1;
        private readonly Func<ulong, ulong> m_key2;
        private readonly Func<ulong, ulong> m_value1;
        private readonly Func<ulong, ulong> m_value2;

        public PointStreamSequential(int start, int count)
            : this(start, count, x => 1 * x, x => 2 * x, x => 3 * x, x => 4 * x)
        {
        }

        public PointStreamSequential(int start, int count, Func<ulong, ulong> key1, Func<ulong, ulong> key2,
                                     Func<ulong, ulong> value1, Func<ulong, ulong> value2)
        {
            m_start = (ulong)start;
            m_count = count;
            m_key1 = key1;
            m_key2 = key2;
            m_value1 = value1;
            m_value2 = value2;
        }

        public override bool Read()
        {
            if (m_count <= 0)
            {
                CurrentKey.Timestamp = 0;
                CurrentKey.PointID = 0;
                CurrentValue.Value3 = 0;
                CurrentValue.Value1 = 0;
                return false;
            }
            m_count--;
            CurrentKey.Timestamp = m_key1(m_start);
            CurrentKey.PointID = m_key2(m_start);
            CurrentValue.Value3 = m_value1(m_start);
            CurrentValue.Value1 = m_value2(m_start);
            m_start++;
            return true;
        }
    }

    public static class PointStreamHelpers
    {
        //public static bool AreEqual(this IStream256 source, IStream256 destination)
        //{
        //    bool isValidA, isValidB;
        //    ulong akey1, akey2, avalue1, avalue2;
        //    ulong bkey1, bkey2, bvalue1, bvalue2;

        //    while (true)
        //    {
        //        isValidA = source.Read(out akey1, out akey2, out avalue1, out avalue2);
        //        isValidB = destination.Read(out bkey1, out bkey2, out bvalue1, out bvalue2);

        //        if (isValidA != isValidB)
        //            return false;
        //        if (isValidA && isValidB)
        //        {
        //            if (akey1 != bkey1)
        //                return false;
        //            if (akey2 != bkey2)
        //                return false;
        //            if (avalue1 != bvalue1)
        //                return false;
        //            if (avalue2 != bvalue2)
        //                return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}

        //public static long Count(this IStream256 stream)
        //{
        //    long x = 0;
        //    ulong akey1, akey2, avalue1, avalue2;
        //    while (stream.Read(out akey1, out akey2, out avalue1, out avalue2))
        //        x++;
        //    return x;
        //}
        //public static long Count(this Stream256Base stream)
        //{
        //    long x = 0;
        //    while (stream.Read())
        //        x++;
        //    return x;
        //}

        public static long Count<TKey, TValue>(this TreeStream<TKey, TValue> stream)
            where TKey : class, new()
            where TValue : class, new()
        {
            long x = 0;
            while (stream.Read())
                x++;
            return x;
        }

        public static long Count(this ArchiveTable<HistorianKey, HistorianValue> stream)
        {
            using (ArchiveTableReadSnapshot<HistorianKey, HistorianValue> read = stream.BeginRead())
            {
                TreeScannerBase<HistorianKey, HistorianValue> scan = read.GetTreeScanner();
                scan.SeekToStart();
                return scan.Count();
            }
        }
    }
}