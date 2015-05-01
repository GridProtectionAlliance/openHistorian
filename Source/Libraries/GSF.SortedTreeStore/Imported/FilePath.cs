//******************************************************************************************************
//  FilePath.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/05/2003 - J. Ritchie Carroll
//       Generated original version of source code.
//  12/29/2005 - Pinal C. Patel
//       Migrated 2.0 version of source code from 1.1 source (GSF.Shared.FilePath).
//  08/22/2007 - Darrell Zuercher
//       Edited code comments.
//  09/19/2008 - J. Ritchie Carroll
//       Converted to C#.
//  10/24/2008 - Pinal C. Patel
//       Edited code comments.
//  12/17/2008 - F. Russell Robertson
//       Fixed issue in GetFilePatternRegularExpression().
//  06/30/2009 - Pinal C. Patel
//       Removed FilePathHasFileName() since the result was error prone.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  09/17/2009 - Pinal C. Patel
//       Modified GetAbsolutePath() to remove dependency on HttpContext.Current.
//  04/19/2010 - Pinal C. Patel
//       Added GetApplicationDataFolder() method.
//  04/21/2010 - Pinal C. Patel
//       Updated GetApplicationDataFolder() to include the company name if available.
//  01/28/2011 - J. Ritchie Carroll
//       Added IsValidFileName function.
//  02/14/2011 - J. Ritchie Carroll
//       Fixed issue in GetDirectoryName where last directory was being truncated as a file name.
//  06/06/2011 - Stephen C. Wills
//       Fixed issue in GetFileName where path suffix was being removed before extracting the file name.
//  07/29/2011 - Pinal C. Patel
//       Updated GetApplicationDataFolder() to use the TEMP directory for web applications.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Web.Hosting;
//using GSF.Identity;
//using GSF.Interop;
using GSF.Reflection;

namespace GSF.IO
{
    /// <summary>
    /// Contains File and Path manipulation methods.
    /// </summary>
    public static class FilePath
    {
        #region [ Members ]

        // Nested Types

        // ReSharper disable MemberCanBePrivate.Local
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NETRESOURCE
        {
            public readonly int dwScope;
            public int dwType;
            public readonly int dwDisplayType;
            public readonly int dwUsage;
            public readonly string lpLocalName;
            public string lpRemoteName;
            public readonly string lpComment;
            public readonly string lpProvider;
        }

        // Constants
        private const int RESOURCETYPE_DISK = 0x1;

        // Fields
        private static readonly string s_fileNameCharPattern;

        #endregion

        #region [ Constructor ]

        static FilePath()
        {
            StringBuilder pattern = new StringBuilder();

            // Defines a regular expression pattern for a valid file name character. We do this by
            // allowing any characters except those that would not be valid as part of a filename.
            // This essentially builds the "?" wildcard pattern match.
            pattern.Append("[^");
            pattern.Append(Path.DirectorySeparatorChar.RegexEncode());
            pattern.Append(Path.AltDirectorySeparatorChar.RegexEncode());
            pattern.Append(Path.PathSeparator.RegexEncode());
            pattern.Append(Path.VolumeSeparatorChar.RegexEncode());

            foreach (char c in Path.GetInvalidPathChars())
            {
                pattern.Append(c.RegexEncode());
            }

            pattern.Append("]");
            s_fileNameCharPattern = pattern.ToString();
        }

        #endregion

        #region [ Methods ]

        ///// <summary>
        ///// Connects to a network share with the specified user's credentials.
        ///// </summary>
        ///// <param name="sharename">UNC share name to connect to.</param>
        ///// <param name="userName">User name to use for connection.</param>
        ///// <param name="password">Password to use for connection.</param>
        ///// <param name="domain">Domain name to use for connection. Specify the computer name for local system accounts.</param>
        //public static void ConnectToNetworkShare(string sharename, string userName, string password, string domain)
        //{
        //    if (Common.IsPosixEnvironment)
        //        throw new NotImplementedException("Failed to connect to network share \"" + sharename + "\" as user " + userName + " - not implemented in POSIX environment");

