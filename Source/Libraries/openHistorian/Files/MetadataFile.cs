//******************************************************************************************************
//  MetadataFile.cs - Gbtc
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
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TVA.IO;

namespace openHistorian.Files
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataFile"/> class.
        /// </summary>
        /// <param name="container"><see cref="IContainer"/> object that contains the <see cref="MetadataFile"/>.</param>
        public MetadataFile(IContainer container)
            : this()
        {
            if (container != null)
                container.Add(this);
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
