# Grafana Data Source Plug-in for openHistorian

This repository defines a Grafana [data source](http://docs.grafana.org/datasources/overview/) plug-in for the [openHistorian](https://github.com/GridProtectionAlliance/openHistorian).

The openHistorian is a back office system developed by the [GridProtectionAlliance](https://www.gridprotectionalliance.org/) that is designed to efficiently integrate and archive process control data, e.g., SCADA, synchrophasor, digital fault recorder, or any other time-series data used to support process operations.

The openHistorian is optimized to store and retrieve large volumes of time-series data quickly and efficiently, including high-resolution sub-second information that is measured very rapidly, e.g., many thousands of times per second.

---

## Usage

Building a metric query using the openHistorian Grafana data source begins with the selection of a query type, one of: _Element List_, _Filter Expression_ or _Text Editor_. The _Element List_ and _Filter Expression_ types are query builder screens that assist with the selection of the desired series. The _Text Editor_ screen allows for manual entry of a query expression that will select the desired series.

### Element List Query Builder

The _Element List_ query builder is used to directly select the series to trend. New elements can be selected and searched by clicking the `+` button at the end of the `ELEMENTS` row. Typing text into the drop-down box will start filtering the available data points for selection.

![Element List Query Type](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/ElementList.png)

### Filter Expression Query Builder

The _Filter Expression_ query builder is used to define an expression (see [filter expression syntax](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md)) to select the series to trend. Complex expressions can be created that will dynamically query data series. Query results will mutate as the availability of the source point data changes, i.e., series derived from the query result will change as data points are added or removed in the openHistorian.

![Filter Expression Query Type](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/FilterExpression.png)

### Text Editor Query Builder

The _Text Editor_ query builder is used to manually specify a text based query expression for openHistorian Grafana data source metrics. The expression can be any combination of directly specified point tag names, Guid identifiers or measurement keys separated by semi-colons - _or_ - a filter expression that will select several series at once.

![Text Editor Query Type](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/TextEditor.png)

> Note that switching from the _Element List_ or _Filter Expression_ query builder screens to the _Text Editor_ will keep the expression as built so far to allow further manual updates to the expression. However, any manual changes made to the filter expression while on the _Text Editor_ query screen will not flow back to the _Element List_ or _Filter Expression_ query builder screens. Moreover, switching back to or between the _Element List_ and _Filter Expression_ query builder screens will automatically clear out any existing expression.

#### Direct Tag Specification

Direct specification of metric queries can be entered using the _Text Editor_ as semi-colon separated point tag references in a variety of forms, e.g., measurement key identifiers: _PPA:4; PPA:2_ - formatted as `{instance}:{id}`, unique Guid based signal identifiers: _538A47B0-F10B-4143-9A0A-0DBC4FFEF1E8; E4BBFE6A-35BD-4E5B-92C9-11FF913E7877_, or point tag names: _GPA_TESTDEVICE:FREQ; GPA_TESTDEVICE:FLAG_.

#### Filter Expressions

Metric queries using the _Text Editor_ can also be specified as filter expressions that use a syntax that is similar to SQL. For example, the following expression would select the first 5 encountered series for any device with a name that starts with _SHELBY_:
```
FILTER TOP 5 ActiveMeasurements WHERE Device LIKE 'SHELBY%'
```

See [filter expression syntax](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md) and the `ActiveMeasurements` [table definition](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md#activemeasurements) for more information.

#### Combined Expressions

When using the _Text Editor_ to build a query, both filter expressions and directly specified tags, with or without series functions, may be selected simultaneously when separated with semi-colons, for example:
```
PPA:15; STAT:20; SetSum(Count(PPA:8; PPA:9; PPA:10));
FILTER ActiveMeasurements WHERE SignalType IN ('IPHA', 'VPHA');
Range(PPA:9; Sum(FILTER ActiveMeasurements WHERE SignalType = 'FREQ'; STAT:2))
```

> Complex combined expressions that contain both directly specified point tags and filter expressions are only available when using the _Text Editor_ query builder.

### Series Functions

The openHistorian Grafana data source includes various aggregation and operational functions, e.g., [Average](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#average) or [StandardDeviation](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#standarddeviation), which can be applied on a per-series and per-group basis. Functions applied to the group of available series can operate either on the entire set, end-to-end, or by time-slice. See [GSF Grafana Functions](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md) for more detail and the full list of available functions.

The _Element List_ and _Filter Expression_ query builder screens define the available functions as pick lists that get applied over the selected series by clicking the `+` button at the end of the `FUNCTIONS` row:

![Filter Expression Query Type with Functions](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/FilterExpressionWithFunctions.png)

Many series functions have parameters that are required or optional &ndash; optional parameter values will always define a default state. Parameter values must be a constant value or, where applicable, a named target available from the expression. Named targets are intended to work with group operations, i.e., [Set](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#set) or [Slice](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#slice), since group operations provide access to multiple series values from within a single series. The actual value used for a named target parameter will be the first encountered value for the target series &ndash; in the case of _Slice_ group operations, this will be the first value encountered in each time-slice. Named target parameters can optionally specify multiple fall-back series and one final default constant value each separated by a semi-colon to use when the named target series is not available, e.g.: `SliceSubtract(1, T1;T2;5.5, T1;T2;T3)`

### Alarm Annotations

The openHistorian Grafana data source supports _Annotation_ style queries for configured time-series alarms. If any alarms are configured for a host system, then they can be accessed from the associated openHistorian Grafana data source. Note that alarm measurements are stored in the local statistics archive by default, e.g., `OHSTAT`, so make sure this is the data source of the configured annotation query.

Supported alarm annotation queries include `#ClearedAlarms` and `#RaisedAlarms`, which will return all alarms for the queried time period:

![Alarm Annotations](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/AlarmAnnotations.png)

Filter expressions of the configured time-series alarms are also supported, e.g.:

```
FILTER TOP 10 ClearedAlarms WHERE Severity >= 500 AND TagName LIKE '%DEVICE1%'
```

or

```
FILTER RaisedAlarms WHERE Description LIKE '%High Frequency%'
```

See `Alarms` [table definition](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/FilterExpressions.md#alarms) for available query fields in the _ClearedAlarms_ and _RaisedAlarms_ datasets. Note that series functions are not currently supported on user specified alarm annotation queries.

> All annotation queries are internally executed using the [_Interval_](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Documentation/GrafanaFunctions.md#interval) function with a time parameter of zero to request non-decimated, full resolution data from the data source to make sure no alarm values are skipped for the specified query range. Although this operation produces the most accurate query results, its use increases query burden on the data source &ndash; as a result, queries for long time ranges using alarm annotations could affect overall dashboard performance.

---

## Configuration

The openHistorian Grafana data source works both for the standalone [openHistorian 2.0](http://www.openHistorian.com/) and the openHistorian 1.0 which is embedded into products like the [openPDC](http://www.openPDC.com/). Configuration options for each of the target openHistorian versions are defined below.

Starting with openHistorian 2.4, Grafana can be seamlessly integrated with the openHistorian such that the openHistorian primary web site can act as a reverse proxy to an instance of Grafana accessible from: [http://localhost:8180/grafana/](http://localhost:8180/grafana/). Deployments of the openHistorian with hosted Grafana integration include pre-configured data sources for the primary data and statistics archives named `OHDATA` and `OHSTAT` respectively.

Configuration of an openHistorian Grafana data source is normally as simple as specification of a URL and proper authentication options. The required authentication options depend on the configuration of the openHistorian web API which can be set as anonymous or require authentication and/or SSL.

### openHistorian 2.0 Configuration

The openHistorian 2.0 automatically includes Grafana web service interfaces starting with [version 2.0.410](https://github.com/GridProtectionAlliance/openHistorian/releases).

For archived time-series data, the Grafana web service is hosted within the existing MVC based web server architecture and is just “_on_” with nothing extra to configure. To use the interface, simply register a new openHistorian Grafana data source using the path `/api/grafana/` from the existing web based user interface URL, typically: [http://localhost:8180/api/grafana/](http://localhost:8180/api/grafana/) [\*](#localhost-note).

When the openHistorian service is hosting multiple historian instances, a specific historian instance can be referenced using a path like `/instance/{instanceName}/grafana/`, e.g.: [http://localhost:8180/instance/ppa/grafana/](http://localhost:8180/instance/ppa/grafana/) [\*](#localhost-note).

The typical HTTP setting for `Access` in any instance of the openHistorian Grafana data source is _proxy_. However, when referencing a hosted Grafana instance that is integrated with the openHistorian 2.0 via reverse proxy, the `Access` setting can be set to _direct_ such that the user's authentication headers can flow back through the openHistorian for user security validation:

![Direct Data Source Configuration](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/DataSourceConfiguration.png)

Using _direct_ access allows the openHistorian authenticated user to be the authenticated user in Grafana. Otherwise, _proxy_ access will also work but will require a user for Grafana to use for authenticating to the data source.

#### openHistorian 2.0 Statistics Data

The openHistorian 2.0 also includes a pre-configured local statistics archive that can be accessed using an instance of the openHistorian Grafana data source with the following URL: [http://localhost:6356/api/grafana/](http://localhost:6356/api/grafana/) [\*](#localhost-note) &ndash; note that the trailing slash is relevant.

Statistical information is archived every ten seconds for a variety of data source and system parameters measured for the openHistorian 2.0 service.

The HTTP setting for `Access` in an instance of the openHistorian Grafana data source that is connecting to the openHistorian 2.0 statistics archive should always be set to _proxy_.

### openHistorian 1.0 Configuration

The openHistorian 1.0 is a core component of the [Grid Solutions Framework Time-series Library](https://www.gridprotectionalliance.org/technology.asp#TSL) and is used for archival of statistics and other time-series data. Applications built using the openHistorian 1.0 can also be integrated with Grafana. 

The HTTP setting for `Access` in an instance of the openHistorian Grafana data source that is connecting to the openHistorian 1.0 should always be set to _proxy_.

#### Time-series Library Applications with Existing Grafana Support

Recent versions of the following Time-series Library (TSL) applications now include support for Grafana. To use the Grafana interface with an existing openHistorian 1.0 archive, simply register a new openHistorian Grafana data source using the appropriate interface URL as defined below [\*](#localhost-note)  &ndash; note that the trailing slashes are relevant:

| TSL Application (min version) | Statistics Interface | Archive Interface (if applicable) |
| ----- |:-----:|:-----:|
| ![openPDC Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/openPDC.png) [openPDC](https://github.com/GridProtectionAlliance/openPDC) (v2.2.133) | [http://localhost:6352/api/grafana/](http://localhost:6352/api/grafana/) | [http://localhost:6452/api/grafana/](http://localhost:6452/api/grafana/) |
| ![SIEGate Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/SIEGate.png) [SIEGate](https://github.com/GridProtectionAlliance/SIEGate) (v1.3.7) | [http://localhost:6354/api/grafana/](http://localhost:6354/api/grafana/) | [http://localhost:6454/api/grafana/](http://localhost:6454/api/grafana/) |
|  ![substationSBG Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/substationSBG.png) [substationSBG](https://github.com/GridProtectionAlliance/substationSBG) (v1.1.7) | [http://localhost:6358/api/grafana/](http://localhost:6358/api/grafana/) | [http://localhost:6458/api/grafana/](http://localhost:6458/api/grafana/) |
|  ![openMIC Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/openMIC.png) [openMIC](https://github.com/GridProtectionAlliance/openMIC) (v0.9.47) | [http://localhost:6364/api/grafana/](http://localhost:6364/api/grafana/) | [http://localhost:6464/api/grafana/](http://localhost:6464/api/grafana/) |
|  ![PDQTracker Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/PDQTracker.png) [PDQTracker](https://github.com/GridProtectionAlliance/pdqtracker) (v1.0.175) | [http://localhost:6360/api/grafana/](http://localhost:6360/api/grafana/) | [http://localhost:6460/api/grafana/](http://localhost:6460/api/grafana/) |
|  ![openECA Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/openECA.png) [openECA](https://github.com/GridProtectionAlliance/openECA) (v0.1.44) | [http://localhost:6362/api/grafana/](http://localhost:6362/api/grafana/) | [http://localhost:6462/api/grafana/](http://localhost:6462/api/grafana/) |

#### Enabling Grafana Services with Custom Time-series Library Applications

If the assembly [`GrafanaAdapters.dll`](https://www.gridprotectionalliance.org/NightlyBuilds/GridSolutionsFramework/Beta/Libraries/) is deployed with an existing Time-series Library based project, e.g., [Project Alpha](https://github.com/GridProtectionAlliance/projectalpha), the 1.0 openHistorian Grafana interfaces will be available per configured openHistorian instance. For Grafana support, the time-series project needs to use [Grid Solutions Framework](https://github.com/GridProtectionAlliance/gsf) dependencies for version 2.1.332 or beyond &mdash; or to be built with Project Alpha starting from version 0.1.159.

When the `GrafanaAdapters.dll` is deployed in the time-series project installation folder, a new Grafana data service entry will be added in the local configuration file, e.g., `ProjectAlpha.exe.config`, for each configured historian when the new DLL is detected and loaded. Each historian web service instance for Grafana will need to be enabled and configured with a unique port:
```xml
    <statGrafanaDataService>
      <add name="Endpoints" value="http.rest://+:6357/api/grafana" description="Semicolon delimited list of URIs where the web service can be accessed." encrypted="false" />
      <add name="Contract" value="GrafanaAdapters.IGrafanaDataService, GrafanaAdapters" description="Assembly qualified name of the contract interface implemented by the web service." encrypted="false" />
      <add name="Singleton" value="True" description="True if the web service is singleton; otherwise False." encrypted="false" />
      <add name="SecurityPolicy" value="" description="Assembly qualified name of the authorization policy to be used for securing the web service." encrypted="false" />
      <add name="PublishMetadata" value="True" description="True if the web service metadata is to be published at all the endpoints; otherwise False." encrypted="false" />
      <add name="AllowCrossDomainAccess" value="False" description="True to allow Silverlight and Flash cross-domain access to the web service." encrypted="false" />
      <add name="AllowedDomainList" value="*" description="Comma separated list of domain names for Silverlight and Flash cross-domain access to use when allowCrossDomainAccess is true. Use * for domain wildcards, e.g., *.consoto.com." encrypted="false" />
      <add name="CloseTimeout" value="00:02:00" description="Maximum time allowed for a connection to close before raising a timeout exception." encrypted="false" />
      <add name="Enabled" value="True" description="Determines if web service should be enabled at startup." encrypted="false" />
    </statGrafanaDataService>
```
If the service is deployed on a Windows machine and is configured using the default `NT SERVICE` account, the service will not have rights to start the web service on a new port and will need to be registered. As an example, the following command can be used to register a new Grafana web service end-point on port 6357 for the `ProjectAlpha` service:
```
netsh http add urlacl url=http://+:6357/api/grafana user="NT SERVICE\ProjectAlpha"
```

> The `netsh` command must be run with administrative privileges. The `+` host designation is used to bind to the URL and port to all local interfaces; otherwise, a specific IP must be provided.

###### LocalHost Note

\* _Replace `localhost` as needed with the IP or DNS name of system hosting the archive._

### Excluded Data Flags

All time-series data stored in the openHistorian includes [measurement state flags](https://github.com/GridProtectionAlliance/gsf/blob/master/Source/Libraries/GSF.TimeSeries/IMeasurement.cs#L46) that describe the data quality state of an archived value. The openHistorian Grafana data source includes the ability to filter queried data to the desired data quality states by excluding specified data flags. Default excluded data flag filters can by defined at a data source level and overridden at an individual metric query level. To change the default flags for an individual metric query, click the `Query Options` button near the end of the query `TYPE` row:

![Excluded Data Flags](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian-grafana/master/src/img/ExcludedDataFlags.png)

 > The initial set of excluded data flags for an individual metric query is inherited from the flags defined for the associated data source and get established when the query is first created. The excluded data flags for an existing metric query will not be affected by any subsequent updates to the flags at the data source level, i.e., any changes made to the excluded data flags at the data source level will only be used as defaults for new metric queries and will not affect any existing queries.

---

## Installation

Starting with openHistorian 2.4, Grafana can be installed along with the openHistorian such that the openHistorian's primary self-hosted web site can act as a reverse proxy to Grafana. When configured in this mode, the openHistorian will auto-launch the `grafana-server` executable and act as a front-end server for Grafana. Additionally, the openHistorian will maintain user security synchronization such that a user with an `Administrator` role in openHistorian will also have an `Admin` role in Grafana, or if a user has an `Editor` role in openHistorian they will have an `Editor` role in Grafana, and so on. 

For installation within a stand-alone instance of Grafana, see the [offical instructions](https://grafana.com/plugins/gridprotectionalliance-openhistorian-datasource/installation) for steps to install the openHistorian Grafana data source using the [Grafana CLI tool](http://docs.grafana.org/plugins/installation/). Note that Grafana 3.0 or better is required to enable plug-in support.

Alternately the openHistorian Grafana data source can be installed into a Grafana instance by cloning this repository directly into the Grafana plug-ins directory, i.e., `data/plugins/`. After cloning, restart the grafana-server and the plug-in should be automatically detected and be available for use, e.g.:

```
git clone https://github.com/GridProtectionAlliance/openHistorian-grafana.git
sudo service grafana-server restart
```
