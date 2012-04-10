//*******************************************************************************************************
//  DataArchiveBase.cs - Gbtc
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
using System.Xml.Serialization;
using TVA;
using TVA.Adapters;

namespace openHistorian.Archives
{
    public abstract class DataArchiveBase : Adapter, IDataArchive
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Occurs when new <see cref="IData"/> is received by the <see cref="IDataArchive"/>.
        /// </summary>
        public event EventHandler<EventArgs<IEnumerable<IData>>> DataReceived;

        // Fields
        private DataArchiveState m_state;
        private DataCache m_cache;

        #endregion

        #region [ Constructors ]

        protected DataArchiveBase()
        {
            m_cache = new DataCache(this);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the current <see cref="DataArchiveState"/> of the <see cref="IDataArchive"/>.
        /// </summary>
        [XmlIgnore()]
        public virtual DataArchiveState State
        {
            get 
            {
                return m_state;
            }
            protected set
            {
                m_state = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether the <see cref="IDataArchive"/> is currently enabled.
        /// </summary>
        [XmlIgnore()]
        public override bool Enabled
        {
            get
            {
                return m_state == DataArchiveState.Open;
            }
            set
            {
                if (value && m_state == DataArchiveState.Closed)
                    Open(true);
                else if (!value && m_state != DataArchiveState.Closed)
                    Close(true);
            }
        }

        /// <summary>
        /// Gets the <see cref="IDataCache"/> for the <see cref="IDataArchive"/>.
        /// </summary>
        public IDataCache Cache
        {
            get 
            { 
                return m_cache; 
            }
        }

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        public abstract void Open(bool openDependencies);

        public abstract void Close(bool closeDependencies);

        public abstract void WriteData(IData data);

        public abstract void WriteMetaData(int key, byte[] metaData);

        public abstract void WriteStateData(int key, byte[] stateData);

        public abstract IEnumerable<IData> ReadData(int key, string startTime, string endTime);

        public abstract byte[] ReadMetaData(int key);

        public abstract byte[] ReadStateData(int key);

        public abstract byte[] ReadMetaDataSummary(int key);

        public abstract byte[] ReadStateDataSummary(int key);

        #endregion

        /// <summary>
        /// Raises the <see cref="DataReceived"/> event.
        /// </summary>
        /// <param name="data"><see cref="IData"/> to send to <see cref="DataReceived"/> event.</param>
        protected void OnDataReceived(IEnumerable<IData> data)
        {
            if (DataReceived != null)
                DataReceived(this, new EventArgs<IEnumerable<IData>>(data));
        }

        private bool m_disposed;

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DataArchiveBase"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                        if (m_cache != null)
                        {
                            m_cache.Dispose();
                        }
                        m_cache = null;
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        #endregion

    }
}
