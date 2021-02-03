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

from openHistorian import openHistorian

def main():
    print("Creating openHistorian API")
    api = openHistorian("localhost")

    print("Connecting to openHistorian...")
    
    try:
        api.Connect()
        
        if api.IsConnected:
            print("Connected! Available openHistorian instances:")

            for instanceName in api.GetInstanceNames():
                print("   " + instanceName)

                instanceInfo = api.GetInstanceInfo(instanceName)

                i = 0
                for mode in instanceInfo.SupportedStreamingModes:
                    i += 1                    
                    print("        Streaming Mode " + str(i))
                    if mode.IsKeyValueEncoded:
                        print("            {" + str(mode.KeyValueEncodingMethod) + "}")
                    else:
                        print("            {" + str(mode.KeyEncodingMethod) + "} / {" + str(mode.ValueEncodingMethod) + "}")
                    print("            Key-Value Encoded: " + str(mode.IsKeyValueEncoded))
                    print("            Fixed Size Encoded: " + str(mode.IsFixedSizeEncoding))                    
        else:
            print("Not connected?")
    except Exception as ex:
        print("Failed to connect: " + str(ex))
    finally:
        if api.IsConnected:
            print("Disconnecting from openHistorian")
        
        api.Disconnect()

if __name__ == "__main__":
    main()
