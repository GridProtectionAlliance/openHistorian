//******************************************************************************************************
//  IDataService.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using openHistorian.Archives;
using TVA.ServiceModel;

namespace openHistorian.DataServices
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates the direction in which data will be flowing from a web service.
    /// </summary>
    public enum DataFlowDirection
    {
        /// <summary>
        /// Data will be flowing in to the web service.
        /// </summary>
        Incoming,
        /// <summary>
        /// Data will be flowing out from the web service.
        /// </summary>
        Outgoing,
        /// <summary>
        /// Data will be flowing both in and out from the web service.
        /// </summary>
        BothWays
    }

    #endregion

    /// <summary>
    /// Defines a web service that can send and receive historian data over REST (Representational State Transfer) interface.
    /// </summary>
    public interface IDataService : ISelfHostingService
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IDataArchive"/> used by the web service for its data.
        /// </summary>
        IDataArchive Archive { get; set; }

        #endregion
    }
}
