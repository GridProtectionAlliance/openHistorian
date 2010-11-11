//******************************************************************************************************
//  DataService.cs - Gbtc
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
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using TVA.Web.Services;

namespace TimeSeriesArchiver.DataServices
{
    /// <summary>
    /// A base class for web service that can send and receive historian data over REST (Representational State Transfer) interface.
    /// </summary>
    public class DataService : SelfHostingService, IDataService
    {
        #region [ Members ]

        // Fields
        private IArchive m_archive;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of historian data web service.
        /// </summary>
        protected DataService()
            : base()
        {
            Singleton = true;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IArchive"/> used by the web service for its data.
        /// </summary>
        public IArchive Archive
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
