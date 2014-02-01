using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Engine;
using GSF.SortedTreeStore.Filters;
using openHistorian.Collections;

namespace openHistorian.Data
{
    public class FilterTimestamp
    {
        public KeyFilter<HistorianKey> FromRange(DateTime firstTime, DateTime lastTime)
        {
            KeyFilter<HistorianKey> filter = new KeyFilter<HistorianKey>();
            filter.KeySeekFilter = TimestampFilter.CreateFromRange<HistorianKey>(firstTime, lastTime);
            return filter;
        }

        public KeyFilter<HistorianKey> FromRange(ulong firstTime, ulong lastTime)
        {
            KeyFilter<HistorianKey> filter = new KeyFilter<HistorianKey>();
            filter.KeySeekFilter = TimestampFilter.CreateFromRange<HistorianKey>(firstTime, lastTime);
            return filter;
        }

        public KeyFilter<HistorianKey> FromDate(ulong time)
        {
            KeyFilter<HistorianKey> filter = new KeyFilter<HistorianKey>();
            filter.KeySeekFilter = TimestampFilter.CreateFromRange<HistorianKey>(time, time);
            return filter;
        }

        public KeyFilter<HistorianKey> FromDate(DateTime time)
        {
            KeyFilter<HistorianKey> filter = new KeyFilter<HistorianKey>();
            filter.KeySeekFilter = TimestampFilter.CreateFromRange<HistorianKey>(time, time);
            return filter;
        }


        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <param name="interval">the exact interval to do the scan</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               Interval = 0.1 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static KeyFilter<HistorianKey> CreateFromIntervalData(DateTime firstTime, DateTime lastTime, TimeSpan interval, TimeSpan tolerance)
        {
            KeyFilter<HistorianKey> filter = new KeyFilter<HistorianKey>();
            filter.KeySeekFilter = TimestampFilter.CreateFromIntervalData<HistorianKey>((ulong)firstTime.Ticks, (ulong)lastTime.Ticks, (ulong)interval.Ticks, (ulong)interval.Ticks, (ulong)tolerance.Ticks);
            return filter;
        }

        /// <summary>
        /// Creates a filter over a single date range. (Inclusive list)
        /// </summary>
        /// <param name="firstTime">the first time if the query (inclusive)</param>
        /// <param name="lastTime">the last time of the query (inclusive)</param>
        /// <param name="mainInterval">the smallest interval that is exact</param>
        /// <param name="subInterval">the interval that will be parsed. Possible to be rounded</param>
        /// <param name="tolerance">the width of every window</param>
        /// <returns>A <see cref="KeySeekFilterBase{TKey}"/> that will be able to do this parsing</returns>
        /// <remarks>
        /// Example uses. FirstTime = 1/1/2013. LastTime = 1/2/2013. 
        ///               Interval = 0.1 seconds.
        ///               Tolerance = 0.001 seconds.
        /// </remarks>
        public static KeyFilter<HistorianKey> CreateFromIntervalData(DateTime firstTime, DateTime lastTime, TimeSpan mainInterval, TimeSpan subInterval, TimeSpan tolerance)
        {
            KeyFilter<HistorianKey> filter = new KeyFilter<HistorianKey>();
            filter.KeySeekFilter = TimestampFilter.CreateFromIntervalData<HistorianKey>((ulong)firstTime.Ticks, (ulong)lastTime.Ticks, (ulong)mainInterval.Ticks, (ulong)subInterval.Ticks, (ulong)tolerance.Ticks);
            return filter;
        }


    }
}
