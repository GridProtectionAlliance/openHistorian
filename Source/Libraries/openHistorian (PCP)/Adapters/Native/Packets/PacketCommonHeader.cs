//******************************************************************************************************
//  PacketCommonHeader.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  04/21/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System;
using TVA;
using TVA.Parsing;

namespace openHistorian.Adapters.Native.Packets
{
    /// <summary>
    /// Represents the common header information that is present in the binary image of all <see cref="Type"/>s that implement the <see cref="IPacket"/> interface.
    /// </summary>
    public class PacketCommonHeader : CommonHeaderBase<short>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PacketCommonHeader"/> class.
        /// </summary>
        /// <param name="binaryImage">Binary image to be used for initializing <see cref="PacketCommonHeader"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="binaryImage"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="binaryImage"/> from <paramref name="startIndex"/>.</param>
         public PacketCommonHeader(byte[] binaryImage, int startIndex, int length)
         {
             if (length > 1)
                 TypeID = EndianOrder.LittleEndian.ToInt16(binaryImage, startIndex);
             else
                 throw new InvalidOperationException("Binary image is malformed");
         }
    }
}
