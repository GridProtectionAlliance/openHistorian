//******************************************************************************************************
//  IBinaryStream.cs - Gbtc
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
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.IO
{
    public interface IBinaryStream : IDisposable
    {
        /// <summary>
        /// Determines if the binary stream can be cloned.  
        /// Since a base stream may only be able to support a definate 
        /// number of concurrent IO Sessions, this should be analized
        /// before cloning the stream.
        /// </summary>
        bool SupportsAnotherClone { get; }

        /// <summary>
        /// Gets/Sets the current position for the stream.
        /// <remarks>It is important to use this to Get/Set the position from the underlying stream since 
        /// this class buffers the results of the query.  Setting this field does not gaurentee that
        /// the underlying stream will get set. Call FlushToUnderlyingStream to acomplish this.</remarks>
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Clones a binary stream if it is supported.  Check <see cref="IBinaryStream.SupportsAnotherClone"/> before calling this method.
        /// </summary>
        /// <returns></returns>
        IBinaryStream CloneStream();

        /// <summary>
        /// Updates the local buffer data.
        /// </summary>
        /// <param name="isWriting">hints to the stream if write access is desired.</param>
        void UpdateLocalBuffer(bool isWriting);

        /// <summary>
        /// Copies a specified number of bytes to a new location
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="length"></param>
        void Copy(long source, long destination, int length);

        /// <summary>
        /// Inserts a certain number of bytes into the stream, shifting valid data to the right.  The stream's position remains unchanged. 
        /// (ie. pointing to the beginning of the newly inserted bytes).
        /// </summary>
        /// <param name="numberOfBytes">The number of bytes to insert</param>
        /// <param name="lengthOfValidDataToShift">The number of bytes that will need to be shifted to perform this insert</param>
        /// <remarks>Internally this fuction merely acomplishes an Array.Copy(stream,position,stream,position+numberOfBytes,lengthOfValidDataToShift)
        /// However, it's much more complicated than this. So this is a pretty useful function.
        /// The newly created space is uninitialized. 
        /// </remarks>
        void InsertBytes(int numberOfBytes, int lengthOfValidDataToShift);

        /// <summary>
        /// Removes a certain number of bytes from the stream, shifting valid data after this location to the left.  The stream's position remains unchanged. 
        /// (ie. pointing to where the data used to exist).
        /// </summary>
        /// <param name="numberOfBytes">The distance to shift.  Positive means shifting to the right (ie. inserting data)
        /// Negative means shift to the left (ie. deleteing data)</param>
        /// <param name="lengthOfValidDataToShift">The number of bytes that will need to be shifted to perform the remove. 
        /// This only includes the data that is valid after the shift is complete, and not the data that will be removed.</param>
        /// <remarks>Internally this fuction merely acomplishes an Array.Copy(stream,position+numberOfBytes,stream,position,lengthOfValidDataToShift)
        /// However, it's much more complicated than this. So this is a pretty useful function.
        /// The space at the end of the copy is uninitialized. 
        /// </remarks>
        void RemoveBytes(int numberOfBytes, int lengthOfValidDataToShift);

        void Write(sbyte value);
        void Write(bool value);
        void Write(ushort value);
        void Write(uint value);
        void Write(ulong value);
        void Write(byte value);
        void Write(short value);
        void Write(int value);
        void Write(float value);
        void Write(long value);
        void Write(double value);
        void Write(DateTime value);
        void Write(decimal value);
        void Write(Guid value);
        void Write7Bit(uint value);
        void Write7Bit(ulong value);
        void Write(byte[] value, int offset, int count);
        sbyte ReadSByte();
        bool ReadBoolean();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
        byte ReadByte();
        short ReadInt16();
        int ReadInt32();
        float ReadSingle();
        long ReadInt64();
        double ReadDouble();
        DateTime ReadDateTime();
        decimal ReadDecimal();
        Guid ReadGuid();
        uint Read7BitUInt32();
        ulong Read7BitUInt64();
        int Read(byte[] value, int offset, int count);
    }
}