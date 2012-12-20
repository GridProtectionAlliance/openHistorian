using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    public class CommitWaitHandlesTest : ISupportsWaitHandles
    {

        bool m_diskCommitted;
        long m_committedSequenceId;
        long m_diskCommittedSequenceId;
        
        long m_currentSequencId;

        bool m_is8Committed;
        bool m_is8DiskCommitted;
        bool m_is20Committed;
        bool m_is20DiskCommitted;
        bool m_is30Committed;
        bool m_is30DiskCommitted;

        CommitWaitHandles wait;
        
        //[Test]
        public void Test()
        {
            wait = new CommitWaitHandles(this);

            Assert.AreEqual(false, wait.IsCommitted(5));
            m_currentSequencId = 5;
            wait.Commit();
            Assert.AreEqual(true, wait.IsCommitted(5));
        }

        void ProcessWaitFor8()
        {
            Assert.IsTrue(wait.WaitForCommit(8, false));
            m_is8Committed = true;
        }
        void ProcessWaitForDisk8()
        {
            Assert.IsTrue(wait.WaitForRollover(8, false));
            m_is8DiskCommitted = true;
        }

        void ProcessWaitFor20()
        {
            Assert.IsTrue(wait.WaitForCommit(20, false));
            m_is20Committed = true;
        }
        void ProcessWaitForDisk20()
        {
            Assert.IsTrue(wait.WaitForRollover(20, false));
            m_is20DiskCommitted = true;
        }

        void ProcessWaitFor30()
        {
            Assert.IsFalse(wait.WaitForCommit(20, false));
            m_is30Committed = true;
        }
        void ProcessWaitForDisk30()
        {
            Assert.IsFalse(wait.WaitForRollover(20, false));
            m_is30DiskCommitted = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            OnThreadQuit();
        }

        public event Action<long> OnNewData;
        public event Action<long> OnCommit;
        public event Action<long> OnRollover;
        public event Action OnThreadQuit;

        public void ForceCommit()
        {
            m_committedSequenceId = m_currentSequencId;
            OnCommit(m_currentSequencId);
        }

        public void ForceQuit()
        {
            OnThreadQuit();
        }

        public void ForceNewFile()
        {
            m_committedSequenceId = m_currentSequencId;
            m_diskCommittedSequenceId = m_currentSequencId;
            OnRollover(m_currentSequencId);
        }
    }
}
