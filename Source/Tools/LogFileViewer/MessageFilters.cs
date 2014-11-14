using System;
using System.Collections.Generic;
using GSF.Diagnostics;

namespace LogFileViewer
{
    public interface IMessageMatch
    {
        bool IsIncluded(LogMessageSerializable log);
        string Description { get; }
        IEnumerable<Tuple<String, Action>> GetMenuButtons();
        void ToggleResult();
    }

    public class MatchType : IMessageMatch
    {
        private string m_typeName;
        private bool m_includeIfMatched;

        public MatchType(LogMessageSerializable typeName)
        {
            m_typeName = typeName.Type;
        }

        public bool IsIncluded(LogMessageSerializable log)
        {
            if (log.Type == m_typeName)
                return m_includeIfMatched;
            return !m_includeIfMatched;
        }

        public string Description
        {
            get
            {
                if (m_includeIfMatched)
                    return "Include if Type: " + m_typeName;
                else
                    return "Exclude if Type: " + m_typeName;
            }
        }

        public IEnumerable<Tuple<string, Action>> GetMenuButtons()
        {
            return new[]
                {
                    Tuple.Create<string, Action>("Include Type", () => { m_includeIfMatched = true; }),
                    Tuple.Create<string, Action>("Exclude Type", () => { m_includeIfMatched = false; })
                };
        }

        public void ToggleResult()
        {
            m_includeIfMatched = !m_includeIfMatched;
        }
    }

    public class MatchVerbose : IMessageMatch
    {
        private VerboseLevel m_typeName;
        private bool m_includeIfMatched;

        public MatchVerbose(LogMessageSerializable typeName)
        {
            m_typeName = typeName.Level;
        }

        public bool IsIncluded(LogMessageSerializable log)
        {
            if (log.Level == m_typeName)
                return m_includeIfMatched;
            return !m_includeIfMatched;
        }

        public string Description
        {
            get
            {
                if (m_includeIfMatched)
                    return "Include Level: " + m_typeName.ToString();
                else
                    return "Exclude Level: " + m_typeName.ToString();
            }
        }

        public IEnumerable<Tuple<string, Action>> GetMenuButtons()
        {
            return new[]
                {
                    Tuple.Create<string, Action>("Include Level", () => { m_includeIfMatched = true; }),
                    Tuple.Create<string, Action>("Exclude Level", () => { m_includeIfMatched = false; })
                };
        }
        public void ToggleResult()
        {
            m_includeIfMatched = !m_includeIfMatched;
        }
    }

    public class MatchEventName : IMessageMatch
    {
        private string m_typeName;
        private string m_eventName;
        private bool m_includeIfMatched;

        public MatchEventName(LogMessageSerializable typeName)
        {
            m_typeName = typeName.Type;
            m_eventName = typeName.EventName;
        }

        public bool IsIncluded(LogMessageSerializable log)
        {
            if (log.Type == m_typeName && log.EventName == m_eventName)
                return m_includeIfMatched;
            return !m_includeIfMatched;
        }

        public string Description
        {
            get
            {
                if (m_includeIfMatched)
                    return "Include if Event: " + m_eventName + "(" + m_typeName + ")";
                else
                    return "Exclude if Event: " + m_eventName + "(" + m_typeName + ")";
            }
        }

        public IEnumerable<Tuple<string, Action>> GetMenuButtons()
        {
            return new[]
                    {
                        Tuple.Create<string, Action>("Include Event", () => { m_includeIfMatched = true; }),
                        Tuple.Create<string, Action>("Exclude Event", () => { m_includeIfMatched = false; })
                    };
        }
        public void ToggleResult()
        {
            m_includeIfMatched = !m_includeIfMatched;
        }
    }



    public class MatchTimestamp : IMessageMatch
    {
        private DateTime m_timestamp;
        private bool m_before;

        public MatchTimestamp(LogMessageSerializable typeName)
        {
            m_timestamp = typeName.UtcTime;
        }

        public bool IsIncluded(LogMessageSerializable log)
        {
            if (m_before)
            {
                return log.UtcTime >= m_timestamp;
            }
            else
            {
                return log.UtcTime <= m_timestamp;
            }
        }

        public string Description
        {
            get
            {
                if (m_before)
                {
                    return "Exclude if before: " + m_timestamp;
                }
                else
                {
                    return "Exclude if after: " + m_timestamp;
                }
            }
        }

        public IEnumerable<Tuple<string, Action>> GetMenuButtons()
        {
            return new[]
                    {
                        Tuple.Create<string, Action>("Exclude Before", () => { m_before = true; }),
                        Tuple.Create<string, Action>("Exclude After", () => { m_before = false; }),
                        Tuple.Create<string, Action>("Exclude 5 Minutes Before", () =>{m_before = true; m_timestamp = m_timestamp.AddMinutes(-5);}),
                        Tuple.Create<string, Action>("Exclude 5 Minutes After", () =>{m_before = false; m_timestamp = m_timestamp.AddMinutes(5);}),
                    };
        }
        public void ToggleResult()
        {
            m_before = !m_before;
        }
    }
}
