#******************************************************************************************************
#  enumerations.py - Gbtc
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
#  01/31/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from enum import IntEnum, Flag

# Defines needed enumerations for SNAPdb server commands and responses

class ServerCommand(IntEnum):
    CONNECTTODATABASE = 0
    DISCONNECTDATABASE = 1
    DISCONNECT = 2
    READ = 3
    CANCELREAD = 4
    WRITE = 5
    SETENCODINGMETHOD = 6
    GETALLDATABASES = 7

class ServerResponse(IntEnum):
    UNHANDLEDEXCEPTION = 0
    UNKNOWNPROTOCOL = 1
    CONNECTEDTOROOT = 2
    LISTOFDATABASES = 3
    DATABASEDOESNOTEXIST = 4
    DATABASEKEYUNKNOWN = 5
    DATABASEVALUEUNKNOWN = 6
    SUCCESSFULLYCONNECTEDTODATABASE = 7
    GOODBYE = 8
    UNKNOWNCOMMAND = 9
    UNKNOWNENCODINGMETHOD = 10
    ENCODINGMETHODACCEPTED = 11
    DATABASEDISCONNECTED = 12
    UNKNOWNDATABASECOMMAND = 13
    UNKNOWNORCORRUPTSEEKFILTER = 14
    UNKNOWNORCORRUPTMATCHFILTER = 15
    UNKNOWNORCORRUPTREADEROPTIONS = 16
    SERIALIZINGPOINTS = 17
    ERRORWHILEREADING = 18
    CANCELEDREAD = 19
    READCOMPLETE = 20
    SERVERNAMETOOLONG = 21
    SERVERNAMEDOESNOTMATCH = 22
    REQUIRESLOGIN = 23
    KNOWNPROTOCOL = 24
    AUTHENTICATIONFAILED = 25

class AuthenticationMode(IntEnum):
    NONE = 1
    SRP = 2
    SCRAM = 3
    INTEGRATED = 4
    CERTIFICATE = 5
    RESUMESESSION = 255

class QualityFlags(Flag):
    # Defines normal state.
    NORMAL = 0
    # Defines bad data state.
    BADDATA = 1
    # Defines suspect data state.
    SUSPECTDATA = 2
    # Defines over range error, i.e., unreasonable high value.
    OVERRANGEERROR = 4
    # Defines under range error, i.e., unreasonable low value.
    UNDERRANGEERROR = 8
    # Defines alarm for high value.
    ALARMHIGH = 16
    # Defines alarm for low value.
    ALARMLOW = 32
    # Defines warning for high value.
    WARNINGHIGH = 64
    # Defines warning for low value.
    WARNINGLOW = 128
    # Defines alarm for flat-lined value, i.e., latched value test alarm.
    FLATLINEALARM = 256
    # Defines comparison alarm, i.e., outside threshold of comparison with a real-time value.
    COMPARISONALARM = 512
    # Defines rate-of-change alarm.
    ROCALARM = 1024
    # Defines bad value received.
    RECEIVEDASBAD = 2048
    # Defines calculated value state.
    CALCULATEDVALUE = 4096
    # Defines calculation error with the value.
    CALCULATIONERROR = 8192
    # Defines calculation warning with the value.
    CALCULATIONWARNING = 16384
    # Defines reserved quality flag.
    RESERVEDQUALITYFLAG = 32768
    # Defines bad time state.
    BADTIME = 65536
    # Defines suspect time state.
    SUSPECTTIME = 131072
    # Defines late time alarm.
    LATETIMEALARM = 262144
    # Defines future time alarm.
    FUTURETIMEALARM = 524288
    # Defines up-sampled state.
    UPSAMPLED = 1048576
    # Defines down-sampled state.
    DOWNSAMPLED = 2097152
    # Defines discarded value state.
    DISCARDEDVALUE = 4194304
    # Defines reserved time flag.
    RESERVEDTIMEFLAG = 8388608
    # Defines user defined flag 1.
    USERDEFINEDFLAG1 = 16777216
    # Defines user defined flag 2.
    USERDEFINEDFLAG2 = 33554432
    # Defines user defined flag 3.
    USERDEFINEDFLAG3 = 67108864
    # Defines user defined flag 4.
    USERDEFINEDFLAG4 = 134217728
    # Defines user defined flag 5.
    USERDEFINEDFLAG5 = 268435456
    # Defines system error state.
    SYSTEMERROR = 536870912
    # Defines system warning state.
    SYSTEMWARNING = 1073741824
    # Defines measurement error flag.
    MEASUREMENTERROR = 2147483648
