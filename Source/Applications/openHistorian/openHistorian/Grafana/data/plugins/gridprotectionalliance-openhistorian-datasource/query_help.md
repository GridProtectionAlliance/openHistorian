### Tag Selection

When adding metric queries from Grafana for the openHistorian, you can enter point information in a variery of forms including direct lists, e.g., measurement IDs "PPA:4; PPA:2", Guids "538A47B0-F10B-4143-9A0A-0DBC4FFEF1E8; E4BBFE6A-35BD-4E5B-92C9-11FF913E7877", or point tag names "GPA_TESTDEVICE:FREQ; GPA_TESTDEVICE:FLAG". Filter expressions are also supported, e.g.:
```
FILTER TOP 5 ActiveMeasurements WHERE SignalType LIKE '%PHA' AND Device LIKE 'SHELBY%' ORDER BY DeviceID
```
This expression would trend the first 5 phase angles, voltages or currents for any device with a name that starts with "SHELBY". See [Filter Expressions](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md) for more information.

### Functions

The Grafana interfaces defined in the [Grid Solutions Framework](https://github.com/GridProtectionAlliance/gsf/) define various aggregation and operational functions, e.g., [Average](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#average) or [StandardDeviation](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#standarddeviation), which can be applied on a per-series and per-group basis. Functions applied to the group of available series can operate either on the entire set, end-to-end, or by time-slice. See [Grafana Functions](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md) for more information.