        //    NETRESOURCE resource = new NETRESOURCE();
        //    int result;

        //    resource.dwType = RESOURCETYPE_DISK;
        //    resource.lpRemoteName = sharename;

        //    if (domain.Length > 0)
        //        userName = domain + "\\" + userName;

        //    result = WNetAddConnection2(ref resource, password, userName, 0);

        //    if (result != 0)
        //        throw new InvalidOperationException("Failed to connect to network share \"" + sharename + "\" as user " + userName + " - " + WindowsApi.GetErrorMessage(result));
        //}

        ///// <summary>
        ///// Disconnects the specified network share.
        ///// </summary>
        ///// <param name="sharename">UNC share name to disconnect from.</param>
        //public static void DisconnectFromNetworkShare(string sharename)
        //{
        //    DisconnectFromNetworkShare(sharename, true);
        //}

        ///// <summary>
        ///// Disconnects the specified network share.
        ///// </summary>
        ///// <param name="sharename">UNC share name to disconnect from.</param>
        ///// <param name="force"><c>true</c> to force a disconnect; otherwise <c>false</c>.</param>
        //public static void DisconnectFromNetworkShare(string sharename, bool force)
        //{
        //    if (Common.IsPosixEnvironment)
        //        throw new NotImplementedException("Failed to disconnect from network share \"" + sharename + "\" - not implemented in POSIX environment");

        //    int result = WNetCancelConnection2(sharename, 0, force);

        //    if (result != 0)
        //        throw new InvalidOperationException("Failed to disconnect from network share \"" + sharename + "\" - " + WindowsApi.GetErrorMessage(result));
        //}

