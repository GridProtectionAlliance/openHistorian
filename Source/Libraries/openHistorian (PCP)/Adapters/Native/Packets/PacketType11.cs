//******************************************************************************************************
//  PacketType11.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  06/18/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/10/2009 - Pinal C. Patel
//       Modified Process() to use deferred execution using "yield" for efficiency.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System.Collections.Generic;
using openHistorian.Archives.V1;
using TVA.Parsing;

namespace openHistorian.Adapters.Native.Packets
{
    /// <summary>
    /// Represents a packet to be used for requesting <see cref="StateRecord.Summary"/> for the <see cref="QueryPacketBase.RequestIDs"/>.
    /// </summary>
    public class PacketType11 : QueryPacketBase
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType11"/> class.
        /// </summary>
        public PacketType11()
            : base(11)
        {
            ProcessHandler = Process;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketType11"/> class.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing <see cref="PacketType11"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        public PacketType11(byte[] buffer, int startIndex, int length)
            : this()
        {
            ParseBinaryImage(buffer, startIndex, length);
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Processes <see cref="PacketType11"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> object containing the binary images of <see cref="StateRecord.Summary"/> for the <see cref="QueryPacketBase.RequestIDs"/>.</returns>
        protected virtual IEnumerable<byte[]> Process()
        {
            if (Archive == null)
                yield break;

            byte[] data;
            if (RequestIDs.Count == 0 || (RequestIDs.Count == 1 && RequestIDs[0] == -1))
            {
                // Information for all defined records is requested.
                int id = 0;
                while (true)
                {
                    data = Archive.ReadStateDataSummary(++id);
                    if (data == null)
                        // No more records.
                        break;
                    else
                        // Yield retrieved data.
                        yield return data;
                }
            }
            else
            {
                // Information for specific records is requested.
                foreach (int id in RequestIDs)
                {
                    data = Archive.ReadStateDataSummary(id);
                    if (data == null)
                        // ID is invalid.
                        continue;
                    else
                        // Yield retrieved data.
                        yield return data;
                }
            }
            yield return new StateRecord(-1).Summary.BinaryImage();   // To indicate EOT.
        }

        #endregion
    }
}