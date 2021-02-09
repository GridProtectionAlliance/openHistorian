#******************************************************************************************************
#  treeStream.py - Gbtc
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

from abc import ABC, abstractmethod
from typing import TypeVar, Generic

TKey = TypeVar('TKey')
TValue = TypeVar('TValue')

class treeStream(ABC, Generic[TKey, TValue]):
    """
    Represents a stream of SNAPdb key/value pairs.
    """

    def __init__(self):
        self.eos = False
        self.disposed = False

    @property
    def EOS(self) -> bool:
        """
        Gets flag indicating if the end of the stream has been read or class has been disposed.
        """
        return self.eos

    @property
    def IsDisposed(self) -> bool:
        """
        Gets flag indicating if class has been disposed.
        """
        return self.disposed

    @property
    def IsAlwaysSequential(self) -> bool:
        """
        Gets flag indicating if the stream is always in sequential order. Do not return true unless
        it is guaranteed that the data read from this stream is sequential.
        """
        return False

    @property        
    def NeverContainsDuplicates(self) -> bool:
        """
        Gets flag indicating if the stream will never return duplicate keys. Do not return true unless
        it is guaranteed that the data read from this stream will never contain duplicates.
        """
        return False

    def Read(self, key: TKey, value: TValue) -> bool:
        """
        Advances the stream to the next value.
        """
        if self.eos or not self.ReadNext(key, value):
            self.EndOfStreamReached()
            return False

        return True

    def EndOfStreamReached(self):
        """
        Occurs when the end of the stream has been reached. The default behavior is to call `Dispose`.
        """
        self.Dispose()

    def SetEOS(self, value: bool):
        """
        Resets the EOS flag.
        """
        if self.disposed and not value:
            raise ValueError(f"Stream {type(self).__name__} has already been disposed")

        self.eos = value

    @abstractmethod
    def ReadNext(self, key: TKey, value: TValue) -> bool:
        """
        Advances the stream to the next value.  If before the beginning of the stream, advances to
        the first value. Returns `True` if the advance was successful; otherwise, `False` if the
        end of the stream was reached.
        """

    def Dispose(self):
        """
        Derived classes should not override `Dispose` method. Instead, derived classes
        should override `Disposing` method for custom class shutdown operations.
        """
        if self.disposed:
            return

        try:
            self.Disposing()
        finally:
            self.eos = True
            self.disposed = True
  
    def Disposing(self):
        """
        Derived classes should override this method for custom class shutdown operations.
        """
        self.eos = True
