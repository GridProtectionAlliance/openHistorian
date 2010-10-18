using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

// Assembly identity attributes.
[assembly: AssemblyVersion("1.3.4.*")]

// Informational attributes.
[assembly: AssemblyCompany("Grid Protection Alliance")]
[assembly: AssemblyCopyright("Copyright © 2010.  All Rights Reserved.")]
[assembly: AssemblyProduct("openHistorian")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDefaultAlias("HistorianPlaybackUtility")]
[assembly: AssemblyDescription("Utility for playing back data from historian data files.")]
[assembly: AssemblyTitle("HistorianPlaybackUtility")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("ddd2158e-a37b-425f-8be5-c86c5a99a31a")]