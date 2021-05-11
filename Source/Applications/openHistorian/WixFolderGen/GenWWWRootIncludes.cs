//******************************************************************************************************
//  GenWWWRootIncludes.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/07/2017 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GSF.IO;

// This application is not a deployed application - it is only used to build Wix setup files
#pragma warning disable SG0018 // Path traversal

namespace WiXFolderGen
{
    public static class GenWWWRootIncludes
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

        // Define list of files types NOT to publish that may be found in wwwroot folder:
        static readonly HashSet<string> ExcludedExtensions = new HashSet<string>(new[] { ".cs", ".vb", ".exe", ".dll", ".pdb", ".db" }, StringComparer.OrdinalIgnoreCase);

        public static void Execute()
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
                string fileName = GetFileName(file);

                if (string.IsNullOrWhiteSpace(fileName))
                    continue;

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
                    string fileName = GetFileName(file);

                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;

                    string fileID = folder + "_" + fileName;
                    string fileSource = $"$(var.SolutionDir){Path.Combine(SolutionRelativeRootFolder, folder, fileName)}";

                    componentGroupTags.Add($"  <Component Id=\"{GetComponentID(fileID)}\">");
                    componentGroupTags.Add($"    <File Id=\"{GetFileID(fileID)}\" Name=\"{fileName}\" Source=\"{fileSource}\" />");
                    componentGroupTags.Add("  </Component>");
                }

                componentGroupTags.Add("</ComponentGroup>");
            }

            return componentGroupTags;
        }

        private static string GetFileName(string filePath)
        {
            return ExcludedExtensions.Contains(FilePath.GetExtension(filePath)) ? null : FilePath.GetFileName(filePath);
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

        private static string GetDirectoryID(string folderName)
        {
            const string Suffix = "FOLDER";
            return Program.GetCleanID(folderName, "", Suffix, MaxWixIDLength - Suffix.Length, removeDirectorySeparatorChar: true, removeUnderscores: true).ToUpperInvariant();
        }

        private static string GetComponentGroupID(string folderName)
        {
            const string Prefix = RootFolderName + "_";
            const string Suffix = "_Components";

            if (string.IsNullOrWhiteSpace(folderName))
                return RootFolderName + Suffix;

            return Program.GetCleanID(folderName, Prefix, Suffix, MaxWixIDLength - Prefix.Length - Suffix.Length, replaceDirectorySeparatorChar: true, replaceSpaces: true);
        }

        private static string GetComponentID(string fileName)
        {
            string suffix = Program.GetCleanID(FilePath.GetExtension(fileName), "", "", MaxWixIDLength, true, false, false, true, true);
            return Program.GetCleanID(FilePath.GetDirectoryName(fileName) + FilePath.GetFileNameWithoutExtension(fileName), "", suffix, MaxWixIDLength - suffix.Length, removeSpaces: true, replaceDot: true);
        }

        private static string GetFileID(string fileName)
        {
            string suffix = Program.GetCleanID(FilePath.GetExtension(fileName), "", "", MaxWixIDLength, true, false, false, true, true);
            return Program.GetCleanID(FilePath.GetDirectoryName(fileName) + FilePath.GetFileNameWithoutExtension(fileName), "", suffix, MaxWixIDLength - suffix.Length, removeSpaces: true);
        }
    }
}
