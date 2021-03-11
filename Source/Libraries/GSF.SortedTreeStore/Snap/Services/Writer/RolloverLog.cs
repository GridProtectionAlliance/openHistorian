//******************************************************************************************************
//  RolloverLog.cs - Gbtc
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;

namespace GSF.Snap.Services.Writer
{
    /// <summary>
    /// The log file that describes the rollover process so if the service crashes during the rollover process,
    /// it can properly be recovered from.
    /// </summary>
    public class RolloverLog
    {
        private readonly RolloverLogSettings m_settings;

        /// <summary>
        /// Creates a new <see cref="RolloverLog"/>
        /// </summary>
        /// <param name="settings">the settings</param>
        /// <param name="list">the list</param>
        public RolloverLog(RolloverLogSettings settings, ArchiveList list)
        {
            m_settings = settings.CloneReadonly();
            m_settings.Validate();

            if (settings.IsFileBacked)
            {
                foreach (string logFile in Directory.GetFiles(settings.LogPath, settings.SearchPattern))
                {
                    RolloverLogFile log = new RolloverLogFile(logFile);
                    if (log.IsValid)
                    {
                        log.Recover(list);
                    }
                    else
                    {
                        log.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a rollover log file
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <param name="destinationFile"></param>
        /// <returns></returns>
        public RolloverLogFile Create(List<Guid> sourceFiles, Guid destinationFile)
        {
            string fileName = m_settings.GenerateNewFileName();
            return new RolloverLogFile(fileName, sourceFiles, destinationFile);
        }

    }
}
