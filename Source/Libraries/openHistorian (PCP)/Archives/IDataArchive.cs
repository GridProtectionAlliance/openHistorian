//******************************************************************************************************
//  IArchive.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  05/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/02/2009 - Pinal C. Patel
//       Modified ReadData() to take start and end times as strings for flexibility.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using TVA;
using TVA.Adapters;

namespace openHistorian.Archives
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates the current state of the <see cref="IDataArchive"/>.
    /// </summary>
    public enum DataArchiveState
    {
        /// <summary>
        /// <see cref="IDataArchive"/> is closed.
        /// </summary>
        Closed = 0,
        /// <summary>
        /// <see cref="IDataArchive"/> is being closed.
        /// </summary>
        Closing = 1,
        /// <summary>
        /// <see cref="IDataArchive"/> is open.
        /// </summary>
        Open = 2,
        /// <summary>
        /// <see cref="IDataArchive"/> is being opened.
        /// </summary>
        Opening = 3,
        /// <summary>
        /// <see cref="IDataArchive"/> is open but performing maintenance.
        /// </summary>
        Maintenance = 4
    }

    #endregion

    /// <summary>
    /// Defines a repository where <see cref="IData"/> is stored.
    /// </summary>
    /// <seealso cref="IData"/>
    public interface IDataArchive : IAdapter
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when new <see cref="IData"/> is received by the <see cref="IDataArchive"/>.
        /// </summary>
        event EventHandler<EventArgs<IEnumerable<IData>>> DataReceived;

        #endregion        

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="DataArchiveState"/> of the <see cref="IDataArchive"/>.
        /// </summary>
        DataArchiveState State 
        { 
            get; 
        }

        /// <summary>
        /// Gets the <see cref="IDataCache"/> for the <see cref="IDataArchive"/>.
        /// </summary>
        IDataCache Cache
        {
            get;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="openDependencies">True to open any dependencies used by the <see cref="IDataArchive"/>, otherwise False.</param>
        void Open(bool openDependencies);

        /// <summary>
        /// Closes the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="closeDependencies">True to close any dependencies used by the <see cref="IDataArchive"/>, otherwise False.</param>
        void Close(bool closeDependencies);

        /// <summary>
        /// Writes time-series data to the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="data"><see cref="IData"/> to be written.</param>
        void WriteData(IData data);

        /// <summary>
        /// Writes meta information for the specified <paramref name="key"/> to the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Historian identifier.</param>
        /// <param name="metaData">Binary image of the meta information.</param>
        void WriteMetaData(int key, byte[] metaData);

        /// <summary>
        /// Writes state information for the specified <paramref name="key"/> to the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Historian identifier.</param>
        /// <param name="stateData">Binary image of the state information.</param>
        void WriteStateData(int key, byte[] stateData);

        /// <summary>
        /// Reads <see cref="IData"/> from the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Identifier for which <see cref="IData"/>s are to be read.</param>
        /// <param name="startTime"><see cref="System.String"/> representation of the start time (in GMT) of the timespan for which <see cref="IData"/>s are to be read.</param>
        /// <param name="endTime"><see cref="System.String"/> representation of the end time (in GMT) of the timespan for which <see cref="IData"/>s are to be read.</param>
        /// <returns><see cref="IEnumerable{T}"/> object containing zero or more <see cref="IData"/>s.</returns>
        IEnumerable<IData> ReadData(int key, string startTime, string endTime);

        /// <summary>
        /// Read meta information for the specified <paramref name="key"/> from the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Historian identifier.</param>
        /// <returns>A <see cref="byte"/> array containing meta information.</returns>
        byte[] ReadMetaData(int key);

        /// <summary>
        /// Reads state information for the specified <paramref name="key"/> from the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Historian identifier.</param>
        /// <returns>A <see cref="byte"/> array containing state information.</returns>
        byte[] ReadStateData(int key);

        /// <summary>
        /// Reads meta information summary for the specified <paramref name="key"/> from the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Historian identifier.</param>
        /// <returns>A <see cref="byte"/> array containing meta information summary.</returns>
        byte[] ReadMetaDataSummary(int key);

        /// <summary>
        /// Read state information summary for the specified <paramref name="key"/> from the <see cref="IDataArchive"/>.
        /// </summary>
        /// <param name="key">Historian identifier.</param>
        /// <returns>A <see cref="byte"/> array containing state information summary.</returns>
        byte[] ReadStateDataSummary(int key);

        #endregion
    }
}
