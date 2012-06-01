//******************************************************************************************************
//  ISupportsBinaryStream.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  2/11/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.IO.Unmanaged
{
    /// <summary>
    /// Implementing this interface allows a binary stream to be attached to a buffer.
    /// </summary>
    public interface ISupportsBinaryStream : IDisposable
    {
        /// <summary>
        /// This event is critical because it will notify a <see cref="BinaryStream"/> that the stream is closed. 
        /// Failing to raise this event on a close may result subsequent calls to the <see cref="BinaryStream"/> to 
        /// corrupt memory.
        /// </summary>
        event EventHandler StreamDisposed;

        /// <summary>
        /// Gets the number of available simultaneous read/write sessions.
        /// </summary>
        /// <remarks>This value is used to determine if a binary stream can be cloned
        /// to improve read/write/copy performance.</remarks>
        int RemainingSupportedIoSessions { get; }

        /// <summary>
        /// Aquire an IO Session.
        /// </summary>
        IBinaryStreamIoSession GetNextIoSession();

        /// <summary>
        /// Creates a new binary from an IO session
        /// </summary>
        /// <returns></returns>
        IBinaryStream CreateBinaryStream();
    
    }
}
