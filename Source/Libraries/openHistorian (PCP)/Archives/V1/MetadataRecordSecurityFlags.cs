//******************************************************************************************************
//  MetadataRecordSecurityFlags.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/06/2007 - Pinal C. Patel
//       Generated original version of code based on DatAWare system specifications by Brian B. Fox, TVA.
//  04/20/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using TVA;
namespace openHistorian.Archives.V1
{
    /// <summary>
    /// Defines the security level for a <see cref="MetadataRecord"/>.
    /// </summary>
    /// <seealso cref="MetadataRecord"/>
    public class MetadataRecordSecurityFlags
    {
        #region [ Members ]

        // Constants
        private const Bits RecordSecurityMask = Bits.Bit00 | Bits.Bit01 | Bits.Bit02;
        private const Bits AccessSecurityMask = Bits.Bit03 | Bits.Bit04 | Bits.Bit05;

        // Fields
        private int m_value;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the access level required for a user to edit the <see cref="MetadataRecord"/>.
        /// </summary>
        public int ChangeSecurity
        {
            get
            {
                return m_value.GetMaskedValue(RecordSecurityMask);
            }
            set
            {
                m_value = m_value.SetMaskedValue(RecordSecurityMask, value);
            }
        }

        /// <summary>
        /// Gets or sets the access level required for a user to retrieve archived data associated to a <see cref="MetadataRecord"/>.
        /// </summary>
        public int AccessSecurity
        {
            get
            {
                return m_value.GetMaskedValue(AccessSecurityMask) >> 3; // <- 1st 3 bits are record security.
            }
            set
            {
                m_value = m_value.SetMaskedValue(AccessSecurityMask, value << 3);
            }
        }

        /// <summary>
        /// Gets or sets the 32-bit integer value used for defining the security level for a <see cref="MetadataRecord"/>.
        /// </summary>
        public int Value
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

        #endregion
    }
}
