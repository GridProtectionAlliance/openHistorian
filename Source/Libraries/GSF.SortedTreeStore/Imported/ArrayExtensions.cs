//******************************************************************************************************
//  ArrayExtensions.cs - Gbtc
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
//  09/19/2008 - J. Ritchie Carroll
//       Generated original version of source code.
//  12/03/2008 - J. Ritchie Carroll
//       Added "Combine" and "IndexOfSequence" overloaded extensions.
//  02/13/2009 - Josh L. Patterson
//       Edited Code Comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/31/2009 - Andrew K. Hill
//       Modified the following methods per unit testing:
//       BlockCopy(T[], int, int)
//       Combine(T[], T[])
//       Combine(T[], int, int, T[], int, int)
//       Combine(T[][])
//       IndexOfSequence(T[], T[])
//       IndexOfSequence(T[], T[], int)
//       IndexOfSequence(T[], T[], int, int)
//  11/22/2011 - J. Ritchie Carroll
//       Added common case array parameter validation extensions
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GSF.IO;

namespace GSF
{
    /// <summary>
    /// Defines extension functions related to <see cref="Array"/> manipulation.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Validates that the specified <paramref name="startIndex"/> and <paramref name="length"/> are valid within the given <paramref name="array"/>.
        /// </summary>
        /// <param name="array">Array to validate.</param>
        /// <param name="startIndex">0-based start index into the <paramref name="array"/>.</param>
        /// <param name="length">Valid number of items within <paramref name="array"/> from <paramref name="startIndex"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <paramref name="length"/> will exceed <paramref name="array"/> length.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateParameters<T>(this T[] array, int startIndex, int length)
        {
            if ((object)array == null || startIndex < 0 || length < 0 || startIndex + length > array.Length)
                RaiseValidationError(array, startIndex, length);
        }

        // This method will raise the actual error - this is needed since .NET will not inline anything that might throw an exception
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RaiseValidationError<T>(T[] array, int startIndex, int length)
        {
            if ((object)array == null)
                throw new ArgumentNullException("array");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", "cannot be negative");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "cannot be negative");

            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException("length", string.Format("startIndex of {0} and length of {1} will exceed array size of {2}", startIndex, length, array.Length));
        }

        /// <summary>
        /// Returns a copy of the specified portion of the <paramref name="array"/> array.
        /// </summary>
        /// <param name="array">Source array.</param>
        /// <param name="startIndex">Offset into <paramref name="array"/> array.</param>
        /// <param name="length">Length of <paramref name="array"/> array to copy at <paramref name="startIndex"/> offset.</param>
        /// <returns>A array of data copied from the specified portion of the source array.</returns>
        /// <remarks>
        /// <para>
        /// Returned array will be extended as needed to make it the specified <paramref name="length"/>, but
        /// it will never be less than the source array length - <paramref name="startIndex"/>.
        /// </para>
        /// <para>
        /// This is a convenience function. If an existing array is already available, using the <see cref="Buffer.BlockCopy"/>
        /// directly instead of this extension method will be optimal since this method always allocates a new return array.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is outside the range of valid indexes for the source array -or-
        /// <paramref name="length"/> is less than 0.
        /// </exception>
        public static T[] BlockCopy<T>(this T[] array, int startIndex, int length)
        {
            if ((object)array == null)
                throw new ArgumentNullException("array");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", "cannot be negative");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "cannot be negative");

            if (startIndex >= array.Length)
                throw new ArgumentOutOfRangeException("startIndex", "not a valid index into the array");

            T[] copiedBytes = new T[array.Length - startIndex < length ? array.Length - startIndex : length];

            Buffer.BlockCopy(array, startIndex, copiedBytes, 0, copiedBytes.Length);

            return copiedBytes;
        }

        /// <summary>
        /// Combines arrays together into a single array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <param name="other">Other array to combine to <paramref name="source"/> array.</param>
        /// <returns>Combined arrays.</returns>
        /// <remarks>
        /// <para>
        /// Only use this function if you need a copy of the combined arrays, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined arrays.
        /// </para>
        /// <para>
        /// This function can easily throw an out of memory exception if there is not enough
        /// contiguous memory to create an array sized with the combined lengths.
        /// </para>
        /// </remarks>
        public static T[] Combine<T>(this T[] source, T[] other)
        {
            if ((object)source == null)
                throw new ArgumentNullException("source");

            if ((object)other == null)
                throw new ArgumentNullException("other");

            return source.Combine(0, source.Length, other, 0, other.Length);
        }

