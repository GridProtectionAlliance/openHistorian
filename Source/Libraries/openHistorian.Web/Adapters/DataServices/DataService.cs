//******************************************************************************************************
//  DataService.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/27/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/02/2009 - Pinal C. Patel
//       Modified configuration of the default WebHttpBinding to enable receiving of large payloads.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/01/2009 - Pinal C. Patel
//       Added a default protected constructor.
//  06/22/2010 - Pinal C. Patel
//       Modified the default constructor to set the base class Singleton property to true.
//  11/07/2010 - Pinal C. Patel
//       Modified to fix breaking changes made to SelfHostingService.
//
//******************************************************************************************************

using openHistorian.Archives;
using TVA.ServiceModel;

namespace openHistorian.DataServices
{
    /// <summary>
    /// A base class for web service that can send and receive historian data over REST (Representational State Transfer) interface.
    /// </summary>
    public class DataService : SelfHostingService, IDataService
    {
        #region [ Members ]

        // Fields
        private IDataArchive m_archive;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of historian data web service.
        /// </summary>
        protected DataService()
            : base()
        {
            Singleton = true;
            PublishMetadata = true;
            PersistSettings = true;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IDataArchive"/> used by the web service for its data.
        /// </summary>
        public IDataArchive Archive
        {
            get
            {
                return m_archive;
            }
            set
            {
                m_archive = value;
            }
        }

        #endregion
    }
}
