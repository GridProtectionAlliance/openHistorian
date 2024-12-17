# GSF Grafana Functions

The Grafana interfaces defined in the Grid Solutions Framework allow for aggregation and operational functions on a per-series and per-group basis. The following defines the [available functions](#available-functions) and [group operations](#group-operations) that are available for a data source implementing the GSF Grafana interface, e.g., [openHistorian](https://github.com/GridProtectionAlliance/openHistorian). Note that any time-series style data source that implements the [GrafanaDataSourceBase](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Libraries/Adapters/GrafanaAdapters/GrafanaDataSourceBase.cs) class will automatically inherit this functionality.

## Series Functions

Various functions are available that can be applied to each series that come from a specified expression, see full list of [available functions](#available-functions) below. Series expressions can be an individual listing of point tag names, Guid-based signal IDs or measurement keys separated by semi-colons - _or_ - a [filter expression](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md) that will select several series at once. Filter expressions and individual points, with or without functions, may be selected simultaneously when separated with semi-colons.

* Example: `PPA:15; STAT:20; SetSum(Count(PPA:8; PPA:9; PPA:10)); FILTER ActiveMeasurements WHERE SignalType IN ('IPHA', 'VPHA'); Range(PPA:99; Sum(FILTER ActiveMeasurements WHERE SignalType = 'FREQ'; STAT:12))`

Many series functions have parameters that can be required or optional &ndash; optional values will always define a default state. Parameter values must be a constant value or, where applicable, a named target available from the expression. Named targets are intended to work with group operations, i.e., [Set](#set) or [Slice](#slice), since group operations provide access to multiple series values from within a single series. The actual value used for a named target parameter will be the first encountered value for the target series &ndash; in the case of slice group operations, this will be the first value encountered in each slice. Named target parameters can optionally specify multiple fall-back series and one final default constant value each separated by a semi-colon to use when the named target series is not available, e.g.: `SliceSubtract(1, T1;T2;5.5, T1;T2;T3)`

To better understand named targets, consider the following steps:

> NOTE: This example is for illustrative purposes only, use the [`Reference`](#reference) function to get a difference between two or more angles taking wrapping and unwrapping into consideration.

 1. The following expression produces two unwrapped voltage phase angle series:

    [`UnwrapAngle(DOM_GPLAINS-BUS1:VH; TVA_SHELBY-BUS1:VH)`](#unwrapangle)

 2. Values from one of the series can now be subtracted from values in both of the series at every 1/30 of a second slice:

    [`SliceSubtract(0.0333, TVA_SHELBY-BUS1:VH, UnwrapAngle(DOM_GPLAINS-BUS1:VH; TVA_SHELBY-BUS1:VH))`](#subtract)

 3. Using a [Slice](#slice) operation on functions that return multiple series can produce multiple values at the same timestamp, however, since values produced by one of the series will now always be zero, the zero values can be excluded:

    [`ExcludeRange(0, 0, SliceSubtract(0.0333, TVA_SHELBY-BUS1:VH, UnwrapAngle(DOM_GPLAINS-BUS1:VH; TVA_SHELBY-BUS1:VH)))`](#excluderange)

### Execution Modes
Each of the series functions include documentation for the mode of execution required by the function. These modes determine the level of processing expense and memory burden incurred by the function. The impacts of the execution modes increase as the time-range or resolution of the series data increases.

| Execution Mode | Description | Impact |
|----------------|-------------|--------|
| _Deferred enumeration_ | Series data will be processed serially outside of function | Minimal processing and memory impact |
| _Immediate enumeration_ | Series data will be processed serially inside the function | Increased processing impact, minimal memory impact |
| _Immediate in-memory array load_ | Series data will be loaded into an array and processed inside the function | Higher processing and memory impact |

## Group Operations

Many Grafana series functions can be operated on in aggregate using a group operator prefix. Each of the series functions includes documentation for the group operation modes allowed by the function.

### Set

Series functions can operate over the set of defined series, producing a single result series, where the target function is executed over each series, horizontally, end-to-end by prefixing the function name with `Set`.

* Example: `SetAverage(FILTER ActiveMeasurements WHERE SignalType='FREQ')`

### Slice

Series functions can operate over the set of defined series, producing one or more result series, where the target function is executed over each series as a group, vertically, per time-slice by prefixing the function name with `Slice`. When operating on a set of series data with a slice function, a new required parameter for time tolerance will be introduced as the first parameter to the function. The parameter is a floating-point value that must be greater than or equal to zero that represents the desired time tolerance, in seconds, for the time slice.

* Example: `SliceSum(0.0333, FILTER ActiveMeasurements WHERE SignalType='IPHM')`

## Special Commands

The following optional special command operations can be specified as part of any filter expression:

| Command | Description |
| ------- | ----------- |
| `DropEmptySeries` | Ensures any empty series are hidden from display. Example: `; dropEmptySeries` |
| `FullResolutionQuery` | Ensures query returns non-decimated, full resolution data. Example: `; fullResolutionData` |
| `IncludePeaks` | Ensures decimated data includes both min/max interval peaks, note this can reduce query performance. Example: `; includePeaks` |
| `RadialDistribution` | Updates query coordinate metadata, i.e., longitude/latitude, where values overlap in a radial distribution. Example: `; radialDistribution` |
| `SquareDistribution` | Updates query coordinate metadata, i.e., longitude/latitude, where values overlap in a square distribution. Example: `; squareDistribution` |
| `Imports={expr}` | Adds custom .NET type imports that can be used with the [`Evaluate`](#evaluate) function. `expr` defines a key-value pair definition of assembly name, i.e., `AssemblyName` = DLL filename without suffix, and type name, i.e., `TypeName` = fully qualified case-sensitive type name, to be imported. Key-value pairs are separated with commas and multiple imports are by separated semi-colons. `expr` must be surrounded by braces. Example: `; imports={AssemblyName=mscorlib, TypeName=System.TimeSpan; AssemblyName=MyCode, TypeName=MyCode.MyClass}` |
