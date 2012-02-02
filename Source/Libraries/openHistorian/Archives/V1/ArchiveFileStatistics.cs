//******************************************************************************************************
//  ArchiveFileStatistics.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  06/26/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/24/2009 - Pinal C. Patel
//       Added member initialization to the constructor.
//
//******************************************************************************************************

using TVA.Units;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// A class that contains the statistics of an <see cref="ArchiveFile"/>.
    /// </summary>
    /// <seealso cref="ArchiveFile"/>
    public class ArchiveFileStatistics
    {
        #region [ Members ]

        // Fields

        /// <summary>
        /// Current usage (in %) of the <see cref="ArchiveFile"/>.
        /// </summary>
        public float FileUsage;

        /// <summary>
        /// Current rate of data compression (in %) in the <see cref="ArchiveFile"/>.
        /// </summary>
        public float CompressionRate;

        /// <summary>
        /// <see cref="Time"/> over which the <see cref="AverageWriteSpeed"/> is calculated.
        /// </summary>
        public Time AveragingWindow;

        /// <summary>
        /// Average number of time-series data points written to the <see cref="ArchiveFile"/> in one second.
        /// </summary>
        public int AverageWriteSpeed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileStatistics"/> class.
        /// </summary>
        internal ArchiveFileStatistics()
        {
            AveragingWindow = Time.MinValue;
        }

        #endregion
    }
}
