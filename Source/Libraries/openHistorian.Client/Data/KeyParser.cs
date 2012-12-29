using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openHistorian.Data
{
    public class KeyParser
    {
        bool m_single;
        ulong m_start;
        ulong m_current;
        ulong m_mainInterval;
        ulong m_subInterval;
        uint m_count;
        uint m_subIntervalPerMainInterval;
        ulong m_tolerance;
        ulong m_stop;

        public KeyParser(ulong start, ulong stop)
        {
            m_single = true;
            m_start = start;
            m_stop = stop;
        }

        public KeyParser(ulong start, ulong stop, ulong mainInterval, ulong subInterval, ulong tolerance)
        {
            if (start > stop)
                throw new Exception();
            if (mainInterval < subInterval)
                throw new Exception();
            if (tolerance >= subInterval)
                throw new Exception();

            m_single = false;
            m_start = start;
            m_stop = stop;
            m_current = start;
            m_mainInterval = mainInterval;
            m_subInterval = subInterval;
            m_subIntervalPerMainInterval = (uint)Math.Round((double)mainInterval / (double)subInterval);
            m_tolerance = tolerance;
            m_count = 0;
        }

        public ulong Start
        {
            get
            {
                return m_start;
            }
        }

        public ulong Stop
        {
            get
            {
                return m_stop;
            }
        }

        public void Reset()
        {
            m_current = m_start;
            m_count = 0;
        }

        //ToDo: make this tolerance work at min/max boundaries.
        public bool GetNextWindow(out ulong startOfWindow, out ulong endOfWindow)
        {
            if (m_single)
            {
                if (m_count > 0)
                {
                    startOfWindow = 0;
                    endOfWindow = 0;
                    return false;
                }
                startOfWindow = m_start;
                endOfWindow = m_stop;
                m_count++;
                return true;
            }

            checked
            {
                ulong middle = m_current + (m_subInterval * m_count);
                startOfWindow = middle - m_tolerance;
                endOfWindow = middle + m_tolerance;

                if (startOfWindow > m_stop)
                {
                    startOfWindow = 0;
                    endOfWindow = 0;
                    return false;
                }

                if (m_count + 1 == m_subIntervalPerMainInterval)
                {
                    m_current += m_mainInterval;
                    m_count = 0;
                }
                else
                {
                    m_count += 1;
                }
                return true;
            }
        }
    }
}
