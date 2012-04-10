//*******************************************************************************************************
//  IMetadata.cs - Gbtc
//
//  Tennessee Valley Authority, 2012
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  01/27/2012 - Pinal C. Patel
//       Generated original version of source code.
//
//*******************************************************************************************************

using TVA.Parsing;

namespace openHistorian.Archives.V1
{
    public interface IMetadata : ISupportBinaryImage
    {
        DataKey Key { get; set; }

        string Name { get; set; }

        string Synonym1 { get; set; }
        
        string Synonym2 { get; set; }
        
        string Description { get; set; }

        int SourceID { get; set; }

        float ScanRate { get; set; }

        int CompressionMaxTime { get; set; }

        int CompressionMinTime { get; set; }
    }
}
