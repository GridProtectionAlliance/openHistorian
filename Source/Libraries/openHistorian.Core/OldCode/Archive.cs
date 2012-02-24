using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Historian.StorageSystem;
using Historian.PointLibrary;
using Historian.PluginDataFeatures.TableOfContents;
using Historian.PluginDataFeatures;
using Historian.PluginDataFeatures.MetaDataLibrary;


namespace Historian
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
            archive.m_storageSystem = Historian.StorageSystem.File.VirtualFileSystem.CreateArchive(FileName, 4096, 10000000);
            return archive;
        }
        public static Archive OpenFileArchive(string FileName)
        {
            Archive archive = new Archive();
            archive.m_storageSystem = Historian.StorageSystem.File.VirtualFileSystem.OpenArchive(FileName,true);
            return archive;
        }

    }
}
