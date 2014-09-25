//******************************************************************************************************
//  LogSourceBase.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  ----------------------------------------------------------------------------------------------------
//  05/24/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Diagnostics
{
    /// <summary>
    /// A base class that assists in logging.
    /// </summary>
    public abstract class LogSourceBase : ILogSourceDetails, IDisposable
    {
        private bool m_disposed;

        /// <summary>
        /// The <see cref="LogSource"/> for logging messages.
        /// </summary>
        protected LogSource Log { get; private set; }

        /// <summary>
        /// Creates a <see cref="LogSourceBase"/>
        /// </summary>
        protected LogSourceBase()
        {
            Log = Logger.CreateSource(this);
        }

        /// <summary>
        /// Creates a <see cref="LogSourceBase"/>
        /// </summary>
        /// <param name="parent">The parent source. If null, uses <see cref="Logger.RootSource"/> to register without a parent.</param>
        protected LogSourceBase(LogSource parent)
        {
            if (parent != null)
            {
                Log = Logger.CreateSource(this, parent, null);
            }
            else
            {
                Log = Logger.CreateSource(this, null, null);
            }
        }

        /// <summary>
        /// Creates a <see cref="LogSourceBase"/>
        /// </summary>
        /// <param name="parent">The parent source. If null, uses <see cref="Logger.RootSource"/> to register without a parent.</param>
        protected LogSourceBase(LogSourceBase parent)
        {
            if (parent != null)
            {
                Log = Logger.CreateSource(this, parent.Log, null);
            }
            else
            {
                Log = Logger.CreateSource(this, null, null);
            }
        }

        string ILogSourceDetails.GetSourceDetails()
        {
            return GetSourceDetails();
        }

#if DEBUG
        //This code is here to detect when finalizers are called rather than a class be properly disposed. 
        //Since Finalizers are expensive, this code is commented 
        ~LogSourceBase()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Error, "Finalize Threw An Exception", null, null, ex);
                throw;
            }
        }
#endif
        /// <summary>
        /// Gets any details specific to the source.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSourceDetails()
        {
            return string.Empty;
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="LogSourceBase"/> object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                Log.Publish(VerboseLevel.Error, "Dispose Threw An Exception", null, null, ex);
                throw;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="LogSourceBase"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (Log.ShouldPublishDebugNormal)
                            Log.Publish(VerboseLevel.DebugNormal, "Object Disposed");
                    }
                    else
                    {
                        if (Log.ShouldPublishInfo)
                            Log.Publish(VerboseLevel.Information, "Object Finalized");
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

    }
}
