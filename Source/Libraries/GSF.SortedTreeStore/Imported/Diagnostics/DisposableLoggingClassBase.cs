//******************************************************************************************************
//  DisposableLoggingClassBase.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  10/24/2016 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace GSF.Diagnostics
{
    /// <summary>
    /// A helper class that implements <see cref="IDisposable"/> that will raise log messages
    /// when this class is not properly disposed of.
    /// </summary>
    public abstract class DisposableLoggingClassBase
        : IDisposable
    {
        private bool m_disposed;

        /// <summary>
        /// The <see cref="LogPublisher"/> for logging messages.
        /// </summary>
        protected LogPublisher Log { get; private set; }

        /// <summary>
        /// Creates a <see cref="DisposableLoggingClassBase"/>
        /// </summary>
        protected DisposableLoggingClassBase(MessageClass messageClassification)
        {
            Log = Logger.CreatePublisher(GetType(), messageClassification);
        }

        //This code is here to detect when finalizers are called rather than a class be properly disposed. 
        ~DisposableLoggingClassBase()
        {
            Log.Publish(MessageLevel.Warning, MessageFlags.UsageIssue, "Finalize call. Object not properly disposed.");
            try
            {
                Dispose(false);
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Error, MessageFlags.UsageIssue, "Finalize Threw An Exception", null, null, ex);
                throw;
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="DisposableLoggingClassBase"/> object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Error, MessageFlags.UsageIssue | MessageFlags.BugReport, "Dispose Threw An Exception", null, null, ex);
                throw;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DisposableLoggingClassBase"/> object and optionally releases the managed resources.
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
                        Log.Publish(MessageLevel.Debug, "Object Disposed");
                    }
                    else
                    {
                        Log.Publish(MessageLevel.Warning, MessageFlags.UsageIssue, "Object Finalized");
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        protected void CheckDisposed()
        {
            if (m_disposed)
                ThrowDisposed();
        }

        private void ThrowDisposed()
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

    }
}
