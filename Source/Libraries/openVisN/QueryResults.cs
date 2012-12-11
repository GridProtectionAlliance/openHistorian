using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN
{
    public class QueryResults
    {
        Dictionary<ulong, List<KeyValuePair<ulong, ulong>>> m_results;
        public QueryResults()
        {
            m_results = new Dictionary<ulong, List<KeyValuePair<ulong, ulong>>>();
        }

        public void AddPointIfNotExists(ulong pointId)
        {
            if (!m_results.ContainsKey(pointId))
                m_results.Add(pointId, new List<KeyValuePair<ulong, ulong>>());
        }

        public List<KeyValuePair<ulong, ulong>> GetPointList(ulong pointId)
        {
            return m_results[pointId];
        }

        public void AddPoint(ulong time, ulong point, ulong value)
        {
            m_results[point].Add(new KeyValuePair<ulong, ulong>(time, value));
        }

        public IEnumerable<ulong> GetAllPoints()
        {
            return m_results.Keys;
        }

    }

}
