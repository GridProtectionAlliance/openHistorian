//******************************************************************************************************
//  BufferedFileStream_IoStream.cs - Gbtc
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
//  4/18/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

namespace GSF.IO.Unmanaged
{
    public partial class BufferedFileStream
    {
        // Nested Types
        private class IoSession : BinaryStreamIoSessionBase
        {
            private readonly BufferedFileStream m_stream;
            private readonly LeastRecentlyUsedPageReplacement.IoSession m_ioSession;

            /// <summary>
            /// Creates a new <see cref="IoSession"/>
            /// </summary>
            /// <param name="stream">the base class</param>
            /// <param name="ioSession">The LRU IO Session</param>
            internal IoSession(BufferedFileStream stream, LeastRecentlyUsedPageReplacement.IoSession ioSession)
            {
                m_stream = stream;
                m_ioSession = ioSession;
            }

            /// <summary>
            /// Releases all the resources used by the <see cref="IoSession"/> object.
            /// </summary>
            public override void Dispose()
            {
                if (!IsDisposed)
                {
                    m_ioSession.Dispose();
                    base.Dispose();
                }
            }

            /// <summary>
            /// Sets the current usage of the <see cref="BinaryStreamIoSessionBase"/> to null.
            /// </summary>
            public override void Clear()
            {
                m_stream.Clear(m_ioSession);
                base.Clear();
            }

            /// <summary>
            /// Gets a block for the following Io session.
            /// </summary>
            public override void GetBlock(BlockArguments args)
            {
                m_stream.GetBlock(m_ioSession, args);
            }
        }
    }
}