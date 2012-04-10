//*******************************************************************************************************
//  DataCache.cs - Gbtc
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
using openHistorian.Archives;
using TVA;

namespace openHistorian
{
    public class DataCache : IDataCache
    {
        #region [ Members ]

        // Fields
        private List<IData> m_data;
        private IDataArchive m_archive;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        internal DataCache(IDataArchive archive)
        {
            m_data = new List<IData>();
            m_archive = archive;
            m_archive.DataReceived += Archive_DataReceived;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="DataCache"/> is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~DataCache()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        public IList<IData> Data
        {
            get
            {
                return m_data.AsReadOnly();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="DataCache"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IList<IData> Filter(IList<DataMapping> mappings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DataCache"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.

                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        if (m_archive != null)
                        {
                            m_archive.DataReceived -= Archive_DataReceived;
                        }
                        m_archive = null;
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        private void Archive_DataReceived(object sender, EventArgs<IEnumerable<IData>> e)
        {
            int key;
            lock (m_data)
            {
                foreach (IData item in e.Argument)
                {
                    key = item.Key;
                    if (key > m_data.Count)
                    {
                        // No data exists for the key, so add one for it and others in-between.
                        for (int i = m_data.Count + 1; i <= key; i++)
                        {
                            m_data.Add(new Data(i));
                        }
                    }

                    // Replace existing data with the new data.
                    if (item.Time >= m_data[key - 1].Time)
                        m_data[key - 1] = item;
                }
            }
        }

        #endregion
    }
}
