using System;
using System.Collections.Generic;

namespace openHistorian.V2.Providers
{
    public interface ITimeSeriesDataProvider
    {
        string InstanceName { get; }
        void AddPoint(long date, int pointId, int flags, float data);
        void GetData(Func<long, int, int, float, bool> callback, long startDate, long stopDate);
        void GetData(Func<long, int, int, float, bool> callback, int pointId, long startDate, long stopDate);
        void GetData(Func<long, int, int, float, bool> callback, IEnumerable<int> pointId, long startDate, long stopDate);
    }
}
