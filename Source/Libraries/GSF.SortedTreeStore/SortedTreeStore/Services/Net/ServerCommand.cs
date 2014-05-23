//******************************************************************************************************
//  ServerCommand.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  9/14/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//  12/8/2012 - Steven E. Chisholm
//       Major change to the Interface by breaking out database/server features. 
//
//******************************************************************************************************

namespace GSF.SortedTreeStore.Services.Net
{
    #region [ Enumerations ]

    /// <summary>
    /// Server commands
    /// </summary>
    public enum ServerCommand : byte
    {
        ConnectToDatabase = 0,
        DisconnectDatabase = 1,
        Disconnect = 2,
        Read = 3,
        CancelRead = 4,
        Write = 5,
        SetEncodingMethod = 6,
        GetAllDatabases = 7
    }

    /// <summary>
    /// Server response
    /// </summary>
    public enum ServerResponse : byte
    {
        UnhandledException=0,
        UnknownProtocolIdentifier=1,
        ConnectedToRoot=2,
        ListOfDatabases=3,
        DatabaseDoesNotExist=4,
        DatabaseKeyUnknown=5,
        DatabaseValueUnknown=6,
        SuccessfullyConnectedToDatabase=7,
        GoodBye=8,
        UnknownCommand=9,
        UnknownEncodingMethod=10,
        EncodingMethodAccepted=11,
        DatabaseDisconnected=12,
        UnknownDatabaseCommand=13,
        UnknownOrCorruptSeekFilter=14,
        UnknownOrCorruptMatchFilter=15,
        UnknownOrCorruptReaderOptions=16,
        SerializingPoints=17,
        ErrorWhileReading=18,
        CanceledRead=19,
        ReadComplete=20
    }

    #endregion
}