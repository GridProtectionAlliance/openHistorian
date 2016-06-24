![Logo](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian/master/Source/Documentation/Readme%20files/openHistorian.png)![Banner](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian/master/Source/Documentation/Readme%20files/openHistorian2.png)

# openHistorian

The openHistorian is a back office system designed to efficiently integrate and archive SCADA, synchrophasor, digital fault recorder and other process control data to support real-time grid     operations and post-disturbance analysis

# Overview
The openHistorian 2 is built using the [GSF SNAPdb Engine](http://www.gridprotectionalliance.org/technology.asp#SnapDB) - a key/value pair archiving technology developed to significantly improve the ability to archive extremely large volumes of real-time streaming data and directly serve the data to consuming applications and systems.


Through use of the [SNAPdb Engine](http://www.gridprotectionalliance.org/technology.asp#SnapDB), the openHistorian inherits very fast performance with very low lag-time for data insertion. The openHistorian 2 is a time-series implementation of the SNABdb engine where the "key" is a tuple of time and measurement ID, and the "value" is the stored data - which can be most any data type and associated flags.

The system comes with a high-speed API that interacts with an in-memory cache for very high speed extraction of near real-time data. The archive files produced by the openHistorian are [ACID Compliant](https://en.wikipedia.org/wiki/ACID) which create a very durable and consistent file structure that is resistant to data corruption. Internally the data structure is based on a [B+ Tree](https://en.wikipedia.org/wiki/B%2B_tree) that allows out-of-order data insertion.


The openHistorian service also hosts the [GSF Time-Series Library (TSL)](http://www.gridprotectionalliance.org/technology.asp#TSL), creating an ideal platform for integrating streaming time-series data processing in real-time:

![openHistorian Overview](http://www.gridprotectionalliance.org/docs/products/openhistorian/OverviewDiagram.png)

Three utilities are currently available to assist in using the openHistorian 2. They are automatically installed alongside openHistorian.

* **Data Migration Utility** - Converts openHistorian 1.0 / DatAWAre Archives to openHistorian 2.0 Format - [View Screen Shot](http://www.gridprotectionalliance.org/images/products/HistorianMigration.png)
* **Data Trending Tool** - Queries Selected Historical Data for Visual Trending Using a Provided Date/Time Range - [View Screen Shot](http://www.gridprotectionalliance.org/images/products/HistorianTrending.png)
* **Data Extraction Utility** - Queries Selected Historian Data for Export to a CSV File Using a Provided Date/Time Range - [View Screen Shot](http://www.gridprotectionalliance.org/images/products/HistorianExtraction.png)

**Where openHistorian Fits In:**
![Where it fits in](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian/master/Source/Documentation/Readme%20files/Where%20it%20fits%20in.png)

# Documentation and Support

* Documentation for openHistorian can be found [here](https://github.com/GridProtectionAlliance/openHistorian/blob/master/Source/Documentation/wiki/openHistorian_Documentation.md).
* Get in contact with our development team on our new [discussion boards](http://discussions.gridprotectionalliance.org/c/gpa-products/openhistorian).
* View old discussion board topics [here](http://openhistorian.codeplex.com/discussions).
* Check out the [openHistorian wiki](https://gridprotectionalliance.org/wiki/doku.php?id=openhistorian:overview).

# Deployment

1. Make sure your system meets all the [requirements](#requirements) below.
* Choose a [download](#downloads) below.
* Unzip if necessary.
* Run "Setup.exe".
* Follow the wizard.
* Enjoy.

## Requirements

* .NET 4.5 or higher.
* 64-bit Windows 7 or newer.
* Database management system such as:
  * SQL Server (Recommended)
  * MySQL
  * Oracle
  * PostgreSQL
  * SQLite (Not recommended for production use) - included.

## Downloads
* Download a stable release [here](https://github.com/GridProtectionAlliance/openHistorian/releases).
* Download the nightly build [here](http://www.gridprotectionalliance.org/nightlybuilds/openHistorian/Beta/openHistorian.Installs.zip).

# Contributing
If you would like to contribute please:

1. Read our [styleguide.](https://www.gridprotectionalliance.org/docs/GPA_Coding_Guidelines_2011_03.pdf)
* Fork the repository.
* Work your magic.
* Create a pull request.

# License
openHistorian is licensed under the [MIT License](https://opensource.org/licenses/MIT).
