<html lang="en">
<head>
</head>
<body>
<!--HtmlToGmd.Body-->
<div id="NavigationMenu">
<h1><a href="https://github.com/GridProtectionAlliance/openHistorian/blob/master/Source/Documentation/wiki/OpenHistorian.md">
<img src="https://github.com/GridProtectionAlliance/openHistorian/blob/master/Source/Documentation/wiki/openHistorian_Logo.png" alt="openHistorian" /></a></h1>
<hr />
<table style="width: 100%; border-collapse: collapse; border: 0px solid gray;">
<tr>
<td style="width: 25%; text-align:center;"><b><a href="http://www.gridprotectionalliance.com">Grid Protection Alliance</a></b></td>
<td style="width: 25%; text-align:center;"><b><a href="https://github.com/GridProtectionAlliance/openHistorian">openHistorian Project on GitHub</a></b></td>
<td style="width: 25%; text-align:center;"><b><a href="https://github.com/GridProtectionAlliance/openHistorian/blob/master/Source/Documentation/wiki/openHistorian.md">openHistorian Wiki</a></b></td>
<td style="width: 25%; text-align:center;"><b><a href="https://github.com/GridProtectionAlliance/openHistorian/blob/master/Source/Documentation/wiki/openHistorian_Documentation.md">openHistorian Documentation</a></b></td>
</tr>
</table>
</div>
<hr />
<!--/HtmlToGmd.Body-->
<div class="WikiContent">
<div class="wikidoc">openHistorian Version 1.0 is an integral part of the openPDC. Please see
<a href="https://github.com/GridProtectionAlliance/openPDC">the openPDC Project on GitHub.</a><br>
<br>
openHistorian Version 2.0 Beta is planned for the Fall of 2015.<br>
<br>
If you are just needing to archive data from the openPDC, it is recommended to setup an Internal Subscription to the openPDC - follow the same steps as you would for SIEGate or other tools:<br>
<br>
<a href="https://github.com/GridProtectionAlliance/SIEGate">Creating Internal Gateway Connections [https://siegate.codeplex.com/wikipage?title=Creating%20internal%20gateway%20connections]</a><br>
<br>
Just make sure you set the subscription historian to &quot;PPA&quot; before you save the connection to make sure all subscribed data gets archived.<br>
<br>
FYI - we have just added SQL Server link for the openHistorian. You can now query data openHistorian from within SQL Server using the GetHistorianData function that can be enabled by running the EnableHistorianSqlClr.sql script included with the latest installation:<br>
<br>
<a href="https://www.gridprotectionalliance.org/NightlyBuilds/openHistorian/Beta/">https://www.gridprotectionalliance.org/NightlyBuilds/openHistorian/Beta/</a><br>
<br>
Example API use to &quot;read&quot; data:<br>
<br>
<pre>
using System;
using System.Collections.Generic;
using GSF.Snap;
using GSF.Snap.Filters;
using GSF.Snap.Services;
using GSF.Snap.Services.Reader;
using openHistorian.Net;
using openHistorian.Snap;

// Define a historian measurement for floating-point values
public struct HistorianMeasurement
{
    public readonly ulong ID;
    public readonly DateTime Time;
    public readonly float Value;

    public HistorianMeasurement(ulong id, DateTime time, float value)
    {
        ID = id;
        Time = time;
        Value = value;
    }
}

// Read historian data from server, e.g., var enumerator = GetHistorianData(&quot;127.0.0.1&quot;, &quot;PPA&quot;, DateTime.UtcNow.AddMinutes(-1.0D), DateTime.UtcNow)
public static IEnumerable&lt;HistorianMeasurement&gt; GetHistorianData(string historianServer, string instanceName, DateTime startTime, DateTime stopTime, string measurementIDs = null)
{
    const int DefaultHistorianPort = 38402;

    if (string.IsNullOrEmpty(historianServer))
        throw new ArgumentNullException(&quot;historianServer&quot;, &quot;Missing historian server parameter&quot;);

    if (string.IsNullOrEmpty(instanceName))
        throw new ArgumentNullException(&quot;instanceName&quot;, &quot;Missing historian instance name parameter&quot;);

    if (startTime &gt; stopTime)
        throw new ArgumentException(&quot;Invalid time range specified&quot;, &quot;startTime&quot;);

    string[] parts = historianServer.Split(&#39;:&#39;);
    string hostName = parts[0];
    int port;

    if (parts.Length &lt; 2 || !int.TryParse(parts[1], out port))
        port = DefaultHistorianPort;

    // Open historian connection
    using (HistorianClient client = new HistorianClient(hostName, port))
    using (ClientDatabaseBase&lt;HistorianKey, HistorianValue&gt; reader = client.GetDatabase&lt;HistorianKey, HistorianValue&gt;(instanceName))
    {
        // Setup time-range and point ID selections
        SeekFilterBase&lt;HistorianKey&gt; timeFilter = TimestampSeekFilter.CreateFromRange&lt;HistorianKey&gt;(startTime, stopTime);
        MatchFilterBase&lt;HistorianKey, HistorianValue&gt; pointFilter = null;
        HistorianKey key = new HistorianKey();
        HistorianValue value = new HistorianValue();

        if (!string.IsNullOrEmpty(measurementIDs))
            pointFilter = PointIdMatchFilter.CreateFromList&lt;HistorianKey, HistorianValue&gt;(measurementIDs.Split(&#39;,&#39;).Select(ulong.Parse));

        // Start stream reader for the provided time window and selected points
        TreeStream&lt;HistorianKey, HistorianValue&gt; stream = reader.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

        while (stream.Read(key, value))
            yield return new HistorianMeasurement(key.PointID, key.TimestampAsDate, value.AsSingle);
    }
}
</pre>
<br>
<br>
Example API use to &quot;write&quot; data:<br>
<br>
<pre>
// Write historian data
public static void WriteHistorianData(string historianServer, string instanceName, IEnumerable&lt;HistorianMeasurement&gt; measurements)
{
    const int DefaultHistorianPort = 38402;

    if (string.IsNullOrEmpty(historianServer))
        throw new ArgumentNullException(&quot;historianServer&quot;, &quot;Missing historian server parameter&quot;);

    if (string.IsNullOrEmpty(instanceName))
        throw new ArgumentNullException(&quot;instanceName&quot;, &quot;Missing historian instance name parameter&quot;);

    if (startTime &gt; stopTime)
        throw new ArgumentException(&quot;Invalid time range specified&quot;, &quot;startTime&quot;);

    string[] parts = historianServer.Split(&#39;:&#39;);
    string hostName = parts[0];
    int port;

    if (parts.Length &lt; 2 || !int.TryParse(parts[1], out port))
        port = DefaultHistorianPort;

    // Open historian connection
    using (HistorianClient client = new HistorianClient(m_hostName, m_port))
    using (ClientDatabaseBase&lt;HistorianKey, HistorianValue&gt; database = client.GetDatabase&lt;HistorianKey, HistorianValue&gt;(m_instanceName))
    using (HistorianInputQueue queue = new HistorianInputQueue(() =&gt; database))
    {
        HistorianKey key = new HistorianKey();
        HistorianValue value = new HistorianValue();
        
        foreach (HistorianMeasurement measurement in measurements)
        {
            key.PointID = measurement.ID;
            key.TimestampAsDate = measurement.Time;
            value.AsSingle = measurement.Value;
            queue.Enqueue(key, value);
        }

        // Wait for queue to be processed
        while (queue.Size &gt; 0)
            Thread.Sleep(1000);
    }
}
</pre>
<br>
<br>
Example code to &quot;reduce&quot;, via skipping, resolution of returned data:<br>
<br>
<pre>
    TimeSpan interval = Resolutions.GetInterval(&quot;Every 30 Seconds&quot;);

    SeekFilterBase&lt;HistorianKey&gt; timeFilter;

    if (interval.Ticks != 0)
       timeFilter = TimestampSeekFilter.CreateFromIntervalData&lt;HistorianKey&gt;(startTime, stopTime, interval, new TimeSpan(TimeSpan.TicksPerMillisecond));
    else
        timeFilter = TimestampSeekFilter.CreateFromRange&lt;HistorianKey&gt;(startTime, stopTime);

    // Some example down-sampling resolutions...        
    public static class Resolutions
    {
        public static List&lt;string&gt; GetAllResolutions()
        {
            List&lt;string&gt; rv = new List&lt;string&gt;();
            rv.Add(&quot;Full&quot;);
            rv.Add(&quot;10 per Second&quot;);
            rv.Add(&quot;Every Second&quot;);
            rv.Add(&quot;Every 10 Seconds&quot;);
            rv.Add(&quot;Every 30 Seconds&quot;);
            rv.Add(&quot;Every Minute&quot;);
            rv.Add(&quot;Every 10 Minutes&quot;);
            rv.Add(&quot;Every 30 Minutes&quot;);
            rv.Add(&quot;Every Hour&quot;);
            return rv;
        }
 
        public static TimeSpan GetInterval(string resolution)
        {
            switch (resolution)
            {
                case &quot;Full&quot;:
                    return TimeSpan.Zero;
                case &quot;10 per Second&quot;:
                    return new TimeSpan(TimeSpan.TicksPerMillisecond * 100);
                case &quot;Every Second&quot;:
                    return new TimeSpan(TimeSpan.TicksPerSecond * 1);
                case &quot;Every 10 Seconds&quot;:
                    return new TimeSpan(TimeSpan.TicksPerSecond * 10);
                case &quot;Every 30 Seconds&quot;:
                    return new TimeSpan(TimeSpan.TicksPerSecond * 30);
                case &quot;Every Minute&quot;:
                    return new TimeSpan(TimeSpan.TicksPerMinute * 1);
                case &quot;Every 10 Minutes&quot;:
                    return new TimeSpan(TimeSpan.TicksPerMinute * 10);
                case &quot;Every 30 Minutes&quot;:
                    return new TimeSpan(TimeSpan.TicksPerMinute * 30);
                case &quot;Every Hour&quot;:
                    return new TimeSpan(TimeSpan.TicksPerHour * 1);
                default:
                    throw new Exception(&quot;Unknown resolution&quot;);
            }
        } 
    }
</pre>
<br>
<br>
Thanks,<br>
Ritchie</div>
</div>
<hr />
<div class="footer">
Last edited May 14, 2015 at 8:09:06 PM by <a id="wikiEditByLink" href="https://github.com/ritchiecarroll">ritchiecarroll</a>, version 7<br />
<!--HtmlToGmd.Migration-->Migrated from <a href="http://openhistorian.codeplex.com/documentation">CodePlex</a> Nov 7, 2015 by <a href="https://github.com/ajstadlin">ajstadlin</a><!--/HtmlToGmd.Migration-->
</div>
</body>
</html>