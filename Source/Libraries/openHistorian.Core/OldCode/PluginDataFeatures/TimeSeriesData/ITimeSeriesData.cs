using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.PluginDataFeatures.TimeSeriesData
{
    interface ITimeSeriesData : IPluginDataFeature
    {
        TimeSpan TimeBucket { get; }
        void AddData();
        void GetData();
    }
}
