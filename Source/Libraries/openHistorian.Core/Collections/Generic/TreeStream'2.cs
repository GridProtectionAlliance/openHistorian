namespace openHistorian.Collections.Generic
{
    public abstract class TreeStream<TKey, TValue>
        where TKey : class, new()
        where TValue : class, new()
    {
        protected TreeStream()
            : this(new TKey(), new TValue())
        {
        }

        protected TreeStream(TKey key, TValue value)
        {
            CurrentKey = key;
            CurrentValue = value;
        }

        public bool IsValid
        {
            get;
            protected set;
        }

        public TKey CurrentKey
        {
            get;
            private set;
        }

        public TValue CurrentValue
        {
            get;
            private set;
        }

        public abstract bool Read();

        public virtual void Cancel()
        {
        }

        protected void SetKeyValueReferences(TKey key, TValue value)
        {
            CurrentKey = key;
            CurrentValue = value;
        }
    }
}