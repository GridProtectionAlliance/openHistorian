//*******************************************************************************************************
//  IDataCache.cs - Gbtc
//
//  Tennessee Valley Authority, 2011
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  12/30/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Collections.Generic;
using openHistorian.Adapters;

namespace openHistorian
{
    public interface IDataCache : IDisposable
    {
        #region [ Properties ]

        IList<IData> Data 
        { 
            get; 
        }

        #endregion

        #region [ Methods ]

        IList<IData> Filter(IList<DataMapping> mappings);

        #endregion
    }
}
