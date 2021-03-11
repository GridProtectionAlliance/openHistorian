//******************************************************************************************************
//  ServerCommand.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  12/08/2012 - Steven E. Chisholm
//       Major change to the Interface by breaking out database/server features. 
//
//******************************************************************************************************

namespace GSF.Snap.Services.Net
{
    #region [ Enumerations ]

    /// <summary>
    /// Server commands
    /// </summary>
    public enum ServerCommand : byte
    {
        /// <summary>
        /// 
        /// </summary>
        ConnectToDatabase = 0,
        /// <summary>
        /// 
        /// </summary>
        DisconnectDatabase = 1,
        /// <summary>
        /// 
        /// </summary>
        Disconnect = 2,
        /// <summary>
        /// 
        /// </summary>
        Read = 3,
        /// <summary>
        /// 
        /// </summary>
        CancelRead = 4,
        /// <summary>
        /// 
        /// </summary>
        Write = 5,
        /// <summary>
        /// 
        /// </summary>
        SetEncodingMethod = 6,
        /// <summary>
        /// 
        /// </summary>
        GetAllDatabases = 7
    }

    /// <summary>
    /// Server response
    /// </summary>
    public enum ServerResponse : byte
    {
        /// <summary>
        /// 
        /// </summary>
        UnhandledException = 0,
        /// <summary>
        /// Occurs at first connection if the protocol version is not recgonized by the server.
        /// </summary>
        UnknownProtocol = 1,
        /// <summary>
        /// 
        /// </summary>
        ConnectedToRoot = 2,
        /// <summary>
        /// 
        /// </summary>
        ListOfDatabases = 3,
        /// <summary>
        /// 
        /// </summary>
        DatabaseDoesNotExist = 4,
        /// <summary>
        /// 
        /// </summary>
        DatabaseKeyUnknown = 5,
        /// <summary>
        /// 
        /// </summary>
        DatabaseValueUnknown = 6,
        /// <summary>
        /// 
        /// </summary>
        SuccessfullyConnectedToDatabase = 7,
        /// <summary>
        /// 
        /// </summary>
        GoodBye = 8,
        /// <summary>
        /// 
        /// </summary>
        UnknownCommand = 9,
        /// <summary>
        /// 
        /// </summary>
        UnknownEncodingMethod = 10,
        /// <summary>
        /// 
        /// </summary>
        EncodingMethodAccepted = 11,
        /// <summary>
        /// 
        /// </summary>
        DatabaseDisconnected = 12,
        /// <summary>
        /// 
        /// </summary>
        UnknownDatabaseCommand = 13,
        /// <summary>
        /// 
        /// </summary>
        UnknownOrCorruptSeekFilter = 14,
        /// <summary>
        /// 
        /// </summary>
        UnknownOrCorruptMatchFilter = 15,
        /// <summary>
        /// 
        /// </summary>
        UnknownOrCorruptReaderOptions = 16,
        /// <summary>
        /// 
        /// </summary>
        SerializingPoints = 17,
        /// <summary>
        /// 
        /// </summary>
        ErrorWhileReading = 18,
        /// <summary>
        /// 
        /// </summary>
        CanceledRead = 19,
        /// <summary>
        /// 
        /// </summary>
        ReadComplete = 20,

        /// <summary>
        /// Occurs during initial connection. 
        /// Indicates that the server name 
        /// string was longer than 100 characters.
        /// </summary>
        ServerNameTooLong = 21,
        /// <summary>
        /// Occurs when the server name 
        /// specificed in the initial connection
        /// does not match this server.
        /// </summary>
        ServerNameDoesNotMatch = 22,

        RequiresLogin = 23,

        KnownProtocol,
        AuthenticationFailed
    }

    #endregion
}