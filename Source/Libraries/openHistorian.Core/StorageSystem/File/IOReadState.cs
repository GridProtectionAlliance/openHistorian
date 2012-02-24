//******************************************************************************************************
//  IOReadState.cs - Gbtc
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
//  1/4/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    /// Since exceptions are very expensive, this enum will be returned for basic
    /// I/O operations to let the reader know what to do with the data.  
    /// </summary>
    /// <remarks>There two overarching conditions.  Valid or not Valid.  
    /// If not valid, the reason why the page failed will be given.
    /// If a page is returned as valid, this does not mean that the 
    /// page being referenced is the correct page, it is up to the class
    /// to check the footer of the page to verify that the page being read
    /// is the correct page.</remarks>
    internal enum IOReadState
    {
        /// <summary>
        /// Indicates that the read completed sucessfully.
        /// </summary>
        Valid,
        /// <summary>
        /// The checksum failed to compute
        /// </summary>
        ChecksumInvalid,
        /// <summary>
        /// Special case if the entire page is zeros. 
        /// This means the page was likely never written to.
        /// However, a disk error what wipes this area with zeros can also generate this case.
        /// </summary>
        ChecksumInvalidBecausePageIsNull,
        /// <summary>
        /// Indicates that the page being read went past the end of the file.
        /// </summary>
        ReadPastThenEndOfTheFile,
        /// <summary>
        /// The page that was requested came from a newer version of the file.
        /// </summary>
        PageNewerThanSnapshotSequenceNumber,
        /// <summary>
        /// The page came from a different file.
        /// </summary>
        FileIDNumberDidNotMatch,
        /// <summary>
        /// The index value did not match that of the file.
        /// </summary>
        IndexNumberMissmatch,
        /// <summary>
        /// The page type requested did not match what was received
        /// </summary>
        BlockTypeMismatch
    }
}
