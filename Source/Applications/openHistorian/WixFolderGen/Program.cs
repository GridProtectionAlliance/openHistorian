using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF;
using GSF.IO;

namespace WixFolderGen
{
    class Program
    {
        const string RootFolderName = "wwwroot";
        const string SolutionRelativeRootFolder = "Applications\\openHistorian\\openHistorian\\" + RootFolderName;

        // Following paths are relative to build output location
        const string ApplicationPath = "..\\..\\..\\..\\..\\Source\\Applications\\openHistorian";
        const string SourceFolder = ApplicationPath + "\\openHistorian\\" + RootFolderName;
        const string WebFeaturesDestinationFile = ApplicationPath + "\\openHistorianSetup\\WebFeatures.wxi";
        const string WebFoldersDestinationFile = ApplicationPath + "\\openHistorianSetup\\WebFolders.wxi";
        const string WebFilesDestinationFile = ApplicationPath + "\\openHistorianSetup\\WebFiles.wxi";

        static void Main(string[] args)
        {

            List<string> folderList = GetFolderList(SourceFolder);
            List<string> componentGroupRefTags = GetComponentRefTags(folderList);
            List<string> directoryTags = GetDirectoryTags(folderList);
            List<string> componentGroupTags = GetComponentGroupTags(SourceFolder, folderList);

            using (FileStream stream = File.Create(WebFeaturesDestinationFile))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("<Include>");
                writer.WriteLine("<Feature Id=\"WebFilesFeature\" Title=\"Web Files\" Description=\"Web Files\">");

                foreach (string tag in componentGroupRefTags)
                    writer.WriteLine("  " + tag);

                writer.WriteLine("</Feature>");
                writer.WriteLine("</Include>");
            }

            using (FileStream stream = File.Create(WebFoldersDestinationFile))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("<Include>");
                writer.WriteLine($"<Directory Id=\"{RootFolderName.ToUpper()}FOLDER\" Name=\"{RootFolderName}\">");

                foreach (string tag in directoryTags)
                    writer.WriteLine("  " + tag);

                writer.WriteLine("</Directory>");
                writer.WriteLine("</Include>");
            }

            using (FileStream stream = File.Create(WebFilesDestinationFile))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("<Include>");

                foreach (string tag in componentGroupTags)
                    writer.WriteLine(tag);

                writer.WriteLine("</Include>");
            }
        }

        private static List<string> GetFolderList(string path)
        {
            List<string> folderList = new List<string>();
            BuildFolderList(folderList, path, string.Empty);
            return folderList;
        }

        private static List<string> GetComponentRefTags(List<string> folderList)
        {
            return new [] { "" }
                .Concat(folderList)
                .Select(folder => folder.Replace(Path.DirectorySeparatorChar, '_').Replace('-', '_'))
                .Select(folder => $"<ComponentGroupRef Id=\"{RootFolderName}_{folder}Components\" />")
                .ToList();
        }

        private static List<string> GetDirectoryTags(List<string> folderList)
        {
            List<string> directoryTags = new List<string>();

            List<string[]> brokenFolderList = folderList
                .Select(FilePath.RemovePathSuffix)
                .Select(folder => folder.Split(Path.DirectorySeparatorChar))
                .ToList();

            BuildDirectoryTags(directoryTags, brokenFolderList, 0);

            return directoryTags;
        }

        private static List<string> GetComponentGroupTags(string path, List<string> folderList)
        {
            List<string> componentGroupTags = new List<string>();

            componentGroupTags.Add($"<ComponentGroup Id=\"{RootFolderName}_Components\" Directory=\"{RootFolderName.ToUpper()}FOLDER\">");

            foreach (string file in Directory.EnumerateFiles(path))
            {
                string fileName = FilePath.GetFileName(file);
                string fileID = fileName.RemoveWhiteSpace().Replace('-', '_').Replace("(", "").Replace(")", "");
                string fileSource = $"$(var.SolutionDir){Path.Combine(SolutionRelativeRootFolder, fileName)}";
                string componentID = fileID.Replace('.', '_');

                componentGroupTags.Add($"  <Component Id=\"{componentID}\">");
                componentGroupTags.Add($"    <File Id=\"{fileID}\" Name=\"{fileName}\" Source=\"{fileSource}\" />");
                componentGroupTags.Add("  </Component>");
            }

            componentGroupTags.Add("</ComponentGroup>");

            foreach (string folder in folderList)
            {
                string groupID = folder.Replace(Path.DirectorySeparatorChar, '_').Replace('-', '_');
                string directory = folder.Replace(Path.DirectorySeparatorChar.ToString(), "").Replace('-', '_').ToUpper();

                componentGroupTags.Add($"<ComponentGroup Id=\"{RootFolderName}_{groupID}Components\" Directory=\"{directory}FOLDER\">");

                foreach (string file in Directory.EnumerateFiles(Path.Combine(path, folder)))
                {
                    string fileName = FilePath.GetFileName(file);

                    if (!fileName.Equals("thumbs.db", System.StringComparison.OrdinalIgnoreCase))
                    {
                        string fileID = (folder.Replace(Path.DirectorySeparatorChar, '_') + "_" + fileName).RemoveWhiteSpace().Replace('-', '_').Replace("(", "").Replace(")", "");
                        string fileSource = $"$(var.SolutionDir){Path.Combine(SolutionRelativeRootFolder, folder, fileName)}";
                        string componentID = fileID.Replace('.', '_');

                        componentGroupTags.Add($"  <Component Id=\"{componentID}\">");
                        componentGroupTags.Add($"    <File Id=\"{fileID}\" Name=\"{fileName}\" Source=\"{fileSource}\" />");
                        componentGroupTags.Add("  </Component>");
                    }
                }

                componentGroupTags.Add("</ComponentGroup>");
            }

            return componentGroupTags;
        }

        private static void BuildFolderList(List<string> folderList, string path, string rootPath)
        {
            string name;

            foreach (string folder in Directory.EnumerateDirectories(path))
            {
                name = FilePath.AddPathSuffix(rootPath + FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(folder)));
                folderList.Add(name);
                BuildFolderList(folderList, folder, name);
            }
        }

        private static void BuildDirectoryTags(List<string> directoryTags, List<string[]> folderList, int level)
        {
            List<IGrouping<string, string[]>> groupings = folderList
                .Where(folder => folder.Length > level)
                .GroupBy(folder => string.Join(Path.DirectorySeparatorChar.ToString(), folder.Take(level + 1)))
                .OrderBy(grouping => grouping.Key)
                .ToList();

            foreach (IGrouping<string, string[]> grouping in groupings)
            {
                if (grouping.Count() == 1)
                {
                    string[] folder = grouping.First();
                    string id = string.Join("", folder).Replace('-', '_').ToUpper();
                    string name = FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(string.Join(Path.DirectorySeparatorChar.ToString(), folder)));

                    directoryTags.Add($"{new string(' ', level * 2)}<Directory Id=\"{id}FOLDER\" Name=\"{name}\" />");
                }
                else
                {
                    List<string[]> subfolderList = grouping
                        .Where(folder => folder.Length > level + 1)
                        .ToList();

                    string id = grouping.Key.Replace(Path.DirectorySeparatorChar.ToString(), "").Replace('-', '_').ToUpper();
                    string name = FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(grouping.Key));

                    directoryTags.Add($"{new string(' ', level * 2)}<Directory Id=\"{id}FOLDER\" Name=\"{name}\">");
                    BuildDirectoryTags(directoryTags, subfolderList, level + 1);
                    directoryTags.Add($"{new string(' ', level * 2)}</Directory>");
                }
            }
        }
    }
}
