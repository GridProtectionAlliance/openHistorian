using System;
using System.Collections.Generic;

namespace HistorianView
{
    public static class Resolutions
    {
        public static List<string> GetAllResolutions()
        {
            List<string> resolutions = new List<string>();

            resolutions.Add("Full");
            resolutions.Add("10 per Second");
            resolutions.Add("Every Second");
            resolutions.Add("Every 10 Seconds");
            resolutions.Add("Every 30 Seconds");
            resolutions.Add("Every Minute");
            resolutions.Add("Every 10 Minutes");
            resolutions.Add("Every 30 Minutes");
            resolutions.Add("Every Hour");

            return resolutions;
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

