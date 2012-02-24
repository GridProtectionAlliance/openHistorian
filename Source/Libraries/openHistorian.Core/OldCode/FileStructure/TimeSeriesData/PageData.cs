using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileArchitecture.Historian.FileStructure
{
    public class PointReader
    {
        private CacheBuffer Cache;
        public PointReader(CacheBuffer C)
        {
            Cache = C;
        }
        public void Find(DateTime Start, DateTime Stop)
        {
            //Cache.GetPage();
        }
    }
}
