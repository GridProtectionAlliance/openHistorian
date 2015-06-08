using System;
using GSF;
using GSF.TimeSeries;
using openHistorian;

namespace SampleFunctions
{
    public class Program
    {

        static void Main(string[] args)
        {
            using (Connection connection = new Connection("127.0.0.1", "PPA"))
            {
                // Query for specific time-range and points at desired down-sampled resolution
                ulong pointID = 2;
                Resolution resolution = Resolution.EverySecond;
                DateTime stopTime = DateTime.UtcNow.BaselinedTimestamp(BaselineTimeInterval.Second);
                DateTime startTime = stopTime.AddMinutes(-1.0D);

                Console.WriteLine("Point to query: {0}", pointID);
                Console.WriteLine("    Resolution: {0}", resolution);
                Console.WriteLine("   Start time = {0:yyyy-MMM-dd HH:mm:ss.ffffff}", startTime);
                Console.WriteLine("    Stop time = {0:yyyy-MMM-dd HH:mm:ss.ffffff}", stopTime);
                Console.WriteLine();

                Console.WriteLine("Press any key to begin...");
                Console.ReadKey();

                int count = 0;

                // Query data for point over specified time range and data resolution
                foreach (IMeasurement measurement in MeasurementAPI.GetHistorianData(connection, startTime, stopTime, pointID.ToString(), resolution))
                    Console.WriteLine("[{0:N0}] {1}:{2} @ {3:yyyy-MMM-dd HH:mm:ss.ffffff} = {4}, quality: {5}", ++count, measurement.Key.Source, measurement.Key.ID, measurement.Timestamp, measurement.Value, measurement.StateFlags);

                count = 0;

                // Get contiguous data regions for specified point
                foreach (Tuple<DateTime, DateTime> region in MeasurementAPI.GetContiguousDataRegions(connection, startTime, stopTime, pointID, resolution))
                    Console.WriteLine("Data region {0:N0}: from {1:yyyy-MMM-dd HH:mm:ss.ffffff} to {2:yyyy-MMM-dd HH:mm:ss.ffffff}", ++count, region.Item1, region.Item2);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
