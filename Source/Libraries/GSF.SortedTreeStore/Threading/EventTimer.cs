//******************************************************************************************************
//  EventTimer.cs - Gbtc
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
//  10/22/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//*****************************************************************************************************

using System;
using GSF.Diagnostics;

namespace GSF.Threading
{
    /// <summary>
    /// A timer event that occurs on a specific interval at a specific offset.
    /// 
    /// This class is thread safe.
    /// </summary>
    public class EventTimer
        : DisposableLoggingClassBase
    {
        /// <summary>
        /// Occurs when the timer elapses.
        /// Event occurs on the ThreadPool
        /// </summary>
        public event Action Elapsed;

        private ScheduledTask m_timer;
        private TimeSpan m_period;
        private TimeSpan m_dayOffset;
        private readonly object m_syncRoot;
        private bool m_isRunning;
        private bool m_stopping;
        private bool m_disposed;

        private readonly LogStackMessages m_message;
        /// <summary>
        /// Creates a <see cref="EventTimer"/>
        /// </summary>
        /// <param name="period"></param>
        /// <param name="dayOffset"></param>
        private EventTimer(TimeSpan period, TimeSpan dayOffset)
            : base(MessageClass.Component)
        {
            m_stopping = false;
            m_syncRoot = new object();
            m_period = period;
            m_dayOffset = dayOffset;

            m_message = LogStackMessages.Empty.Union("Event Timer Details", string.Format("EventTimer: {0} in {1}", m_period, m_dayOffset));
            Log.InitialStackMessages.Union("Event Timer Details", string.Format("EventTimer: {0} in {1}", m_period, m_dayOffset));
        }

        /// <summary>
        /// This timer will reliably fire the directory polling every interval.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_timer_Running(object sender, EventArgs<ScheduledTaskRunningReason> e)
        {
            //This cannot be combined with m_directoryPolling because 
            //Scheduled task does not support managing multiple conflicting timers.
            if (e.Argument == ScheduledTaskRunningReason.Disposing)
                return;
            if (m_stopping)
                return;

            if (Elapsed != null)
            {
                try
                {
                    using (Logger.AppendStackMessages(m_message))
                    {
                        Elapsed();
                    }
                }
                catch (Exception ex)
                {
                    Log.Publish(MessageLevel.Error, "Event Timer Exception on raising event.", null, null, ex);
                }
            }

            if (m_stopping)
                return;

            RestartTimer();
        }

        /// <summary>
        /// Gets/sets if the timer is enabled.
        /// </summary>
        public bool Enabled
        {
            get => m_isRunning;
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        /// <summary>
        /// The remaining time until the next execution. This is a calculated value and may not exactly represent 
        /// the next time <see cref="Elapsed"/> will run.
        /// </summary>
        public TimeSpan TimeUntilNextExecution
        {
            get
            {
                long current = DateTime.UtcNow.Ticks;
                long subtractOffset = current - m_dayOffset.Ticks;
                long remainderTicks = m_period.Ticks - subtractOffset % m_period.Ticks;
                int delay = (int)(remainderTicks / TimeSpan.TicksPerMillisecond) + 1;
                if (delay < 10)
                    delay += (int)m_period.TotalMilliseconds;
                return new TimeSpan(delay * TimeSpan.TicksPerMillisecond);
            }
        }

        private void RestartTimer()
        {
            long current = DateTime.UtcNow.Ticks;
            long subtractOffset = current - m_dayOffset.Ticks;
            long remainderTicks = m_period.Ticks - subtractOffset % m_period.Ticks;
            int delay = (int)(remainderTicks / TimeSpan.TicksPerMillisecond) + 1;
            if (delay < 10)
                delay += (int)m_period.TotalMilliseconds;

            m_timer.Start(delay);
        }

        /// <summary>
        /// Starts the watching
        /// </summary>
        public void Start()
        {
            lock (m_syncRoot)
            {
                if (m_disposed)
                    return;
                if (m_isRunning)
                    return;
                m_isRunning = true;
                m_timer = new ScheduledTask();
                m_timer.Running += m_timer_Running;
                RestartTimer();
            }
            Log.Publish(MessageLevel.Info, "EventTimer Started");
        }

        /// <summary>
        /// Stops the timer. The call that stops the timer
        /// will pause until the event is stopped. Any subsequent
        /// calls will return immediately.
        /// </summary>
        public void Stop()
        {
            lock (m_syncRoot)
            {
                if (!m_isRunning)
                    return;
                if (m_stopping)
                    return;
                m_stopping = true;
            }
            m_timer.Dispose();
            lock (m_syncRoot)
            {
                m_timer = null;
                m_stopping = false;
                m_isRunning = false;
            }
            Log.Publish(MessageLevel.Info, "EventTimer Started");
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                m_disposed = true;
                Stop();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates an <see cref="EventTimer"/> with the supplied inputs.
        /// </summary>
        /// <param name="period">the period of the timer</param>
        /// <param name="dayOffset">the day offset when the timer will run.</param>
        /// <returns></returns>
        public static EventTimer Create(TimeSpan period, TimeSpan dayOffset = default)
        {
            return new EventTimer(period, dayOffset);
        }

        /// <summary>
        /// Creates an <see cref="EventTimer"/> with the supplied inputs.
        /// </summary>
        /// <param name="periodInSecond">the period of the timer in seconds</param>
        /// <param name="dayOffsetInSecond">the offset from the top of the day in seconds. </param>
        /// <returns></returns>
        public static EventTimer CreateSeconds(double periodInSecond, double dayOffsetInSecond = 0)
        {
            return new EventTimer(new TimeSpan((long)(periodInSecond * TimeSpan.TicksPerSecond)), new TimeSpan((long)(dayOffsetInSecond * TimeSpan.TicksPerSecond)));
        }

        /// <summary>
        /// Creates an <see cref="EventTimer"/> with the supplied inputs.
        /// </summary>
        /// <param name="periodInMinutes">he period of the timer in minutes</param>
        /// <param name="dayOffsetInMinutes"></param>
        /// <returns></returns>
        public static EventTimer CreateMinutes(double periodInMinutes, double dayOffsetInMinutes = 0)
        {
            return new EventTimer(new TimeSpan((long)(periodInMinutes * TimeSpan.TicksPerMinute)), new TimeSpan((long)(dayOffsetInMinutes * TimeSpan.TicksPerMinute)));
        }

    }
}
