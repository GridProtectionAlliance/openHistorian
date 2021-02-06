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
from historianInstance import historianInstance
from typing import Optional

def main():
    print("Creating openHistorian API")
    
    api = openHistorian("localhost")
    instance: Optional[historianInstance] = None

    print("Connecting to openHistorian...")
    
    try:
        initialInstance: Optional[str] = None

        api.Connect()
        
        if api.IsConnected:
            print("Connected! Available openHistorian instances:")

            for instanceName in api.InstanceNames:
                print("   " + instanceName)

                if initialInstance is None:
                    initialInstance = instanceName

                instanceInfo = api.GetInstanceInfo(instanceName)
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

                instance = api.OpenInstance(initialInstance)
        else:
            print("Not connected? Unexpected.")
    except Exception as ex:
        print("Failed to connect: " + str(ex))
    finally:
        if instance is not None and not instance.IsDisposed:
            print("Closing \"" + initialInstance + "\" database instance...")
            instance.Dispose()

        if api.IsConnected:
            print("Disconnecting from openHistorian")
        
        api.Disconnect()

if __name__ == "__main__":
    main()
