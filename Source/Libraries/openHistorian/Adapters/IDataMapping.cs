//*******************************************************************************************************
//  IDataMapping.cs - Gbtc
//
//  Tennessee Valley Authority, 2012
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  01/19/2012 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

namespace openHistorian.Adapters
{
    public interface IDataMapping
    {
        string Source { get; set; }
        string Target { get; set; }
    }
}
