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

from openHistorian.historianInstance import historianInstance
from openHistorian.historianKey import historianKey
from openHistorian.historianValue import historianValue
from openHistorian.metadata import metadata
from snapDB.snapConnection import snapConnection
from snapDB.encodingDefinition import encodingDefinition
from gsf.streamEncoder import streamEncoder
from gsf import override
from typing import Optional
from enum import IntEnum
import socket
import gzip
import numpy as np

class ServerCommand(IntEnum):
    # Meta data refresh command.
    METADATAREFRESH = 0x01
    # Define operational modes for subscriber connection.
    DEFINEOPERATIONALMODES = 0x06

class ServerResponse(IntEnum):
    # Command succeeded response.
    SUCCEEDED = 0x80
    # Command failed response.
    FAILED = 0x81

class historianConnection(snapConnection[historianKey, historianValue]):
    """
    Defines API functionality for connecting to an openHistorian instance then
    reading and writing measurement data from the instance.

    This class is an instance of the `snapConnection` implemented for the
    openHistorian `historianKey` and `historianValue` SNAPdb types.
    """

    def __init__(self, hostAddress: str):
        super().__init__(hostAddress, historianKey(), historianValue())
        self.metadata: Optional[metadata] = None

    @override
    def OpenInstance(self, instanceName: str, definition: Optional[encodingDefinition] = None) -> historianInstance:
        return super().OpenInstance(instanceName, definition)

    @property
    def Metadata(self) -> Optional[metadata]:
        return self.metadata

    def RefreshMetadata(self, sttpPort: int = 7175):
        """
        Requests updated metadata from openHistorian connection.
        """
        sttpSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM, socket.IPPROTO_TCP)
        socketRead = lambda length: sttpSocket.recv(length)
        socketWrite = lambda buffer: sttpSocket.send(buffer)
        stream = streamEncoder(socketRead, socketWrite)

        # Using STTP connection to get metadata only (no subscription), hence the following operational modes:
        # OperationalModes.CompressMetadata | CompressionModes.GZip | OperationalEncoding.UTF8
        operationalModes = 0x80000220

        try:
            sttpSocket.connect((self.HostIPAddress, str(sttpPort)))

            # Establish operational modes for STTP connection
            stream.WriteByte(ServerCommand.DEFINEOPERATIONALMODES)
            stream.WriteUInt32(operationalModes, "big")

            # Request metadata refresh
            stream.WriteByte(ServerCommand.METADATAREFRESH)

            # Get server response
            responseCode = ServerResponse(stream.ReadByte())
            commandCode = ServerCommand(stream.ReadByte())
            length = stream.ReadInt32("big")

            if commandCode != ServerCommand.METADATAREFRESH:
                raise RuntimeError("Unexpected STTP server response for metadata request, received response for command: " + str(commandCode))
            
            if responseCode == ServerResponse.SUCCEEDED:
                # Read and decompress full metadata response XML
                buffer = gzip.decompress(historianConnection.ReadBytes(stream, length))

                # Create metadata class from metadata XML
                self.metadata = metadata(buffer.decode("utf-8"))
            else:
                raise RuntimeError("Failure code received in response to STTP metadata refresh request: " + historianConnection.ReadString(stream, length))
        finally:
            sttpSocket.close()

    @staticmethod
    def ReadBytes(stream: streamEncoder, length: int) -> bytes:
        buffer = bytearray(length)
        position = 0

        while length > 0:
            bytesRead = stream.Read(buffer, position, length)
            
            if bytesRead == 0:
                raise RuntimeError("End of stream")

            length -= bytesRead
            position += bytesRead

        return bytes(buffer)

    @staticmethod
    def ReadString(stream: streamEncoder, length: int) -> str:
        return historianConnection.ReadBytes(stream, length).decode("utf-8")