        /// <summary>
        /// Combines specified portions of arrays together into a single array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <param name="sourceOffset">Offset into <paramref name="source"/> array to begin copy.</param>
        /// <param name="sourceCount">Number of bytes to copy from <paramref name="source"/> array.</param>
        /// <param name="other">Other array to combine to <paramref name="source"/> array.</param>
        /// <param name="otherOffset">Offset into <paramref name="other"/> array to begin copy.</param>
        /// <param name="otherCount">Number of bytes to copy from <paramref name="other"/> array.</param>
        /// <returns>Combined specified portions of both arrays.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceOffset"/> or <paramref name="otherOffset"/> is outside the range of valid indexes for the associated array -or-
        /// <paramref name="sourceCount"/> or <paramref name="otherCount"/> is less than 0 -or- 
        /// <paramref name="sourceOffset"/> or <paramref name="otherOffset"/>, 
        /// and <paramref name="sourceCount"/> or <paramref name="otherCount"/> do not specify a valid section in the the associated array.
        /// </exception>
        /// <remarks>
        /// <para>
        /// Only use this function if you need a copy of the combined arrays, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined arrays.
        /// </para>
        /// <para>
        /// This function can easily throw an out of memory exception if there is not enough
        /// contiguous memory to create an array sized with the combined lengths.
        /// </para>
        /// </remarks>
        public static T[] Combine<T>(this T[] source, int sourceOffset, int sourceCount, T[] other, int otherOffset, int otherCount)
        {
            if ((object)source == null)
                throw new ArgumentNullException("source");

            if ((object)other == null)
                throw new ArgumentNullException("other");

            if (sourceOffset < 0)
                throw new ArgumentOutOfRangeException("sourceOffset", "cannot be negative");

            if (otherOffset < 0)
                throw new ArgumentOutOfRangeException("otherOffset", "cannot be negative");

            if (sourceCount < 0)
                throw new ArgumentOutOfRangeException("sourceCount", "cannot be negative");

            if (otherCount < 0)
                throw new ArgumentOutOfRangeException("otherCount", "cannot be negative");

            if (sourceOffset >= source.Length)
                throw new ArgumentOutOfRangeException("sourceOffset", "not a valid index into source array");

            if (otherOffset >= other.Length)
                throw new ArgumentOutOfRangeException("otherOffset", "not a valid index into other array");

            if (sourceOffset + sourceCount > source.Length)
                throw new ArgumentOutOfRangeException("sourceCount", "exceeds source array size");

            if (otherOffset + otherCount > other.Length)
                throw new ArgumentOutOfRangeException("otherCount", "exceeds other array size");

            // Overflow is possible, but unlikely.  Therefore, this is omitted for performance
            // if ((int.MaxValue - sourceCount - otherCount) < 0)
            //    throw new ArgumentOutOfRangeException("sourceCount + otherCount", "exceeds maximum array size");

            // Combine arrays together as a single image
            T[] combinedBuffer = new T[sourceCount + otherCount];

            Buffer.BlockCopy(source, sourceOffset, combinedBuffer, 0, sourceCount);
            Buffer.BlockCopy(other, otherOffset, combinedBuffer, sourceCount, otherCount);

            return combinedBuffer;
        }

        /// <summary>
        /// Combines arrays together into a single array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <param name="other1">First array to combine to <paramref name="source"/> array.</param>
        /// <param name="other2">Second array to combine to <paramref name="source"/> array.</param>
        /// <returns>Combined arrays.</returns>
        /// <remarks>
        /// <para>
        /// Only use this function if you need a copy of the combined arrays, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined arrays.
        /// </para>
        /// <para>
        /// This function can easily throw an out of memory exception if there is not enough
        /// contiguous memory to create an array sized with the combined lengths.
        /// </para>
        /// </remarks>
        public static T[] Combine<T>(this T[] source, T[] other1, T[] other2)
        {
            return (new[] { source, other1, other2 }).Combine();
        }

