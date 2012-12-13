using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN
{
    public interface IMeasurement
    {
        int PointCounts { get; }
        int[] PointBreaks { get; }
        KeyValuePair<double, double> this[int index] { get; }

    }
}
