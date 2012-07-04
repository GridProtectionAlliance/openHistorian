//******************************************************************************************************
//  RolloverGenerationPromotion.cs - Gbtc
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
//  7/3/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************


using System;
using System.Diagnostics;
using System.Threading;

namespace openHistorian.V2.Server.Database
{
    class RolloverGenerationPromotion : IDisposable
    {
        int m_generation;
        ResourceEngine m_resources;

        volatile bool m_disposed;
        int m_generationCount = 0;
        long m_generationSize = 0;
        Stopwatch m_generationInterval = new Stopwatch();
        int m_generationCountLimit = 1000;
        long m_generationSizeLimit = 100L * 1024 * 1024;
        TimeSpan m_generationIntervalLimit = new TimeSpan(0, 0, 0, 10); //10 seconds
        Thread m_processRolloverThread;
        ManualResetEvent m_manualResetEventThreadGeneration;

        public RolloverGenerationPromotion(int generation, int onCommitCount, long onSize, TimeSpan onInterval)
        {
            m_generationCountLimit = onCommitCount;
            m_generationSizeLimit = onSize;
            m_generationIntervalLimit = onInterval;

            m_generation = generation;
            m_generationInterval.Start();
            m_manualResetEventThreadGeneration = new ManualResetEvent(false);
            m_processRolloverThread = new Thread(ProcessRollover);
            m_processRolloverThread.Start();
        }

        void ProcessRollover()
        {
            while (!m_disposed)
            {
                m_manualResetEventThreadGeneration.WaitOne(500);
                m_manualResetEventThreadGeneration.Reset();

                if ((m_generationInterval.Elapsed > m_generationIntervalLimit) ||
                    (m_generationCount > m_generationCountLimit) ||
                    (m_generationSize > m_generationSizeLimit))
                {

                    using (var rolloverMode = m_resources.StartPartitionRolloverMode(m_generation))
                    {
                        var source = rolloverMode.SourcePartition.ActiveSnapshot.OpenInstance();
                        var dest = rolloverMode.DestinationPartition.PartitionFileFile;

                        dest.BeginEdit();

                        var reader = source.GetDataRange();
                        reader.SeekToKey(0, 0);

                        ulong value1, value2, key1, key2;
                        while (reader.GetNextKey(out key1, out key2, out value1, out value2))
                        {
                            dest.AddPoint(key1, key2, value1, value2);
                        }

                        dest.CommitEdit();

                        rolloverMode.Commit();
                    }

                    m_generationCount = 0;
                    m_generationInterval.Restart();
                }
            }
        }

        public void SignalRolloverGeneration()
        {
            m_manualResetEventThreadGeneration.Set();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                SignalRolloverGeneration();
            }
        }

    }
}
