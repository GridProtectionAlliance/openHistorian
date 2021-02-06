#******************************************************************************************************
#  __init__.py - Gbtc
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
#  02/01/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from snapDB.enumerations import *
from gsf.binaryStream import binaryStream
from struct import pack
import numpy as np

class BitConvert:
    @staticmethod
    def ToUInt64(value: np.float32) -> np.uint64:
        return np.frombuffer(struct.pack("f", np.float64(value)), np.uint64)[0]

    @staticmethod
    def ToSingle(value: np.uint64) -> np.float32:
        return np.frombuffer(np.uint32(value).to_bytes(ByteSize.UINT32, "little"), np.float32)[0]

class Server:
    """
    Defines helper functions for common server-based `remoteBinaryStream` calls
    """

    @staticmethod
    def ReadResponse(stream: binaryStream) -> int:
        response = stream.ReadByte()

        if response == ServerResponse.UNHANDLEDEXCEPTION:
            raise RuntimeError("Server unhandled exception:" + stream.ReadString())

        return response

    @staticmethod
    def ValidateExpectedResponse(response: int, expectedResponse: ServerCommand):
        if not response == expectedResponse:
            raise RuntimeError("Unexpected server response: " + str(response))

    @staticmethod
    def ValidateExpectedResponses(response: int, *expectedResponses: ServerCommand):
        foundValidResponse = False
        
        for expectedResponse in expectedResponses:
            if response == expectedResponse:
                foundValidResponse = True
                break

        if not foundValidResponse:
            raise RuntimeError("Unexpected server response: " + str(response))

    @staticmethod
    def ValidateExpectedReadResponse(stream: binaryStream, expectedResponse: ServerCommand):
        Server.ValidateExpectedResponse(Server.ReadResponse(stream), expectedResponse)

    @staticmethod
    def ValidateExpectedReadResponses(stream: binaryStream, *expectedResponses: ServerCommand):
        Server.ValidateExpectedResponses(Server.ReadResponse(stream), expectedResponses)