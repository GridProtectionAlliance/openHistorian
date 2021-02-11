![Logo](http://gridprotectionalliance.org/images/products/icons%2064/openHistorian.png)![Banner](https://raw.githubusercontent.com/GridProtectionAlliance/openHistorian/master/Source/Documentation/Readme%20files/openHistorian2.png)

# openHistorian SNAPdb Implementation in Python

This Python based openHistorian implementation of the generic [SNAPdb](../snapDB) Engine, a key/value pair archive, uses a time-series "key" (see [historianKey.py](historianKey.py)), a tuple of time and measurement ID, and a "value" (see [historianValue.py](historianValue.py)) a tuple of three uin64 values which represents the time-series value and quality flags to store.

The openHistorian implementation also uses an [STTP](https://github.com/sttp/) connection for acquiring metadata and exposing it as a cache (see [metadataCache.py](metadataCache.py)) in the openHistorian connection (see [historianConnection.py](historianConnection.py)).
