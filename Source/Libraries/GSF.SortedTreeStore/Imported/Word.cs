//******************************************************************************************************
//  Word.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  04/10/2009 - James R. Carroll
//       Generated original version of source code.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System.Runtime.CompilerServices;

namespace GSF
{
    /// <summary>
    /// Represents functions and extensions related to 16-bit words, 32-bit double-words and 64-bit quad-words.
    /// </summary>
    public static class Word
    {
        /// <summary>
        /// Aligns word value on a 16-bit boundary.
        /// </summary>
        /// <param name="word">Word value to align.</param>
        /// <returns>Word value aligned to next 16-bit boundary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short AlignWord(this short word)
        {
            return (short)(word + 1 - (word - 1) % 2);
        }

        /// <summary>
        /// Aligns word value on a 16-bit boundary.
        /// </summary>
        /// <param name="word">Word value to align.</param>
        /// <returns>Word value aligned to next 16-bit boundary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort AlignWord(this ushort word)
        {
            return (ushort)(word + 1 - (word - 1) % 2);
        }

        /// <summary>
        /// Aligns double-word value on a 32-bit boundary.
        /// </summary>
        /// <param name="doubleWord">Double-word value to align.</param>
        /// <returns>Double-word value aligned to next 32-bit boundary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AlignDoubleWord(this int doubleWord)
        {
            return doubleWord + 3 - (doubleWord - 1) % 4;
        }

        /// <summary>
        /// Aligns double-word value on a 32-bit boundary.
        /// </summary>
        /// <param name="doubleWord">Double-word value to align.</param>
        /// <returns>Double-word value aligned to next 32-bit boundary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AlignDoubleWord(this uint doubleWord)
        {
            return doubleWord + 3 - (doubleWord - 1) % 4;
        }

        /// <summary>
        /// Aligns quad-word value on a 64-bit boundary.
        /// </summary>
        /// <param name="quadWord">Quad-word value to align.</param>
        /// <returns>Quad-word value aligned to next 64-bit boundary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AlignQuadWord(this long quadWord)
        {
            return quadWord + 7 - (quadWord - 1) % 8;
        }

        /// <summary>
        /// Aligns quad-word value on a 64-bit boundary.
        /// </summary>
        /// <param name="quadWord">Quad-word value to align.</param>
        /// <returns>Quad-word value aligned to next 64-bit boundary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AlignQuadWord(this ulong quadWord)
        {
            return quadWord + 7 - (quadWord - 1) % 8;
        }

        /// <summary>
        /// Returns the high-nibble (high 4-bits) from a byte.
        /// </summary>
        /// <param name="value">Byte value.</param>
        /// <returns>The high-nibble of the specified byte value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HighNibble(this byte value)
        {
            return (byte)((value & (byte)0xF0) >> 4);
        }

        /// <summary>
        /// Returns the high-byte from an unsigned word (UInt16).
        /// </summary>
        /// <param name="word">2-byte, 16-bit unsigned integer value.</param>
        /// <returns>The high-order byte of the specified 16-bit unsigned integer value.</returns>
        /// <remarks>
        /// On little-endian architectures (e.g., Intel platforms), this will be the byte value whose in-memory representation
        /// is the same as the right-most, most-significant-byte of the integer value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HighByte(this ushort word)
        {
            return (byte)((word & (ushort)0xFF00) >> 8);
        }

        /// <summary>
        /// Returns the unsigned high-word (UInt16) from an unsigned double-word (UInt32).
        /// </summary>
        /// <param name="doubleWord">4-byte, 32-bit unsigned integer value.</param>
        /// <returns>The unsigned high-order word of the specified 32-bit unsigned integer value.</returns>
        /// <remarks>
        /// On little-endian architectures (e.g., Intel platforms), this will be the word value
        /// whose in-memory representation is the same as the right-most, most-significant-word
        /// of the integer value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort HighWord(this uint doubleWord)
        {
            return (ushort)((doubleWord & 0xFFFF0000U) >> 16);
        }

