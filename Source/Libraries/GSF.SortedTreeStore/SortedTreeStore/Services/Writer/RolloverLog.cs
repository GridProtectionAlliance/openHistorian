//******************************************************************************************************
//  RolloverLog.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  10/04/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services.Writer
{
    /// <summary>
    /// The log file that describes the rollover process so if the service crashes during the rollover process,
    /// it can properly be recovered from.
    /// </summary>
    public class RolloverLog<TKey, TValue>
        where TKey : SortedTreeTypeBase<TKey>, new()
        where TValue : SortedTreeTypeBase<TValue>, new()
    {
        private RolloverLogSettings m_settings;

        /// <summary>
        /// Creates a new <see cref="RolloverLog{TKey,TValue}"/>
        /// </summary>
        /// <param name="settings">the settings</param>
        /// <param name="list">the list</param>
        public RolloverLog(RolloverLogSettings settings, ArchiveList<TKey, TValue> list)
        {
            m_settings = settings;

            if (settings.IsFileBacked)
            {
                foreach (var logFile in Directory.GetFiles(settings.LogPath, settings.SearchPattern))
                {
                    var log = new RolloverLogFile(logFile);
                    if (log.IsValid)
                    {
                        log.Recover(list);
                    }
                    else
                    {
                        //log.Delete();
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
