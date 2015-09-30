//******************************************************************************************************
//  AssemblyInfo.cs - Gbtc
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
//  04/29/2005 - Pinal C. Patel
//       Generated original version of source code.
//  12/29/2005 - Pinal C. Patel
//       Migrated 2.0 version of source code from 1.1 source (GSF.Shared.Assembly).
//  12/12/2007 - Darrell Zuercher
//       Edited Code Comments.
//  09/08/2008 - J. Ritchie Carroll
//       Converted to C# as AssemblyInformation.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/21/2009 - Pinal C. Patel
//       Added error checking to assembly attribute properties.
//  09/28/2010 - Pinal C. Patel
//       Modified EntryAssembly to perform a reflection only load of the currently executing process 
//       to deal with entry assembly not being available in non-default application domains.
//       Changed GetCustomAttribute() to return CustomAttributeData instead of Object to deal with
//       possible reflection only load being performed in EntryAssembly.
//       Removed debuggable property since it was not very useful and added complexity when extracting.
//  09/21/2011 - J. Ritchie Carroll
//       Added Mono implementation exception regions.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
#if !MONO
using System.Linq;
#endif
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using GSF.IO;

namespace GSF.Reflection
{
    /// <summary>
    /// Represents a common information provider for an assembly.
    /// </summary>
    public class AssemblyInfo
    {
        #region [ Members ]

        // Fields
        private readonly Assembly m_assemblyInstance;

        #endregion

        #region [ Constructors ]

        /// <summary>Initializes a new instance of the <see cref="AssemblyInfo"/> class.</summary>
        /// <param name="assemblyInstance">An <see cref="Assembly"/> object.</param>
        public AssemblyInfo(Assembly assemblyInstance)
        {
            m_assemblyInstance = assemblyInstance;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the underlying <see cref="Assembly"/> being represented by this <see cref="AssemblyInfo"/> object.
        /// </summary>
        public Assembly Assembly
        {
            get
            {
                return m_assemblyInstance;
            }
        }

        /// <summary>
        /// Gets the title information of the <see cref="Assembly"/>.
        /// </summary>
        public string Title
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyTitleAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the description information of the <see cref="Assembly"/>.
        /// </summary>
        public string Description
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyDescriptionAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the company name information of the <see cref="Assembly"/>.
        /// </summary>
        public string Company
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyCompanyAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the product name information of the <see cref="Assembly"/>.
        /// </summary>
        public string Product
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyProductAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the copyright information of the <see cref="Assembly"/>.
        /// </summary>
        public string Copyright
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyCopyrightAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the trademark information of the <see cref="Assembly"/>.
        /// </summary>
        public string Trademark
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyTrademarkAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the configuration information of the <see cref="Assembly"/>.
        /// </summary>
        public string Configuration
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyConfigurationAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating if the <see cref="Assembly"/> has been built as delay-signed.
        /// </summary>
        public bool DelaySign
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyDelaySignAttribute));

                if ((object)attribute == null)
                    return false;

