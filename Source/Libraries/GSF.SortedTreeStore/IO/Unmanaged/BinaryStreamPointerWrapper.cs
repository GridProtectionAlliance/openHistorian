//******************************************************************************************************
//  BinaryStreamPointerWrapper.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  4/6/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Creates a <see cref="BinaryStreamBase"/> that wraps a single pointer.
    /// </summary>
    public unsafe class BinaryStreamPointerWrapper
        : BinaryStreamPointerBase
    {
        /// <summary>
        /// Creates a <see cref="BinaryStreamPointerWrapper"/>.
        /// </summary>
        /// <param name="stream">the byte pointer that starts the stream</param>
        /// <param name="length">the valid length of the pointer.</param>
        public BinaryStreamPointerWrapper(byte* stream, int length)
        {
            if (!BitConverter.IsLittleEndian)
                throw new Exception("Only designed to run on a little endian processor.");

            FirstPosition = 0;
            LastPosition = length;
            Current = stream;
            First = stream;
            LastRead = stream + length;
            LastWrite = stream + length;
        }

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        public override void UpdateLocalBuffer(bool isWriting)
        {
            if (Position >= LastPosition)
                throw new EndOfStreamException("Read past the end of the buffer.");
            if (Position < 0)
                throw new EndOfStreamException("Before the start of the buffer buffer.");

            //Do Nothing
        }

    }
}