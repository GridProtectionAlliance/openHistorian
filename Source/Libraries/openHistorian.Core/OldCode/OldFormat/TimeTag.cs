using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian
{
    class TimeTag
    {
        static DateTime Jan11995 = DateTime.Parse("01/01/1995");
        public static System.DateTime Convert(double D)
        {
            return Jan11995.AddSeconds(D);
        }
        public static System.DateTime Convert(long D)
        {
            return Jan11995.AddTicks(D * 0x2710L);
        }
    }
}
