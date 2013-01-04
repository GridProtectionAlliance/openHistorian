//******************************************************************************************************
//  SortedTree256Initializer.cs - Gbtc
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
//  4/5/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.IO;
using openHistorian.Archive;

namespace openHistorian.Collections.KeyValue
{
    internal static class SortedTree256Initializer
    {
        public static SortedTree256Base Create(BinaryStreamBase stream, int blockSize, CompressionMethod method)
        {
            switch (method)
            {
                case CompressionMethod.None:
                    return new SortedTree256(stream, blockSize);
                case CompressionMethod.DeltaEncoded:
                    return new SortedTree256DeltaEncoded(stream, blockSize);
                case CompressionMethod.TimeSeriesEncoded:
                    return new SortedTree256TSEncoded(stream, blockSize);
                case CompressionMethod.TimeSeriesEncoded2:
                    return new SortedTree256TS32Encoded(stream, blockSize);
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
        }
        public static SortedTree256Base Open(BinaryStreamBase stream)
        {
            stream.Position = 0;
            Guid type = stream.ReadGuid();
            if (type == SortedTree256.GetFileType())
                return new SortedTree256(stream);
            if (type == SortedTree256DeltaEncoded.GetFileType())
                return new SortedTree256DeltaEncoded(stream);
            if (type == SortedTree256TSEncoded.GetFileType())
                return new SortedTree256TSEncoded(stream);
            if (type == SortedTree256TS32Encoded.GetFileType())
                return new SortedTree256TS32Encoded(stream);
            throw new ArgumentOutOfRangeException("method");
        }

    }
}
