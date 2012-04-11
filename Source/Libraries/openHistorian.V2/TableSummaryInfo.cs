using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TVA;

namespace openHistorian.V2
{
    class TableSummaryInfo
    {
        public TableSummaryInfo()
        {
            m_isReadOnly = false;
        }

        public enum MatchMode : byte
        {
            /// <summary>
            /// Set if the cache is empty.
            /// Matches No Case
            /// </summary>
            EmptyEntry = 0,
            /// <summary>
            /// Set if there are both upper and lower bounds present.
            /// Matches [LowerBound,UpperBound]
            /// </summary>
            Bounded = 3,
            /// <summary>
            /// Set if there is only a lower bound.
            /// Matches [LowerBound, infinity)
            /// </summary>
            UpperIsMissing = 1,
            /// <summary>
            /// Set if there is only an upper bound.
            /// Matches (-infinity, UpperBound]
            /// </summary>
            LowerIsMissing = 2,
            /// <summary>
            /// Matches unconditionally.
            /// </summary>
            UniverseEntry = 4,

            LowerIsValidMask = 1,
            UpperIsValidMask = 2,
        }
        [Flags]
        public enum DataClass : long
        {
            All = -1,
            None = 0,
            Synchrophasor = 1,
            Scada = 2,
        }

        bool m_isReadOnly;
        Ticks m_firstTime;
        Ticks m_lastTime;
        MatchMode m_timeMatchMode;
        DataClass m_containsPointClasses;
        Archive m_archiveFile;
        object m_activeSnapshot;

        public TableSummaryInfo CloneEditableCopy()
        {
            TableSummaryInfo table = new TableSummaryInfo();
            table.m_firstTime = m_firstTime;
            table.m_lastTime = m_lastTime;
            table.m_timeMatchMode = m_timeMatchMode;
            table.m_containsPointClasses = m_containsPointClasses;
            table.m_archiveFile = m_archiveFile;
            table.m_activeSnapshot = m_activeSnapshot;
            return table;
        }
        public bool IsReadOnly
        {
            get
            {
                return m_isReadOnly;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_isReadOnly = value;
            }
        }
        public Archive ArchiveFile
        {
            get
            {
                return m_archiveFile;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_archiveFile = value;
            }
        }
        public Ticks FirstTime
        {
            get
            {
                return m_firstTime;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_firstTime = value;
            }
        }
        public Ticks LastTime
        {
            get
            {
                return m_lastTime;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_lastTime = value;
            }
        }
        public MatchMode TimeMatchMode
        {
            get
            {
                return m_timeMatchMode;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_timeMatchMode = value;
            }
        }
        public DataClass ContainsPointClasses
        {
            get
            {
                return m_containsPointClasses;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_containsPointClasses = value;
            }
        }
        public object ActiveSnapshot
        {
            get
            {
                return m_activeSnapshot;
            }
            set
            {
                if (m_isReadOnly)
                    throw new ReadOnlyException("Object is read only");
                m_activeSnapshot = value;
            }
        }

        public bool Contains(Ticks startTime, Ticks stopTime)
        {
            //ToDo: Don't be lazy and always return true
            return true;
        }
    }
}