                return (bool)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the version information of the <see cref="Assembly"/>.
        /// </summary>
        public string InformationalVersion
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the name of the file containing the key pair used to generate a strong name for the attributed <see cref="Assembly"/>.
        /// </summary>
        public string KeyFile
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(AssemblyKeyFileAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the culture name of the <see cref="Assembly"/>.
        /// </summary>
        public string CultureName
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(NeutralResourcesLanguageAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the assembly version used to instruct the System.Resources.ResourceManager to ask for a particular
        /// version of a satellite assembly to simplify updates of the main assembly of an application.
        /// </summary>
        public string SatelliteContractVersion
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(SatelliteContractVersionAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the string representing the assembly version used to indicate to a COM client that all classes
        /// in the current version of the assembly are compatible with classes in an earlier version of the assembly.
        /// </summary>
        public string ComCompatibleVersion
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(ComCompatibleVersionAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return attribute.ConstructorArguments[0].Value + "." +
                       attribute.ConstructorArguments[1].Value + "." +
                       attribute.ConstructorArguments[2].Value + "." +
                       attribute.ConstructorArguments[3].Value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating if the <see cref="Assembly"/> is exposed to COM.
        /// </summary>
        public bool ComVisible
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(ComVisibleAttribute));

                if ((object)attribute == null)
                    return false;

                return (bool)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the GUID that is used as an ID if the <see cref="Assembly"/> is exposed to COM.
        /// </summary>
        public string Guid
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(GuidAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return (string)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the string representing the <see cref="Assembly"/> version number in MajorVersion.MinorVersion format.
        /// </summary>
        public string TypeLibVersion
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(TypeLibVersionAttribute));

                if ((object)attribute == null)
                    return string.Empty;

                return attribute.ConstructorArguments[0].Value + "." +
                       attribute.ConstructorArguments[1].Value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the <see cref="Assembly"/> is CLS-compliant.
        /// </summary>
        public bool CLSCompliant
        {
            get
            {
                CustomAttributeData attribute = GetCustomAttribute(typeof(CLSCompliantAttribute));

                if ((object)attribute == null)
                    return false;

                return (bool)attribute.ConstructorArguments[0].Value;
            }
        }

        /// <summary>
        /// Gets the path or UNC location of the loaded file that contains the manifest.
        /// </summary>
        public string Location
        {
            get
            {
                return m_assemblyInstance.Location;
            }
        }

        /// <summary>
        /// Gets the location of the <see cref="Assembly"/> as specified originally.
        /// </summary>
        public string CodeBase
        {
            get
            {
                return m_assemblyInstance.CodeBase.Replace("file:///", "");
            }
        }

        /// <summary>
        /// Gets the display name of the <see cref="Assembly"/>.
        /// </summary>
        public string FullName
        {
            get
            {
                return m_assemblyInstance.FullName;
            }
        }

        /// <summary>
        /// Gets the simple, unencrypted name of the <see cref="Assembly"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return m_assemblyInstance.GetName().Name;
            }
        }

        /// <summary>
        /// Gets the major, minor, revision, and build numbers of the <see cref="Assembly"/>.
        /// </summary>
        public Version Version
        {
            get
            {
                return m_assemblyInstance.GetName().Version;
            }
        }

        /// <summary>
        /// Gets the string representing the version of the common language runtime (CLR) saved in the file
        /// containing the manifest.
        /// </summary>
        public string ImageRuntimeVersion
        {
            get
            {
                return m_assemblyInstance.ImageRuntimeVersion;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the <see cref="Assembly"/> was loaded from the global assembly cache.
        /// </summary>
        public bool GACLoaded
        {
            get
            {
                return m_assemblyInstance.GlobalAssemblyCache;
            }
        }

        /// <summary>
        /// Gets the date and time when the <see cref="Assembly"/> was built.
        /// </summary>
        public DateTime BuildDate
        {
            get
            {
                return File.GetLastWriteTime(m_assemblyInstance.Location);
            }
        }

        /// <summary>
        /// Gets the root namespace of the <see cref="Assembly"/>.
        /// </summary>
        public string RootNamespace
        {
            get
            {
                return m_assemblyInstance.GetExportedTypes()[0].Namespace;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>Gets a collection of assembly attributes exposed by the assembly.</summary>
        /// <returns>A System.Specialized.KeyValueCollection of assembly attributes.</returns>
        public NameValueCollection GetAttributes()
        {
            NameValueCollection assemblyAttributes = new NameValueCollection();

            //Add some values that are not in AssemblyInfo.
            assemblyAttributes.Add("Full Name", FullName);
            assemblyAttributes.Add("Name", Name);
            assemblyAttributes.Add("Version", Version.ToString());
            assemblyAttributes.Add("Image Runtime Version", ImageRuntimeVersion);
            assemblyAttributes.Add("Build Date", BuildDate.ToString());
            assemblyAttributes.Add("Location", Location);
            assemblyAttributes.Add("Code Base", CodeBase);
            assemblyAttributes.Add("GAC Loaded", GACLoaded.ToString());

            //Add all attributes available from AssemblyInfo.
            assemblyAttributes.Add("Title", Title);
            assemblyAttributes.Add("Description", Description);
            assemblyAttributes.Add("Company", Company);
            assemblyAttributes.Add("Product", Product);
            assemblyAttributes.Add("Copyright", Copyright);
            assemblyAttributes.Add("Trademark", Trademark);
            assemblyAttributes.Add("Configuration", Configuration);
            assemblyAttributes.Add("Delay Sign", DelaySign.ToString());
            assemblyAttributes.Add("Informational Version", InformationalVersion);
            assemblyAttributes.Add("Key File", KeyFile);
            assemblyAttributes.Add("Culture Name", CultureName);
            assemblyAttributes.Add("Satellite Contract Version", SatelliteContractVersion);
            assemblyAttributes.Add("Com Compatible Version", ComCompatibleVersion);
            assemblyAttributes.Add("Com Visible", ComVisible.ToString());
            assemblyAttributes.Add("Guid", Guid);
            assemblyAttributes.Add("Type Lib Version", TypeLibVersion);
            assemblyAttributes.Add("CLS Compliant", CLSCompliant.ToString());

            return assemblyAttributes;
        }

        /// <summary>Gets the specified assembly attribute if it is exposed by the assembly.</summary>
        /// <param name="attributeType">Type of the attribute to get.</param>
        /// <returns>The requested assembly attribute if it exists; otherwise null.</returns>
        /// <remarks>
        /// This method always returns <c>null</c> under Mono deployments.
        /// </remarks>
        public CustomAttributeData GetCustomAttribute(Type attributeType)
        {
#if MONO
            return null;
#else
            //Returns the requested assembly attribute.
            return m_assemblyInstance.GetCustomAttributesData().FirstOrDefault(assemblyAttribute => assemblyAttribute.Constructor.DeclaringType == attributeType);
#endif
        }

        ///// <summary>Gets the specified embedded resource from the assembly.</summary>
        ///// <param name="resourceName">The full name (including the namespace) of the embedded resource to get.</param>
        ///// <returns>The embedded resource.</returns>
        //public Stream GetEmbeddedResource(string resourceName)
        //{
        //    //Extracts and returns the requested embedded resource.
        //    return m_assemblyInstance.GetEmbeddedResource(resourceName);
        //}

        #endregion

        #region [ Static ]

        // Static Fields
        private static AssemblyInfo s_callingAssembly;
        private static AssemblyInfo s_entryAssembly;
        private static AssemblyInfo s_executingAssembly;
        private static Dictionary<string, Assembly> s_assemblyCache;
        private static bool s_addedResolver;

        // Static Properties

        /// <summary>Gets the <see cref="AssemblyInfo"/> object of the assembly that invoked the currently executing method.</summary>
        public static AssemblyInfo CallingAssembly
        {
            get
            {
                if ((object)s_callingAssembly == null)
                {
                    // We have to find the calling assembly of the caller.
                    StackTrace trace = new StackTrace();
                    Assembly caller = Assembly.GetCallingAssembly();
                    Assembly current = Assembly.GetExecutingAssembly();

                    StackFrame[] stackFrames = trace.GetFrames();

                    if ((object)stackFrames != null)
                    {
                        foreach (StackFrame frame in stackFrames)
                        {
                            if ((object)frame != null)
                            {
                                MethodBase method = frame.GetMethod();

                                if ((object)method != null && (object)method.DeclaringType != null)
                                {
                                    Assembly assembly = Assembly.GetAssembly(method.DeclaringType);

                                    if (assembly != caller && assembly != current)
                                    {
                                        // Assembly is neither the current assembly or the calling assembly.
                                        s_callingAssembly = new AssemblyInfo(assembly);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                return s_callingAssembly;
            }
        }

        /// <summary>Gets the <see cref="AssemblyInfo"/> object of the process executable in the default application domain.</summary>
        public static AssemblyInfo EntryAssembly
        {
            get
            {
                if ((object)s_entryAssembly == null)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if ((object)entryAssembly == null)
                        entryAssembly = Assembly.ReflectionOnlyLoadFrom(Process.GetCurrentProcess().MainModule.FileName);

                    s_entryAssembly = new AssemblyInfo(entryAssembly);
                }

                return s_entryAssembly;
            }
        }

        /// <summary>Gets the <see cref="AssemblyInfo"/> object of the assembly that contains the code that is currently executing.</summary>
        public static AssemblyInfo ExecutingAssembly
        {
            get
            {
                if ((object)s_executingAssembly == null)
                    // Caller's assembly will be the executing assembly for the caller.
                    s_executingAssembly = new AssemblyInfo(Assembly.GetCallingAssembly());

                return s_executingAssembly;
            }
        }

        // Static Methods

        /// <summary>Loads the specified assembly that is embedded as a resource in the assembly.</summary>
        /// <param name="assemblyName">Name of the assembly to load.</param>
        /// <remarks>This cannot be used to load GSF.Core itself.</remarks>
        [SecurityCritical]
        public static void LoadAssemblyFromResource(string assemblyName)
        {
            // Hooks into assembly resolve event for current domain so it can load assembly from embedded resource.
            if (!s_addedResolver)
            {
                AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyFromResource;
                s_addedResolver = true;
            }

            // Loads the assembly (This will invoke event that will resolve assembly from resource.).
            AppDomain.CurrentDomain.Load(assemblyName);
        }

        private static Assembly ResolveAssemblyFromResource(object sender, ResolveEventArgs e)
        {
            Assembly resourceAssembly;
            string shortName = e.Name.Split(',')[0];

            if ((object)s_assemblyCache == null)
                s_assemblyCache = new Dictionary<string, Assembly>();

            resourceAssembly = s_assemblyCache[shortName];

            if ((object)resourceAssembly == null)
            {
                // Loops through all of the resources in the executing assembly.
                foreach (string name in Assembly.GetEntryAssembly().GetManifestResourceNames())
                {
                    // Sees if the embedded resource name matches the assembly it is trying to load.
                    if (string.Compare(FilePath.GetFileNameWithoutExtension(name), EntryAssembly.RootNamespace + "." + shortName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // If so, loads embedded resource assembly into a binary buffer.
                        Stream resourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(name);

                        if ((object)resourceStream != null)
                        {
                            byte[] buffer = new byte[resourceStream.Length];
                            resourceStream.Read(buffer, 0, (int)resourceStream.Length);
                            resourceStream.Close();

                            // Loads assembly from binary buffer.
                            resourceAssembly = Assembly.Load(buffer);

                            // Add assembly to the cache
                            s_assemblyCache.Add(shortName, resourceAssembly);
                        }
                        break;
                    }
                }
            }

            return resourceAssembly;
        }

        #endregion
    }
}