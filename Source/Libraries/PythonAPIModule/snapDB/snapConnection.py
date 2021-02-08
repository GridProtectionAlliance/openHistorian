#******************************************************************************************************
#  snapConnection.py - Gbtc
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
#  02/06/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from snapDB.databaseInfo import databaseInfo
from snapDB.encodingDefinition import encodingDefinition
from snapDB.snapClientDatabase import snapClientDatabase
from snapDB.snapTypeBase import snapTypeBase
from snapDB.enumerations import *
from snapDB import Server
from gsf.streamEncoder import streamEncoder
from gsf.binaryStream import binaryStream
from typing import TypeVar, Generic, List, Optional
import socket
import numpy as np

TKey = TypeVar('TKey', bound=snapTypeBase)
TValue = TypeVar('TValue', bound=snapTypeBase)

class snapConnection(Generic[TKey, TValue]):
    """
    Represents a generic SNAPdb connection.
    """

    DefaultPort = 38402

    def __init__(self, hostAddress: str, key: TKey, value: TValue):
        parts = hostAddress.split(":")
        
        if len(parts) > 1:
            self.hostAddress = part[1].strip()
            self.port = int(np.uint16(part[2].strip()))
        else:
            self.hostAddress = hostAddress
            self.port = snapConnection.DefaultPort
        
        try:
            self.hostEndPoint = socket.getaddrinfo(self.hostAddress, self.port, family=socket.AF_INET, proto=socket.IPPROTO_TCP)[0][4]
        except:
            self.hostEndPoint = (self.hostAddress, self.port)

        self.instances = dict()
        self.socket : Optional[socket.socket] = None
        self.stream : Optional[binaryStream] = None
        self.instance : Optional[snapClientDatabase[TKey, TValue]] = None
        self.key = key
        self.value = value

    def __socketRead(self, length: int) -> bytes:
        return self.socket.recv(length)

    def __socketWrite(self, buffer: bytes) -> int:
        return self.socket.send(buffer)
    
    @property
    def HostAddress(self) -> str:
        """
        Gets SNABdb server address and port, e.g., "localhost:38402".
        """
        return self.hostAddress + ":" + str(self.port)

    @property
    def HostEndPoint(self) -> (str, int):
        """
        Gets SNABdb server end point, e.g., ("127.0.0.1:, "38402").
        """
        return self.hostEndPoint

    @property
    def HostIPAddress(self) -> str:
        """
        Gets SNABdb server address, e.g., "127.0.0.1"
        """
        return self.hostEndPoint[0]

    @property
    def HostPort(self) -> int:
        """
        Gets SNABdb server port, e.g., 38402.
        """
        return self.hostEndPoint[1]

    @property
    def IsConnected(self) -> bool:
        """
        Gets flag indicating if connected to SNABdb server.
        """
        return self.stream is not None
    
    @property
    def InstanceNames(self) -> List[str]:
        """
        Gets list of client database instances available in SNABdb server connection.
        """
        return list(self.instances.keys())

    def Connect(self):
        """
        Attempts connection to SNAPdb server.
        """
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM, socket.IPPROTO_TCP)
            self.socket.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)

            stream = streamEncoder(self.__socketRead, self.__socketWrite)
            self.socket.connect(self.hostEndPoint)

            useSsl = False

            # Initialize SNAPdb client stream
            stream.WriteInt64(0x2BA517361121)
            stream.WriteBoolean(useSsl)

            response = stream.ReadByte()

            if response == ServerResponse.UNKNOWNPROTOCOL:
                raise RuntimeError("SNAPdb client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.")

            Server.ValidateExpectedResponse(response, ServerResponse.KNOWNPROTOCOL)

            useSsl = stream.ReadBoolean()

            if not self.__tryAuthenticate(stream, useSsl):
                raise RuntimeError("SNAPdb authentication failed")

            # Establish buffered I/O for socket connected to remote binary stream
            self.stream = binaryStream(stream)

            response = Server.ReadResponse(self.stream)

            if response == ServerResponse.UNKNOWNPROTOCOL:
                raise RuntimeError("SNAPdb client and server cannot agree on a protocol, this is commonly because they are running incompatible versions.")

            Server.ValidateExpectedResponse(response, ServerResponse.CONNECTEDTOROOT)

            self.stream.WriteByte(ServerCommand.GETALLDATABASES)
            self.stream.Flush()
        
            Server.ValidateExpectedReadResponse(self.stream, ServerResponse.LISTOFDATABASES)

            self.instances = dict()
            count = self.stream.ReadInt32()

            while count > 0:
                count -= 1
                info = databaseInfo(self.stream)
                self.instances[info.DatabaseName] = info
        except:
            self.Disconnect()
            raise

    def Disconnect(self):
        """
        Disconnects from SNAPdb server.
        """
        self.CloseInstance()

        if self.stream is not None:
            self.stream.WriteByte(ServerCommand.DISCONNECT)
            self.stream.Flush()
            self.stream = None

        if self.socket is not None:
            self.socket.close()
            self.socket = None

    def InstanceExists(self, instanceName: str) -> bool:
        """
        Gets flag indicating if `instanceName` exists for list of client database
        instances available in SNABdb server connection.
        """
        return self.instances[instanceName] is not None

    def GetInstanceInfo(self, instanceName: str) -> Optional[databaseInfo]:
        """
        Gets details about SNAPdb client database instance.
        """
        return self.instances[instanceName]

    def OpenInstance(self, instanceName: str, definition: Optional[encodingDefinition] = None) -> snapClientDatabase[TKey, TValue]:
        """
        Attempts to open a connection to SNAPdb client database instance.
        If successful, connection instance will be returned and can be used to
        read and write key / value data for SNAPdb client database instance.
        """
        if self.instance is not None and not self.instance.IsDisposed:
            raise RuntimeError("SNAPdb instance \"" + self.instance.Info.DatabaseName + "\" is currently open. Only one SNAPdb instance can be open at once, call CloseInstance() API method first.")
        
        info = self.GetInstanceInfo(instanceName)

        if info is None:
            raise RuntimeError("Failed to find SNAPdb instance \"" + instanceName  + "\"")

        if definition is None:
            if len(info.SupportedEncodings) == 0:
                raise RuntimeError("Failed to find any encoding definitions for SNAPdb instance \"" + instanceName  + "\"")

            definition = info.SupportedEncodings[0]

        self.stream.WriteByte(ServerCommand.CONNECTTODATABASE)
        self.stream.WriteString(instanceName)
        self.stream.WriteGuid(self.key.TypeID)
        self.stream.WriteGuid(self.value.TypeID)
        self.stream.Flush()
        
        response = Server.ReadResponse(self.stream)

        if response == ServerResponse.DATABASEDOESNOTEXIST:
            raise RuntimeError("SNAPdb server reports instance \"" + instanceName  + "\" does not exist")

        if response == ServerResponse.DATABASEKEYUNKNOWN:
            raise RuntimeError("SNAPdb server reports SNABdb key type {" + self.key.TypeID  + "} does not match type defined for instance")

        if response == ServerResponse.DATABASEVALUEUNKNOWN:
            raise RuntimeError("SNAPdb server reports SNABdb value type {" + self.value.TypeID + "} does not match type defined for instance")

        Server.ValidateExpectedResponse(response, ServerResponse.SUCCESSFULLYCONNECTEDTODATABASE)

        self.instance = snapClientDatabase[TKey, TValue](self.stream, info, self.key, self.value)
        self.instance.SetEncodingDefinition(definition)

        return self.instance

    def CloseInstance(self):
        """
        Closes last connection to SNAPdb client database instance.
        """
        if self.instance is not None:
            self.instance.Dispose()
            self.instance = None

    def __tryAuthenticate(self, stream: streamEncoder, useSsl: bool) -> bool:
        if useSsl:
            raise RuntimeError("SNAPdb instance is configured to require SSL. This version of the SNAPdb Python API does not support SSL.")

        # Future updates can attempt to try resuming an existing secure session:
        # if __tryResumeSession(self.secureStream, stream2, certSignatures):
        #     return True

        stream.WriteByte(AuthenticationMode.NONE)

        if stream.ReadBoolean():
            if stream.ReadBoolean():
                #self.resumeTicket = stream2.ReadBytes(stream2.ReadNextByte())
                #self.sessionSecret = stream2.ReadBytes(stream2.ReadNextByte())
                raise RuntimeError("Unexpected secure stream response from SNAPdb")
                
            return True
        
        return False
