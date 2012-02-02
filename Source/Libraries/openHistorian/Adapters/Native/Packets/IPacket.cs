//******************************************************************************************************
//  IPacket.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  07/27/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Archives;
using TVA.Parsing;

namespace openHistorian.Adapters.Native.Packets
{
    /// <summary>
    /// Defines a binary packet received by a historian.
    /// </summary>
    public interface IPacket : ISupportBinaryImage, ISupportFrameImage<short>
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the current <see cref="IDataArchive"/>.
        /// </summary>
        IDataArchive Archive { get; set; }

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
        /// <returns>An <see cref="IEnumerable{T}"/> object of <see cref="IData"/>s if the packet contains time-series data; otherwise null.</returns>
        IEnumerable<IData> ExtractTimeSeriesData();

        #endregion
    }
}