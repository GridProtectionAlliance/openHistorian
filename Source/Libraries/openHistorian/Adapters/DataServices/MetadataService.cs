//******************************************************************************************************
//  MetadataService.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  08/28/2009 - Pinal C. Patel
//       Generated original version of source code.
//  09/10/2009 - Pinal C. Patel
//       Modified ReadMetadata() overloads to remove try-catch and check for null reference instead.
//  09/15/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/15/2009 - Pinal C. Patel
//       Changed the default port for the service from 5151 to 6151.
//  03/30/2010 - Pinal C. Patel
//       Added check to verify that the service is enabled before processing incoming requests.
//  11/07/2010 - Pinal C. Patel
//       Modified to fix breaking changes made to SelfHostingService.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using openHistorian.Archives.V1;
using TVA.Parsing;

namespace openHistorian.DataServices
{
    /// <summary>
    /// Represents a REST web service for historian metadata.
    /// </summary>
    /// <seealso cref="SerializableMetadata"/>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MetadataService : DataService, IMetadataService
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataService"/> class.
        /// </summary>
        public MetadataService()
            :base()
        {
            Endpoints = "http.rest://localhost:6151/rest; http.soap11://localhost:6151/soap";
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Writes received <paramref name="metadata"/> to the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="metadata">An <see cref="SerializableMetadata"/> object.</param>
        public void WriteMetadata(SerializableMetadata metadata)
        {
            try
            {
                // Abort if services is not enabled.
                if (!Enabled)
                    return;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Write all metadata records to the archive.
                foreach (SerializableMetadataRecord record in metadata.MetadataRecords)
                {
                    Archive.WriteMetaData(record.Key, record.Deflate().BinaryImage());
                }
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        /// <summary>
        /// Reads all metadata from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadAllMetadata()
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read all metadata records from the archive.
                int id = 0;
                byte[] buffer = null;
                SerializableMetadata metadata = new SerializableMetadata();
                List<SerializableMetadataRecord> records = new List<SerializableMetadataRecord>();
                while (true)
                {
                    buffer = Archive.ReadMetaData(++id);
                    if (buffer == null)
                        // No more records.
                        break;
                    else
                        // Add to resultset.
                        records.Add(new SerializableMetadataRecord(new MetadataRecord(id, buffer, 0, buffer.Length)));
                }
                metadata.MetadataRecords = records.ToArray();

                return metadata;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="idList">A comma or semi-colon delimited list of IDs for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadSelectMetadata(string idList)
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read specified metadata records from the archive.
                int id = 0;
                byte[] buffer = null;
                SerializableMetadata metadata = new SerializableMetadata();
                List<SerializableMetadataRecord> records = new List<SerializableMetadataRecord>();
                foreach (string singleID in idList.Split(',', ';'))
                {
                    buffer = Archive.ReadMetaData(id = int.Parse(singleID));
                    if (buffer == null)
                        // ID is invalid.
                        continue;
                    else
                        // Add to resultset.
                        records.Add(new SerializableMetadataRecord(new MetadataRecord(id, buffer, 0, buffer.Length)));
                }
                metadata.MetadataRecords = records.ToArray();

                return metadata;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        /// <summary>
        /// Reads a subset of metadata from the <see cref="DataService.Archive"/>.
        /// </summary>
        /// <param name="fromID">Starting ID in the ID range for which metadata is to be read.</param>
        /// <param name="toID">Ending ID in the ID range for which metadata is to be read.</param>
        /// <returns>An <see cref="SerializableMetadata"/> object.</returns>
        public SerializableMetadata ReadRangeMetadata(string fromID, string toID)
        {
            try
            {
                // Support JSON response.
                SupportJson();

                // Abort if services is not enabled.
                if (!Enabled)
                    return null;

                // Ensure that data archive is available.
                if (Archive == null)
                    throw new ArgumentNullException("Archive");

                // Read specified metadata records from the archive.
                byte[] buffer = null;
                SerializableMetadata metadata = new SerializableMetadata();
                List<SerializableMetadataRecord> records = new List<SerializableMetadataRecord>();
                for (int id = int.Parse(fromID); id <= int.Parse(toID); id++)
                {
                    buffer = Archive.ReadMetaData(id);
                    if (buffer == null)
                        // ID is invalid.
                        continue;
                    else
                        // Add to resultset.
                        records.Add(new SerializableMetadataRecord(new MetadataRecord(id, buffer, 0, buffer.Length)));
                }
                metadata.MetadataRecords = records.ToArray();

                return metadata;
            }
            catch (Exception ex)
            {
                // Notify about the encountered processing exception.
                OnServiceProcessException(ex);
                throw;
            }
        }

        private void SupportJson()
        {
            // Check if requestor accepts response in JSON format.
            string acceptHeader = ((HttpRequestMessageProperty)OperationContext.Current.RequestContext.RequestMessage.Properties[HttpRequestMessageProperty.Name]).Headers["Accept"];
            if (!string.IsNullOrEmpty(acceptHeader) && acceptHeader.Contains("application/json"))
                WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
        }

        #endregion
    }
}
