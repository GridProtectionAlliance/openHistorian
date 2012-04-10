//******************************************************************************************************
//  IArchive.cs - Gbtc
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
//  05/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/02/2009 - Pinal C. Patel
//       Modified ReadData() to take start and end times as strings for flexibility.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System.Collections.Generic;

namespace openHistorian
{
    /// <summary>
    /// Defines a repository where time-series data is warehoused by a historian.
    /// </summary>
    /// <seealso cref="IDataPoint"/>
    public interface IArchive
    {
        #region [ Methods ]

        /// <summary>
        /// Opens the repository.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the repository.
        /// </summary>
        void Close();

        /// <summary>
        /// Writes time-series data to the repository.
        /// </summary>
        /// <param name="dataPoint"><see cref="IDataPoint"/> to be written.</param>
        void WriteData(IDataPoint dataPoint);

        /// <summary>
        /// Reads time-series data from the repository.
        /// </summary>
        /// <param name="historianID">Historian identifier for which <see cref="IDataPoint"/>s are to be read.</param>
        /// <param name="startTime"><see cref="System.String"/> representation of the start time (in UTC) of the timespan for which <see cref="IDataPoint"/>s are to be read.</param>
        /// <param name="endTime"><see cref="System.String"/> representation of the end time (in UTC) of the timespan for which <see cref="IDataPoint"/>s are to be read.</param>
        /// <returns><see cref="IEnumerable{T}"/> object containing zero or more <see cref="IDataPoint"/>s.</returns>
        IEnumerable<IDataPoint> ReadData(int historianID, string startTime, string endTime);

        /// <summary>
        /// Reads time-series data from the repository.
        /// </summary>
        /// <param name="historianIDs">Historian identifiers for which <see cref="IDataPoint"/>s are to be read.</param>
        /// <param name="startTime"><see cref="System.String"/> representation of the start time (in UTC) of the timespan for which <see cref="IDataPoint"/>s are to be read.</param>
        /// <param name="endTime"><see cref="System.String"/> representation of the end time (in UTC) of the timespan for which <see cref="IDataPoint"/>s are to be read.</param>
        /// <returns><see cref="IEnumerable{T}"/> object containing zero or more <see cref="IDataPoint"/>s.</returns>
        IEnumerable<IDataPoint> ReadData(IEnumerable<int> historianIDs, string startTime, string endTime);

        ///// <summary>
        ///// Read meta information for the specified <paramref name="historianID"/>.
        ///// </summary>
        ///// <param name="historianID">Historian identifier.</param>
        ///// <returns>A <see cref="byte"/> array containing meta information.</returns>
        //byte[] ReadMetaData(int historianID);

        ///// <summary>
        ///// Writes meta information for the specified <paramref name="historianID"/> to the repository.
        ///// </summary>
        ///// <param name="historianID">Historian identifier.</param>
        ///// <param name="metaData">Binary image of the meta information.</param>
        //void WriteMetaData(int historianID, byte[] metaData);

        ///// <summary>
        ///// Reads state information for the specified <paramref name="historianID"/>.
        ///// </summary>
        ///// <param name="historianID">Historian identifier.</param>
        ///// <returns>A <see cref="byte"/> array containing state information.</returns>
        //byte[] ReadStateData(int historianID);

        ///// <summary>
        ///// Writes state information for the specified <paramref name="historianID"/> to the repository.
        ///// </summary>
        ///// <param name="historianID">Historian identifier.</param>
        ///// <param name="stateData">Binary image of the state information.</param>
        //void WriteStateData(int historianID, byte[] stateData);

        ///// <summary>
        ///// Reads meta information summary for the specified <paramref name="historianID"/>.
        ///// </summary>
        ///// <param name="historianID">Historian identifier.</param>
        ///// <returns>A <see cref="byte"/> array containing meta information summary.</returns>
        //byte[] ReadMetaDataSummary(int historianID);

        ///// <summary>
        ///// Read state information summary for the specified <paramref name="historianID"/>.
        ///// </summary>
        ///// <param name="historianID">Historian identifier.</param>
        ///// <returns>A <see cref="byte"/> array containing state information summary.</returns>
        //byte[] ReadStateDataSummary(int historianID);

        #endregion
    }
}
