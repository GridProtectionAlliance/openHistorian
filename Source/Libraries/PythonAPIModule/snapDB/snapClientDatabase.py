#******************************************************************************************************
#  snapClientDatabase.py - Gbtc
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
#  02/05/2021 - J. Ritchie Carroll
#       Generated original version of source code.
#
#******************************************************************************************************

from .databaseInfo import databaseInfo
from .encodingDefinition import encodingDefinition
from .snapTypeBase import snapTypeBase
from .keyValueEncoderBase import keyValueEncoderBase
from .pointReader import pointReader
from .treeStream import treeStream
from .seekFilterBase import seekFilterBase
from .matchFilterBase import matchFilterBase
from .readerOptions import readerOptions
from .library import library
from .enumerations import *
from . import Server
from gsf.binaryStream import binaryStream
from typing import TypeVar, Generic, Optional, Callable

TKey = TypeVar('TKey', bound=snapTypeBase)
TValue = TypeVar('TValue', bound=snapTypeBase)

class snapClientDatabase(Generic[TKey, TValue]):
    """
    Represents a single historian database, a.k.a., an "instance", that
    reads and writes data from an underlying stream, e.g., a `socket`.
    """
    
    # Source C# reference: StreamingClientDatabase<TKey, TValue>

    def __init__(self, stream: binaryStream, info: databaseInfo, onDispose: Callable[[], None], key: TKey, value: TValue):
        self.stream = stream
        self.info = info
        self.onDispose = onDispose
        self.tempKey = key
        self.tempValue = value
        self.encoder : Optional[keyValueEncoderBase[TKey, TValue]] = None
        self.reader : Optional[pointReader[TKey, TValue]] = None
        self.disposed = False

    @property
    def Info(self) -> databaseInfo:
        """
        Gets basic information about the current database instance.
        """
        return self.info

    @property
    def Name(self) -> str:
        """
        Gets the name of the current database instance.
        """
        return self.info.DatabaseName

    @property
    def IsDisposed(self) -> bool:
        """
        Gets flag that determines if client database is disposed.
        """
        return self.disposed

    @property
    def EncodingDefinition(self) -> encodingDefinition:
        """
        Gets the assigned encoding definition for this client database instance.
        """
        if self.encoder is None:
            return None

        return self.encoder.Definition

    def SetEncodingDefinition(self, definition: encodingDefinition):
        """
        Assigns an encoder based on encoding definition for this client database instance.
        This should only be called once after database is opened.
        """
        self.encoder = library.LookupEncoder(definition)

        if self.encoder is None:
            raise RuntimeError(f"Provided encoding method {definition.ToString()} is not registered")

        self.stream.WriteByte(ServerCommand.SETENCODINGMETHOD)
        definition.Save(self.stream)
        self.stream.Flush()

        response = Server.ReadResponse(self.stream)

        if response == ServerResponse.UNKNOWNENCODINGMETHOD:
            raise RuntimeError(f"SNAPdb server reports encoding method {definition.ToString()} is unrecognized, hence it is unsupported")

        Server.ValidateExpectedResponse(response, ServerResponse.ENCODINGMETHODACCEPTED)

    def Read(self, keySeekFilter: Optional[seekFilterBase], keyMatchFilter: Optional[matchFilterBase], options: Optional[readerOptions] = None) -> treeStream[TKey, TValue]:
        """
        Reads data from the SNAPdb client database instance with the provided server side filters and read options.
        
        Parameters
        ----------
        keySeekFilter: A seek based filter to follow. Can be `None`.
        keyMatchFilter: A match based filer to follow. Can be `None`.
        readerOptions: Read options supplied to the reader. Can be `None`.
        
        Returns
        -------
        A stream that will read the specified data.
        """

        if self.reader is not None and not self.reader.IsDiposed:
            raise RuntimeError("Concurrent readers are not supported. Dispose old reader.")

        self.stream.WriteByte(ServerCommand.READ)

        if keySeekFilter is None:
            self.stream.WriteBoolean(False)
        else:
            self.stream.WriteBoolean(True)
            self.stream.WriteGuid(keySeekFilter.TypeID)
            keySeekFilter.Save(self.stream)

        if keyMatchFilter is None:
            self.stream.WriteBoolean(False)
        else:
            self.stream.WriteBoolean(True)
            self.stream.WriteGuid(keyMatchFilter.TypeID)
            keyMatchFilter.Save(self.stream)

        if options is None:
            self.stream.WriteBoolean(False)
        else:
            self.stream.WriteBoolean(True)
            options.Save(self.stream)

        self.stream.Flush()

        response = Server.ReadResponse(self.stream)

        if response == ServerResponse.UNKNOWNORCORRUPTSEEKFILTER:
            raise RuntimeError("SNAPdb server reports key seek filter is unrecognized or corrupted")

        if response == ServerResponse.UNKNOWNORCORRUPTMATCHFILTER:
            raise RuntimeError("SNAPdb server reports key match filter is unrecognized or corrupted")

        if response == ServerResponse.UNKNOWNORCORRUPTREADEROPTIONS:
            raise RuntimeError("SNAPdb server reports reader options are unrecognized or corrupted")

        if response == ServerResponse.ERRORWHILEREADING:
            raise RuntimeError(f"SNAPdb server reported an exception while reading: {self.stream.ReadString()}")

        Server.ValidateExpectedResponse(response, ServerResponse.SERIALIZINGPOINTS)

        self.reader = pointReader(self.encoder, self.stream, self.__clearReaderRef, self.tempKey, self.tempValue)
        return self.reader

    def __clearReaderRef(self):
        self.reader = None

    def WriteTreeStream(self, stream: treeStream[TKey, TValue]):
        """
        Writes all key/value pairs of the tree `stream` to the SNAPdb client database instance.
        """

        if self.reader is not None and not self.reader.IsDiposed:
            raise RuntimeError("Concurrent writing while reading is not supported. Dispose of active reader before writing.")
        
        self.stream.WriteByte(ServerCommand.WRITE)
        self.encoder.ResetEncoder()

        while stream.Read(self.tempKey, self.tempValue):
            self.encoder.StreamEncode(self.stream, self.tempKey, self.tempValue)

        self.encoder.WriteEndOfStream(self.stream)
        self.stream.Flush()

    def Write(self, key: TKey, value: TValue):
        """
        Writes an individual key/value pair to the SNAPdb client database instance.
        """

        if self.reader is not None and not self.reader.IsDiposed:
            raise RuntimeError("Concurrent writing while reading is not supported. Dispose of active reader before writing.")
        
        self.stream.WriteByte(ServerCommand.WRITE)
        self.encoder.ResetEncoder()
        self.encoder.StreamEncode(self.stream, key, value)
        self.encoder.WriteEndOfStream(self.stream)
        self.stream.Flush()

    def Dispose(self):
        if self.disposed:
            return

        self.disposed = True
        
        if self.reader is not None:
            self.Dispose()

        self.stream.WriteByte(ServerCommand.DISCONNECTDATABASE)
        self.stream.Flush()
        self.onDispose()

        Server.ValidateExpectedReadResponse(self.stream, ServerResponse.DATABASEDISCONNECTED)


