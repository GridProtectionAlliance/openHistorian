//******************************************************************************************************
//  IMetadataService.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/28/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//
//******************************************************************************************************

using System.ServiceModel;
using System.ServiceModel.Web;

namespace openHistorian.DataServices
{
    /// <summary>
    /// Defines a REST web service for historian metadata.
    /// </summary>
    /// <seealso cref="SerializableMetadata"/>
    [ServiceContract()]
    public interface IMetadataService
    {
        #region [ Methods ]

        /// <summary>
        /// Writes received <paramref name="metadata"/> to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="metadata">An <see cref="SerializableMetadata"/> object.</param>
        [OperationContract(), 
        WebInvoke(Method = "POST", UriTemplate = "/metadata/write")]
        void WriteMetadata(SerializableMetadata metadata);

        /// <summary>
        /// Reads all metadata from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/metadata/read")]
        SerializableMetadata ReadAllMetadata();

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/metadata/read/{idList}")]
        SerializableMetadata ReadSelectMetadata(string idList);

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which metadata is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(UriTemplate = "/metadata/read/{fromID}-{toID}")]
        SerializableMetadata ReadRangeMetadata(string fromID, string toID);

        #endregion
    }
}
