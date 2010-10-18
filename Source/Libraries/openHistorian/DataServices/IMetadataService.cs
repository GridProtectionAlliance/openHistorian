//******************************************************************************************************
//  IMetadataService.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/28/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/11/2010 - Mihir Brahmbhatt
//       Updated header and license agreement.
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
        /// Writes <paramref name="metadata"/> received in <see cref="WebMessageFormat.Xml"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="metadata">An <see cref="SerializableMetadata"/> object.</param>
        [OperationContract(), 
        WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Xml, UriTemplate = "/metadata/write/xml")]
        void WriteMetadataAsXml(SerializableMetadata metadata);

        /// <summary>
        /// Writes <paramref name="metadata"/> received in <see cref="WebMessageFormat.Json"/> format to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="metadata">An <see cref="SerializableMetadata"/> object.</param>
        [OperationContract(), 
        WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, UriTemplate = "/metadata/write/json")]
        void WriteMetadataAsJson(SerializableMetadata metadata);

        /// <summary>
        /// Reads all metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/metadata/read/xml")]
        SerializableMetadata ReadAllMetadataAsXml();

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/metadata/read/{idList}/xml")]
        SerializableMetadata ReadSelectMetadataAsXml(string idList);

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Xml"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which metadata is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/metadata/read/{fromID}-{toID}/xml")]
        SerializableMetadata ReadRangeMetadataAsXml(string fromID, string toID);

        /// <summary>
        /// Reads all metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/metadata/read/json")]
        SerializableMetadata ReadAllMetadataAsJson();

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/metadata/read/{idList}/json")]
        SerializableMetadata ReadSelectMetadataAsJson(string idList);

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/> and sends it in <see cref="WebMessageFormat.Json"/> format.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which metadata is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        [OperationContract(), 
        WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/metadata/read/{fromID}-{toID}/json")]
        SerializableMetadata ReadRangeMetadataAsJson(string fromID, string toID);

        #endregion
    }
}
