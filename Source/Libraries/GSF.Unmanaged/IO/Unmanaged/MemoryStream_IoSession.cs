//******************************************************************************************************
//  MemoryStream.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  5/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.IO.Unmanaged
{
    /// <summary>
    /// Provides a in memory stream that uses pages that are pooled in the unmanaged buffer pool.
    /// </summary>
    public partial class MemoryStream
    {
        // Nested Types
        class IoSession : IBinaryStreamIoSession
        {
            MemoryStream m_stream;

            public IoSession(MemoryStream stream)
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");
                m_stream = stream;
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public void Dispose()
            {
                m_stream = null;
            }

            /// <summary>
            /// Gets a block for the following Io session.
            /// </summary>
            /// <param name="position">the block returned must contain this position</param>
            /// <param name="isWriting">indicates if the stream plans to write to this block</param>
            /// <param name="firstPointer">the pointer for the first byte of the block</param>
            /// <param name="firstPosition">the position that corresponds to <see cref="firstPointer"/></param>
            /// <param name="length">the length of the block</param>
            /// <param name="supportsWriting">notifies the calling class if this block supports 
            /// writing without requiring this function to be called again if <see cref="isWriting"/> was set to false.</param>
            public void GetBlock(long position, bool isWriting, out IntPtr firstPointer, out long firstPosition, out int length, out bool supportsWriting)
            {
                if (m_stream == null)
                    throw new ObjectDisposedException(GetType().FullName);
                supportsWriting = true;
                m_stream.GetBlock(position, out firstPointer, out firstPosition, out length);
            }

            /// <summary>
            /// Sets the current usage of the <see cref="IBinaryStreamIoSession"/> to null.
            /// </summary>
            public void Clear()
            {
            }

            /// <summary>
            /// Gets if the object has been disposed
            /// </summary>
            public bool IsDisposed
            {
                get
                {
                    return (m_stream == null);
                }
            }
        }
    }
}
