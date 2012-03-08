using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Archives.V2
{
    public class ArchiveFile : IDataArchive
    {
        public event EventHandler<TVA.EventArgs<IEnumerable<IData>>> DataReceived;

        public DataArchiveState State
        {
            get { throw new NotImplementedException(); }
        }

        public IDataCache Cache
        {
            get { throw new NotImplementedException(); }
        }

        public void Open(bool openDependencies)
        {
            throw new NotImplementedException();
        }

        public void Close(bool closeDependencies)
        {
            throw new NotImplementedException();
        }

        public void WriteData(IData data)
        {
            throw new NotImplementedException();
        }

        public void WriteMetaData(int key, byte[] metaData)
        {
            throw new NotImplementedException();
        }

        public void WriteStateData(int key, byte[] stateData)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IData> ReadData(int key, string startTime, string endTime)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadMetaData(int key)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadStateData(int key)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadMetaDataSummary(int key)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadStateDataSummary(int key)
        {
            throw new NotImplementedException();
        }

        public AppDomain Domain
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<TVA.EventArgs<string, Exception>> ExecutionException;

        public string HostFile
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double MemoryUsage
        {
            get { throw new NotImplementedException(); }
        }

        public double ProcessorUsage
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<TVA.EventArgs<TVA.UpdateType, string>> StatusUpdate;

        public string TypeName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler Disposed;

        public bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Status
        {
            get { throw new NotImplementedException(); }
        }

        public void LoadSettings()
        {
            throw new NotImplementedException();
        }

        public bool PersistSettings
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SaveSettings()
        {
            throw new NotImplementedException();
        }

        public string SettingsCategory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