        /// <summary>
        /// Combines arrays together into a single array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <param name="other1">First array to combine to <paramref name="source"/> array.</param>
        /// <param name="other2">Second array to combine to <paramref name="source"/> array.</param>
        /// <param name="other3">Third array to combine to <paramref name="source"/> array.</param>
        /// <returns>Combined arrays.</returns>
        /// <remarks>
        /// <para>
        /// Only use this function if you need a copy of the combined arrays, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined arrays.
        /// </para>
        /// <para>
        /// This function can easily throw an out of memory exception if there is not enough
        /// contiguous memory to create an array sized with the combined lengths.
        /// </para>
        /// </remarks>
        public static T[] Combine<T>(this T[] source, T[] other1, T[] other2, T[] other3)
        {
            return (new[] { source, other1, other2, other3 }).Combine();
        }

        /// <summary>
        /// Combines arrays together into a single array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <param name="other1">First array to combine to <paramref name="source"/> array.</param>
        /// <param name="other2">Second array to combine to <paramref name="source"/> array.</param>
        /// <param name="other3">Third array to combine to <paramref name="source"/> array.</param>
        /// <param name="other4">Fourth array to combine to <paramref name="source"/> array.</param>
        /// <returns>Combined arrays.</returns>
        /// <remarks>
        /// <para>
        /// Only use this function if you need a copy of the combined arrays, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined arrays.
        /// </para>
        /// <para>
        /// This function can easily throw an out of memory exception if there is not enough
        /// contiguous memory to create an array sized with the combined lengths.
        /// </para>
        /// </remarks>
        public static T[] Combine<T>(this T[] source, T[] other1, T[] other2, T[] other3, T[] other4)
        {
            return (new[] { source, other1, other2, other3, other4 }).Combine();
        }

        /// <summary>
        /// Combines array of arrays together into a single array.
        /// </summary>
        /// <param name="arrays">Array of arrays to combine.</param>
        /// <returns>Combined arrays.</returns>
        /// <remarks>
        /// <para>
        /// Only use this function if you need a copy of the combined arrays, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined arrays.
        /// </para>
        /// <para>
        /// This function can easily throw an out of memory exception if there is not enough
        /// contiguous memory to create an array sized with the combined lengths.
        /// </para>
        /// </remarks>
        public static T[] Combine<T>(this T[][] arrays)
        {
            if ((object)arrays == null)
                throw new ArgumentNullException("arrays");

            int size = arrays.Sum(array => array.Length);
            int length, offset = 0;

            // Combine arrays together as a single image
            T[] combinedBuffer = new T[size];

            for (int x = 0; x < arrays.Length; x++)
            {
                if ((object)arrays[x] == null)
                    throw new ArgumentNullException("arrays[" + x + "]");

                length = arrays[x].Length;

                if (length > 0)
                {
                    Buffer.BlockCopy(arrays[x], 0, combinedBuffer, offset, length);
                    offset += length;
                }
            }

            return combinedBuffer;
        }

        /// <summary>
        /// Searches for the specified <paramref name="sequenceToFind"/> and returns the index of the first occurrence within the <paramref name="array"/>.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="sequenceToFind">Sequence of items to search for.</param>
        /// <returns>The zero-based index of the first occurrence of the <paramref name="sequenceToFind"/> in the <paramref name="array"/>, if found; otherwise, -1.</returns>
        public static int IndexOfSequence<T>(this T[] array, T[] sequenceToFind) where T : IComparable<T>
        {
            if ((object)array == null)
                throw new ArgumentNullException("array");

            if ((object)sequenceToFind == null)
                throw new ArgumentNullException("sequenceToFind");

            return array.IndexOfSequence(sequenceToFind, 0, array.Length);
        }

