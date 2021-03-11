using System;

namespace GSF.Snap.Tree
{
    public static class KeyValueStreamExtension
    {
        public static TreeStreamSequential<TKey, TValue> TestSequential<TKey, TValue>(this TreeStream<TKey, TValue> stream)
            where TKey : SnapTypeBase<TKey>, new()
            where TValue : SnapTypeBase<TValue>, new()
        {
            return new TreeStreamSequential<TKey, TValue>(stream);
        }

    }

    /// <summary>
    /// This class will throw exceptions if the bahavior of a KeyValueStream is not sequential.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TreeStreamSequential<TKey, TValue>
        : TreeStream<TKey, TValue>
        where TKey : SnapTypeBase<TKey>, new()
        where TValue : SnapTypeBase<TValue>, new()
    {

        bool m_isEndOfStream;
        private bool IsValid;
        private readonly TKey CurrentKey;
        private readonly TValue CurrentValue;

        private bool m_baseStreamIsValid;
        private readonly TKey m_baseStreamCurrentKey;
        private readonly TValue m_baseStreamCurrentValue;


        readonly TreeStream<TKey, TValue> m_baseStream;

        public TreeStreamSequential(TreeStream<TKey, TValue> baseStream)
        {
            m_isEndOfStream = false;
            m_baseStream = baseStream;
            IsValid = false;
            CurrentKey = new TKey();
            CurrentValue = new TValue();
            m_baseStreamCurrentKey = new TKey();
            m_baseStreamCurrentValue = new TValue();
            m_baseStreamIsValid = false;
        }

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        protected override bool ReadNext(TKey key, TValue value)
        {
            if (m_isEndOfStream)
            {
                if (m_baseStream.Read(key, value))
                    throw new Exception("Data exists past the end of the stream");
                if (m_baseStream.EOS)
                    throw new Exception("Should not be valid");
                return false;
            }

            m_baseStreamIsValid = m_baseStream.Read(m_baseStreamCurrentKey, m_baseStreamCurrentValue);
            if (m_baseStreamIsValid)
            {
                if (!m_baseStreamIsValid)
                    throw new Exception("Should be valid");
                if (IsValid)
                    if (CurrentKey.IsGreaterThanOrEqualTo(m_baseStreamCurrentKey))// CurrentKey.IsGreaterThanOrEqualTo(m_baseStream.CurrentKey))
                        throw new Exception("Stream is not sequential");

                IsValid = true;
                m_baseStreamCurrentKey.CopyTo(CurrentKey); //m_baseStream.CurrentKey.CopyTo(CurrentKey);
                m_baseStreamCurrentValue.CopyTo(CurrentValue); // m_baseStream.CurrentValue.CopyTo(CurrentValue);
                return true;
            }
            else
            {
                m_baseStreamIsValid = m_baseStream.Read(m_baseStreamCurrentKey, m_baseStreamCurrentValue);
                if (m_baseStreamIsValid)
                    throw new Exception("Data exists past the end of the stream");
                if (m_baseStreamIsValid)
                    throw new Exception("Should not be valid");

                m_isEndOfStream = true;
                IsValid = false;
                return false;
            }
        }
    }
}
