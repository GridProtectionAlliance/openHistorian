//******************************************************************************************************
//  IHistorianDataReader.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Data;

namespace openHistorian
{
    /// <summary>
    /// Represents a data reader that can be used to get data from the database.
    /// </summary>
    /// <remarks>
    /// Only one read operation is supported at a time per <see cref="IHistorianDataReader"/>.
    /// This is because a lock on the database occurs when reading and any files
    /// being accessed cannot be deleted until the read completes.
    /// </remarks>
    public interface IHistorianDataReader : IDisposable
    {
        /// <summary>
        /// Reads all of the points where the first key is equal to <see cref="key1"/>
        /// </summary>
        /// <param name="key1"></param>
        /// <returns></returns>
        IPointStream Read(ulong key1);
        /// <summary>
        /// Reads all of the points where the first key is between <see cref="startKey1"/> and <see cref="endKey1"/> inclusive.
        /// </summary>
        /// <param name="startKey1">the lower bounds inclusive</param>
        /// <param name="endKey1">the upper bounds inclusive</param>
        /// <returns></returns>
        IPointStream Read(ulong startKey1, ulong endKey1);
        /// <summary>
        /// Reads all of the points where key2 are included in <see cref="listOfKey2"/>  and where the first key 
        /// is between <see cref="startKey1"/> and <see cref="endKey1"/> inclusive.
        /// </summary>
        /// <param name="startKey1"></param>
        /// <param name="endKey1"></param>
        /// <param name="listOfKey2"></param>
        /// <returns></returns>
        IPointStream Read(ulong startKey1, ulong endKey1, IEnumerable<ulong> listOfKey2);

        /// <summary>
        /// Reads all of the data where key1 exists in <see cref="KeyParserPrimary"/> and key2 is contained within <see cref="listOfKey2"/>.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="listOfKey2"></param>
        /// <returns></returns>
        IPointStream Read(KeyParserPrimary key1, IEnumerable<ulong> listOfKey2);

        /// <summary>
        /// Reads all of the data where key1 exists in <see cref="KeyParserPrimary"/> and key2 is contained within <see cref="key2"/>.
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="readerOptions"></param>
        /// <returns></returns>
        IPointStream Read(KeyParserPrimary key1, KeyParserSecondary key2, DataReaderOptions readerOptions);
        
        /// <summary>
        /// Closes the current reader.
        /// </summary>
        void Close();
    }
}
