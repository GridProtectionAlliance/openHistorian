//******************************************************************************************************
//  TransactionalBlocking.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  1/19/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GSF.Threading;

namespace openHistorian.Engine.ArchiveWriters
{
    internal partial class WriteProcessor
    {
        /// <summary>
        /// This class handles all of the blocking and callbacks that are required by clients
        /// </summary>
        class TransactionalBlocking
        {

            WriteProcessor m_write;
            long m_lastCommitedSequenceNumber;
            long m_lastRolloverSequenceNumber;
            long m_latestSequenceId;
            bool m_threadHasQuit;
            bool m_disposed;

            object m_syncRoot;

            public TransactionalBlocking(WriteProcessor writer)
            {
                m_write = writer;
                m_syncRoot = new object();

            }

            public void WaitForCommit(bool executeNow, bool hardCommit, long sequenceNumber)
            {
                
            }

            public void RegisterCallback(bool executeNow, bool hardCommit, long sequenceNumber, Action callback)
            {
                
            }

            void SoftCommitSequenceNumber(long sequenceNumber)
            {
                
            }

            void HardCommitSequenceNumber(long sequenceNumber)
            {
                
            }

            public void Dispose()
            {
                if (!m_disposed)
                {
                    lock (m_syncRoot)
                    {
                        m_disposed = true;
                        //ShouldQuit = true;
                        //AsyncProcess.Dispose();
                    }
                }
            }
        }
    }
}
