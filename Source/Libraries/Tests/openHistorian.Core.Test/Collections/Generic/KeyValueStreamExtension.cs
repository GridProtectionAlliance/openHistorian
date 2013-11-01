using System;
using openHistorian.Collections;
using openHistorian.Collections.Generic;

namespace openHistorian.Collections.Generic
{
    public static class KeyValueStreamExtension
    {
        public static KeyValueStreamSequential<TKey, TValue> TestSequential<TKey, TValue>(this KeyValueStream<TKey, TValue> stream)
            where TKey : class, ISortedTreeKey<TKey>, new()
            where TValue : class, ISortedTreeValue<TValue>, new()
        {
            return new KeyValueStreamSequential<TKey, TValue>(stream);
        }

    }

    /// <summary>
    /// This class will throw exceptions if the bahavior of a KeyValueStream is not correct.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyValueStreamSequential<TKey, TValue>
        : KeyValueStream<TKey, TValue>
        where TKey : class, ISortedTreeKey<TKey>, new()
        where TValue : class, ISortedTreeValue<TValue>, new()
    {

        bool m_isEndOfStream;
        KeyValueStream<TKey, TValue> m_baseStream;
        TreeKeyMethodsBase<TKey> m_keyMethods;
        TreeValueMethodsBase<TValue> m_valueMethods;

        public KeyValueStreamSequential(KeyValueStream<TKey, TValue> baseStream)
        {
            m_keyMethods = new TKey().CreateKeyMethods();
            m_valueMethods = new TValue().CreateValueMethods();
            m_isEndOfStream = false;
            m_baseStream = baseStream;
            IsValid = false;
        }

        /// <summary>
        /// Advances the stream to the next value. 
        /// If before the beginning of the stream, advances to the first value
        /// </summary>
        /// <returns>True if the advance was successful. False if the end of the stream was reached.</returns>
        public override bool Read()
        {
            if (m_isEndOfStream)
            {
                if (m_baseStream.Read())
                    throw new Exception("Data exists past the end of the stream");
                if (m_baseStream.IsValid)
                    throw new Exception("Should not be valid");
                return false;
            }

            if (m_baseStream.Read())
            {
                if (!m_baseStream.IsValid)
                    throw new Exception("Should be valid");
                if (IsValid)
                    if (m_keyMethods.IsGreaterThanOrEqualTo(CurrentKey,m_baseStream.CurrentKey))// CurrentKey.IsGreaterThanOrEqualTo(m_baseStream.CurrentKey))
                        throw new Exception("Stream is not sequential");

                IsValid = true;
                m_keyMethods.Copy(m_baseStream.CurrentKey, CurrentKey); //m_baseStream.CurrentKey.CopyTo(CurrentKey);
                m_valueMethods.Copy(m_baseStream.CurrentValue, CurrentValue); // m_baseStream.CurrentValue.CopyTo(CurrentValue);
                return true;
            }
            else
            {
                if (m_baseStream.Read())
                    throw new Exception("Data exists past the end of the stream");
                if (m_baseStream.IsValid)
                    throw new Exception("Should not be valid");

                m_isEndOfStream = true;
                IsValid = false;
                return false;
            }
        }
    }
}