        /// <summary>
        /// Searches for the specified <paramref name="sequenceToFind"/> and returns the index of the first occurrence within the range of elements in the <paramref name="array"/>
        /// that starts at the specified index.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="sequenceToFind">Sequence of items to search for.</param>
        /// <param name="startIndex">Start index in the <paramref name="array"/> to start searching.</param>
        /// <returns>The zero-based index of the first occurrence of the <paramref name="sequenceToFind"/> in the <paramref name="array"/>, if found; otherwise, -1.</returns>
        public static int IndexOfSequence<T>(this T[] array, T[] sequenceToFind, int startIndex) where T : IComparable<T>
        {
            if ((object)array == null)
                throw new ArgumentNullException("array");

            if ((object)sequenceToFind == null)
                throw new ArgumentNullException("sequenceToFind");

            return array.IndexOfSequence(sequenceToFind, startIndex, array.Length - startIndex);
        }

        /// <summary>
        /// Searches for the specified <paramref name="sequenceToFind"/> and returns the index of the first occurrence within the range of elements in the <paramref name="array"/>
        /// that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="sequenceToFind">Sequence of items to search for.</param>
        /// <param name="startIndex">Start index in the <paramref name="array"/> to start searching.</param>
        /// <param name="length">Number of bytes in the <paramref name="array"/> to search through.</param>
        /// <returns>The zero-based index of the first occurrence of the <paramref name="sequenceToFind"/> in the <paramref name="array"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequenceToFind"/> is null or has zero length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is outside the range of valid indexes for the source array -or-
        /// <paramref name="length"/> is less than 0.
        /// </exception>
        public static int IndexOfSequence<T>(this T[] array, T[] sequenceToFind, int startIndex, int length) where T : IComparable<T>
        {
            if ((object)array == null)
                throw new ArgumentNullException("array");

            if ((object)sequenceToFind == null || sequenceToFind.Length == 0)
                throw new ArgumentNullException("sequenceToFind");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", "cannot be negative");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "cannot be negative");

            if (startIndex >= array.Length)
                throw new ArgumentOutOfRangeException("startIndex", "not a valid index into source array");

            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException("length", "exceeds array size");

            // Overflow is possible, but unlikely.  Therefore, this is omitted for performance
            // if ((int.MaxValue - startIndex - length) < 0)
            //    throw new ArgumentOutOfRangeException("startIndex + length", "exceeds maximum array size");            

            // Search for first item in the sequence, if this doesn't exist then sequence doesn't exist
            int index = Array.IndexOf(array, sequenceToFind[0], startIndex, length);

            if (sequenceToFind.Length > 1)
            {
                bool foundSequence = false;

                while (index > -1 && !foundSequence)
                {
                    // See if next bytes in sequence match
                    for (int x = 1; x < sequenceToFind.Length; x++)
                    {
                        // Make sure there's enough array remaining to accommodate this item
                        if (index + x < startIndex + length)
                        {
                            // If sequence doesn't match, search for next first-item
                            if (array[index + x].CompareTo(sequenceToFind[x]) != 0)
                            {
                                index = Array.IndexOf(array, sequenceToFind[0], index + 1, length - (index + 1));
                                break;
                            }

                            // If each item to find matched, we found the sequence
                            foundSequence = (x == sequenceToFind.Length - 1);
                        }
                        else
                        {
                            // Ran out of array, return -1
                            index = -1;
                        }
                    }
                }
            }

            return index;
        }

        /// <summary>Returns comparison results of two binary arrays.</summary>
        /// <param name="source">Source array.</param>
        /// <param name="other">Other array to compare to <paramref name="source"/> array.</param>
        /// <returns>
        /// <para>
        /// A signed integer that indicates the relative comparison of <paramref name="source"/> array and <paramref name="other"/> array.
        /// </para>
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <description>Source array is less than other array.</description>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <description>Source array is equal to other array.</description>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <description>Source array is greater than other array.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </returns>
        public static int CompareTo<T>(this T[] source, T[] other) where T : IComparable<T>
        {
            // If both arrays are assumed equal if both are nothing
            if ((object)source == null && (object)other == null)
                return 0;

            // If other array has data and source array is nothing, other array is assumed larger
            if ((object)source == null)
                return 1;

            // If source array has data and other array is nothing, source array is assumed larger
            if ((object)other == null)
                return -1;

            int length1 = source.Length;
            int length2 = other.Length;

            // If array lengths are unequal, array with largest number of elements is assumed to be largest
            if (length1 != length2)
                return length1.CompareTo(length2);

            int comparision = 0;

            // Compares elements of arrays that are of equal size.
            for (int x = 0; x < length1; x++)
            {
                comparision = source[x].CompareTo(other[x]);

                if (comparision != 0)
                    break;
            }

            return comparision;
        }

