//******************************************************************************************************
//  AssemblyInfo.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  05/09/2013 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("HistorianView2")]
[assembly: AssemblyDefaultAlias("HistorianView")]
[assembly: AssemblyDescription("Utility for Trending and Exporting Data from Historian Data Files.")]
[assembly: AssemblyCompany("Grid Protection Alliance")]
[assembly: AssemblyProduct("openHistorian")]
[assembly: AssemblyCopyright("Copyright © GPA, 2016.  All Rights Reserved.")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("2.6.30.0")]
[assembly: AssemblyVersion("2.6.30.0")]
[assembly: AssemblyFileVersion("2.6.30.0")]
