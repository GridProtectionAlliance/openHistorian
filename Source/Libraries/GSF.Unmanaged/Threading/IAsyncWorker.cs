//using System;

//namespace GSF.Threading
//{
//    interface IAsyncWorker : IDisposable
//    {
//        /// <summary>
//        /// Event occurs on a seperate thread and will be repeatedly
//        /// called evertime <see cref="AsyncWorkerForeground.RunWorker"/> is called unless 
//        /// <see cref="AsyncWorkerForeground.Dispose"/> is called first.
//        /// </summary>
//        event EventHandler<AsyncWorkerEventArgs> DoWork;

//        /// <summary>
//        /// Event occurs only once on a seperate thread
//        /// when this class is disposed.
//        /// </summary>
//        event EventHandler CleanupWork;

//        /// <summary>
//        /// Gets if the class has been disposed;
//        /// </summary>
//        bool IsDisposed { get; }

//        /// <summary>
//        /// Gets if this class is no longer going to support
//        /// running again. Check <see cref="IsDisposed"/> to see
//        /// if the class has been completely disposed.
//        /// </summary>
//        bool IsDisposing { get; }

//        /// <summary>
//        /// Makes sure that the process begins immediately if it is currently not running.
//        /// If it is running, it tells the process to run at least one more time. 
//        /// </summary>
//        void RunWorker();

//        /// <summary>
//        /// Called when the finalizer of <see cref="AsyncWorker"/> is called. 
//        /// </summary>
//        void FinalizedDisposed();
//    }
//}