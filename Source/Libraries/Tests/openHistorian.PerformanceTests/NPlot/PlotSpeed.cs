using System;
using System.Collections.Generic;
using System.Diagnostics;
using NPlot;
using NUnit.Framework;
using PlotSurface2D = NPlot.Bitmap.PlotSurface2D;
using ST = NPlot.StepTimer;

namespace openHistorian.PerformanceTests.NPlot
{
    [TestFixture]
    public class PlotSpeed
    {
        [Test]
        public void RefreshSpeed()
        {
            List<double> xVal = new List<double>();
            List<double> yVal = new List<double>();

            for (int x = 0; x < 100000; x++)
            {
                xVal.Add(x);
                yVal.Add(1 - x);
            }
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            LinePlot p1 = new LinePlot(yVal, xVal);

            PlotSurface2D plot = new PlotSurface2D(640, 480);

            sw.Start();

            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);

            sw2.Start();
            plot.Refresh();
            sw2.Stop();
            sw.Stop();

            Console.WriteLine(sw2.Elapsed.TotalSeconds.ToString() + " seconds to refresh");
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString() + " seconds To add and refresh");
        }

        [Test]
        public void RefreshSpeedTest()
        {
            ST.Reset();

            DebugStopwatch sw = new DebugStopwatch();
            double time = sw.TimeEvent(RefreshSpeed);
            Console.WriteLine(time.ToString() + " seconds to on average");

            Console.WriteLine(ST.GetResultsPercent());
        }
    }
}