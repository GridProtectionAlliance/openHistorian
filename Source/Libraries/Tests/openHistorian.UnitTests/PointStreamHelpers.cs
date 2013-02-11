using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GSF;
using openHistorian.Archive;

namespace openHistorian
{

    public class PointStreamSequential : IStream256
    {
        ulong m_start;
        int m_count;
        Func<ulong, ulong> m_key1;
        Func<ulong, ulong> m_key2;
        Func<ulong, ulong> m_value1;
        Func<ulong, ulong> m_value2;
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

        public bool Read(out ulong key1, out ulong key2, out ulong value1, out ulong value2)
        {
            if (m_count <= 0)
            {
                key1 = 0;
                key2 = 0;
                value1 = 0;
                value2 = 0;
                return false;
            }
            m_count--;
            key1 = m_key1(m_start);
            key2 = m_key2(m_start);
            value1 = m_value1(m_start);
            value2 = m_value2(m_start);
            m_start++;
            return true;
        }
    }

    public static class PointStreamHelpers
    {
        public static bool AreEqual(this IStream256 source, IStream256 destination)
        {
            bool isValidA, isValidB;
            ulong akey1, akey2, avalue1, avalue2;
            ulong bkey1, bkey2, bvalue1, bvalue2;

            while (true)
            {
                isValidA = source.Read(out akey1, out akey2, out avalue1, out avalue2);
                isValidB = destination.Read(out bkey1, out bkey2, out bvalue1, out bvalue2);

                if (isValidA != isValidB)
                    return false;
                if (isValidA && isValidB)
                {
                    if (akey1 != bkey1)
                        return false;
                    if (akey2 != bkey2)
                        return false;
                    if (avalue1 != bvalue1)
                        return false;
                    if (avalue2 != bvalue2)
                        return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static long Count(this IStream256 stream)
        {
            long x = 0;
            ulong akey1, akey2, avalue1, avalue2;
            while (stream.Read(out akey1, out akey2, out avalue1, out avalue2))
                x++;
            return x;
        }
        public static long Count(this ArchiveFile stream)
        {
            using (var read = stream.BeginRead())
            {
                var scan = read.GetTreeScanner();
                scan.SeekToKey(0,0);
                return scan.Count();
            }
        }
    }
}