        /// <summary>
        /// Returns comparison results of two binary arrays.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <param name="sourceOffset">Offset into <paramref name="source"/> array to begin compare.</param>
        /// <param name="other">Other array to compare to <paramref name="source"/> array.</param>
        /// <param name="otherOffset">Offset into <paramref name="other"/> array to begin compare.</param>
        /// <param name="count">Number of bytes to compare in both arrays.</param>
        /// <returns>
        /// <para>
        /// A signed integer that indicates the relative comparison of <paramref name="source"/> array and <paramref name="other"/> array.
        /// </para>
        /// <para>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <description>Source array is less than other array.</description>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <description>Source array is equal to other array.</description>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <description>Source array is greater than other array.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="sourceOffset"/> or <paramref name="otherOffset"/> is outside the range of valid indexes for the associated array -or-
        /// <paramref name="count"/> is less than 0 -or- 
        /// <paramref name="sourceOffset"/> or <paramref name="otherOffset"/> and <paramref name="count"/> do not specify a valid section in the the associated array.
        /// </exception>
        public static int CompareTo<T>(this T[] source, int sourceOffset, T[] other, int otherOffset, int count) where T : IComparable<T>
        {
            // If both arrays are assumed equal if both are nothing
            if ((object)source == null && (object)other == null)
                return 0;

            // If other array has data and source array is nothing, other array is assumed larger
            if ((object)source == null)
                return 1;

            // If source array has data and other array is nothing, source array is assumed larger
            if ((object)other == null)
                return -1;

            if (sourceOffset < 0)
                throw new ArgumentOutOfRangeException("sourceOffset", "cannot be negative");

            if (otherOffset < 0)
                throw new ArgumentOutOfRangeException("otherOffset", "cannot be negative");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "cannot be negative");

            if (sourceOffset >= source.Length)
                throw new ArgumentOutOfRangeException("sourceOffset", "not a valid index into source array");

            if (otherOffset >= other.Length)
                throw new ArgumentOutOfRangeException("otherOffset", "not a valid index into other array");

            if (sourceOffset + count > source.Length)
                throw new ArgumentOutOfRangeException("count", "exceeds source array size");

            if (otherOffset + count > other.Length)
                throw new ArgumentOutOfRangeException("count", "exceeds other array size");

            // Overflow is possible, but unlikely.  Therefore, this is omitted for performance
            // if ((int.MaxValue - sourceOffset - count) < 0)
            //    throw new ArgumentOutOfRangeException("sourceOffset + count", "exceeds maximum array size");

            // Overflow is possible, but unlikely.  Therefore, this is omitted for performance
            // if ((int.MaxValue - otherOffset - count) < 0)
            //    throw new ArgumentOutOfRangeException("sourceOffset + count", "exceeds maximum array size");

            int comparision = 0;

            // Compares elements of arrays that are of equal size.
            for (int x = 0; x < count; x++)
            {
                comparision = source[sourceOffset + x].CompareTo(other[otherOffset + x]);

                if (comparision != 0)
                    break;
            }

