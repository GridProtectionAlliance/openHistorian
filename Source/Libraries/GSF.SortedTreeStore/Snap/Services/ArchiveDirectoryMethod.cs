//******************************************************************************************************
//  ArchiveDirectoryMethod.cs - Gbtc
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
//  10/01/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

namespace GSF.Snap.Services
{
    /// <summary>
    /// Specifies the directory structure to follow when writing archive files to the disk.
    /// </summary>
    public enum ArchiveDirectoryMethod : int
    {
        /// <summary>
        /// Writes all files in the top directory
        /// </summary>
        TopDirectoryOnly,
        /// <summary>
        /// Writes all files based on the starting year
        /// </summary>
        Year,
        /// <summary>
        /// Writes all files based on 'YearMonth'
        /// </summary>
        YearMonth,
        /// <summary>
        /// Writes all files based on 'Year\Month'
        /// </summary>
        YearThenMonth
    }
}