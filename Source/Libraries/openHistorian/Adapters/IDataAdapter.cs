//******************************************************************************************************
//  IAdapter.cs - Gbtc
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

using System.Collections.Generic;
using openHistorian.Archives;
using TVA.Adapters;

namespace openHistorian.Adapters
{
    #region [ Enumerations ]

    /// <summary>
    /// Indicates the current state of the <see cref="IDataAdapter"/>.
    /// </summary>
    public enum DataAdapterState
    {
        /// <summary>
        /// <see cref="IDataAdapter"/> is stopped.
        /// </summary>
        Stopped = 0,
        /// <summary>
        /// <see cref="IDataAdapter"/> is being stopped.
        /// </summary>
        Stopping = 1,
        /// <summary>
        /// <see cref="IDataAdapter"/> is running.
        /// </summary>
        Started = 2,
        /// <summary>
        /// <see cref="IDataAdapter"/> is being started.
        /// </summary>
        Starting = 3
    }

    #endregion

    /// <summary>
    /// Defines an <see cref="IAdapter"/> that interacts with <see cref="IData"/> in <see cref="IDataArchive"/>s.
    /// </summary>
    /// <seealso cref="IData"/>
    /// <seealso cref="IDataArchive"/>
    public interface IDataAdapter : IAdapter
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a list of all loaded <see cref="IDataArchive"/>s.
        /// </summary>
        IList<IDataArchive> Archives
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current state of the <see cref="IDataAdapter"/>.
        /// </summary>
        DataAdapterState State
        {
            get;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Starts the <see cref="IDataAdapter"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the <see cref="IDataAdapter"/>.
        /// </summary>
        void Stop();

        #endregion
    }
}
