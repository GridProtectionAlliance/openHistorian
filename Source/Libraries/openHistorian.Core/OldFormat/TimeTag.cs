using System;

namespace openHistorian
{
    internal class TimeTag
    {
        private static DateTime Jan11995 = DateTime.Parse("01/01/1995");

        public static DateTime Convert(double D)
        {
            return Jan11995.AddSeconds(D);
        }

        public static DateTime Convert(long D)
        {
            return Jan11995.AddTicks(D * 0x2710L);
        }
    }
}