//*******************************************************************************************************
//  Data.cs - Gbtc
//
//  Tennessee Valley Authority, 2011
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/15/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Globalization;
using openHistorian.Archives;

namespace openHistorian
{
    /// <summary>
    /// Represents an event stored in an <see cref="IDataArchive"/>.
    /// </summary>
    /// <seealso cref="IData"/>
    public class Data : IData, IComparable, IFormattable
    {
        #region [ Members ]

        // Fields
        private DataKey m_key;
        private TimeTag m_time;
        private float m_value;
        private Quality m_quality;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of <see cref="Data"/>.
        /// </summary>
        /// <param name="key"><see cref="DataKey"/> that identifies the <see cref="Data"/>.</param>
        public Data(DataKey key)
        {
            Key = key;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="DataKey"/> of the <see cref="Data"/>.
        /// </summary>
        public virtual DataKey Key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeTag"/> of the <see cref="Data"/>.
        /// </summary>
        public virtual TimeTag Time
        {
            get
            {
                return m_time;
            }
            set
            {
                m_time = value;
            }
        }

        /// <summary>
        /// Gets or sets the value <see cref="object"/> of the <see cref="Data"/>.
        /// </summary>
        public virtual float Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Quality"/> of the <see cref="Data"/>'s <see cref="Value"/>.
        /// </summary>
        public virtual Quality Quality
        {
            get
            {
                return m_quality;
            }
            set
            {
                m_quality = value;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="Data"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">Always</exception>
        public virtual int BinaryLength
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Initializes the <see cref="Data"/> from the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">Binary image to be used for initializing the <see cref="Data"/>.</param>
        /// <param name="startIndex">0-based starting index of initialization data in the <paramref name="buffer"/>.</param>
        /// <param name="length">Valid number of bytes in <paramref name="buffer"/> from <paramref name="startIndex"/>.</param>
        /// <returns>Number of bytes used from the <paramref name="buffer"/> for initializing the <see cref="Data"/>.</returns>
        public virtual int ParseBinaryImage(byte[] buffer, int startIndex, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Generates binary image of the <see cref="Data"/> and copies it into the given buffer, for <see cref="BinaryLength"/> bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to hold generated binary image of the source object.</param>
        /// <param name="startIndex">0-based starting index in the <paramref name="buffer"/> to start writing.</param>
        /// <returns>The number of bytes written to the <paramref name="buffer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <see cref="BinaryLength"/> is less than 0 -or- 
        /// <paramref name="startIndex"/> and <see cref="BinaryLength"/> will exceed <paramref name="buffer"/> length.
        /// </exception>
        public virtual int GenerateBinaryImage(byte[] buffer, int startIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Compares the current <see cref="Data"/> object to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="Data"/> object is to be compared.</param>
        /// <returns>
        /// Negative value if the current <see cref="Data"/> object is less than <paramref name="obj"/>, 
        /// Zero if the current <see cref="Data"/> object is equal to <paramref name="obj"/>, 
        /// Positive value if the current <see cref="Data"/> object is greater than <paramref name="obj"/>.
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            Data other = obj as Data;
            if (other == null)
            {
                return 1;
            }
            else
            {
                int result = Key.CompareTo(other.Key);
                if (result != 0)
                    return result;
                else
                    return Time.CompareTo(other.Time);
            }
        }

        /// <summary>
        /// Determines whether the current <see cref="Data"/> object is equal to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">Object against which the current <see cref="Data"/> object is to be compared for equality.</param>
        /// <returns>true if the current <see cref="Data"/> object is equal to <paramref name="obj"/>; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Returns the text representation of <see cref="Data"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/> value.</returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Returns the text representation of <see cref="Data"/> object in the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="format">Format of text output (I for ID, T for Time, V for Value, Q for Quality).</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public virtual string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns the text representation of <see cref="Data"/> object using the specified <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public virtual string ToString(IFormatProvider provider)
        {
            return ToString(null, provider);
        }

        /// <summary>
        /// Returns the text representation of <see cref="Data"/> object in the specified <paramref name="format"/> 
        /// using the specified <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="format">Format of text output (I for ID, T for Time, V for Value, Q for Quality).</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            switch (format)
            {
                case "I":
                    return Key.ToString(formatProvider);
                case "T":
                    return Time.ToString(formatProvider);
                case "V":
                    return Value.ToString(formatProvider);
                case "Q":
                    return Quality.ToString();
                default:
                    return string.Format("ID={0}; Time={1}; Value={2}; Quality={3}",
                                         Key.ToString(formatProvider), Time.ToString(formatProvider), Value.ToString(formatProvider), Quality.ToString());
            }
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Data"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer value.</returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion

        #region [ Static ]

        // Static Methods

        /// <summary>
        /// Gets a boolean value that indicates whether <see cref="Data"/> contains any data.
        /// </summary>
        public static bool IsEmpty(IData value)
        {
            return ((value.Time == TimeTag.MinValue) &&
                    (value.Value == default(float)) &&
                    (value.Quality == Quality.Unknown));
        }

        #endregion
    }
}
