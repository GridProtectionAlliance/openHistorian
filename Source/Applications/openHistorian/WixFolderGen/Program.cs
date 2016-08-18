using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GSF;
using GSF.IO;
using GSF.IO.Checksums;

namespace WixFolderGen
{
    class Program
    {
        const string ProjectName = "openHistorian";
        const string RootFolderName = "wwwroot";
        const string SolutionRelativeRootFolder = "Applications\\" + ProjectName + "\\" + ProjectName + "\\" + RootFolderName;
        const int MaxWixIDLength = 72;

        // Following paths are relative to build output location
        const string ApplicationPath = "..\\..\\..\\..\\..\\Source\\Applications\\" + ProjectName;
        const string SourceFolder = ApplicationPath + "\\" + ProjectName + "\\" + RootFolderName;
        const string WebFeaturesDestinationFile = ApplicationPath + "\\" + ProjectName + "Setup\\WebFeatures.wxi";
        const string WebFoldersDestinationFile = ApplicationPath + "\\" + ProjectName + "Setup\\WebFolders.wxi";
        const string WebFilesDestinationFile = ApplicationPath + "\\" + ProjectName + "Setup\\WebFiles.wxi";

        static void Main()
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
                writer.WriteLine($"<Directory Id=\"{GetDirectoryID(RootFolderName)}\" Name=\"{RootFolderName}\">");

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
            return new[] { "" }
                .Concat(folderList)
                .Select(folder => $"<ComponentGroupRef Id=\"{GetComponentGroupID(folder)}\" />")
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

            componentGroupTags.Add($"<ComponentGroup Id=\"{GetComponentGroupID(null)}\" Directory=\"{GetDirectoryID(RootFolderName)}\">");

            foreach (string file in Directory.EnumerateFiles(path))
            {
                string fileName = FilePath.GetFileName(file);
                string fileSource = $"$(var.SolutionDir){Path.Combine(SolutionRelativeRootFolder, fileName)}";

                componentGroupTags.Add($"  <Component Id=\"{GetComponentID(fileName)}\">");
                componentGroupTags.Add($"    <File Id=\"{GetFileID(fileName)}\" Name=\"{fileName}\" Source=\"{fileSource}\" />");
                componentGroupTags.Add("  </Component>");
            }

            componentGroupTags.Add("</ComponentGroup>");

            foreach (string folder in folderList)
            {
                componentGroupTags.Add($"<ComponentGroup Id=\"{GetComponentGroupID(folder)}\" Directory=\"{GetDirectoryID(folder)}\">");

                foreach (string file in Directory.EnumerateFiles(Path.Combine(path, folder)))
                {
                    string fileName = FilePath.GetFileName(file);

                    if (!fileName.Equals("thumbs.db", System.StringComparison.OrdinalIgnoreCase))
                    {
                        string fileID = folder + "_" + fileName;
                        string fileSource = $"$(var.SolutionDir){Path.Combine(SolutionRelativeRootFolder, folder, fileName)}";

                        componentGroupTags.Add($"  <Component Id=\"{GetComponentID(fileID)}\">");
                        componentGroupTags.Add($"    <File Id=\"{GetFileID(fileID)}\" Name=\"{fileName}\" Source=\"{fileSource}\" />");
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
                    string name = FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(string.Join(Path.DirectorySeparatorChar.ToString(), folder)));

                    directoryTags.Add($"{new string(' ', level * 2)}<Directory Id=\"{GetDirectoryID(string.Join("", folder))}\" Name=\"{name}\" />");
                }
                else
                {
                    List<string[]> subfolderList = grouping
                        .Where(folder => folder.Length > level + 1)
                        .ToList();

                    string name = FilePath.GetLastDirectoryName(FilePath.AddPathSuffix(grouping.Key));

                    directoryTags.Add($"{new string(' ', level * 2)}<Directory Id=\"{GetDirectoryID(grouping.Key)}\" Name=\"{name}\">");
                    BuildDirectoryTags(directoryTags, subfolderList, level + 1);
                    directoryTags.Add($"{new string(' ', level * 2)}</Directory>");
                }
            }
        }

        private static string GetCleanID(string path, int limit,
            bool replaceDot = false,
            bool removeDot = false,
            bool replaceDirectorySeparatorChar = false,
            bool removeDirectorySeparatorChar = false,
            bool replaceSpaces = false,
            bool removeSpaces = false,
            bool removeUnderscores = false)
        {
            if (replaceDot)
                path = path.Replace('.', '_');

            if (removeDot)
                path = path.Replace(".", "");

            if (replaceDirectorySeparatorChar)
                path = path.Replace(Path.DirectorySeparatorChar, '_');

            if (removeDirectorySeparatorChar)
                path = path.Replace(Path.DirectorySeparatorChar.ToString(), "");

            if (replaceSpaces)
                path = path.ReplaceWhiteSpace('_');

            if (removeSpaces || !replaceSpaces)
                path = path.RemoveWhiteSpace();

            path = new Regex("[^a-zA-Z0-9_.]").Replace(path, "_");

            path = path.RemoveDuplicates("_").TrimEnd('_').TrimStart('_');

            if (removeUnderscores)
                path = path.Replace("_", "");

            if (path.Length > limit)
            {
                byte[] nameBytes = Encoding.Default.GetBytes(path);
                uint crc = nameBytes.Crc32Checksum(0, nameBytes.Length);
                string suffix = crc.ToString();
                path = path.Substring(0, limit - suffix.Length) + suffix;
            }

            return path;
        }

        private static string GetDirectoryID(string folderName)
        {
            const string Suffix = "FOLDER";
            return GetCleanID(folderName, MaxWixIDLength - Suffix.Length, removeDirectorySeparatorChar: true, removeUnderscores: true).ToUpperInvariant() + Suffix;
        }

        private static string GetComponentGroupID(string folderName)
        {
            const string Prefix = RootFolderName + "_";
            const string Suffix = "_Components";

            if (string.IsNullOrWhiteSpace(folderName))
                return RootFolderName + Suffix;

            return Prefix + GetCleanID(folderName, MaxWixIDLength - Prefix.Length - Suffix.Length, replaceDirectorySeparatorChar: true, replaceSpaces: true) + Suffix;
        }

        private static string GetComponentID(string fileName)
        {
            string suffix = FilePath.GetExtension(fileName).Replace('.', '_');
            return GetCleanID(FilePath.GetDirectoryName(fileName) + FilePath.GetFileNameWithoutExtension(fileName), MaxWixIDLength - suffix.Length, removeSpaces: true, replaceDot: true) + suffix;
        }

        private static string GetFileID(string fileName)
        {
            string suffix = FilePath.GetExtension(fileName);
            return GetCleanID(FilePath.GetDirectoryName(fileName) + FilePath.GetFileNameWithoutExtension(fileName), MaxWixIDLength - suffix.Length, removeSpaces: true) + suffix;
        }
    }
}
