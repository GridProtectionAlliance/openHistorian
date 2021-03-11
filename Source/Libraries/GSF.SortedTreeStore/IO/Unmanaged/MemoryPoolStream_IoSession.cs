//******************************************************************************************************
//  MemoryPoolStream_IoSession.cs - Gbtc
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
    public partial class MemoryPoolStream
    {
        // Nested Types
        private class IoSession 
            : BinaryStreamIoSessionBase
        {
            private readonly MemoryPoolStream m_stream;

            public IoSession(MemoryPoolStream stream)
            {
                if (stream is null)
                    throw new ArgumentNullException("stream");
                m_stream = stream;
            }

            public override void GetBlock(BlockArguments args)
            {
                args.SupportsWriting = true;
                m_stream.GetBlock(args);
            }

            public override void Clear()
            {
            }
        }
    }
}