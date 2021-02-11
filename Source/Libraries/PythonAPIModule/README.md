![Logo](http://gridprotectionalliance.org/images/products/icons%2064/openHistorian.png)![Banner](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian/master/Source/Documentation/Readme%20files/openHistorian2.png)

# openHistorian Python API

This is the Python API used for high-speed reading and writing of time-series data with the openHistorian.

The openHistorian is a back office system designed to efficiently integrate and archive process control data, e.g., SCADA, synchrophasor, digital fault recorder or any other time-series data used to support process operations. The openHistorian is optimized to store and retrieve large volumes of time-series data quickly and efficiently, including high-resolution sub-second information that is measured very rapidly, e.g., many thousands of times per second.

# Overview
The openHistorian 2 is built using the [SNAPdb Engine](http://www.gridprotectionalliance.org/technology.asp#SnapDB) - a key/value pair archiving technology. SNAPdb was developed to significantly improve the ability to handle extremely large volumes of real-time streaming data and directly serve the data to consuming applications and systems. See the Python API implementation of [SNAPdb](snapDB).

Through use of the [SNAPdb Engine](http://www.gridprotectionalliance.org/technology.asp#SnapDB), the openHistorian inherits very fast performance with very low lag-time for data insertion. The openHistorian 2 is a time-series implementation of the SNABdb engine where the "[key](openHistorian/historianKey.py)" is a tuple of time and measurement ID, and the "[value](openHistorian/historianValue.py)" is the stored data - which can be most any data type and associated flags. See the Python API implementation of the [openHistorian instance of SNAPdb](openHistorian)

The Python API for openHistorian is designed as a socket-based, high-speed API that interacts directly with the openHistorian in-memory cache for very high speed extraction of near real-time data. The archive files produced by the openHistorian are [ACID Compliant](https://en.wikipedia.org/wiki/ACID) which create a very durable and consistent file structure that is resistant to data corruption. Internally the data structure is based on a [B+ Tree](https://en.wikipedia.org/wiki/B%2B_tree) that allows out-of-order data insertion.

# License
openHistorian and the Python API are licensed under the [MIT License](https://opensource.org/licenses/MIT).
