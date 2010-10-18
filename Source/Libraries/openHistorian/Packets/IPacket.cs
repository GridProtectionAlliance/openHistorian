//******************************************************************************************************
//  IPacket.cs - Gbtc
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
//  07/27/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using TVA.Parsing;

namespace openHistorian.Packets
{
    /// <summary>
    /// Defines a binary packet received by a historian.
    /// </summary>
    public interface IPacket : ISupportBinaryImage, ISupportFrameImage<short>
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the current <see cref="IArchive"/>.
        /// </summary>
        IArchive Archive { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> that processes the packet.
        /// </summary>
        /// <remarks>
        /// <see cref="Func{TResult}"/> returns an <see cref="IEnumerable{T}"/> object containing the binary data to be sent back to the packet sender.
        /// </remarks>
        Func<IEnumerable<byte[]>> ProcessHandler { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Delegate"/> that pre-processes the packet.
        /// </summary>
        /// <remarks>
        /// <see cref="Func{TResult}"/> returns an <see cref="IEnumerable{T}"/> object containing the binary data to be sent back to the packet sender.
        /// </remarks>
        Func<IEnumerable<byte[]>> PreProcessHandler { get; set; }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Extracts time-series data from the packet.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object of <see cref="IDataPoint"/>s if the packet contains time-series data; otherwise null.</returns>
        IEnumerable<IDataPoint> ExtractTimeSeriesData();

        #endregion
    }
}