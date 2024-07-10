# Grafana Data Source Plug-in for openHistorian

This Grafana [data source](https://grafana.com/grafana/plugins/data-source-plugins/) plug-in provides access to the [openHistorian](https://github.com/GridProtectionAlliance/openHistorian) – the high-performance time-series data store optimized for quick and efficient storage and retrieval. Built by the [GridProtectionAlliance](https://www.gridprotectionalliance.org/), the openHistorian is an archival and visualization platform for process control data like SCADA, synchrophasor, digital fault recorder, and other essential time-series data.

## What's New

- **Simplified Query Building**: We’ve merged the `Element List Builder` and `Filter Expression Query Builder` into one intuitive interface, accessible via `Query Wizard`. The Query Wizard Mode provides a guided experience for creating queries with signal search or filter expression building, function picker with detailed parameter breakouts, and helpful toggles for including peaks, dropping empty series, and more.
- **Metadata Selections**: Metadata can now be combined with queried data to support custom panels, like the [Geomap](https://grafana.com/docs/grafana/latest/panels-visualizations/visualizations/geomap/) plug-in.
- **Multiple Data Source Value Types**: Multiple data source value types are now supported. In addition to the standard `DataSourceValue` type which consists of a `Value` and a `Times`, custom types, like a `PhasorValue` type, which consists of a tuple of `Magnitude` and `Angle` values and a `Time`, are now available.
- **Custom User Functions**: The backend Grafana adapters of the openHistorian now supports user-defined custom functions.
- **Fully Async Interfaces**: All operations, including data queries and function processing, are all processed asynchronously. 

## Usage

Building a metric query using the openHistorian Grafana data source begins with a selection between two modes "Query Wizard Mode" or "Text Editor Mode".

### Query Wizard Mode

The _Query Wizard_ mode is used to select series to trend. This guided wizard allows crafting metric selection queries by direct tag selection or using FILTER expressions with a user-friendly interface, with options to control the granularity and specifics of the data being queried. Behavioral query settings include `Drop Empty Series`, `Include Peaks`, `Full Resolution Data`, and `Radial Geo Distribution`. Additionally, the wizard allows the selection of avaliable functions, with detailed descriptions for all the required and optional parameters.

![Query Wizard Mode](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/QueryWizardMode.png?raw=true)

### Text Editor Mode

For a more hands-on approach, the _Text Editor_ mode allows direct entry of query expressions, with the freedom to write and edit queries in raw form.

![Text Editor Mode](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/TextEditorMode.png?raw=true)

> Note: The transition between `Query Wizard` and `Text Editor` modes is seamless, retaining your expressions as you switch. However, manual edits made in `Text Editor` mode won't be reflected back in `Query Wizard` mode.

#### Manual Queries

- **Direct Tag Specification**: Input point tags, GUIDs, or measurement keys directly.
- **Filter Expressions**: Leverage a SQL-like syntax for dynamic and complex queries.
- **Combined Expressions**: Mix and match direct specifications with filter expressions for granular control.


## Filter Expressions

Filter expressions use a syntax that is analogous to SQL. For example, the following expression would select the first 5 encountered time-series metrics for any device with a name that starts with _SHELBY_:
```
FILTER TOP 5 ActiveMeasurements WHERE Device LIKE 'SHELBY%'
```

### Filter Builder from the Query Wizard

![Filter Wizard](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/FilterWizard.png?raw=true)

## Series Functions

A suite of functions like `Average`, `StandardDeviation`, and more, can be leveraged to perform calculations across selected data series. Functions are easily applied in either `Query Wizard` mode or `Text Editor` mode. Additionally, many functions support group operations by time-slice or over the entire set of series.

See [GSF Grafana Functions](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md) for more detail and the full list of available functions.

### Function Selection from the Query Wizard

![Function Wizard](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/FunctionWizard.png?raw=true)

## Metadata Selections

Query results can easily now be combined with metadata. For example, selecting `Longitude` and `Latitude` metadata will provide geo-coordinates for maps:

![Metadata](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/Metadata.png?raw=true)

## Alarm Annotations

Time-series alarms can be visualized with annotation queries for immediate insight into `#ClearedAlarms` and `#RaisedAlarms` (or just `#Alarms` for both cleared and raised), providing insights into series data quality.

![Alarm Annotations](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/AlarmAnnotations.png?raw=true)

Filter expressions of the configured time-series alarms are also supported, e.g.:

```
FILTER TOP 10 ClearedAlarms WHERE Severity >= 500 AND TagName LIKE '%DEVICE1%'
```

or

```
FILTER RaisedAlarms WHERE Description LIKE '%High Frequency%'
```

See `Alarms` [table definition](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md#alarms) for available query fields in the _ClearedAlarms_ and _RaisedAlarms_ datasets. Note that series functions are not currently supported on user specified alarm annotation queries.

> All annotation queries are internally executed as non-decimated, full resolution data from the data source to make sure no alarm values are skipped for the specified query range. Although this operation produces the most accurate query results, its use increases query burden on the data source &ndash; as a result, queries for long time ranges using alarm annotations could affect overall dashboard performance.

## Configuration

Configuring your data source is straightforward, with support for openHistorian 2.0 and up. Seamless integration with Grafana allows for easy setup, authentication, and data flag exclusions for desired data quality in visualizations.

The openHistorian Grafana data source works both for the standalone [openHistorian 2.0](https://gridprotectionalliance.org/phasor-Historian.html) and the openHistorian 1.0 which is embedded into products like the [openPDC](https://gridprotectionalliance.org/phasor-PDC.html).

Configuration of an openHistorian Grafana data source is normally as simple as specification of a URL and proper authentication options. The required authentication options depend on the configuration of the openHistorian web API which can be set as anonymous or require authentication and/or SSL.

![Connection Settings](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/ConnectionSettings.png?raw=true)

### Excluded Data Flags

All time-series data stored in the openHistorian includes [measurement state flags](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Libraries/GSF.TimeSeries/IMeasurement.cs#L46) that describe the data quality state of an archived value. The openHistorian Grafana data source includes the ability to filter queried data to the desired data quality states by excluding specified data flags.

![Excluded Data Flags](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/ExcludedDataFlags.png?raw=true)

### Data Source Type

As part of a data source configuration, selection of the data source value type is now an option:

![Data Source Value Type](https://github.com/GridProtectionAlliance/openHistorian-grafana/blob/master/src/img/DataSourceValueType.png?raw=true)

## Installation

Deploying the openHistorian Grafana data source is simple, with support for auto-launch and user security synchronization in openHistorian 2.4 and beyond. For standalone Grafana instances, installation via the Grafana CLI tool or direct repository cloning is available.

For detailed installation steps, visit the [official installation guide](https://grafana.com/grafana/plugins/gridprotectionalliance-openhistorian-datasource/?tab=installation).