            return comparision;
        }

        // Handling byte arrays as a special case for combining multiple buffers since this can
        // use a block allocated memory stream

        /// <summary>
        /// Combines buffers together as a single image.
        /// </summary>
        /// <param name="source">Source buffer.</param>
        /// <param name="other1">First buffer to combine to <paramref name="source"/> buffer.</param>
        /// <param name="other2">Second buffer to combine to <paramref name="source"/> buffer.</param>
        /// <returns>Combined buffers.</returns>
        /// <exception cref="InvalidOperationException">Cannot create a byte array with more than 2,147,483,591 elements.</exception>
        /// <remarks>
        /// Only use this function if you need a copy of the combined buffers, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined buffers.
        /// </remarks>
        public static byte[] Combine(this byte[] source, byte[] other1, byte[] other2)
        {
            return (new[] { source, other1, other2 }).Combine();
        }

        /// <summary>
        /// Combines buffers together as a single image.
        /// </summary>
        /// <param name="source">Source buffer.</param>
        /// <param name="other1">First buffer to combine to <paramref name="source"/> buffer.</param>
        /// <param name="other2">Second buffer to combine to <paramref name="source"/> buffer.</param>
        /// <param name="other3">Third buffer to combine to <paramref name="source"/> buffer.</param>
        /// <returns>Combined buffers.</returns>
        /// <exception cref="InvalidOperationException">Cannot create a byte array with more than 2,147,483,591 elements.</exception>
        /// <remarks>
        /// Only use this function if you need a copy of the combined buffers, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined buffers.
        /// </remarks>
        public static byte[] Combine(this byte[] source, byte[] other1, byte[] other2, byte[] other3)
        {
            return (new[] { source, other1, other2, other3 }).Combine();
        }

        /// <summary>
        /// Combines buffers together as a single image.
        /// </summary>
        /// <param name="source">Source buffer.</param>
        /// <param name="other1">First buffer to combine to <paramref name="source"/> buffer.</param>
        /// <param name="other2">Second buffer to combine to <paramref name="source"/> buffer.</param>
        /// <param name="other3">Third buffer to combine to <paramref name="source"/> buffer.</param>
        /// <param name="other4">Fourth buffer to combine to <paramref name="source"/> buffer.</param>
        /// <returns>Combined buffers.</returns>
        /// <exception cref="InvalidOperationException">Cannot create a byte array with more than 2,147,483,591 elements.</exception>
        /// <remarks>
        /// Only use this function if you need a copy of the combined buffers, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined buffers.
        /// </remarks>
        public static byte[] Combine(this byte[] source, byte[] other1, byte[] other2, byte[] other3, byte[] other4)
        {
            return (new[] { source, other1, other2, other3, other4 }).Combine();
        }

        /// <summary>
        /// Combines an array of buffers together as a single image.
        /// </summary>
        /// <param name="buffers">Array of byte buffers.</param>
        /// <returns>Combined buffers.</returns>
        /// <exception cref="InvalidOperationException">Cannot create a byte array with more than 2,147,483,591 elements.</exception>
        /// <remarks>
        /// Only use this function if you need a copy of the combined buffers, it will be optimal
        /// to use the Linq function <see cref="Enumerable.Concat{T}"/> if you simply need to
        /// iterate over the combined buffers.
        /// </remarks>
        public static byte[] Combine(this byte[][] buffers)
        {
            if ((object)buffers == null)
                throw new ArgumentNullException("buffers");

            using (BlockAllocatedMemoryStream combinedBuffer = new BlockAllocatedMemoryStream())
            {
                // Combine all currently queued buffers
                for (int x = 0; x < buffers.Length; x++)
                {
                    if ((object)buffers[x] == null)
                        throw new ArgumentNullException("buffers[" + x + "]");

                    combinedBuffer.Write(buffers[x], 0, buffers[x].Length);
                }

                // return combined data buffers
                return combinedBuffer.ToArray();
            }
        }

        /// <summary>
        /// Reads a structure from a byte array.
        /// </summary>
        /// <typeparam name="T">Type of structure to read.</typeparam>
        /// <param name="bytes">Bytes containing structure.</param>
        /// <returns>A structure from <paramref name="bytes"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static T ReadStructure<T>(this byte[] bytes) where T : struct
        {
            T structure;

            fixed (byte* ptrToBytes = bytes)
            {
                structure = (T)Marshal.PtrToStructure(new IntPtr(ptrToBytes), typeof(T));
            }

            return structure;
        }

        /// <summary>
        /// Reads a structure from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <typeparam name="T">Type of structure to read.</typeparam>
        /// <param name="reader"><see cref="BinaryReader"/> positioned at desired structure.</param>
        /// <returns>A structure read from <see cref="BinaryReader"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadStructure<T>(this BinaryReader reader) where T : struct
        {
            return reader.ReadBytes(Marshal.SizeOf(typeof(T))).ReadStructure<T>();
        }
    }
}
