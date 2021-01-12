![Logo](https://www.gridprotectionalliance.org/images/products/Producttitles75/snap.png)

The SNAPdb Engine is a high-performance key/value pair archiving system.
The SNAPdb is the archiving engine for the openHistorian 2.0.
This technology was developed as a core data archival and retrieval system designed to significantly improve the ability to archive extremely large volumes of real-time streaming data and directly serve these large data volumes to consuming applications and systems.

The "SNAP" of the SNAPdb engine is an acronym for: 
[**S**erialized](https://en.wikipedia.org/wiki/Serialization),
[**N**oSQL](https://en.wikipedia.org/wiki/NoSQL),
[**A**CID Compliant](https://en.wikipedia.org/wiki/ACID) and
[**P**erformant.](https://docs.microsoft.com/en-us/previous-versions/dotnet/articles/ms973839(v=msdn.10)?redirectedfrom=MSDN)

You can think of the SNAPdb engine like a high-speed, file-backed [dictionary or hash-table](https://en.wikipedia.org/wiki/Hash_table).

The primary development goals of the SNAPdb engine were:

### Speed
* In-memory cache for very high speed extraction of near-real time data
* Low data insertion lag time
* High-speed API for historical data extraction

### Reliability
* ACID-based system design objectives with emphasis on “durability”
* File structure resistant to data corruption

### Capability
* B+Tree based that supports out-of-sequence insertion
* Transaction-like data updates allowed
* Loss-less data compression
* Time support to +/- 100 nanoseconds (with extended time precision fields available)
* Accommodate multiple data types (any data type to value structure size)

### Other Key Features
* Store data sequentially in compressed block structures
* Distribute and archive real-time data with low latencies
* Support for high-frame rate application refresh
* Quick query and application response time
* Ability to extract or process large and very large data blocks, e.g., post-process day of high-resolution data
* Use sequential I/O to increase disk performance with lower hardware burden (SSDs not required)
