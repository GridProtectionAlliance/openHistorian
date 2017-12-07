## openHistorian Grafana Data Source

This repository defines a Grafana [data source](http://docs.grafana.org/datasources/overview/) plug-in for the [openHistorian](https://github.com/GridProtectionAlliance/openHistorian).

The openHistorian is a back office system developed by the [GridProtectionAlliance](https://www.gridprotectionalliance.org/) that is designed to efficiently integrate and archive process control data, e.g., SCADA, synchrophasor, digital fault recorder, or any other time-series data used to support process operations.

The openHistorian is optimized to store and retrieve large volumes of time-series data quickly and efficiently, including high-resolution sub-second information that is measured very rapidly, e.g., many thousands of times per second.

### Configuration for openHistorian 2.0

The openHistorian 2.0 automatically includes Grafana web service interfaces starting with [version 2.0.410](https://github.com/GridProtectionAlliance/openHistorian/releases).

For archived time-series data, the Grafana web service is hosted within the existing MVC based web server architecture and is just “on” with nothing extra to configure. To use the interface, simply register a new openHistorian Grafana data source using the path “/api/grafana” from the existing web based user interface URL, typically: http://localhost:8180/api/grafana/ [\*](#localhost).

The openHistorian 2.0 also includes a pre-configured local statistics archive web service interface that can be accessed from http://localhost:6356/api/grafana/ [\*](#localhost) &mdash; note that the trailing slash is relevant.

Statistical information is archived every ten seconds for a variety of data source and system parameters.

### Configuration for openHistorian 1.0 and Statistics Archives

The openHistorian 1.0 is a core component of the [Grid Solutions Framework Time-series Library](https://www.gridprotectionalliance.org/technology.asp#TSL) and is used for archival of statistics and other time-series data. Applications built using the openHistorian 1.0 can also be integrated with Grafana. 

#### Time-series Library Applications with Existing Grafana Support

Recent versions of the following Time-series Library (TSL) applications now include support for Grafana. To use the Grafana interface with an existing openHistorian 1.0 archive, simply register a new openHistorian Grafana data source using the appropriate interface URL as defined below [\*](#localhost):

| TSL Application (min version) | Statistics Interface | Archive Interface (if applicable) |
| ----- |:-----:|:-----:|
| ![openPDC Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/openPDC.png) [openPDC](https://github.com/GridProtectionAlliance/openPDC) (v2.2.133) | http://localhost:6352/api/grafana/ | http://localhost:6452/api/grafana/ |
| ![SIEGate Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/SIEGate.png) [SIEGate](https://github.com/GridProtectionAlliance/SIEGate) (v1.3.7) | http://localhost:6354/api/grafana/ | http://localhost:6454/api/grafana/ |
|  ![substationSBG Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/substationSBG.png) [substationSBG](https://github.com/GridProtectionAlliance/substationSBG) (v1.1.7) | http://localhost:6358/api/grafana/ | http://localhost:6458/api/grafana/ |
|  ![openMIC Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/openMIC.png) [openMIC](https://github.com/GridProtectionAlliance/openMIC) (v0.9.47) | http://localhost:6364/api/grafana/ | http://localhost:6464/api/grafana/ |
|  ![PDQTracker Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/PDQTracker.png) [PDQTracker](https://github.com/GridProtectionAlliance/pdqtracker) (v1.0.175) | http://localhost:6360/api/grafana/ | http://localhost:6460/api/grafana/ |
|  ![openECA Logo](https://www.gridprotectionalliance.org/images/products/icons%2016/openECA.png) [openECA](https://github.com/GridProtectionAlliance/openECA) (v0.1.44) | http://localhost:6362/api/grafana/ | http://localhost:6462/api/grafana/ |

#### Enabling Grafana Services with Custom Time-series Library Applications

If the “[GrafanaAdapters.dll](https://www.gridprotectionalliance.org/NightlyBuilds/GridSolutionsFramework/Beta/Libraries/)” is deployed with an existing Time-series Library based project, e.g., [Project Alpha](https://github.com/GridProtectionAlliance/projectalpha), the 1.0 openHistorian Grafana interfaces will be available per configured openHistorian instance. For Grafana support, the time-series project needs to use [Grid Solutions Framework](https://github.com/GridProtectionAlliance/gsf) dependencies for version 2.1.332 or beyond &mdash; or to be built with Project Alpha starting from version 0.1.159.

When the GrafanaAdapters.dll is deployed in the time-series project installation folder, a new Grafana data service entry will be added in the local configuration file for each configured historian when the new DLL is detected and loaded. Each historian web service instance for Grafana will need to be enabled and configured with a unique port:
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
If the service is using the default NT SERVICE account, the service will likely not have rights to start the web service on a new port and will need to be registered. As an example, to register a new Grafana web service end-point on port 6357 for the ProjectAlpha service, you would use the following command:
```
netsh http add urlacl url=http://+:6357/api/grafana user="NT SERVICE\ProjectAlpha"
```
This command must be run with administrative privileges.

\* _Replace "localhost" as needed with the IP or DNS name of system hosting the archive._

### Tag Selection

When adding metric queries from Grafana for the openHistorian, you can enter point information in a variery of forms including direct lists, e.g., measurement IDs "PPA:4; PPA:2", Guids "538A47B0-F10B-4143-9A0A-0DBC4FFEF1E8; E4BBFE6A-35BD-4E5B-92C9-11FF913E7877", or point tag names "GPA_TESTDEVICE:FREQ; GPA_TESTDEVICE:FLAG". Filter expressions are also supported, e.g.:
```
FILTER TOP 5 ActiveMeasurements WHERE SignalType LIKE '%PHA' AND Device LIKE 'SHELBY%' ORDER BY DeviceID
```
This expression would trend 5 phase angles, voltages or currents, for any device with a name that starts with "SHELBY".

See [syntax documentation](https://github.com/GridProtectionAlliance/openPDC/blob/master/Source/Documentation/wiki/Connection_Strings.md#input_and_output_syntax) for time-series framework input measurement keys more information.

### Alarm Annotations

The openHistorian Grafana interface supports “annotations” for Alarms. If any alarms are configured for a host system, then they can be accessed from the associated Grafana data source. Note that alarm measurements are stored in the local statistics archive by default.

Supported queries include “#ClearedAlarms” and “#RaisedAlarms”, which will return all alarms for queried time period. Filter expressions of these data sets are also supported, e.g.:
```
FILTER TOP 10 ClearedAlarms WHERE Severity >= 500 AND TagName LIKE '%TESTDEVICE%'
```
or
```
FILTER RaisedAlarms WHERE Description LIKE '%High Frequency%'
```

### Suggested Installation Method

Use the grafana-cli tool to install the openHistorian data source from the command line:

```
grafana-cli plugins install gridprotectionalliance-openhistorian-datasource
```

The plugin will be installed into the grafana plugins directory.

More instructions on the cli tool can be found here: [http://docs.grafana.org/v3.0/plugins/installation/](http://docs.grafana.org/v3.0/plugins/installation/)

You need to use Grafana 3.0 or better to enable plugin support: [http://grafana.org/download/builds.html](http://grafana.org/download/builds.html)

### Alternate Installation Method

It is also possible to clone this repository directly into the Grafana plugins directory.

After cloning, restart the grafana-server and the plugin should be automatically detected and be available for use:
```
git clone https://github.com/GridProtectionAlliance/openHistorian-grafana.git
sudo service grafana-server restart
```
