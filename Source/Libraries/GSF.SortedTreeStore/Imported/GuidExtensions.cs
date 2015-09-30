//******************************************************************************************************
//  GuidExtensions.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/07/2014 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GSF
{
    /// <summary>
    /// Extension methods for <see cref="Guid"/>.
    /// </summary>
    public unsafe static class GuidExtensions
    {
        #region [ Big Endian Based Encoding (ie RFC 4122) ]

        /// <summary>
        /// Encodes a <see cref="Guid"/> following RFC 4122.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to serialize.</param>
        /// <param name="buffer">Destination buffer to hold serialized <paramref name="guid"/>.</param>
        /// <param name="startingIndex">Starting index in <paramref name="buffer"/>.</param>
        public static int ToRfcBytes(this Guid guid, byte[] buffer, int startingIndex)
        {
            // Since Microsoft is not very clear how Guid.ToByteArray() performs on big endian processors
            // we are assuming that the internal structure of a Guid will always be the same. Reviewing
            // mono source code the internal structure is also the same.
            buffer.ValidateParameters(startingIndex, 16);

            byte* src = (byte*)&guid;

            fixed (byte* dst = &buffer[startingIndex])
            {
                if (BitConverter.IsLittleEndian)
                {
                    // Guid._a (int)
                    dst[0] = src[3];
                    dst[1] = src[2];
                    dst[2] = src[1];
                    dst[3] = src[0];

                    // Guid._b (short)
                    dst[4] = src[5];
                    dst[5] = src[4];

                    // Guid._c (short)
                    dst[6] = src[7];
                    dst[7] = src[6];

                    // Guid._d - Guid._k (8 bytes)
                    // Since already encoded as big endian, just copy the data
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }
                else
                {
                    // All fields are encoded big-endian, just copy
                    *(long*)(dst + 0) = *(long*)(src + 0);
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }
            }
            return 16;
        }

        /// <summary>
        /// Encodes a <see cref="Guid"/> following RFC 4122.
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> to serialize.</param>
        /// <param name="buffer">Destination buffer to hold serialized <paramref name="guid"/>.</param>
        public static int ToRfcBytes(this Guid guid, byte* buffer)
        {
            byte* src = (byte*)&guid;
            if (BitConverter.IsLittleEndian)
            {
                // Guid._a (int)
                buffer[0] = src[3];
                buffer[1] = src[2];
                buffer[2] = src[1];
                buffer[3] = src[0];

                // Guid._b (short)
                buffer[4] = src[5];
                buffer[5] = src[4];

                // Guid._c (short)
                buffer[6] = src[7];
                buffer[7] = src[6];

                // Guid._d - Guid._k (8 bytes)
                // Since already encoded as big endian, just copy the data
                *(long*)(buffer + 8) = *(long*)(src + 8);
            }
            else
            {
                // All fields are encoded big-endian, just copy
                *(long*)(buffer + 0) = *(long*)(src + 0);
                *(long*)(buffer + 8) = *(long*)(src + 8);
            }
            return 16;
        }

        /// <summary>
        /// Encodes a <see cref="Guid"/> following RFC 4122.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/> to serialize.</param>
        /// <returns>A <see cref="byte"/> array that represents a big-endian encoded <see cref="Guid"/>.</returns>
        public static byte[] ToRfcBytes(this Guid guid)
        {
            byte[] rv = new byte[16];
            guid.ToRfcBytes(rv, 0);
            return rv;
        }

        /// <summary>
        /// Decodes a <see cref="Guid"/> following RFC 4122
        /// </summary>
        /// <param name="buffer">Buffer containing a serialized big-endian encoded <see cref="Guid"/>.</param>
        /// <returns><see cref="Guid"/> deserialized from <paramref name="buffer"/>.</returns>
        public static Guid ToRfcGuid(this byte[] buffer)
        {
            return buffer.ToRfcGuid(0);
        }

        /// <summary>
        /// Decodes a <see cref="Guid"/> following RFC 4122
        /// </summary>
        /// <param name="buffer">Buffer containing a serialized big-endian encoded <see cref="Guid"/>.</param>
        /// <returns><see cref="Guid"/> deserialized from <paramref name="buffer"/>.</returns>
        public static Guid ToRfcGuid(byte* buffer)
        {
            // Since Microsoft is not very clear how Guid.ToByteArray() performs on big endian processors
            // we are assuming that the internal structure of a Guid will always be the same. Reviewing
            // mono source code the internal structure is also the same.
            Guid rv;
            byte* dst = (byte*)&rv;

            if (BitConverter.IsLittleEndian)
            {
                // Guid._a (int)
                dst[0] = buffer[3];
                dst[1] = buffer[2];
                dst[2] = buffer[1];
                dst[3] = buffer[0];

                // Guid._b (short)
                dst[4] = buffer[5];
                dst[5] = buffer[4];

                // Guid._c (short)
                dst[6] = buffer[7];
                dst[7] = buffer[6];

                // Guid._d - Guid._k (8 bytes)
                // Since already encoded as big endian, just copy the data
                *(long*)(dst + 8) = *(long*)(buffer + 8);
            }
            else
            {
                // All fields are encoded big-endian, just copy
                *(long*)(dst + 0) = *(long*)(buffer + 0);
                *(long*)(dst + 8) = *(long*)(buffer + 8);
            }

            return rv;
        }

        /// <summary>
        /// Decodes a <see cref="Guid"/> following RFC 4122
        /// </summary>
        /// <param name="buffer">Buffer containing a serialized big-endian encoded <see cref="Guid"/>.</param>
        /// <param name="startingIndex">Starting index in <paramref name="buffer"/>.</param>
        /// <returns><see cref="Guid"/> deserialized from <paramref name="buffer"/>.</returns>
        public static Guid ToRfcGuid(this byte[] buffer, int startingIndex)
        {
            buffer.ValidateParameters(startingIndex, 16);

            // Since Microsoft is not very clear how Guid.ToByteArray() performs on big endian processors
            // we are assuming that the internal structure of a Guid will always be the same. Reviewing
            // mono source code the internal structure is also the same.
            Guid rv;
            byte* dst = (byte*)&rv;

            fixed (byte* src = &buffer[startingIndex])
            {
                if (BitConverter.IsLittleEndian)
                {
                    // Guid._a (int)
                    dst[0] = src[3];
                    dst[1] = src[2];
                    dst[2] = src[1];
                    dst[3] = src[0];

                    // Guid._b (short)
                    dst[4] = src[5];
                    dst[5] = src[4];

                    // Guid._c (short)
                    dst[6] = src[7];
                    dst[7] = src[6];

                    // Guid._d - Guid._k (8 bytes)
                    // Since already encoded as big endian, just copy the data
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }
                else
                {
                    // All fields are encoded big-endian, just copy
                    *(long*)(dst + 0) = *(long*)(src + 0);
                    *(long*)(dst + 8) = *(long*)(src + 8);
                }

                return rv;
            }
        }

        #endregion

        #region [ Little Endian Encoding ]

        //---------------------------------------------------------------------------------------------------------
        // To encourage the use of RFCBytes, these methods will not be extension methods. 
        //---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the little endian encoded bytes.
        /// </summary>
        /// <param name="guid">the <see cref="Guid"/> to serialize</param>
        /// <returns>A <see cref="byte"/>[] that represents a big endian encoded Guid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToLittleEndianBytes(Guid guid)
        {
            byte[] rv = new byte[16];
            CopyLittleEndianBytes(guid, rv, 0);
            return rv;
        }

        /// <summary>
        /// Writes a Guid in Little Endian byte order.
        /// </summary>
        /// <param name="guid">the <see cref="Guid"/> to serialize</param>
        /// <param name="buffer">where to store the <paramref name="guid"/></param>
        /// <param name="startingIndex">the starting index in <paramref name="buffer"/></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CopyLittleEndianBytes(Guid guid, byte[] buffer, int startingIndex)
        {
            buffer.ValidateParameters(startingIndex, 16);
            fixed (byte* dst = &buffer[startingIndex])
                return CopyLittleEndianBytes(guid, dst);
        }

        /// <summary>
        /// Writes a Guid in Little Endian byte order.
        /// </summary>
        /// <param name="guid">the <see cref="Guid"/> to serialize</param>
        /// <param name="buffer">where to store the <paramref name="guid"/></param>
        public static int CopyLittleEndianBytes(Guid guid, byte* buffer)
        {
            byte* src = (byte*)&guid;
            if (BitConverter.IsLittleEndian)
            {
                //just copy the data
                *(long*)(buffer + 0) = *(long*)(src + 0);
                *(long*)(buffer + 8) = *(long*)(src + 8);
            }
            else
            {
                //Guid._a (int)  //swap endian
                buffer[0] = src[3];
                buffer[1] = src[2];
                buffer[2] = src[1];
                buffer[3] = src[0];
                //Guid._b (short) //swap endian
                buffer[4] = src[5];
                buffer[5] = src[4];
                //Guid._c (short) //swap endian
                buffer[6] = src[7];
                buffer[7] = src[6];
                //Guid._d - Guid._k (8 bytes)
                *(long*)(buffer + 8) = *(long*)(src + 8);
            }
            return 16;
        }

        /// <summary>
        /// Reads a Guid in Little Endian byte order.
        /// </summary>
        /// <param name="buffer">where to read the <see cref="Guid"/>.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ToLittleEndianGuid(byte[] buffer)
        {
            buffer.ValidateParameters(0, 16);
            fixed (byte* src = buffer)
                return ToLittleEndianGuid(src);
        }

        /// <summary>
        /// Mimicks the encoding that was present in LittleEndianOrder on a Little Endian CPU. 
        /// </summary>
        /// <param name="buffer">where to read the <see cref="Guid"/>.</param>
        /// <param name="startingIndex">the starting index in <paramref name="buffer"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ToLittleEndianGuid(byte[] buffer, int startingIndex)
        {
            buffer.ValidateParameters(startingIndex, 16);
            fixed (byte* src = &buffer[startingIndex])
                return ToLittleEndianGuid(src);
        }

        /// <summary>
        /// Reads a Guid in Little Endian byte order.
        /// </summary>
        /// <param name="buffer">where to read the <see cref="Guid"/>.</param>
        /// <returns></returns>
        public static Guid ToLittleEndianGuid(byte* buffer)
        {
            Guid rv;
            byte* dst = (byte*)&rv;
            if (BitConverter.IsLittleEndian)
            {
                //internal stucture is correct, just copy
                *(long*)(dst + 0) = *(long*)(buffer + 0);
                *(long*)(dst + 8) = *(long*)(buffer + 8);
            }
            else
            {
                //Guid._a (int) //swap endian
                dst[0] = buffer[3];
                dst[1] = buffer[2];
                dst[2] = buffer[1];
                dst[3] = buffer[0];
                //Guid._b (short) //swap endian
                dst[4] = buffer[5];
                dst[5] = buffer[4];
                //Guid._c (short) //swap endian
                dst[6] = buffer[7];
                dst[7] = buffer[6];
                //Guid._d - Guid._k (8 bytes)
                *(long*)(dst + 8) = *(long*)(buffer + 8);
            }
            return rv;
        }

        #endregion

        //---------------------------------------------------------------------------------------------------------
        // Obsolete methods for compatibility with incorrectly implemented old code in EndianOrder's Guid methods
        // EndianOrder's encoding methods were not architecture independent. Therefore, the little endian
        // Architecture was assumed and these methods were deprecated
        //---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Mimicks the encoding that was present in BigEndianOrder. 
        /// </summary>
        /// <param name="guid">the <see cref="Guid"/> to serialize</param>
        /// <returns>A <see cref="byte"/>[] that represents a big endian encoded Guid.</returns>
        [Obsolete("This method is for backwards compatibility only. Use ToRfcBytes from now on.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[] __ToBigEndianOrderBytes(Guid guid)
        {
            byte[] rv = new byte[16];
            __ToBigEndianOrderBytes(guid, rv, 0);
            return rv;
        }

        /// <summary>
        /// Mimicks the encoding that was present in BigEndianOrder. 
        /// </summary>
        /// <param name="guid">the <see cref="Guid"/> to serialize</param>
        /// <param name="buffer">where to store the <paramref name="guid"/></param>
        /// <param name="startingIndex">the starting index in <paramref name="buffer"/></param>
        [Obsolete("This method is for backwards compatibility only. Use ToRfcBytes from now on.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int __ToBigEndianOrderBytes(Guid guid, byte[] buffer, int startingIndex)
        {
            buffer.ValidateParameters(startingIndex, 16);

            byte* src = (byte*)&guid;
            fixed (byte* dst = &buffer[startingIndex])
            {
                if (BitConverter.IsLittleEndian)
                {
                    dst[15] = src[0];
                    dst[14] = src[1];
                    dst[13] = src[2];
                    dst[12] = src[3];
                    dst[11] = src[4];
                    dst[10] = src[5];
                    dst[9] = src[6];
                    dst[8] = src[7];
                    dst[7] = src[8];
                    dst[6] = src[9];
                    dst[5] = src[10];
                    dst[4] = src[11];
                    dst[3] = src[12];
                    dst[2] = src[13];
                    dst[1] = src[14];
                    dst[0] = src[15];
                }
                else
                {
                    //Guid._a (int)  //swap endian
                    dst[15] = src[3];
                    dst[14] = src[2];
                    dst[13] = src[1];
                    dst[12] = src[0];
                    //Guid._b (short) //swap endian
                    dst[11] = src[5];
                    dst[10] = src[4];
                    //Guid._c (short) //swap endian
                    dst[9] = src[7];
                    dst[8] = src[6];
                    //Guid._d - Guid._k (8 bytes)
                    dst[7] = src[8];
                    dst[6] = src[9];
                    dst[5] = src[10];
                    dst[4] = src[11];
                    dst[3] = src[12];
                    dst[2] = src[13];
                    dst[1] = src[14];
                    dst[0] = src[15];
                }
            }
            return 16;
        }

        /// <summary>
        /// Mimicks the encoding that was present in BigEndianOrder. 
        /// </summary>
        /// <param name="buffer">where to read the <see cref="Guid"/>.</param>
        /// <returns></returns>
        [Obsolete("This method is for backwards compatibility only. Use ToRfcGuid from now on.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid __ToBigEndianOrderGuid(byte[] buffer)
        {
            return __ToBigEndianOrderGuid(buffer, 0);
        }

        /// <summary>
        /// Mimicks the encoding that was present in BigEndianOrder. 
        /// </summary>
        /// <param name="buffer">where to read the <see cref="Guid"/>.</param>
        /// <param name="startingIndex">the starting index in <paramref name="buffer"/></param>
        /// <returns></returns>
        [Obsolete("This method is for backwards compatibility only. Use ToRfcGuid from now on.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid __ToBigEndianOrderGuid(byte[] buffer, int startingIndex)
        {
            buffer.ValidateParameters(startingIndex, 16);

            Guid rv;
            byte* dst = (byte*)&rv;
            fixed (byte* src = &buffer[startingIndex])
            {
                if (BitConverter.IsLittleEndian)
                {
                    dst[15] = src[0];
                    dst[14] = src[1];
                    dst[13] = src[2];
                    dst[12] = src[3];
                    dst[11] = src[4];
                    dst[10] = src[5];
                    dst[9] = src[6];
                    dst[8] = src[7];
                    dst[7] = src[8];
                    dst[6] = src[9];
                    dst[5] = src[10];
                    dst[4] = src[11];
                    dst[3] = src[12];
                    dst[2] = src[13];
                    dst[1] = src[14];
                    dst[0] = src[15];
                }
                else
                {
                    //ToDo: Test this on a big endian architecture.

                    //Guid._a (int)  //swap endian
                    dst[15] = src[3];
                    dst[14] = src[2];
                    dst[13] = src[1];
                    dst[12] = src[0];
                    //Guid._b (short) //swap endian
                    dst[11] = src[5];
                    dst[10] = src[4];
                    //Guid._c (short) //swap endian
                    dst[9] = src[7];
                    dst[8] = src[6];
                    //Guid._d - Guid._k (8 bytes)
                    dst[7] = src[8];
                    dst[6] = src[9];
                    dst[5] = src[10];
                    dst[4] = src[11];
                    dst[3] = src[12];
                    dst[2] = src[13];
                    dst[1] = src[14];
                    dst[0] = src[15];
                }
                return rv;
            }
        }


      
    }
}
