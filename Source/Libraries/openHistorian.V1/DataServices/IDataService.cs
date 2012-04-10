//******************************************************************************************************
//  IDataService.cs - Gbtc
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
//  08/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using openHistorian.V1.Files;
using TVA.ServiceModel;

namespace openHistorian.V1.DataServices
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
        /// Gets or sets the <see cref="IArchive"/> used by the web service for its data.
        /// </summary>
        ArchiveFile Archive
        {
            get;
            set;
        }

        #endregion
    }
}
