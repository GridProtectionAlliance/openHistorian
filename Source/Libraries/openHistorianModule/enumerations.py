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

from enum import IntEnum

# Defines needed enumerations for openHistorian API calls

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