        /// <summary>
        /// Returns the unsigned high-double-word (UInt32) from an unsigned quad-word (UInt64).
        /// </summary>
        /// <param name="quadWord">8-byte, 64-bit unsigned integer value.</param>
        /// <returns>The high-order double-word of the specified 64-bit unsigned integer value.</returns>
        /// <remarks>
        /// On little-endian architectures (e.g., Intel platforms), this will be the word value
        /// whose in-memory representation is the same as the right-most, most-significant-word
        /// of the integer value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint HighDoubleWord(this ulong quadWord)
        {
            return (uint)((quadWord & 0xFFFFFFFF00000000UL) >> 32);
        }

        /// <summary>
        /// Returns the low-nibble (low 4-bits) from a byte.
        /// </summary>
        /// <param name="value">Byte value.</param>
        /// <returns>The low-nibble of the specified byte value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LowNibble(this byte value)
        {
            return (byte)(value & (byte)0x0F);
        }

        /// <summary>
        /// Returns the low-byte from an unsigned word (UInt16).
        /// </summary>
        /// <param name="word">2-byte, 16-bit unsigned integer value.</param>
        /// <returns>The low-order byte of the specified 16-bit unsigned integer value.</returns>
        /// <remarks>
        /// On little-endian architectures (e.g., Intel platforms), this will be the byte value
        /// whose in-memory representation is the same as the left-most, least-significant-byte
        /// of the integer value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LowByte(this ushort word)
        {
            return (byte)(word & (ushort)0x00FF);
        }

        /// <summary>
        /// Returns the unsigned low-word (UInt16) from an unsigned double-word (UInt32).
        /// </summary>
        /// <param name="doubleWord">4-byte, 32-bit unsigned integer value.</param>
        /// <returns>The unsigned low-order word of the specified 32-bit unsigned integer value.</returns>
        /// <remarks>
        /// On little-endian architectures (e.g., Intel platforms), this will be the word value
        /// whose in-memory representation is the same as the left-most, least-significant-word
        /// of the integer value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort LowWord(this uint doubleWord)
        {
            return (ushort)(doubleWord & 0x0000FFFFU);
        }

        /// <summary>
        /// Returns the unsigned low-double-word (UInt32) from an unsigned quad-word (UInt64).
        /// </summary>
        /// <param name="quadWord">8-byte, 64-bit unsigned integer value.</param>
        /// <returns>The low-order double-word of the specified 64-bit unsigned integer value.</returns>
        /// <remarks>
        /// On little-endian architectures (e.g., Intel platforms), this will be the word value
        /// whose in-memory representation is the same as the left-most, least-significant-word
        /// of the integer value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LowDoubleWord(this ulong quadWord)
        {
            return (uint)(quadWord & 0x00000000FFFFFFFFUL);
        }

        /// <summary>
        /// Makes an unsigned word (UInt16) from two bytes.
        /// </summary>
        /// <param name="high">High byte.</param>
        /// <param name="low">Low byte.</param>
        /// <returns>An unsigned 16-bit word made from the two specified bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort MakeWord(byte high, byte low)
        {
            return (ushort)(low + ((ushort)high << 8));
        }

        /// <summary>
        /// Makes an unsigned double-word (UInt32) from two unsigned words (UInt16).
        /// </summary>
        /// <param name="high">High word.</param>
        /// <param name="low">Low word.</param>
        /// <returns>An unsigned 32-bit double-word made from the two specified unsigned 16-bit words.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MakeDoubleWord(ushort high, ushort low)
        {
            return (uint)(low + ((uint)high << 16));
        }

        /// <summary>
        /// Makes an unsigned quad-word (UInt64) from two unsigned double-words (UInt32).
        /// </summary>
        /// <param name="high">High double-word.</param>
        /// <param name="low">Low double-word.</param>
        /// <returns>An unsigned 64-bit quad-word made from the two specified unsigned 32-bit double-words.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MakeQuadWord(uint high, uint low)
        {
            return (ulong)(low + ((ulong)high << 32));
        }
    }
}