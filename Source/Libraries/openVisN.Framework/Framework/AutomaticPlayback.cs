//******************************************************************************************************
//  AutomaticPlayback.cs - Gbtc
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
//  1/7/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace openVisN.Framework
{
    /// <summary>
    /// This class controls the automatic playback functionality of the historian.
    /// </summary>
    public class AutomaticPlayback
    {
        DateTime m_playbackDate;
        Stopwatch m_timeDuration;
        double m_playbackSpeed;
        TimeSpan m_windowSize;

        public AutomaticPlayback()
        {
            m_timeDuration = new Stopwatch();
            m_playbackSpeed = 1;
            m_playbackDate = DateTime.UtcNow;
            m_windowSize = new TimeSpan(5 * TimeSpan.TicksPerMinute);
        }

        public double PlaybackSpeed
        {
            get
            {
                return m_playbackSpeed;
            }
            set
            {
                LiveModeSelected();
                m_playbackSpeed = value;
            }
        }

        public void GetTimes(out DateTime startTime, out DateTime stopTime)
        {
            stopTime = m_playbackDate.AddTicks((long)(m_timeDuration.Elapsed.Ticks * m_playbackSpeed));
            startTime = stopTime.Subtract(m_windowSize);
        }

        public void StartPlaybackFrom(DateTime time)
        {
            m_playbackDate = time;
            m_timeDuration.Restart();
        }

        public void ChangeWindowSize(TimeSpan windowSize)
        {
            m_windowSize = windowSize;
        }

        public void LiveModeSelected()
        {
            StartPlaybackFrom(m_playbackDate.AddTicks((long)(m_timeDuration.Elapsed.Ticks * m_playbackSpeed)));
        }

    }
}
