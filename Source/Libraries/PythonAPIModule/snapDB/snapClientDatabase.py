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

from snapDB.databaseInfo import databaseInfo
from snapDB.encodingDefinition import encodingDefinition
from snapDB.snapTypeBase import snapTypeBase
from snapDB.keyValueEncoderBase import keyValueEncoderBase
from snapDB.pointReader import pointReader
from snapDB.treeStream import treeStream
from snapDB.seekFilterBase import seekFilterBase
from snapDB.readerOptions import readerOptions
from snapDB.library import library
from snapDB.enumerations import *
from snapDB import Server
from gsf.binaryStream import binaryStream
from typing import TypeVar, Generic, Optional

TKey = TypeVar('TKey', bound=snapTypeBase)
TValue = TypeVar('TValue', bound=snapTypeBase)

class snapClientDatabase(Generic[TKey, TValue]):
    """
    Represents a single historian database, a.k.a., an "instance", that
    reads and writes data from an underlying stream, e.g., a `socket`.
    """
    
    # Source C# reference: StreamingClientDatabase<TKey, TValue>

    def __init__(self, stream: binaryStream, info: databaseInfo, key: TKey, value: TValue):
        self.stream = stream
        self.info = info
        self.tempKey = key
        self.tempValue = value
        self.encoder : Optional[keyValueEncoderBase] = None
        self.reader : Optional[pointReader] = None
        self.disposed = False

    @property
    def Info(self) -> databaseInfo:
        """
        Gets basic information about the current database instance.
        """
        return self.info

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
            raise RuntimeError("Provided encoding method " + definition.ToString() + " is not registered")

        self.stream.WriteByte(ServerCommand.SETENCODINGMETHOD)
        definition.Save(self.stream)
        self.stream.Flush()

        response = Server.ReadResponse(self.stream)

        if response == ServerResponse.UNKNOWNENCODINGMETHOD:
            raise RuntimeError("SNAPdb server reports encoding method " + definition.ToString() + " is unrecognized, hence it is unsupported")

        Server.ValidateExpectedResponse(response, ServerResponse.ENCODINGMETHODACCEPTED)

    # keyMatchFilter: Optional[matchFilterBase[TKey, TValue]]
    def Read(keySeekFilter: Optional[seekFilterBase[TKey]], keyMatchFilter, options: Optional[readerOptions] = readerOptions()) -> treeStream[TKey, TValue]:
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

        if options is None:
            options = readerOptions()

    def Dispose(self):
        if self.disposed:
            return

        self.disposed = True

        if self.reader is not None:
            self.reader.Dispose()

        self.stream.WriteByte(ServerCommand.DISCONNECTDATABASE)
        self.stream.Flush()

        Server.ValidateExpectedReadResponse(self.stream, ServerResponse.DATABASEDISCONNECTED)


