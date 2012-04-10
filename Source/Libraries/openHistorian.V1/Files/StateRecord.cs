//******************************************************************************************************
//  StateRecord.cs - Gbtc
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
//  03/08/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  03/31/2008 - Pinal C. Patel
//       Removed Obsolete tag from ActiveDataBlockIndex and ActiveDataBlockSlot as it's being used.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/01/2009 - Pinal C. Patel
//       Modified ActiveDataBlockIndex to default to -1 for values that are less than 0.
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
    /// Represents a record in the <see cref="StateFile"/> that contains the state information associated to a <see cref="HistorianID"/>.
    /// </summary>
    /// <seealso cref="StateFile"/>    
    /// <seealso cref="StateRecordSummary"/>
    /// <seealso cref="StateRecordDataPoint"/>
    public class StateRecord : ISupportBinaryImage, IComparable
    {
        // **************************************************************************************************
        // *                                        Binary Structure                                        *
        // **************************************************************************************************
        // * # Of Bytes Byte Index Data Type  Property Name                                                 *
        // * ---------- ---------- ---------- --------------------------------------------------------------*
        // * 16         0-15       Byte(16)   ArchivedData                                                  *
        // * 16         16-31      Byte(16)   PreviousData                                                  *
        // * 16         32-47      Byte(16)   CurrentData                                                   *
        // * 4          48-51      Int32      ActiveDataBlockIndex                                          *
        // * 4          52-55      Int32      ActiveDataBlockSlot                                           *
        // * 8          56-63      Double     Slope1                                                        *
        // * 8          64-71      Double     Slope2                                                        *
        // **************************************************************************************************

        #region [ Members ]

        // Constants

        /// <summary>
        /// Specifies the number of bytes in the binary image of <see cref="StateRecord"/>.
        /// </summary>
        public const int FixedLength = 72;

        // Fields
        private int m_historianID;
        private StateRecordDataPoint m_archivedData;
        private StateRecordDataPoint m_previousData;
        private StateRecordDataPoint m_currentData;
        private int m_activeDataBlockIndex;
        private int m_activeDataBlockSlot;
        private double m_slope1;
        private double m_slope2;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecord"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="StateRecord"/>.</param>
        public StateRecord(int historianID)
        {
            m_historianID = historianID;
            m_archivedData = new StateRecordDataPoint(m_historianID);
            m_previousData = new StateRecordDataPoint(m_historianID);
            m_currentData = new StateRecordDataPoint(m_historianID);
            m_activeDataBlockIndex = -1;
            m_activeDataBlockSlot = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRecord"/> class.
        /// </summary>
        /// <param name="historianID">Historian identifier of <see cref="StateRecord"/>.</param>
        /// <param name="buffer">Binary image to be used for initializing <see cref="StateRecord"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public StateRecord(int historianID, byte[] buffer, int startIndex, int length)
            : this(historianID)
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the most recently archived <see cref="StateRecordDataPoint"/> for the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public StateRecordDataPoint ArchivedData
        {
            get
            {
                return m_archivedData;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_archivedData = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous <see cref="StateRecordDataPoint"/> received for the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public StateRecordDataPoint PreviousData
        {
            get
            {
                return m_previousData;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_previousData = value;
            }
        }

        /// <summary>
        /// Gets or sets the most current <see cref="StateRecordDataPoint"/> received for the <see cref="HistorianID"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public StateRecordDataPoint CurrentData
        {
            get
            {
                return m_currentData;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_currentData = value;
            }
        }

        /// <summary>
        /// Gets or sets the 0-based index of the active <see cref="ArchiveDataBlock"/> for the <see cref="HistorianID"/>.
        /// </summary>
        /// <remarks>Index is persisted to disk as 1-based index for backwards compatibility.</remarks>
        public int ActiveDataBlockIndex
        {
            get
            {
                if (m_activeDataBlockIndex < 0)
                    return m_activeDataBlockIndex;
                else
                    return m_activeDataBlockIndex - 1;
            }
            set
            {
                if (value < 0)
                    m_activeDataBlockIndex = -1;
                else
                    m_activeDataBlockIndex = value + 1;
            }
        }

        /// <summary>
        /// Gets or sets the next slot position in the active <see cref="ArchiveDataBlock"/> for the <see cref="HistorianID"/> where data can be written.
        /// </summary>
        public int ActiveDataBlockSlot
        {
            get
            {
                return m_activeDataBlockSlot;
            }
            set
            {
                if (value <= 0)
                    m_activeDataBlockSlot = 1;
                else
                    m_activeDataBlockSlot = value;
            }
        }

        /// <summary>
        /// Gets or sets slope #1 used in the piece-wise linear compression of data.
        /// </summary>
        public double Slope1
        {
            get
            {
                return m_slope1;
            }
            set
            {
                m_slope1 = value;
            }
        }

        /// <summary>
        /// Gets or sets slope #2 used in the piece-wise linear compression of data.
        /// </summary>
        public double Slope2
        {
            get
            {
                return m_slope2;
            }
            set
            {
                m_slope2 = value;
            }
        }

        /// <summary>
        /// Gets the historian identifier of <see cref="StateRecord"/>.
        /// </summary>
        public int HistorianID
        {
            get
            {
                return m_historianID;
            }
        }

        /// <summary>
        /// Gets the <see cref="StateRecordSummary"/> object for <see cref="StateRecord"/>.
        /// </summary>
        public StateRecordSummary Summary
        {
            get
            {
                return new StateRecordSummary(this);
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="StateRecord"/>.
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
        /// Initializes <see cref="StateRecord"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="StateRecord"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing <see cref="StateRecord"/>.</returns>
        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            if (length >= FixedLength)
            {
                // Binary image has sufficient data.
                m_archivedData.ParseBinaryImage(buffer, startIndex, length);
                m_previousData.ParseBinaryImage(buffer, startIndex + 16, length - 16);
                m_currentData.ParseBinaryImage(buffer, startIndex + 32, length - 32);
                ActiveDataBlockIndex = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 48) - 1;
                ActiveDataBlockSlot = EndianOrder.LittleEndian.ToInt32(buffer, startIndex + 52);
                Slope1 = EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 56);
                Slope2 = EndianOrder.LittleEndian.ToDouble(buffer, startIndex + 64);

                return FixedLength;
            }
            else
            {
                // Binary image does not have sufficient data.
                return 0;
            }
        }

        /// <summary>
        /// Generates binary image of the <see cref="StateRecord"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
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

            m_archivedData.GenerateBinaryImage(buffer, startIndex);
            m_previousData.GenerateBinaryImage(buffer, startIndex + 16);
            m_currentData.GenerateBinaryImage(buffer, startIndex + 32);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_activeDataBlockIndex), 0, buffer, startIndex + 48, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_activeDataBlockSlot), 0, buffer, startIndex + 52, 4);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_slope1), 0, buffer, startIndex + 56, 8);
            Buffer.BlockCopy(EndianOrder.LittleEndian.GetBytes(m_slope2), 0, buffer, startIndex + 64, 8);

            return length;
        }

        /// <summary>
        /// Compares the current <see cref="StateRecord"/> object to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="StateRecord"/> object is to be compared.</param>
        /// <returns>
        /// Negative value if the current <see cref="StateRecord"/> object is less than <paramref name="obj"/>, 
        /// Zero if the current <see cref="StateRecord"/> object is equal to <paramref name="obj"/>, 
        /// Positive value if the current <see cref="StateRecord"/> object is greater than <paramref name="obj"/>.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            StateRecord other = obj as StateRecord;
            if (other == null)
                return 1;
            else
                return m_historianID.CompareTo(other.HistorianID);
        }

        /// <summary>
        /// Determines whether the current <see cref="StateRecord"/> object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="StateRecord"/> object is to be compared for equality.</param>
        /// <returns>true if the current <see cref="StateRecord"/> object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Returns the text representation of <see cref="StateRecord"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        public override string ToString()
        {
            return string.Format("ID={0}; ActiveDataBlock={1}", m_historianID, m_activeDataBlockIndex);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="StateRecord"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return m_historianID.GetHashCode();
        }

        #endregion
    }
}
