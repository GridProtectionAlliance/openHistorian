#******************************************************************************************************
#  main.py - Gbtc
#
#  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
#
#  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
#  the NOTICE file distributed with this work for additional information regarding copyright ownership.
#  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
#  file except in compliance with the License. You may obtain a copy of the License at:
#
#      http://opensource.org/licenses/MIT
#
#  Unless agreed to in writing, the subject software distributed under the License is distributed on an
#  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
#  License for the specific language governing permissions and limitations.
#
#  Code Modification History:
#  ----------------------------------------------------------------------------------------------------
#  02/03/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from openHistorian.historianConnection import historianConnection
from openHistorian.historianInstance import historianInstance
from openHistorian.historianKey import historianKey
from openHistorian.historianValue import historianValue
from openHistorian.metadataCache import metadataCache
from openHistorian.measurementRecord import measurementRecord, SignalType
from snapDB.timestampSeekFilter import timestampSeekFilter
from snapDB.pointIDMatchFilter import pointIDMatchFilter
from snapDB.enumerations import QualityFlags
from gsf import Ticks
from typing import Optional, List
from datetime import datetime, timedelta
from time import time, sleep
import numpy as np

def main():
    print("Creating openHistorian API")
    
    historian = historianConnection("localhost")
    instance: Optional[historianInstance] = None

    print("Connecting to openHistorian...")
    
    try:
        initialInstance: Optional[str] = None

        historian.Connect()
        
        if historian.IsConnected:
            print(f"Connected to \"{historian.HostAddress}\"!")

            # Suppress default output with: historian.RefreshMetadata(logOutput = lambda value: None)
            recordCount = historian.RefreshMetadata()

            print("Available openHistorian instances:")
            for instanceName in historian.InstanceNames:
                print(f"   {instanceName}")

                if initialInstance is None:
                    initialInstance = instanceName

                instanceInfo = historian.GetInstanceInfo(instanceName)
                keyTypeName = "Unregistered key type" if instanceInfo.KeyTypeName is None else instanceInfo.KeyTypeName
                valueTypeName = "Unregistered value type" if instanceInfo.ValueTypeName is None else instanceInfo.ValueTypeName

                print(f"          Key Type: {keyTypeName} {{{instanceInfo.KeyTypeID}}}")
                print(f"        Value Type: {valueTypeName} {{{instanceInfo.ValueTypeID}}}")
                print(f"    Encoding Modes: {len(instanceInfo.SupportedEncodings):,}")
                print("")

                i = 0
                for mode in instanceInfo.SupportedEncodings:
                    i += 1                    
                    print(f"        Encoding Mode {i} {mode.ToString()}")
                    print(f"             Key-Value Encoded: {mode.IsKeyValueEncoded}")
                    print(f"            Fixed Size Encoded: {mode.IsFixedSizeEncoding}")
                    print("")

            if initialInstance is None:
                print("No openHistorian instances detected!")
            else:
                print(f"Opening \"{initialInstance}\" database instance...")
                instance = historian.OpenInstance(initialInstance)

                # Execute a test write and verify read
                TestWriteAndVerify(instance)
                
                # Get a reference to the openHistorian metadata cache
                metadata = historian.Metadata

                # Execute a test read for old data
                startTime = datetime.strptime("2020-12-04 00:00:00.000", "%Y-%m-%d %H:%M:%S.%f")
                endTime = startTime + timedelta(milliseconds = 33 + 1) # Add one ms to ensure end time is inclusive

                # Create a list of various point IDs to query
                pointIDList = np.arange(1, 25000, 100) 
                    
                print(f"Starting read for {len(pointIDList):,} points from {startTime} to {endTime}...\r\n")
                    
                TestRead(instance, historian.Metadata, startTime, endTime, pointIDList)

                # Execute a test read for new data
                endTime = datetime.utcnow() - timedelta(seconds = 10)
                startTime = endTime - timedelta(milliseconds = 33 + 1) # Add one ms to ensure end time is inclusive

                # Lookup measurements for frequency signals
                records = metadata.GetMeasurementsBySignalType(SignalType.FREQ, instance.Name)

                # Lookup measurements for voltage phase magnitudes
                records.extend(metadata.GetMeasurementsBySignalType(SignalType.VPHM, instance.Name))

                # Lookup devices by matching text fields
                for device in metadata.GetDevicesByTextSearch("WESTPNT"):
                    records.extend(device.Measurements)
    
                recordCount = len(records)

                print(f"Queried {recordCount:,} metadata records associated with \"{instanceName}\" database instance.")

                if recordCount > 0:
                    pointIDList = metadataCache.ToPointIDList(records)
                    
                    print(f"Starting read for {len(pointIDList):,} points from {startTime} to {endTime}...\r\n")
                    
                    TestRead(instance, historian.Metadata, startTime, endTime, pointIDList)

        else:
            print("Not connected? Unexpected.")
    except Exception as ex:
        print(f"Failed to connect: {ex}")
    finally:
        if instance is not None:
            instance.Dispose()

        if historian.IsConnected:
            print("Disconnecting from openHistorian")
        
        historian.Disconnect()

