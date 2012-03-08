using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.Core.StorageSystem;
using openHistorian.Core.PointLibrary;
using openHistorian.Core.PluginDataFeatures.TableOfContents;
using openHistorian.Core.PluginDataFeatures;
using openHistorian.Core.PluginDataFeatures.MetaDataLibrary;


namespace openHistorian.Core
{
    public class Archive
    {
        private IStorageSystem m_storageSystem;
        private ITableOfContents m_tableOfContents;
        private IMetaDataLibrary m_metaDataLibrary;
        private List<IPluginDataFeature> FeatureList;

        public static Archive CreateDatabaseArchive(string FileName)
        {
            throw new NotSupportedException();
            //return new Archive();
        }
        public static Archive OpenDatabaseArchive(string FileName)
        {
            throw new NotSupportedException();
            //return new Archive();
        }
        public static Archive CreateFileArchive(string FileName)
        {
            Archive archive = new Archive();
            archive.m_storageSystem = openHistorian.Core.StorageSystem.File.VirtualFileSystem.CreateArchive(FileName);
            return archive;
        }
        public static Archive OpenFileArchive(string FileName)
        {
            Archive archive = new Archive();
            archive.m_storageSystem = openHistorian.Core.StorageSystem.File.VirtualFileSystem.OpenArchive(FileName, true);
            return archive;
        }

    }
}
