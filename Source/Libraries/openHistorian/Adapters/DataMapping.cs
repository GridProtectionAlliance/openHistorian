//*******************************************************************************************************
//  DataMapping.cs - Gbtc
//
//  Tennessee Valley Authority, 2012
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  01/17/2012 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

namespace openHistorian.Adapters
{
    public class DataMapping : IDataMapping
    {
        public DataMapping()
            : this(string.Empty, string.Empty)
        {
        }

        public DataMapping(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public string Source { get; set; }

        public string Target { get; set; }
    }
}
