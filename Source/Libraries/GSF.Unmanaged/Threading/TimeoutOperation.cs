using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSF.Threading
{

    public class TimeoutOperation
    {
        object m_syncRoot = new object();
        RegisteredWaitHandle m_registeredHandle;
        ManualResetEvent m_resetEvent;
        Action m_callback;

        public void RegisterTimeout(TimeSpan interval, Action callback)
        {

            lock (m_syncRoot)
            {
                if (m_callback != null)
                    throw new Exception("Duplicate calls are not permitted");

                m_callback = callback;
                m_resetEvent = new ManualResetEvent(false);
                m_registeredHandle = ThreadPool.RegisterWaitForSingleObject(m_resetEvent, BeginRun, null, interval, true);
            }

        }

        void BeginRun(object state, bool isTimeout)
        {
            lock (m_syncRoot)
            {
                if (m_registeredHandle == null)
                    return;
                m_registeredHandle.Unregister(null);
                m_resetEvent.Dispose();
                m_callback();
                m_resetEvent = null;
                m_registeredHandle = null;
                m_callback = null;
            }

        }

        public void Cancel()
        {
            lock (m_syncRoot)
            {
                if (m_registeredHandle != null)
                {
                    m_registeredHandle.Unregister(null);
                    m_resetEvent.Dispose();
                    m_resetEvent = null;
                    m_registeredHandle = null;
                    m_callback = null;
                }
            }

        }
    }
}
