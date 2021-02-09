#******************************************************************************************************
#  pointReader.py - Gbtc
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

from .snapTypeBase import snapTypeBase
from .keyValueEncoderBase import keyValueEncoderBase
from .treeStream import treeStream
from .enumerations import *
from . import Server
from gsf.binaryStream import binaryStream
from gsf import override
from typing import TypeVar, Generic, Callable

TKey = TypeVar('TKey', bound=snapTypeBase)
TValue = TypeVar('TValue', bound=snapTypeBase)

class pointReader(Generic[TKey, TValue], treeStream[TKey, TValue]):
    """
    Reads SNAPdb key/value pairs from a `stream`
    """
    
    def __init__(self, encoder: keyValueEncoderBase[TKey, TValue], stream: binaryStream, onComplete: Callable[[], None], key: TKey, value: TValue):
        super().__init__()
        
        self.encoder = encoder
        self.stream = stream
        self.onComplete = onComplete
        self.completed = False
        self.tempKey = key
        self.tempValue = value
        
        self.encoder.ResetEncoder()

    @override
    def ReadNext(self, key: TKey, value: TValue) -> bool:
        """
        Advances the stream to the next value.  If before the beginning of the stream, advances to
        the first value. Returns `True` if the advance was successful; otherwise, `False` if the
        end of the stream was reached.
        """
        if not self.completed and self.encoder.TryStreamDecode(self.stream, key, value):
            return True

        self.Complete()
        return False

    def Cancel(self):
        if self.completed:
            return

        # Flush the remainder of the data off of the receive queue
        while self.encoder.TryStreamDecode(self.stream, self.tempKey, self.tempValue):
            pass

        self.Complete()

    def Complete(self):
        if self.completed:
            return

        self.completed = True
        self.onComplete()

        response = Server.ReadResponse(self.stream)

        if response == ServerResponse.ERRORWHILEREADING:
            raise RuntimeError(f"SNAPdb server exception encountered while reading: {self.stream.ReadString()}")

        Server.ValidateExpectedResponses(response, ServerResponse.READCOMPLETE, ServerResponse.CANCELEDREAD)

    def Disposing(self):
        self.Cancel()
        super().Disposing()