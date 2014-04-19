//******************************************************************************************************
//  ConsoleLogger.cs - Gbtc
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
//  4/11/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Logging;

namespace GSF.Diagnostics
{
    /// <summary>
    /// Creates a Log Handler that will write log messages to the System.Console.
    /// </summary>
    public class ConsoleLogger
    {
        Logger m_logger;
        LogHandler m_handler;

        public ConsoleLogger(Logger log)
        {
            m_logger = log;
            m_handler = log.Handle();
            m_handler.Log += Handler_Log;
            m_handler.Verbose = VerboseLevel.All;
        }

        void Handler_Log(LogMessage logMessage)
        {
            System.Console.WriteLine(logMessage.GetMessage(true));
        }
    }
}