def TestRead(instance: historianInstance, metadata: metadataCache, startTime: datetime, endTime: datetime, pointIDList: List[np.uint64]):
    timeFilter = timestampSeekFilter.CreateFromRange(startTime, endTime)
    pointFilter = pointIDMatchFilter.CreateFromList(pointIDList)

    opStart = time()
    reader = instance.Read(timeFilter, pointFilter)
    count = 0
                
    key = historianKey()
    value = historianValue()

    while reader.Read(key, value):
        count += 1
        print(f"    Point {key.ToString(metadata)} = {value.ToString()}")

    print(f"\r\nRead complete for {count:,} points in {(time() - opStart):.2f} seconds.\r\n")

def TestWriteAndVerify(instance: historianInstance):
    print("\r\nExecuting test write and read verification...\r\n")

    utcTime = datetime.utcnow()
    pointID = 1
    pointTime = Ticks.FromDateTime(utcTime)
    pointValue = np.float32(1000.98)
    pointQuality = QualityFlags.WARNINGHIGH
                
    key = historianKey()
    value = historianValue()

    key.PointID = pointID
    key.Timestamp = pointTime
    value.AsSingle = pointValue
    value.AsQuality = pointQuality

    print("Source values compared to those assigned to key/value pair:")
    print(f"    Point ID Match      = {key.PointID}, match: {pointID == key.PointID}")
    print(f"    Point Time Match    = {key.AsDateTime}, match: {pointTime == key.Timestamp}")
    print(f"    Point Value Match   = {value.AsSingle}, match: {pointValue == value.AsSingle}")
    print(f"    Point Quality Match = {value.AsQuality}, match: {pointQuality == value.AsQuality}")

    print("\r\nWriting a test point...")

    instance.Write(key, value)
    sleep(0.5) # Wait a moment before read

    timeFilter = timestampSeekFilter.CreateFromRange(utcTime - timedelta(milliseconds=1), utcTime)
    pointFilter = pointIDMatchFilter.CreateFromList([pointID])
                
    reader = instance.Read(timeFilter, pointFilter)
    count = 0

    print("\r\nReading test point...\r\n")

    while reader.Read(key, value):
        count += 1
        print("Source values compared to those read from historian into key/value pair:")
        print(f"    Point ID Match      = {key.PointID}, match: {pointID == key.PointID}")
        print(f"    Point Time Match    = {key.AsDateTime}, match: {pointTime == key.Timestamp}")
        print(f"    Point Value Match   = {value.AsSingle}, match: {pointValue == value.AsSingle}")
        print(f"    Point Quality Match = {value.AsQuality}, match: {pointQuality == value.AsQuality}")
                
    print(f"    Point Count Match   = {count == 1}\r\n")

if __name__ == "__main__":
    main()
