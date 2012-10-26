//******************************************************************************************************
//  ArchiveWriterSettings.cs - Gbtc
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
//  7/24/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2.Server.Configuration
{
    /// <summary>
    /// Controls the initial write stages when writing in a stream-like fassion.
    /// </summary>
    public class ArchiveWriterSettings
    {
        /// <summary>
        /// On this interval, new data that is added to the stream will be written
        /// so it can be referenced by the end user. This is typically the 
        /// memory commit interval and is short. Like a fraction of a second.
        /// </summary>
        public TimeSpan CommitOnInterval;
        /// <summary>
        /// On this interval, the existing archive file will no longer be appended to.
        /// The edit lock will be released and a new file will be created to append data
        /// to. This is typically the disk commit interval since all of the contents of
        /// this file will usually be rolled over into a higher generation file
        /// that is disk backed. Recommended ranges of 1-30 seconds.
        /// </summary>
        public TimeSpan NewFileOnInterval;
    }
}
