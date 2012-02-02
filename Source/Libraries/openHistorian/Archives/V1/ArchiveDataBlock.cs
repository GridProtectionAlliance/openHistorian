//******************************************************************************************************
//  ArchiveDataBlock.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  02/24/2007 - Pinal C. Patel
//       Generated original version of source code.
//  01/23/2008 - Pinal C. Patel
//       Removed IsForHistoricData and added IsActive to keep track of activity.
//  03/31/2008 - Pinal C. Patel
//       Modified code to use the same FileStream object used by FAT instead to creating a new one.
//       Removed IDisposable interface implementation and Size property.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  08/05/2009 - Josh L. Patterson
//       Edited Comments.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/23/2009 - Pinal C. Patel
//       Edited code comments.
//       Removed the dependency on ArchiveDataPoint.
//  10/14/2009 - Pinal C. Patel
//       Modified Write() to seek only when necessary.
//  11/18/2010 - J. Ritchie Carroll
//       Added a exception handler for reading (exposed via DataReadException event) to make sure
//       bad data or corruption in an archive file does not stop the read process.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using TVA;
using TVA.Parsing;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// Represents a block of <see cref="ArchiveData"/>s in an <see cref="ArchiveFile"/>.
    /// </summary>
    /// <seealso cref="ArchiveFile"/>
    /// <seealso cref="ArchiveData"/>
    public class ArchiveDataBlock
    {
        #region [ Members ]

        // Constants

        /// <summary>
        /// Time in seconds after which the block is considered inactive if no reads or writes were performed.
        /// </summary>
        private const int InactivityPeriod = 300;

        // Fields
        private int m_index;
        private int m_key;
        private ArchiveFile m_parent;
        private byte[] m_readBuffer;
        private long m_writeCursor;
        private DateTime m_lastActivityTime;

        /// <summary>
        /// Occurs when an <see cref="Exception"/> is encountered while reading <see cref="IData"/> from the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        [Category("Data"),
        Description("Occurs when an Exception is encountered while reading IDataPoint from the ArchiveDataBlock.")]
        public event EventHandler<EventArgs<Exception>> DataReadException;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveDataBlock"/> class. 
        /// </summary>
        /// <param name="parent">An <see cref="ArchiveFile"/> object.</param>
        /// <param name="index">0-based index of the <see cref="ArchiveDataBlock"/>.</param>
        /// <param name="key">Historian identifier whose <see cref="ArchiveData"/> is stored in the <see cref="ArchiveDataBlock"/>.</param>
        /// <param name="reset">true if the <see cref="ArchiveDataBlock"/> is to be <see cref="Reset()"/>; otherwise false.</param>
        /// <param name="preRead">true to pre-read data to locate write cursor.</param>
        internal ArchiveDataBlock(ArchiveFile parent, int index, int key, bool reset, bool preRead = true)
        {
            m_parent = parent;
            m_index = index;
            m_key = key;
            m_readBuffer = new byte[ArchiveData.ByteCount];
            m_writeCursor = Location;
            m_lastActivityTime = DateTime.Now;

            if (reset)
            {
                // Clear existing data.
                Reset();
            }
            else if (preRead)
            {
                // Scan through existing data to locate write cursor
                foreach (ArchiveData dataPoint in Read())
                {
                }
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the 0-based index of the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets the start location (byte position) of the <see cref="ArchiveDataBlock"/> in the <see cref="ArchiveFile"/>.
        /// </summary>
        public long Location
        {
            get
            {
                return (m_index * (m_parent.DataBlockSize * 1024));
            }
        }

        /// <summary>
        /// Gets the maximum number of <see cref="ArchiveData"/>s that can be stored in the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        public int Capacity
        {
            get
            {
                return ((m_parent.DataBlockSize * 1024) / ArchiveData.ByteCount);
            }
        }

        /// <summary>
        /// Gets the number of <see cref="ArchiveData"/>s that have been written to the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        public int SlotsUsed
        {
            get
            {
                return (int)((m_writeCursor - Location) / ArchiveData.ByteCount);
            }
        }

        /// <summary>
        /// Gets the number of <see cref="ArchiveData"/>s that can to written to the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        public int SlotsAvailable
        {
            get
            {
                return (Capacity - SlotsUsed);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the <see cref="ArchiveDataBlock"/> is being actively used.
        /// </summary>
        public bool IsActive
        {
            get
            {
                double inactivity = DateTime.Now.Subtract(m_lastActivityTime).TotalSeconds;
                if (inactivity <= InactivityPeriod)
                {
                    return true;
                }
                else
                {
                    Trace.WriteLine(string.Format("Inactive for {0} seconds (Last activity = {1}; Time now = {2})", inactivity, m_lastActivityTime, DateTime.Now));
                    return false;
                }
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Reads existing <see cref="ArchiveData"/>s from the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        /// <returns>Returns <see cref="ArchiveData"/>s from the <see cref="ArchiveDataBlock"/>.</returns>
        public IEnumerable<IData> Read()
        {
            ArchiveData dataPoint;

            lock (m_parent.FileData)
            {
                // We'll start reading from where the data block begins.
                m_parent.FileData.Seek(Location, SeekOrigin.Begin);

                for (int i = 0; i < Capacity; i++)
                {
                    // Read the data in the block.
                    m_lastActivityTime = DateTime.Now;
                    m_parent.FileData.Read(m_readBuffer, 0, m_readBuffer.Length);

                    // Attempt to parse archive data point
                    try
                    {
                        dataPoint = new ArchiveData(m_key, m_readBuffer, 0, m_readBuffer.Length);
                    }
                    catch (Exception ex)
                    {
                        dataPoint = null;
                        OnDataReadException(ex);
                    }

                    if (dataPoint != null && !Data.IsEmpty(dataPoint))
                    {
                        // There is data - use it.
                        m_writeCursor = m_parent.FileData.Position;
                        yield return dataPoint;
                    }
                    else
                    {
                        // Data is empty - stop reading.
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the <paramref name="dataPoint"/> to the <see cref="ArchiveDataBlock"/>.
        /// </summary>
        /// <param name="dataPoint"><see cref="IData"/> to write.</param>
        public void Write(IData dataPoint)
        {
            if (SlotsAvailable > 0)
            {
                // We have enough space to write the provided point data to the data block.
                m_lastActivityTime = DateTime.Now;
                lock (m_parent.FileData)
                {
                    // Write the data.
                    if (m_writeCursor != m_parent.FileData.Position)
                        m_parent.FileData.Seek(m_writeCursor, SeekOrigin.Begin);
                    dataPoint.CopyBinaryImageToStream(m_parent.FileData);
                    // Update the write cursor.
                    m_writeCursor = m_parent.FileData.Position;
                    // Flush the data if configured.
                    if (!m_parent.CacheWrites)
                        m_parent.FileData.Flush();
                }
            }
            else
            {
                throw (new InvalidOperationException("No slots available for writing new data."));
            }
        }

        /// <summary>
        /// Resets the <see cref="ArchiveDataBlock"/> by overwriting existing <see cref="ArchiveData"/>s with empty <see cref="ArchiveData"/>s.
        /// </summary>
        public void Reset()
        {
            m_writeCursor = Location;
            for (int i = 1; i <= Capacity; i++)
            {
                Write(new ArchiveData(m_key));
            }
            m_writeCursor = Location;
        }

        /// <summary>
        /// Raises the <see cref="DataReadException"/> event.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> to send to <see cref="DataReadException"/> event.</param>
        protected virtual void OnDataReadException(Exception ex)
        {
            if (DataReadException != null)
                DataReadException(this, new EventArgs<Exception>(ex));
        }

        #endregion
    }
}