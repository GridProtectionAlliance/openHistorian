#******************************************************************************************************
#  openHistorian.py - Gbtc
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

from streamEncoding import streamEncoding
from remoteBinaryStream import remoteBinaryStream
from databaseInfo import databaseInfo
from enumerations import *
from typing import List
import socket
import numpy as np

class openHistorian:
    """
    Defines API functionality for connecting to an openHistorian instance then
    reading and writing measurement data from the instance.
    """

    DefaultPort = 38402

    def __init__(self, hostAddress: str):
        parts = hostAddress.split(":")
        
        if len(parts) > 1:
            self.hostAddress = part[1].strip()
            self.port = int(np.uint16(part[2].strip()))
        else:
            self.hostAddress = hostAddress
            self.port = openHistorian.DefaultPort
        
        try:
            self.hostEndPoint = socket.getaddrinfo(self.hostAddress, str(self.port), family=socket.AF_INET, proto=socket.IPPROTO_TCP)[0][4]
        except:
            self.hostEndPoint = (self.hostAddress, str(self.port))
        
        self.databases = dict()
        self.socket = None
        self.stream = None

    def __socketRead(self, length: int) -> bytes:
        return self.socket.recv(length)

    def __socketWrite(self, buffer: bytes) -> int:
        return self.socket.send(buffer)

    @property
    def HostAddress(self) -> str:
        return self.hostAddress + ":" + str(self.port)

    @property
    def HostEndPoint(self) -> (str, str):
        return self.hostEndPoint

    @property
    def HostIPAddress(self) -> str:
        return self.hostEndPoint[0]

    @property
    def HostPort(self) -> int:
        return int(self.hostEndPoint[1])

    @property
    def IsConnected(self) -> bool:
        return self.stream is not None

    def Connect(self):
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM, socket.IPPROTO_TCP)
            self.socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)

            stream = streamEncoding(self.__socketRead, self.__socketWrite)
            self.socket.connect(self.hostEndPoint)

            useSsl = False

            # Initialize openHistorian client stream
            stream.WriteInt64(0x2BA517361121)
            stream.WriteBoolean(useSsl)

            response = stream.ReadByte()

            if response == ServerResponse.UNKNOWNPROTOCOL:
                raise RuntimeError("Client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.")

            if response != ServerResponse.KNOWNPROTOCOL:
                raise RuntimeError("Unexpected server response: " + str(response))

            useSsl = stream.ReadBoolean()

            if not self.__tryAuthenticate(stream, useSsl):
                raise RuntimeError("Authentication failed")

            # Establish buffered I/O for socket connected to remote binary stream
            self.stream = remoteBinaryStream(stream)

            response = self.__readServerResponse()

            if response == ServerResponse.UNKNOWNPROTOCOL:
                raise RuntimeError("Client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.")

            if response != ServerResponse.CONNECTEDTOROOT:
                raise RuntimeError("Unexpected server response: " + str(response))

            self.stream.WriteByte(ServerCommand.GETALLDATABASES)
            self.stream.Flush()
        
            response = self.__readServerResponse()

            if response != ServerResponse.LISTOFDATABASES:
                raise RuntimeError("Unexpected server response: " + str(response))

            self.databases = dict()
            count = self.stream.ReadInt32()

            while count > 0:
                count -= 1
                info = databaseInfo(self.stream)
                self.databases[info.DatabaseName] = info
        except:
            self.Disconnect()
            raise

    def Disconnect(self):
        if self.stream is not None:
            self.stream.WriteByte(ServerCommand.DISCONNECT)
            self.stream.Flush()
            self.stream = None

        if self.socket is not None:
            self.socket.close()

    def GetInstanceNames(self) -> List[str]:
        return list(self.databases.keys())

    def InstanceExists(self, instanceName: str) -> bool:
        return self.databases[instanceName] is not None

    def GetInstanceInfo(self, instanceName: str) -> databaseInfo:
        return self.databases[instanceName]

    def __readServerResponse(self) -> int:
        response = self.stream.ReadByte()

        if response == ServerResponse.UNHANDLEDEXCEPTION:
            exception = self.stream.ReadString()
            raise RuntimeError("Server unhandled exception:" + exception)

        return response

    def __tryAuthenticate(self, stream: streamEncoding, useSsl: bool) -> bool:
        if useSsl:
            raise RuntimeError("openHistorian instance is configured to require SSL. This version of the openHistorian Python API does not support SSL.")

        # Future updates can attempt to try resuming an existing secure session:
        # if __tryResumeSession(self.secureStream, stream2, certSignatures):
        #     return True

        stream.WriteByte(AuthenticationMode.NONE)

        if stream.ReadBoolean():
            if stream.ReadBoolean():
                #self.resumeTicket = stream2.ReadBytes(stream2.ReadNextByte())
                #self.sessionSecret = stream2.ReadBytes(stream2.ReadNextByte())
                raise RuntimeError("Unexpected secure stream response from openHistorian")
                
            return True
        
        return False