        /// <summary>
        /// Determines whether the specified file name matches any of the given file specs (wildcards are defined as '*' or '?' characters).
        /// </summary>
        /// <param name="fileSpecs">The file specs used for matching the specified file name.</param>
        /// <param name="fileName">The file name to be tested against the specified file specs for a match.</param>
        /// <param name="ignoreCase"><c>true</c> to specify a case-insensitive match; otherwise <c>false</c>.</param>
        /// <returns><c>true</c> if the specified file name matches any of the given file specs; otherwise <c>false</c>.</returns>
        public static bool IsFilePatternMatch(string[] fileSpecs, string fileName, bool ignoreCase)
        {
            foreach (string fileSpec in fileSpecs)
            {
                if (IsFilePatternMatch(fileSpec, fileName, ignoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified file name matches the given file spec (wildcards are defined as '*' or '?' characters).
        /// </summary>
        /// <param name="fileSpec">The file spec used for matching the specified file name.</param>
        /// <param name="fileName">The file name to be tested against the specified file spec for a match.</param>
        /// <param name="ignoreCase"><c>true</c> to specify a case-insensitive match; otherwise <c>false</c>.</param>
        /// <returns><c>true</c> if the specified file name matches the given file spec; otherwise <c>false</c>.</returns>
        public static bool IsFilePatternMatch(string fileSpec, string fileName, bool ignoreCase)
        {
            return (new Regex(GetFilePatternRegularExpression(fileSpec), (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None))).IsMatch(fileName);
        }

        /// <summary>
        /// Determines if the specified file name and path is valid.
        /// </summary>
        /// <param name="filePath">The file name with optional path to test for validity.</param>
        /// <returns><c>true</c> if the specified <paramref name="filePath"/> is a valid name; otherwise <c>false</c>.</returns>
        public static bool IsValidFileName(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                // Attempt to parse directory and file name - this will catch most issues
                string directory = Path.GetDirectoryName(filePath);
                string filename = Path.GetFileName(filePath);

                // Check for invalid directory characters
                if (!string.IsNullOrWhiteSpace(directory) && directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return false;

                // Check for invalid file name characters
                if (string.IsNullOrWhiteSpace(filename) || filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    return false;

                // Recurse in to check validity of each directory name
                if (!string.IsNullOrWhiteSpace(directory) && !string.IsNullOrWhiteSpace(Path.GetFileName(directory)))
                    return IsValidFileName(directory);
            }
            catch
            {
                // If you can't parse a file name or directory, file path is definitely not valid
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a valid file name by replacing invalid file name characters with <paramref name="replaceWithCharacter"/>.
        /// </summary>
        /// <param name="filePath">File path to validate.</param>
        /// <param name="replaceWithCharacter">Character to replace invalid file name characters with. Set to '\0' to remove invalid file name characters.</param>
        /// <returns>A valid file name with no invalid file name characters.</returns>
        public static string GetValidFileName(string filePath, char replaceWithCharacter = '_')
        {
            if (replaceWithCharacter == '\0')
                return filePath.RemoveInvalidFileNameCharacters();

            return filePath.ReplaceInvalidFileNameCharacters(replaceWithCharacter);
        }

        /// <summary>
        /// Gets the file name and extension from the specified file path.
        /// </summary>
        /// <param name="filePath">The file path from which the file name and extension is to be obtained.</param>
        /// <returns>File name and extension if the file path has it; otherwise empty string.</returns>
        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        /// <summary>
        /// Gets the extension from the specified file path.
        /// </summary>
        /// <param name="filePath">The file path from which the extension is to be obtained.</param>
        /// <returns>File extension.</returns>
        public static string GetExtension(string filePath)
        {
            return Path.GetExtension(RemovePathSuffix(filePath));
        }

        /// <summary>
        /// Gets the file name without extension from the specified file path.
        /// </summary>
        /// <param name="filePath">The file path from which the file name is to be obtained.</param>
        /// <returns>File name without the extension if the file path has it; otherwise empty string.</returns>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(RemovePathSuffix(filePath));
        }

        /// <summary>
        /// Gets the size of the specified file.
        /// </summary>
        /// <param name="fileName">Name of file whose size is to be retrieved.</param>
        /// <returns>The size of the specified file.</returns>
        public static long GetFileLength(string fileName)
        {
            try
            {
                return (new FileInfo(fileName)).Length;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets a list of files under the specified path. Search wildcard pattern (c:\Data\*.dat) can be used for 
        /// including only the files matching the pattern or path wildcard pattern (c:\Data\*\*.dat) to indicate the 
        /// inclusion of files under all subdirectories in the list.
        /// </summary>
        /// <param name="path">The path for which a list of files is to be returned.</param>
        /// <returns>A list of files under the given path.</returns>
        public static string[] GetFileList(string path)
        {
            string directory = GetDirectoryName(path);
            string filePattern = GetFileName(path);
            SearchOption options = SearchOption.TopDirectoryOnly;

            if (string.IsNullOrEmpty(filePattern))
            {
                // No wildcard pattern was specified, so get a listing of all files.
                filePattern = "*.*";
            }

            if (GetLastDirectoryName(directory) == "*")
            {
                // Path wildcard pattern is used to specify the option to include subdirectories.
                options = SearchOption.AllDirectories;
                directory = directory.Remove(directory.LastIndexOf("*", StringComparison.OrdinalIgnoreCase));
            }

            return Directory.GetFiles(directory, filePattern, options);
        }

        /// <summary>
        /// Gets a regular expression pattern that simulates wildcard matching for filenames (wildcards are defined as '*' or '?' characters).
        /// </summary>
        /// <param name="fileSpec">The file spec for which the regular expression pattern if to be generated.</param>
        /// <returns>Regular expression pattern that simulates wildcard matching for filenames.</returns>
        public static string GetFilePatternRegularExpression(string fileSpec)
        {
            // Replaces wildcard file patterns with their equivalent regular expression.
            fileSpec = fileSpec.Replace("\\", "\\u005C"); // Backslash in Regex means special sequence. Here, we really want a backslash.
            fileSpec = fileSpec.Replace(".", "\\u002E"); // Dot in Regex means any character. Here, we really want a dot.
            fileSpec = fileSpec.Replace("?", s_fileNameCharPattern);
            fileSpec = fileSpec.Replace("*", "(" + s_fileNameCharPattern + ")*");

            return "^" + fileSpec + "$";
        }

        /// <summary>
        /// Gets the directory information from the specified file path.
        /// </summary>
        /// <param name="filePath">The file path from which the directory information is to be obtained.</param>
        /// <returns>Directory information.</returns>
        public static string GetDirectoryName(string filePath)
        {
            return AddPathSuffix(Path.GetDirectoryName(filePath) ?? filePath);
        }

        /// <summary>
        /// Gets the last directory name from a file path.
        /// </summary>
        /// <param name="filePath">The file path from where the last directory name is to be retrieved.</param>
        /// <returns>The last directory name from a file path.</returns>
        /// <remarks>
        /// <see cref="GetLastDirectoryName(string)"/> would return sub2 from c:\windows\sub2\filename.ext.
        /// </remarks>
        public static string GetLastDirectoryName(string filePath)
        {
            // Test case should verify the following:
            //   FilePath.GetLastDirectoryName(@"C:\Test\sub") == "Test" <-- sub assumed to be filename
            //   FilePath.GetLastDirectoryName(@"C:\Test\sub\") == "sub" <-- sub assumed to be directory

            if (!string.IsNullOrEmpty(filePath))
            {
                int index;
                char[] dirVolChars = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };

                // Remove file name and trailing directory separator character from the file path.
                filePath = RemovePathSuffix(GetDirectoryName(filePath));
                // Keep going through the file path until all directory separator characters are removed.
                while ((index = filePath.IndexOfAny(dirVolChars)) > -1)
                {
                    filePath = filePath.Substring(index + 1);
                }

                return filePath;
            }

            throw new ArgumentNullException("filePath");
        }

        /// <summary>
        /// Gets the absolute file path for the specified file name or relative file path.
        /// </summary>
        /// <param name="filePath">File name or relative file path.</param>
        /// <returns>Absolute file path for the specified file name or relative file path.</returns>
        public static string GetAbsolutePath(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                //// The specified path is a relative one since it is not rooted.
                //switch (Common.GetApplicationType())
                //{
                //    // Prepends the application's root to the file path.
                //    case ApplicationType.Web:
                //        filePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, filePath);
                //        break;
                //    default:
                        filePath = Path.Combine(GetDirectoryName(AssemblyInfo.EntryAssembly.Location), filePath);
                //        break;
                //}
            }

            return RemovePathSuffix(filePath);
        }

        /// <summary>
        /// Determines if the specified <paramref name="filePath"/> is contained with the current executable path.
        /// </summary>
        /// <param name="filePath">File name or relative file path.</param>
        /// <returns><c>true</c> if the specified <paramref name="filePath"/> is contained with the current executable path; otherwise <c>false</c>.</returns>
        public static bool InApplicationPath(string filePath)
        {
            return GetAbsolutePath(filePath).StartsWith(GetAbsolutePath(""), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the path to the folder where data related to the current application can be stored.
        /// </summary>
        /// <returns>Path to the folder where data related to the current application can be stored.</returns>
        public static string GetApplicationDataFolder()
        {
            string rootFolder;

            //switch (Common.GetApplicationType())
            //{
            //    case ApplicationType.Web:
            //        // Treat web application special.
            //        if (HostingEnvironment.ApplicationVirtualPath == "/")
            //            rootFolder = Path.Combine(Path.GetTempPath(), HostingEnvironment.SiteName);
            //        else
            //            rootFolder = Path.Combine(Path.GetTempPath(), HostingEnvironment.ApplicationVirtualPath.ToNonNullString().Trim('/'));

            //        // Create a user folder if ID is available.
            //        string userID = UserInfo.RemoteUserID;

            //        // ID is not available.
            //        if (string.IsNullOrEmpty(userID))
            //            return rootFolder;

            //        // Remove domain from ID.
            //        if (!userID.Contains("\\"))
            //            return Path.Combine(rootFolder, userID);

            //        return Path.Combine(rootFolder, userID.Remove(0, userID.IndexOf('\\') + 1));
            //    default:
                    rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                    if (string.IsNullOrEmpty(AssemblyInfo.EntryAssembly.Company))
                        return Path.Combine(rootFolder, AssemblyInfo.EntryAssembly.Name);

                    return Path.Combine(rootFolder, AssemblyInfo.EntryAssembly.Company, AssemblyInfo.EntryAssembly.Name);
            //}
        }

        /// <summary>
        /// Gets the path to the folder where data related to the current
        /// application can be stored as well as shared among different users.
        /// </summary>
        /// <returns>Path to the folder where data related to the current application can be stored.</returns>
        public static string GetCommonApplicationDataFolder()
        {
            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            if (string.IsNullOrEmpty(AssemblyInfo.EntryAssembly.Company))
                return Path.Combine(rootFolder, AssemblyInfo.EntryAssembly.Name);

            return Path.Combine(rootFolder, AssemblyInfo.EntryAssembly.Company, AssemblyInfo.EntryAssembly.Name);
        }

        /// <summary>
        /// Makes sure path is suffixed with standard <see cref="Path.DirectorySeparatorChar"/>.
        /// </summary>
        /// <param name="filePath">The file path to be suffixed.</param>
        /// <returns>Suffixed path.</returns>
        public static string AddPathSuffix(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.DirectorySeparatorChar.ToString();
            }
            else
            {
                char suffixChar = filePath[filePath.Length - 1];

                if (suffixChar != Path.DirectorySeparatorChar && suffixChar != Path.AltDirectorySeparatorChar)
                    filePath += Path.DirectorySeparatorChar;
            }

            return filePath;
        }

        /// <summary>
        /// Makes sure path is not suffixed with <see cref="Path.DirectorySeparatorChar"/> or <see cref="Path.AltDirectorySeparatorChar"/>.
        /// </summary>
        /// <param name="filePath">The file path to be unsuffixed.</param>
        /// <returns>Unsuffixed path.</returns>
        public static string RemovePathSuffix(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "";
            }
            else
            {
                char suffixChar = filePath[filePath.Length - 1];

                while ((suffixChar == Path.DirectorySeparatorChar || suffixChar == Path.AltDirectorySeparatorChar) && filePath.Length > 0)
                {
                    filePath = filePath.Substring(0, filePath.Length - 1);

                    if (filePath.Length > 0)
                        suffixChar = filePath[filePath.Length - 1];
                }
            }

            return filePath;
        }

        /// <summary>
        /// Remove any path root present in the path.
        /// </summary>
        /// <param name="filePath">The file path whose root is to be removed.</param>
        /// <returns>The path with the root removed if it was present.</returns>
        public static string DropPathRoot(string filePath)
        {
            // JRC: Changed this to the following more simple algorithm
            if (string.IsNullOrEmpty(filePath))
                return "";

            return filePath.Remove(0, Path.GetPathRoot(filePath).Length);

            #region [ Original Code ]
            //string result = filePath;

            //if (!string.IsNullOrEmpty(filePath))
            //{
            //    if ((filePath[0] == '\\') || (filePath[0] == '/'))
            //    {
            //        // UNC name ?
            //        if ((filePath.Length > 1) && ((filePath[1] == '\\') || (filePath[1] == '/')))
            //        {
            //            int index = 2;
            //            int elements = 2;

            //            // Scan for two separate elements \\machine\share\restofpath
            //            while ((index <= filePath.Length) &&
            //                (((filePath[index] != '\\') && (filePath[index] != '/')) || (--elements > 0)))
            //            {
            //                index++;
            //            }

            //            index++;

            //            if (index < filePath.Length)
            //            {
            //                result = filePath.Substring(index);
            //            }
            //            else
            //            {
            //                result = "";
            //            }
            //        }
            //    }
            //    else if ((filePath.Length > 1) && (filePath[1] == ':'))
            //    {
            //        int dropCount = 2;
            //        if ((filePath.Length > 2) && ((filePath[2] == '\\') || (filePath[2] == '/')))
            //        {
            //            dropCount = 3;
            //        }
            //        result = result.Remove(0, dropCount);
            //    }
            //}

            //return result;
            #endregion
        }

        /// <summary>
        /// Returns a file name, for display purposes, of the specified length using "..." to indicate a longer name.
        /// </summary>
        /// <param name="fileName">The file path to be trimmed.</param>
        /// <param name="length">The maximum length of the trimmed file path.</param>
        /// <returns>Trimmed file path.</returns>
        /// <remarks>
        /// Minimum value for the <paramref name="length" /> parameter is 12. 12 will be used for any value 
        /// specified as less than 12.
        /// </remarks>
        public static string TrimFileName(string fileName, int length)
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = "";
            else
                fileName = fileName.Trim();

            if (length < 12)
                length = 12;

            if (fileName.Length > length)
            {
                string justName = GetFileName(fileName);

                if (justName.Length == fileName.Length)
                {
                    // This is just a file name. Make sure extension shows.
                    string justExtension = GetExtension(fileName);
                    string trimName = GetFileNameWithoutExtension(fileName);

                    if (trimName.Length > 8)
                    {
                        if (justExtension.Length > length - 8)
                            justExtension = justExtension.Substring(0, length - 8);

                        double offsetLen = (length - justExtension.Length - 3) / 2.0D;

                        return trimName.Substring(0, (int)(Math.Ceiling(offsetLen))) + "..." +
                            trimName.Substring((int)Math.Round(trimName.Length - Math.Floor(offsetLen) + 1.0D)) + justExtension;
                    }

                    // We can not trim file names less than 8 with a "...", so we truncate long extension.
                    return trimName + justExtension.Substring(0, length - trimName.Length);
                }

                // File name alone exceeds length - recurses into function without path.
                if (justName.Length > length)
                    return TrimFileName(justName, length);

                // File name contains path. Trims path before file name.
                string justFilePath = GetDirectoryName(fileName);
                int offset = length - justName.Length - 4;

                if (justFilePath.Length > offset && offset > 0)
                    return justFilePath.Substring(0, offset) + "..." + Path.DirectorySeparatorChar + justName;

                // Can not fit path. Trims file name.
                return TrimFileName(justName, length);
            }

            // Full file name fits within requested length.
            return fileName;
        }

        /// <summary>
        /// Gets a unique file path for the given file by checking for name collisions and
        /// adding a sequence number to the end of the file name if there is a collision.
        /// </summary>
        /// <param name="originalFilePath">The path to the original file before adding the sequence number.</param>
        /// <returns>The unique path to the file.</returns>
        /// <remarks>
        /// This method is designed to handle the case where the user wishes to create a file in a folder
        /// with a given name when there is a possibility that the name is already taken. Using this method,
        /// it is possible to create files with names in the following format:
        /// 
        /// <ul>
        ///     <li>File.ext</li>
        ///     <li>File (1).ext</li>
        ///     <li>File (2).ext</li>
        ///     <li>...</li>
        /// </ul>
        /// 
        /// This method uses a linear search to find a unique file name, so it is suitable for situations where
        /// there are a small number of collisions for each file name. This will detect and fill gaps that can
        /// occur when files are deleted (for instance, if "File (1).ext" were deleted from the list above).
        /// </remarks>
        public static string GetUniqueFilePath(string originalFilePath)
        {
            string uniqueFilePath = GetAbsolutePath(originalFilePath);
            string directory = GetDirectoryName(uniqueFilePath);
            string originalFileRoot = GetFileNameWithoutExtension(uniqueFilePath);
            string fileExtension = GetExtension(uniqueFilePath);
            int i = 1;

            while (File.Exists(uniqueFilePath))
            {
                uniqueFilePath = Path.Combine(directory, string.Format("{0} ({1}){2}", originalFileRoot, i, fileExtension));
                i++;
            }

            return uniqueFilePath;
        }

        /// <summary>
        /// Gets a unique file path for the given file by checking for name collisions and
        /// adding a sequence number to the end of the file name if there is a collision.
        /// </summary>
        /// <param name="originalFilePath">The path to the original file before adding the sequence number.</param>
        /// <returns>The unique path to the file.</returns>
        /// <remarks>
        /// This method is designed to handle the case where the user wishes to create a file in a folder
        /// with a given name when there is a possibility that the name is already taken. Using this method,
        /// it is possible to create files with names in the following format:
        /// 
        /// <ul>
        ///     <li>File.ext</li>
        ///     <li>File (1).ext</li>
        ///     <li>File (2).ext</li>
        ///     <li>...</li>
        /// </ul>
        /// 
        /// This method uses a binary search to find a unique file name, so it is suitable for situations where
        /// a large number of files will be created with the same file name, and the next unique file name needs
        /// to be found relatively quickly. It will not always detect gaps in the sequence numbers that can occur
        /// when files are deleted (for instance, if "File (1).ext" were deleted from the list above).
        /// </remarks>
        public static string GetUniqueFilePathWithBinarySearch(string originalFilePath)
        {
            string uniqueFilePath = GetAbsolutePath(originalFilePath);
            string directory = GetDirectoryName(uniqueFilePath);
            string originalFileRoot = GetFileNameWithoutExtension(uniqueFilePath);
            string fileExtension = GetExtension(uniqueFilePath);

            int i = 1;
            int j = 1;
            int k = 1;

            if (!File.Exists(uniqueFilePath))
                return uniqueFilePath;

            while (File.Exists(uniqueFilePath))
            {
                uniqueFilePath = Path.Combine(directory, string.Format("{0} ({1}){2}", originalFileRoot, i, fileExtension));
                j = k;
                k = i;
                i *= 2;
            }

            while (j < k)
            {
                i = (j + k) / 2;
                uniqueFilePath = Path.Combine(directory, string.Format("{0} ({1}){2}", originalFileRoot, i, fileExtension));

                if (File.Exists(uniqueFilePath))
                    j = i + 1;
                else
                    k = i;
            }

            return Path.Combine(directory, string.Format("{0} ({1}){2}", originalFileRoot, k, fileExtension));
        }

        ///// <summary>
        ///// Attempts to get read access on a file.
        ///// </summary>
        ///// <param name="fileName">The file to check for read access.</param>
        ///// <returns>True if read access is obtained; false otherwise.</returns>
        //public static bool TryGetReadLock(string fileName)
        //{
        //    try
        //    {
        //        using (File.OpenRead(fileName))
        //            return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Attempts to get read access on a file.
        ///// </summary>
        ///// <param name="fileName">The file to check for read access.</param>
        ///// <returns>True if read access is obtained; false otherwise.</returns>
        //public static bool TryGetReadLockExclusive(string fileName)
        //{
        //    try
        //    {
        //        using (new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
        //            return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Attempts to get write access on a file.
        ///// </summary>
        ///// <param name="fileName">The file to check for write access.</param>
        ///// <returns>True if write access is obtained; false otherwise.</returns>
        //public static bool TryGetWriteLock(string fileName)
        //{
        //    try
        //    {
        //        using (File.OpenWrite(fileName))
        //            return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Waits for the default duration (5 seconds) for read access on a file.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for to obtain read access.</param>
        //public static void WaitForReadLock(string fileName)
        //{
        //    WaitForReadLock(fileName, 5);
        //}

        ///// <summary>
        ///// Waits for read access on a file for the specified number of seconds.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for to obtain read access.</param>
        ///// <param name="secondsToWait">The time to wait for in seconds to obtain read access on a file. Set to zero to wait infinitely.</param>
        //public static void WaitForReadLock(string fileName, double secondsToWait)
        //{
        //    if (!File.Exists(fileName))
        //        throw new FileNotFoundException("Could not test file lock for \"" + fileName + "\", file does not exist", fileName);

        //    // Keeps trying for a file lock.
        //    double startTime = Common.SystemTimer;

        //    while (!TryGetReadLock(fileName))
        //    {
        //        if (secondsToWait > 0)
        //        {
        //            if (Common.SystemTimer > startTime + secondsToWait)
        //                throw new IOException("Could not open \"" + fileName + "\" for read access, tried for " + secondsToWait + " seconds");
        //        }

        //        // Yields to all other system threads.
        //        Thread.Sleep(250);
        //    }
        //}

        ///// <summary>
        ///// Waits for the default duration (5 seconds) for read access on a file.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for to obtain read access.</param>
        //public static void WaitForReadLockExclusive(string fileName)
        //{
        //    WaitForReadLockExclusive(fileName, 5);
        //}

        ///// <summary>
        ///// Waits for read access on a file for the specified number of seconds.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for to obtain read access.</param>
        ///// <param name="secondsToWait">The time to wait for in seconds to obtain read access on a file. Set to zero to wait infinitely.</param>
        //public static void WaitForReadLockExclusive(string fileName, double secondsToWait)
        //{
        //    if (!File.Exists(fileName))
        //        throw new FileNotFoundException("Could not test file lock for \"" + fileName + "\", file does not exist", fileName);

        //    // Keeps trying for a file lock.
        //    double startTime = Common.SystemTimer;

        //    while (!TryGetReadLockExclusive(fileName))
        //    {
        //        if (secondsToWait > 0)
        //        {
        //            if (Common.SystemTimer > startTime + secondsToWait)
        //                throw new IOException("Could not open \"" + fileName + "\" for read access, tried for " + secondsToWait + " seconds");
        //        }

        //        // Yields to all other system threads.
        //        Thread.Sleep(250);
        //    }
        //}

        ///// <summary>
        ///// Waits for the default duration (5 seconds) for write access on a file.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for to obtain write access.</param>
        //public static void WaitForWriteLock(string fileName)
        //{
        //    WaitForWriteLock(fileName, 5);
        //}

        ///// <summary>
        ///// Waits for write access on a file for the specified number of seconds.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for to obtain write access.</param>
        ///// <param name="secondsToWait">The time to wait for in seconds to obtain write access on a file. Set to zero to wait infinitely.</param>
        //public static void WaitForWriteLock(string fileName, double secondsToWait)
        //{
        //    if (!File.Exists(fileName))
        //        throw new FileNotFoundException("Could not test file lock for \"" + fileName + "\", file does not exist", fileName);

        //    // Keeps trying for a file lock.
        //    double startTime = Common.SystemTimer;

        //    while (!TryGetWriteLock(fileName))
        //    {
        //        if (secondsToWait > 0)
        //        {
        //            if (Common.SystemTimer > startTime + secondsToWait)
        //                throw new IOException("Could not open \"" + fileName + "\" for write access, tried for " + secondsToWait + " seconds");
        //        }

        //        // Yields to all other system threads.
        //        Thread.Sleep(250);
        //    }
        //}

        ///// <summary>
        ///// Waits for the default duration (5 seconds) for a file to exist.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for until it is created.</param>
        //public static void WaitTillExists(string fileName)
        //{
        //    WaitTillExists(fileName, 5);
        //}

        ///// <summary>
        ///// Waits for a file to exist for the specified number of seconds.
        ///// </summary>
        ///// <param name="fileName">The name of the file to wait for until it is created.</param>
        ///// <param name="secondsToWait">The time to wait for in seconds for the file to be created. Set to zero to wait infinitely.</param>
        //public static void WaitTillExists(string fileName, double secondsToWait)
        //{
        //    // Keeps waiting for a file to be created.
        //    double startTime = Common.SystemTimer;

        //    while (!File.Exists(fileName))
        //    {
        //        if (secondsToWait > 0)
        //        {
        //            if (Common.SystemTimer > startTime + secondsToWait)
        //                throw new IOException("Waited for \"" + fileName + "\" to exist for " + secondsToWait + " seconds, but it was never created");
        //        }

        //        // Yields to all other system threads.
        //        Thread.Sleep(250);
        //    }
        //}

        //[DllImport("mpr.dll", EntryPoint = "WNetAddConnection2W", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        //private static extern int WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        //[DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2W", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        //private static extern int WNetCancelConnection2(string lpName, int dwFlags, [MarshalAs(UnmanagedType.Bool)] bool fForce);

        #endregion
    }
}