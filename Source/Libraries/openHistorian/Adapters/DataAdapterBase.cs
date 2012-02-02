//******************************************************************************************************
//  Adapter.cs - Gbtc
//
//  Tennessee Valley Authority
//  No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  04/05/2011 - Pinal C. Patel
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;
using openHistorian.Archives;
using TVA.Adapters;

namespace openHistorian.Adapters
{
    /// <summary>
    /// Base class for an <see cref="IDataAdapter"/>.
    /// </summary>
    /// <seealso cref="IData"/>
    /// <seealso cref="IDataAdapter"/>
    public abstract class DataAdapterBase : Adapter, IDataAdapter
    {
        #region [ Members ]

        // Fields
        private List<DataMapping> m_mappings;
        private IList<IDataArchive> m_archives;
        private DataAdapterState m_state;
        private bool m_disposed;
        private bool m_initialized;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="IDataAdapter"/>.
        /// </summary>
        protected DataAdapterBase()
        {
            m_mappings = new List<DataMapping>();
            m_archives = new List<IDataArchive>();
        }

        #endregion

        #region [ Properties ]

        public virtual List<DataMapping> Mappings
        {
            get
            {
                return m_mappings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_mappings = value;
            }
        }

        /// <summary>
        /// Gets or sets a list of all loaded <see cref="IDataArchive"/>s.
        /// </summary>
        [XmlIgnore()]
        public virtual IList<IDataArchive> Archives
        {
            get
            {
                return m_archives;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_archives = value;
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="DataAdapterState"/> of the <see cref="IDataAdapter"/>.
        /// </summary>
        [XmlIgnore()]
        public virtual DataAdapterState State
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
        /// Gets or sets a boolean value that indicates whether the <see cref="IDataAdapter"/> is currently enabled.
        /// </summary>
        [XmlIgnore()]
        public override bool Enabled
        {
            get
            {
                return m_state == DataAdapterState.Started;
            }
            set
            {
                if (value && m_state == DataAdapterState.Stopped)
                    Start();
                else if (!value && m_state != DataAdapterState.Stopped)
                    Stop();
            }
        }

        #endregion

        #region [ Methods ]

        #region [ Abstract ]

        /// <summary>
        /// When overridden in a derived class, starts the <see cref="IDataAdapter"/>.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// When overridden in a derived class, Stops the <see cref="IDataAdapter"/>.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// When overridden in a derived class, saves reference to the added <paramref name="archive"/> in needed.
        /// </summary>
        /// <param name="archive"><see cref="IDataArchive"/> that was added.</param>
        protected abstract void OnArchiveAdded(IDataArchive archive);

        /// <summary>
        /// When overridden in a derived class, removes saved reference to the removed <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive"><see cref="IDataArchive"/> that was removed.</param>
        protected abstract void OnArchiveRemoved(IDataArchive archive);

        #endregion

        /// <summary>
        /// Initializes the <see cref="IDataAdapter"/>.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            if (!m_initialized)
            {
                // Register for changes to loaded archives.
                if (m_archives != null)
                    ((INotifyCollectionChanged)m_archives).CollectionChanged += Archives_CollectionChanged;

                m_initialized = true;
            }
        }

        /// <summary>
        /// Finds the source <see cref="IDataArchives"/>s for the <see cref="IDataAdapter"/>.
        /// </summary>
        /// <param name="minRequiredArchives">Minumum number of source <see cref="IDataArchive"/>s required by the <see cref="IDataAdapter"/>.</param>
        /// <param name="maxSupportedArchives">Maximum number of source <see cref="IDataArchive"/>s supported by the <see cref="IDataAdapter"/>.</param>
        /// <returns><see cref="Dictionary{string, IDataArchive}"/> with one item for each distinct source <see cref="IDataArchive"/>.</returns>
        protected virtual IDictionary<string, IDataArchive> FindSourceArchives(int minRequiredArchives, int maxSupportedArchives)
        {
            // Make sure Mapping property is initialized.
            if (m_mappings == null)
                throw new ArgumentNullException("Mappings property cannot be null");

            // Find distinct archives for the sources specified in the mappings.
            var sources = (from m in m_mappings
                           select ((DataKey)m.Source).Instance).Distinct().ToList();

            // Make sure we have the minimum number of source archives.
            if (sources.Count < minRequiredArchives)
                throw new ArgumentOutOfRangeException(string.Format("At least {0} source archives must be specified in data mappings", minRequiredArchives));

            // Make sure we don't exceed the maximum number of source archives.
            if (sources.Count > maxSupportedArchives)
                throw new ArgumentOutOfRangeException(string.Format("No more than {0} source archives can be specified in data mappings", maxSupportedArchives));

            return FindArchives(sources);
        }

        /// <summary>
        /// Finds the target <see cref="IDataArchives"/>s for the <see cref="IDataAdapter"/>.
        /// </summary>
        /// <param name="minRequiredArchives">Minumum number of target <see cref="IDataArchive"/>s required by the <see cref="IDataAdapter"/>.</param>
        /// <param name="maxSupportedArchives">Maximum number of target <see cref="IDataArchive"/>s supported by the <see cref="IDataAdapter"/>.</param>
        /// <returns><see cref="Dictionary{string, IDataArchive}"/> with one item for each distinct target <see cref="IDataArchive"/>.</returns>
        protected virtual IDictionary<string, IDataArchive> FindTargetArchives(int minRequiredArchives, int maxSupportedArchives)
        {
            // Make sure Mapping property is initialized.
            if (m_mappings == null)
                throw new ArgumentNullException("Mappings property cannot be null");

            // Find distinct archives for the targets specified in the mappings.
            var targets = (from m in m_mappings
                           select ((DataKey)m.Target).Instance).Distinct().ToList();

            // Make sure we have the minimum number of target archives.
            if (targets.Count < minRequiredArchives)
                throw new ArgumentOutOfRangeException(string.Format("At least {0} target archives must be specified in data mappings", minRequiredArchives));

            // Make sure we don't exceed the maximum number of target archives.
            if (targets.Count > maxSupportedArchives)
                throw new ArgumentOutOfRangeException(string.Format("No more than {0} target archives can be specified in data mappings", maxSupportedArchives));

            return FindArchives(targets);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="IDataAdapter"/> and optionally releases the managed resources.
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
                        Stop();

                        if (Archives != null)
                            ((INotifyCollectionChanged)Archives).CollectionChanged -= Archives_CollectionChanged;
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        private IDictionary<string, IDataArchive> FindArchives(IList<string> archiveNames)
        {
            Dictionary<string, IDataArchive> archives = new Dictionary<string, IDataArchive>();
            if (m_mappings != null && m_archives != null)
            {
                lock (m_archives)
                {
                    foreach (string name in archiveNames)
                    {
                        archives.Add(name, Archives.FirstOrDefault(archive => string.Compare(name, archive.Name) == 0));
                    }
                }
            }

            return archives;
        }

        private void Archives_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                // Notify of added archive.
                OnArchiveAdded((IDataArchive)e.NewItems[0]);
            else if (e.Action == NotifyCollectionChangedAction.Remove)
                // Notify of removed data archive.
                OnArchiveRemoved((IDataArchive)e.NewItems[0]);
        }

        #endregion
    }
}
