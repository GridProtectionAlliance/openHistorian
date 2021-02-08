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
from openHistorian.metadataRecord import metadataRecord, SignalType
from snapDB.timestampSeekFilter import timestampSeekFilter
from snapDB.pointIDMatchFilter import pointIDMatchFilter
from snapDB.enumerations import QualityFlags
from datetime import datetime, timedelta
import time
from typing import Optional

def main():
    print("Creating openHistorian API")
    
    historian = historianConnection("localhost")
    instance: Optional[historianInstance] = None

    print("Connecting to openHistorian...")
    
    try:
        initialInstance: Optional[str] = None

        historian.Connect()
        
        if historian.IsConnected:
            print("Connected to \"" + historian.HostAddress + "\"!")

            print("Refreshing metadata...")                
            recordCount = historian.RefreshMetadata()
            print("Parsed " + str(recordCount) + " metadata records.")

            print("Available openHistorian instances:")
            for instanceName in historian.InstanceNames:
                print("   " + instanceName)

                if initialInstance is None:
                    initialInstance = instanceName

                instanceInfo = historian.GetInstanceInfo(instanceName)
                keyTypeName = "Unregistered key type" if instanceInfo.KeyTypeName is None else instanceInfo.KeyTypeName
                valueTypeName = "Unregistered value type" if instanceInfo.ValueTypeName is None else instanceInfo.ValueTypeName

                print("          Key Type: " + keyTypeName + " {" + str(instanceInfo.KeyTypeID) + "}")
                print("        Value Type: " + valueTypeName + " {" + str(instanceInfo.ValueTypeID) + "}")
                print("    Encoding Modes: " + str(len(instanceInfo.SupportedEncodings)))
                print("")

                i = 0
                for mode in instanceInfo.SupportedEncodings:
                    i += 1                    
                    print("        Encoding Mode " + str(i) + " " + mode.ToString())                   
                    print("             Key-Value Encoded: " + str(mode.IsKeyValueEncoded))
                    print("            Fixed Size Encoded: " + str(mode.IsFixedSizeEncoding))                    
                    print("")

            if initialInstance is None:
                print("No openHistorian instances detected!")
            else:
                print("Opening \"" + initialInstance + "\" database instance...")
                instance = historian.OpenInstance(initialInstance)

                # Get metadata cache
                metadata = historian.Metadata                
                
                # Lookup point IDs for frequency signals
                records = metadata.MatchSignalType(SignalType.FREQ, initialInstance)

                # Lookup points IDs for voltage phase magnitudes
                records.extend(metadata.MatchSignalType(SignalType.VPHM, initialInstance))

                recordCount = len(records)

                print("Queried " + str(recordCount) + " metadata records associated with \"" + initialInstance + "\" database instance.")

                if recordCount > 0:
                    pointIDList = metadataCache.ToPointIDList(records)

                    #startTime = datetime.strptime("2020-11-10 00:00:00", "%Y-%m-%d %H:%M:%S")
                    #endTime = datetime.strptime("2020-11-10 00:00:01", "%Y-%m-%d %H:%M:%S")
                    endTime = datetime.utcnow() - timedelta(seconds = 10)
                    startTime = endTime - timedelta(milliseconds = 60)

                    print("Starting read for " + str(len(pointIDList)) + " points from " + str(startTime) + " to " + str(endTime) + "...\r\n")

                    timeFilter = timestampSeekFilter.CreateFromRange(startTime, endTime)
                    pointFilter = pointIDMatchFilter.CreateFromList(pointIDList)

                    readStart = time.time()
                    reader = instance.Read(timeFilter, pointFilter)
                    count = 0
                
                    key = historianKey()
                    value = historianValue()

                    while reader.Read(key, value):
                        count += 1
                        print("    Point " + key.ToString() + " = " + value.ToString())

                    print("\r\nRead complete for " + str(count) + " points in %s seconds." % (time.time() - readStart))
        else:
            print("Not connected? Unexpected.")
    except Exception as ex:
        print("Failed to connect: " + str(ex))
    finally:
        if instance is not None and not instance.IsDisposed:
            instance.Dispose()

        if historian.IsConnected:
            print("Disconnecting from openHistorian")
        
        historian.Disconnect()

if __name__ == "__main__":
    main()

#print("Writing a test point...")
                
#pointID = 1
#pointTime = datetime.utcnow()
#pointValue = 1000.98
#pointQuality = QualityFlags.WARNINGHIGH

#key.PointID = pointID
#key.AsDateTime = pointTime
#value.AsSingle = pointValue
#value.AsQuality = pointQuality

#instance.Write(key, value)

#print("Reading test point...")

#timeFilter = timestampSeekFilter.CreateFromRange(pointTime - timedelta(milliseconds=1), pointTime)
#pointFilter = pointIDMatchFilter.CreateFromList([pointID])
                
#reader = instance.Read(timeFilter, pointFilter)
#count = 0

#while reader.Read(key, value):
#    count += 1
#    print("    Point ID Match      = " + str(pointID == key.PointID))
#    print("    Point Time Match    = " + str(pointTime == key.AsDateTime))
#    print("    Point Value Match   = " + str(pointValue == value.AsSingle))
#    print("    Point Quality Match = " + str(pointQuality == value.AsQuality))
                
#print("    Point Count Match   = " + str(count == 1))
#reader.Dispose()