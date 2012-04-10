//******************************************************************************************************
//  PacketCommonHeader.cs - Gbtc
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
//  04/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//  11/30/2011 - J. Ritchie Carroll
//       Modified to support buffer optimized ISupportBinaryImage.
//
//******************************************************************************************************

using System;
using TVA;
using TVA.Parsing;

namespace openHistorian.V1.Adapters
{
    /// <summary>
    /// Represents the common header information that is present in the binary image of all <see cref="Type"/>s that implement the <see cref="IPacket"/> interface.
    /// </summary>
    public class PacketCommonHeader : CommonHeaderBase<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketCommonHeader"/> class.
        /// </summary>
        /// <param name="buffer">Buffer containing binary image to be used for initializing <see cref="PacketCommonHeader"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <exception cref="InvalidOperationException">Not enough <paramref name="length"/> available to parse <see cref="PacketCommonHeader"/>.</exception>
        public PacketCommonHeader(byte[] buffer, int startIndex, int length)
        {
            if (length > 1)
                TypeID = EndianOrder.LittleEndian.ToInt16(buffer, startIndex);
            else
                throw new InvalidOperationException("Not enough length available to parse common header");
        }
    }
}
