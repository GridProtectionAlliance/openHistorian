//******************************************************************************************************
//  ISupportsBinaryStream.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/11/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

namespace openHistorian.Core.StorageSystem
{
    /// <summary>
    /// Implementing this interface allows a binary stream to be attached to a buffer.
    /// </summary>
    public interface ISupportsBinaryStream2
    {
        /// <summary>
        /// Returns an underlying buffer that can be used to read/write data from the stream.
        /// </summary>
        /// <param name="position">The position to use when referencing</param>
        /// <param name="isWriting">Notifies the class the caller's intent to write to this block.</param>
        /// <param name="buffer">a buffer where reads/writes can be performed</param>
        /// <param name="firstIndex">the first valid index in the buffer</param>
        /// <param name="lastIndex">the last valid index in the buffer</param>
        /// <param name="currentIndex">the index of the current position</param>
        void GetCurrentBlock(long position, bool isWriting, out byte[] buffer, out int firstIndex, out int lastIndex, out int currentIndex);
        /// <summary>
        /// Reads data from the underlying stream. Advancing the current position of the stream.
        /// </summary>
        /// <param name="position">The position to use when referencing</param>
        /// <param name="data">the buffer to read into</param>
        /// <param name="start">the starting postion in data</param>
        /// <param name="count">the number of bytes to read.</param>
        /// <returns>the number of bytes read.</returns>
        int Read(long position, byte[] data, int start, int count);
        /// <summary>
        /// Writes data to the underlying stream. Advancing the current position of the stream.
        /// </summary>
        /// <param name="position">The position to use when referencing</param>
        /// <param name="data">the buffer to write to</param>
        /// <param name="start">the starting position in data.</param>
        /// <param name="count">the number of bytes to write.</param>
        void Write(long position, byte[] data, int start, int count);
        /// <summary>
        /// Modifies the position of the underlying stream.
        /// </summary>
        long Position { get; set; }
    }
}
