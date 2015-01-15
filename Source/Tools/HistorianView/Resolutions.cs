using GSF;

namespace HistorianView
{
    public static class Resolutions
    {
        public static readonly string[] Names =
        {
            /* 0 */ "Full",
            /* 1 */ "10 per Second",
            /* 2 */ "Every Second",
            /* 3 */ "Every 10 Seconds",
            /* 4 */ "Every 30 Seconds",
            /* 5 */ "Every Minute",
            /* 6 */ "Every 10 Minutes",
            /* 7 */ "Every 30 Minutes",
            /* 8 */ "Every Hour"
        };

        public static readonly long[] Values =
        {
            /* 0 */ 0L,
            /* 1 */ Ticks.PerMillisecond * 100L,
            /* 2 */ Ticks.PerSecond,
            /* 3 */ Ticks.PerSecond * 10L,
            /* 4 */ Ticks.PerSecond * 30L,
            /* 5 */ Ticks.PerMinute,
            /* 6 */ Ticks.PerMinute * 10L,
            /* 7 */ Ticks.PerMinute * 30L,
            /* 8 */ Ticks.PerHour
        };
    }
}

