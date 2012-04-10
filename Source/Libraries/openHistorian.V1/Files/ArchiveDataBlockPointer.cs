//******************************************************************************************************
//  ArchiveDataBlockPointer.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
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
//  -----------------------------------------------------------------------------------------------------
//  02/18/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/23/2009 - Pinal C. Patel
//       Edited code comments.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using TVA;
using TVA.Parsing;

namespace openHistorian.V1.Files
{
    /// <summary>
    /// Represents a pointer to an <see cref="ArchiveDataBlock"/>.
    /// </summary>
    /// <seealso cref="ArchiveFile"/>
    /// <seealso cref="ArchiveDataBlock"/>
    public class ArchiveDataBlockPointer : IComparable, ISupportBinaryImage
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="ArchiveDataBlockPointer"/>.
        /// </summary>
        public const int FixedLength = 12;

        // Fields
        private int m_index;
        private int m_historianID;
        private TimeTag m_startTime;
        private ArchiveFile m_parent;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataBlockPointer"/> class.
        /// </summary>
        /// <param name="parent">An <see cref="ArchiveFile"/> object.</param>
        /// <param name="index">0-based index of the <see cref="ArchiveDataBlockPointer"/>.</param>
        internal ArchiveDataBlockPointer(ArchiveFile parent, int index)
        {
            m_parent = parent;
            m_index = index;
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataBlockPointer"/> class.
        /// </summary>
        /// <param name="parent">An <see cref="ArchiveFile"/> object.</param>
        /// <param name="index">0-based index of the <see cref="ArchiveDataBlockPointer"/>.</param>
        /// <param name="buffer">Binary image to be used for initializing <see cref="ArchiveDataBlockPointer"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        internal ArchiveDataBlockPointer(ArchiveFile parent, int index, byte[] buffer, int startIndex, int length)
            : this(parent, index)
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the historian identifier of <see cref="ArchiveDataBlockPointer"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not positive or -1.</exception>
        public int HistorianID
        {
            get
            {
                return m_historianID;
            }
            set
            {
                if (value < 1 && value != -1)
                    throw new ArgumentException("Value must be positive or -1");

                m_historianID = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of first <see cref="ArchiveDataPoint"/> in the <see cref="DataBlock"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not between 01/01/1995 and 01/19/2063.</exception>
        public TimeTag StartTime
        {
            get
            {
                return m_startTime;
            }
            set
            {
                if (value.CompareTo(TimeTag.MinValue) < 0 || value.CompareTo(TimeTag.MaxValue) > 0)
                    throw new ArgumentException("Value must between 01/01/1995 and 01/19/2063");

                m_startTime = value;
            }
        }

        /// <summary>
        /// Gets the 0-based index of the <see cref="ArchiveDataBlockPointer"/>.
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets the <see cref="ArchiveDataBlock"/> corresponding to the <see cref="ArchiveDataBlockPointer"/>.
        /// </summary>
        public ArchiveDataBlock DataBlock
        {
            get
            {
                return GetDataBlock(true);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the <see cref="DataBlock"/> has been allocated to contain <see cref="ArchiveDataPoint"/>s.
        /// </summary>
        public bool IsAllocated
        {
            get
            {
                return m_historianID != -1 && m_startTime.CompareTo(TimeTag.MinValue) != 0;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="ArchiveDataBlockPointer"/>.
        /// </summary>
        public int BinaryLength
        {
            get
            {
                return FixedLength;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the <see cref="ArchiveDataBlock"/> associated with this <see cref="ArchiveDataBlockPointer"/>.
        /// </summary>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        /// <returns>The <see cref="ArchiveDataBlock"/> associated with this <see cref="ArchiveDataBlockPointer"/>.</returns>
        public ArchiveDataBlock GetDataBlock(bool preRead)
        {
            return new ArchiveDataBlock(m_parent, m_index, m_historianID, false, preRead);
        }

        /// <summary>
        /// Deallocates the <see cref="DataBlock"/> to store new <see cref="ArchiveDataPoint"/>s.
        /// </summary>
        public void Reset()
        {
            m_historianID = -1;
            m_startTime = TimeTag.MinValue;
        }

        /// <summary>
        /// Initializes <see cref="ArchiveDataBlockPointer"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="ArchiveDataBlockPointer"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="ArchiveDataBlockPointer"/>.</returns>
        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= FixedLength)
            {
                // Binary image has sufficient data.
                HistorianID = EndianOrder.LittleEndian.ToInt32(buffer, startIndex);
                StartTime = new TimeTag(EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 4));

                return FixedLength;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="ArchiveDataBlockPointer"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            int length = BinaryLength;

            buffer.ValidateParameters(startIndex, length);

            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_historianID), 0, buffer, startIndex, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_startTime.Value), 0, buffer, startIndex + 4, 8);

            return length;
        }

        /// <summary>
        /// Compares the current <see cref="ArchiveDataBlockPointer"/> object to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="ArchiveDataBlockPointer"/> object is to be compared.</param>
        /// <returns>
        /// Negative value if the current <see cref="ArchiveDataBlockPointer"/> object is less than <paramref name="obj"/>, 
        /// Zero if the current <see cref="ArchiveDataBlockPointer"/> object is equal to <paramref name="obj"/>, 
        /// Positive value if the current <see cref="ArchiveDataBlockPointer"/> object is greater than <paramref name="obj"/>.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            ArchiveDataBlockPointer other = obj as ArchiveDataBlockPointer;
            if (other == null)
            {
                return 1;
            }
            else
            {
                int result = m_historianID.CompareTo(other.HistorianID);
                if (result != 0)
                    return result;
                else
                    return m_startTime.CompareTo(other.StartTime);
            }
        }

        /// <summary>
        /// Determines whether the current <see cref="ArchiveDataBlockPointer"/> object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="ArchiveDataBlockPointer"/> object is to be compared for equality.</param>
        /// <returns>true if the current <see cref="ArchiveDataBlockPointer"/> object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Returns the text representation of <see cref="ArchiveDataBlockPointer"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        public override string ToString()
        {
            return string.Format("ID: {0}; Start time: {1}", m_historianID, m_startTime);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="ArchiveDataBlock"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return m_historianID.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for the <see cref="ArchiveDataBlockPointer"/>.
    /// </summary>
    public static class ArchiveDataBlockPointerExtensions
    {
        /// <summary>
        /// Tests if the <paramref name="dataBlockPointer"/> matches the specified search criteria.
        /// </summary>
        /// <param name="dataBlockPointer"><see cref="ArchiveDataBlockPointer"/> to test.</param>
        /// <param name="historianID">Desired historian ID.</param>
        /// <param name="startTime">Desired start time.</param>
        /// <param name="endTime">Desired end time.</param>
        /// <returns><c>true</c> if the specified <paramref name="dataBlockPointer"/> is for <paramref name="historianID"/> and falls within the <paramref name="startTime"/> and <paramref name="endTime"/>; otherwise <c>false</c>.</returns>
        public static bool Matches(this ArchiveDataBlockPointer dataBlockPointer, int historianID, TimeTag startTime, TimeTag endTime)
        {
            if (dataBlockPointer != null)
                // Note: The StartTime value of the pointer is ignored if m_searchStartTime = TimeTag.MinValue and
                //       m_searchEndTime = TimeTag.MaxValue. In this case only the PointID value is compared. This
                //       comes in handy when the first or last pointer is to be found from the list of pointers for
                //       a point ID in addition to all the pointers for a point ID.
                return dataBlockPointer.HistorianID == historianID &&
                        (startTime.CompareTo(TimeTag.MinValue) == 0 || dataBlockPointer.StartTime.CompareTo(startTime) >= 0) &&
                        (endTime.CompareTo(TimeTag.MaxValue) == 0 || dataBlockPointer.StartTime.CompareTo(endTime) <= 0);
            else
                return false;
        }
    }
}