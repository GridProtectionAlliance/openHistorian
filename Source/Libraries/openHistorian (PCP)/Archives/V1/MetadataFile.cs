//******************************************************************************************************
//  MetadataFile.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/08/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/03/2009 - Pinal C. Patel
//       Added Read() overload that takes string as its parameter for performing flexible reads.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/16/2009 - Pinal C. Patel
//       Changed the default value for SettingsCategory property to the type name.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using TVA.IO;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// Represents a file containing <see cref="MetadataRecord"/>s.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    [ToolboxBitmap(typeof(MetadataFile))]
    public class MetadataFile : IsamDataFileBase<MetadataRecord>
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataFile"/> class.
        /// </summary>
        public MetadataFile()
            : base()
        {
            SettingsCategory = this.GetType().Name;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Reads <see cref="MetadataRecord"/>s that matches the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">Comma or semi-colon delimited list of IDs or text for which the matching <see cref="MetadataRecord"/>s are to be retrieved.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> object of <see cref="MetadataRecord"/>.</returns>
        public IEnumerable<MetadataRecord> Read(string searchPattern)
        {
            int id;
            foreach (string searchPart in searchPattern.Split(',', ';'))
            {
                // Iterate through all parts.
                if (int.TryParse(searchPattern, out id))
                {
                    // Exact id is specified.
                    yield return Read(id);
                }
                else
                {
                    // Text is specfied, so search for matches.
                    foreach (MetadataRecord record in Read())
                    {
                        if (record.Name.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            record.Synonym1.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            record.Synonym2.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            record.Synonym3.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            record.Description.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            record.Remarks.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            record.HardwareInfo.IndexOf(searchPart, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            yield return record;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the binary size of a <see cref="MetadataRecord"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer.</returns>
        protected override int GetRecordSize()
        {
            return MetadataRecord.ByteCount;
        }

        /// <summary>
        /// Creates a new <see cref="MetadataRecord"/> with the specified <paramref name="recordIndex"/>.
        /// </summary>
        /// <param name="recordIndex">1-based index of the <see cref="MetadataRecord"/>.</param>
        /// <returns>A <see cref="MetadataRecord"/> object.</returns>
        protected override MetadataRecord CreateNewRecord(int recordIndex)
        {
            return new MetadataRecord(recordIndex);
        }

        #endregion
    }
}
