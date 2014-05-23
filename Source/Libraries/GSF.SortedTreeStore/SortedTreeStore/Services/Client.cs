using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Tree;

namespace GSF.SortedTreeStore.Services
{
    public abstract class Client : IDisposable
    {
        private bool m_disposed;

        /// <summary>
        /// Gets the database that matches <see cref="databaseName"/>
        /// </summary>
        /// <param name="databaseName">the case insensitive name of the databse</param>
        /// <returns></returns>
        public abstract ClientDatabaseBase GetDatabase(string databaseName);

        /// <summary>
        /// Accesses <see cref="ClientDatabaseBase{TKey,TValue}"/> for the empty string database.
        /// </summary>
        /// <returns><see cref="ClientDatabaseBase{TKey,TValue}"/> for empty string database.</returns>
        public ClientDatabaseBase<TKey, TValue> GetDatabase<TKey, TValue>()
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return GetDatabase<TKey, TValue>(string.Empty);
        }

        /// <summary>
        /// Accesses <see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.
        /// </summary>
        /// <param name="databaseName">Name of database instance to access.</param>
        /// <returns><see cref="ClientDatabaseBase{TKey,TValue}"/> for given <paramref name="databaseName"/>.</returns>
        public ClientDatabaseBase<TKey, TValue> GetDatabase<TKey, TValue>(string databaseName)
            where TKey : SortedTreeTypeBase<TKey>, new()
            where TValue : SortedTreeTypeBase<TValue>, new()
        {
            return (ClientDatabaseBase<TKey, TValue>)GetDatabase(databaseName);
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="Client"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Client"/> object and optionally releases the managed resources.
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
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        #region [ Static ]

        /// <summary>
        /// Connects to a local <see cref="Server"/>.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Client Connect(Server host)
        {
            return new Server.Client(host);
        }

        public static Client Connect(string serverOrIp, int port, string password)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
