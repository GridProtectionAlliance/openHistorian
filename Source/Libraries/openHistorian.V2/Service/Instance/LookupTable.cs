
namespace openHistorian.V2.Service.Instance
{
    class LookupTable
    {
        LookupTableSnapshot m_activeSnapshot;

        public LookupTable()
        {
            m_activeSnapshot = new LookupTableSnapshot();
        }

        public LookupTableSnapshot GetLatestSnapshot()
        {
            lock (this)
            {
                return m_activeSnapshot;
            }
        }
    }
}
