// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace openHistorian.Model
{
    public class AlarmLog
    {
        public int ID
        {
            get;
            set;
        }

        public Guid SignalID
        {
            get;
            set;
        }

        public int? PreviousState
        {
            get;
            set;
        }

        public int? NewState
        {
            get;
            set;
        }

        public long Ticks
        {
            get;
            set;
        }

        public DateTime Timestamp
        {
            get;
            set;
        }

        public double Value
        {
            get;
            set;
        }
    }
}