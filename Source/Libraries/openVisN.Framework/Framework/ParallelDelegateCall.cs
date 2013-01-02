using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace openVisN.Framework
{
    public static class ParallelDelegateCall
    {
        public static void ParallelRunAndWait<T>(this EventHandler<T> eventToRun, object sender, T variable)
        {
            var type = new ParallelRun<T>(eventToRun, sender, variable);
        }
        class ParallelRun<T>
        {
            int m_numberOfDelegates;
            int m_delegatesExecuted;
            object m_sender;
            T m_variable;
            ManualResetEvent m_waitForComplete;
            List<Exception> m_exceptionsThrown;

            //StringBuilder m_sb;

            public ParallelRun(EventHandler<T> eventToRun, object sender, T variable)
            {
                //m_sb = new StringBuilder();
                m_sender = sender;
                m_variable = variable;
                var delegates = eventToRun.GetInvocationList();
                m_numberOfDelegates = delegates.Count();
                m_delegatesExecuted = 0;
                m_waitForComplete = new ManualResetEvent(false);
                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

                int workerThreads;
                int completionPortThreads;
                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
                foreach (var d in delegates)
                {
                    ThreadPool.QueueUserWorkItem(Process, d);
                }
                m_waitForComplete.WaitOne();
                //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;
                //Clipboard.SetText(m_sb.ToString());

                if (m_exceptionsThrown != null)
                    throw new Exception("There were errors");
            }

            public void Process(object callback)
            {
                //lock (m_sb)
                //{
                //    m_sb.AppendLine("Start");
                //}
                EventHandler<T> d2 = (EventHandler<T>)callback;
                //try
                //{
                d2.Invoke(m_sender, m_variable);
                //}
                //catch (Exception ex)
                //{
                //    lock (this)
                //    {
                //        if (m_exceptionsThrown == null)
                //            m_exceptionsThrown = new List<Exception>();
                //        m_exceptionsThrown.Add(ex);
                //    }
                //}

                //lock (m_sb)
                //{
                //    m_sb.AppendLine("Stop");
                //}
                if (Interlocked.Increment(ref m_delegatesExecuted) == m_numberOfDelegates)
                {
                    m_waitForComplete.Set();
                }
            }



        }
    }
}
