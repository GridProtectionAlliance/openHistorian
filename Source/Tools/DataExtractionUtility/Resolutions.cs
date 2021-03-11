using System;
using System.Collections.Generic;

namespace DataExtractionUtility
{
    public static class Resolutions
    {
        public static List<string> GetAllResolutions()
        {
            List<string> rv = new List<string>();
            rv.Add("Full");
            rv.Add("10 per Second");
            rv.Add("Every Second");
            rv.Add("Every 10 Seconds");
            rv.Add("Every 30 Seconds");
            rv.Add("Every Minute");
            rv.Add("Every 10 Minutes");
            rv.Add("Every 30 Minutes");
            rv.Add("Every Hour");
            return rv;
        }

        public static TimeSpan GetInterval(string resolution)
        {
            switch (resolution)
            {
                case "Full":
                    return TimeSpan.Zero;
                case "10 per Second":
                    return new TimeSpan(TimeSpan.TicksPerMillisecond * 100);
                case "Every Second":
                    return new TimeSpan(TimeSpan.TicksPerSecond * 1);
                case "Every 10 Seconds":
                    return new TimeSpan(TimeSpan.TicksPerSecond * 10);
                case "Every 30 Seconds":
                    return new TimeSpan(TimeSpan.TicksPerSecond * 30);
                case "Every Minute":
                    return new TimeSpan(TimeSpan.TicksPerMinute * 1);
                case "Every 10 Minutes":
                    return new TimeSpan(TimeSpan.TicksPerMinute * 10);
                case "Every 30 Minutes":
                    return new TimeSpan(TimeSpan.TicksPerMinute * 30);
                case "Every Hour":
                    return new TimeSpan(TimeSpan.TicksPerHour * 1);
                default:
                    throw new Exception("Unknown resolution");
            }
        }

    }
}

