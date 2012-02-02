//*******************************************************************************************************
//  DataKey.cs - Gbtc
//
//  Tennessee Valley Authority, 2011
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/15/2011 - Pinal C. Patel
//       Generated original version of source code.
//  01/12/2012 - Pinal C. Patel
//       Updated implicit conversion from string to DataKey.
//
//*******************************************************************************************************

using System;
using System.Globalization;
using openHistorian.Archives;

namespace openHistorian
{
    /// <summary>
    /// Represents the primary key for an <see cref="IData"/>.
    /// </summary>
    /// <seealso cref="IData"/>
    public class DataKey : IComparable, IFormattable
    {
        #region [ Members ]

        // Fields
        private int m_id;
        private string m_instance;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DataKey"/> class.
        /// </summary>
        /// <param name="id">Numeric identifier of the <see cref="IData"/>.</param>
        public DataKey(int id)
            : this(id, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataKey"/> class.
        /// </summary>
        /// <param name="id">Numeric identifier of the <see cref="IData"/>.</param>
        /// <param name="instance"><see cref="IDataArchive"/> instance name where the <see cref="IData"/> is stored.</param>
        public DataKey(int id, string instance)
        {
            Id = id;
            Instance = instance;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the numeric identifier of the <see cref="IData"/> that this <see cref="DataKey"/> represents.
        /// </summary>
        /// <exception cref="ArgumentException">The value being assigned is not a positive number or -1.</exception>
        public int Id
        {
            get
            {
                return m_id;
            }
            set
            {
                if (value < 1 && value != -1)
                    throw new ArgumentException("Value must be a positive number");

                m_id = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDataArchive"/> instance name where the <see cref="IData"/> represented by this <see cref="DataKey"/> is stored.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value being assigned is null.</exception>
        public string Instance
        {
            get
            {
                return m_instance.ToUpper();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_instance = value;
            }
        }

        #endregion

        #region [ Methods ]

        public int CompareTo(object obj)
        {
            DataKey other = obj as DataKey;
            if (object.Equals(other, null))
            {
                return 1;
            }
            else
            {
                int result = string.Compare(m_instance, other.Instance, true);
                if (result != 0)
                    return result;
                else
                    return m_id.CompareTo(other.Id);
            }
        }

        public override bool Equals(object obj)
        {
            return (CompareTo(obj) == 0);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            switch (format)
            {
                case "I":
                    return m_instance.ToString(formatProvider);
                case "D":
                    return m_id.ToString(formatProvider);
                default:
                    if (string.IsNullOrEmpty(m_instance))
                        return m_id.ToString();
                    else
                        return string.Format("{0}:{1}", m_instance, m_id);
            }
        }

        #endregion

        #region [ Operators ]

        #region [ Comparision Operators ]

        public static bool operator ==(DataKey value1, DataKey value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(DataKey value1, DataKey value2)
        {
            return !value1.Equals(value2);
        }

        public static bool operator <(DataKey value1, DataKey value2)
        {
            return (value1.CompareTo(value2) < 0);
        }

        public static bool operator <=(DataKey value1, DataKey value2)
        {
            return (value1.CompareTo(value2) <= 0);
        }

        public static bool operator >(DataKey value1, DataKey value2)
        {
            return (value1.CompareTo(value2) > 0);
        }

        public static bool operator >=(DataKey value1, DataKey value2)
        {
            return (value1.CompareTo(value2) >= 0);
        }

        #endregion

        #region [ Type Conversion Operators ]

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to an <see cref="DataKey"/>.
        /// </summary>
        /// <param name="value"><see cref="int"/> value to be converted.</param>
        /// <returns>An <see cref="DataKey"/> object.</returns>
        public static implicit operator DataKey(int value)
        {
            return new DataKey(value);
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to an <see cref="DataKey"/>.
        /// </summary>
        /// <param name="value"><see cref="string"/> value to be converted.</param>
        /// <returns>An <see cref="DataKey"/> object.</returns>
        public static implicit operator DataKey(string value)
        {
            int id;
            string[] valueParts = value.Split(':');
            if (valueParts.Length == 1)
            {
                // Format - <Instance> | <Id>
                if (int.TryParse(valueParts[0], out id))
                    // Format - <Id>
                    return new DataKey(id);
                else
                    // Format - <Instance>
                    return new DataKey(-1, valueParts[0]);
            }
            else
            {
                // Format - <Instance>:<Id>
                if (int.TryParse(valueParts[1], out id))
                    // Format - <Instance>:<Id>
                    return new DataKey(id, valueParts[0]);
                else
                    // Format - <Instance>:<Not a valid Id>
                    return new DataKey(-1, valueParts[0]);
            }
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="value"><see cref="DataKey"/> object to be converted.</param>
        /// <returns>An <see cref="int"/> value.</returns>
        public static implicit operator int(DataKey value)
        {
            return value.Id;
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="value"><see cref="DataKey"/> object to be converted.</param>
        /// <returns>A <see cref="string"/> value.</returns>
        public static implicit operator string(DataKey value)
        {
            return value.ToString();
        }

        #endregion

        #endregion
    }
}
