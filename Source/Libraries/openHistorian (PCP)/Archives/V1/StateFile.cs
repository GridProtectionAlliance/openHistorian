//******************************************************************************************************
//  StateFile.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/08/2007 - Pinal C. Patel
//       Generated original version of source code.
//  04/21/2009 - Pinal C. Patel
//       Converted to C#.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/16/2009 - Pinal C. Patel
//       Changed the default value for SettingsCategory property to the type name.
//
//******************************************************************************************************

using System.Drawing;
using TVA.IO;

namespace openHistorian.Archives.V1
{
    /// <summary>
    /// Represents a file containing <see cref="StateRecord"/>s.
    /// </summary>
    /// <seealso cref="StateRecord"/>
    [ToolboxBitmap(typeof(StateFile))]
    public class StateFile : IsamDataFileBase<StateRecord>
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StateFile"/> class.
        /// </summary>
        public StateFile()
            : base()
        {
            SettingsCategory = this.GetType().Name;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the binary size of a <see cref="StateRecord"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer.</returns>
        protected override int GetRecordSize()
        {
            return StateRecord.ByteCount;
        }

        /// <summary>
        /// Creates a new <see cref="StateRecord"/> with the specified <paramref name="recordIndex"/>.
        /// </summary>
        /// <param name="recordIndex">1-based index of the <see cref="StateRecord"/>.</param>
        /// <returns>A <see cref="StateRecord"/> object.</returns>
        protected override StateRecord CreateNewRecord(int recordIndex)
        {
            return new StateRecord(recordIndex);
        }

        #endregion
    }
}
