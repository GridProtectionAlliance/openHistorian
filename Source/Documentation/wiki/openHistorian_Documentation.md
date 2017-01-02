[![](files/openH2_icon.png)![openHistorian](files/openHistorian2_Logo2016.png)](https://github.com/GridProtectionAlliance/openHistorian "openHistorian")

|   |   |   |   |   |
|---|---|---|---|---|
| **[Grid Protection Alliance](http://www.gridprotectionalliance.org "Grid Protection Alliance Home Page")** | **[openHistorian Project](https://github.com/GridProtectionAlliance/openHistorian "openPDC Project on GitHub")** | **[openHistorian Wiki](https://gridprotectionalliance.org/wiki/doku.php?id=openHistorian:overview "openHistorian Wiki")** | **[Documentation](openHistorian_Documentation.md "openHistorian")** | **[Latest Release](https://github.com/GridProtectionAlliance/openHistorian/releases "openHistorian Releases")** |

# openHistorian Documentation

openHistorian Version 1.0 is an integral part of the openPDC. Please see the [openPDC](https://github.com/GridProtectionAlliance/openPDC) on GitHub.

openHistorian Version 2.0 Beta is planned for the Fall of 2015.

If you are just needing to archive data from the openPDC, it is recommended to setup an Internal Subscription to the openPDC - follow the same steps as you would for SIEGate or other tools:

[Creating Internal Gateway Connections](https://github.com/GridProtectionAlliance/SIEGate/Source/Documentation/wiki/Creating_Internal_Gateway_Connections.md)

Just make sure you set the subscription historian to PPA before you save the connection to make sure all subscribed data gets archived.

FYI - we have just added SQL Server link for the openHistorian. You can now query data openHistorian from within SQL Server using the GetHistorianData function that can be enabled by running the EnableHistorianSqlClr.sql script included with the [latest release](https://github.com/GridProtectionAlliance/openHistorian/releases)

---

## Example API use to **read** data

```cs
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
```

---

## Example API use to **write** data

```cs
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
```

---

## Example code to **reduce**, via skipping, resolution of returned data

```cs
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
```

Thanks, Ritchie

---

May 14, 2015 8:09 PM - Last edited by [ritchiecarroll](https://github.com/ritchiecarroll), version 7  
Nov 7, 2015 - Migrated from [CodePlex](http://openhistorian.codeplex.com/documentation) by [aj](https://github.com/ajstadlin)

---

Copyright 2015 [Grid Protection Alliance](http://www.gridprotectionalliance.org)