//******************************************************************************************************
//  BinaryStreamIoSessionBase.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
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
//  4/26/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;

namespace GSF.IO.Unmanaged
{
    public enum IoSessionStatusChanged
    {
        Cleared,
        Disposed
    }

    public class IoSessionStatusChangedEventArgs : EventArgs
    {
        public IoSessionStatusChangedEventArgs(IoSessionStatusChanged status)
        {
            Status = status;
        }

        public IoSessionStatusChanged Status
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Implementing this interface allows a binary stream to be attached to a buffer.
    /// </summary>
    public abstract class BinaryStreamIoSessionBase : IDisposable
    {
        /// <summary>
        /// Gets if the object has been disposed
        /// </summary>
        public bool IsDisposed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Occurs when the status of the ioSession changes. 
        /// This will be used to notify higher layers that they need to dereference any blocks requested.
        /// </summary>
        public event EventHandler<IoSessionStatusChangedEventArgs> BaseStatusChanged;

        protected void OnBaseStatusChanged(IoSessionStatusChangedEventArgs e)
        {
            EventHandler<IoSessionStatusChangedEventArgs> handler = BaseStatusChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets a block for the following Io session.
        /// </summary>
        public abstract void GetBlock(BlockArguments args);

        /// <summary>
        /// Sets the current usage of the <see cref="BinaryStreamIoSessionBase"/> to null.
        /// </summary>
        public virtual void Clear()
        {
            OnBaseStatusChanged(new IoSessionStatusChangedEventArgs(IoSessionStatusChanged.Cleared));
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                try
                {
                    OnBaseStatusChanged(new IoSessionStatusChangedEventArgs(IoSessionStatusChanged.Disposed));
                }
                finally
                {
                    IsDisposed = true;
                }
            }
        }
    }

    public class BlockArguments
    {
        /// <summary>
        /// the block returned must contain this position
        /// </summary>
        public long position;

        /// <summary>
        /// indicates if the stream plans to write to this block
        /// </summary>
        public bool isWriting;

        /// <summary>
        /// the pointer for the first byte of the block
        /// </summary>
        public IntPtr firstPointer;

        /// <summary>
        /// the position that corresponds to <see cref="firstPointer"/>
        /// </summary>
        public long firstPosition;

        /// <summary>
        /// the length of the block
        /// </summary>
        public int length;

        /// <summary>
        /// notifies the calling class if this block supports 
        /// writing without requiring this function to be called again if <see cref="isWriting"/> was set to false.
        /// </summary>
        public bool supportsWriting;
    }
}