//******************************************************************************************************
//  LogFileReader.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using GSF.IO;
using Ionic.Zlib;

namespace GSF.Diagnostics
{
    /// <summary>
    /// A method to read all of the logs in a single file.
    /// </summary>
    public static class LogFileReader
    {
        /// <summary>
        /// Reads all log messages from the supplied file.
        /// </summary>
        public static List<LogMessage> Read(string logFileName)
        {
            List<LogMessage> lst = new List<LogMessage>();
            PathHelpers.ValidatePathName(logFileName);
            using (var stream = new FileStream(logFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                try
                {
                    LogMessageSaveHelper helper;
                    int version = stream.ReadInt32();
                    switch (version)
                    {
                        case 282497:
                            helper = LogMessageSaveHelper.Create(); //VersionNumber: Compressed. With LogSaveHelper
                            break;
                        default:
                            throw new VersionNotFoundException();
                    }

                    using (var zipStream = new DeflateStream(stream, CompressionMode.Decompress, true))
                    using (var bs = new BufferedStream(zipStream))
                    {
                        while (bs.ReadBoolean())
                        {
                            var message = new LogMessage(bs, helper);
                            lst.Add(message);
                        }
                        bs.Dispose();
                    }
                }
                catch (EndOfStreamException)
                {

                }
                catch (ZlibException)
                {

                }

            }
            return lst;
        }
    }
}
