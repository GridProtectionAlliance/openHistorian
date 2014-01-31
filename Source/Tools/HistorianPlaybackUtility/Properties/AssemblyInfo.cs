using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("2.0.35.0")]

// Informational attributes.
[assembly: AssemblyTitle("HistorianPlaybackUtility")]
[assembly: AssemblyDescription("Utility for playing back and/or exporting data from historian data files.")]
[assembly: AssemblyCompany("Grid Protection Alliance")]
[assembly: AssemblyCopyright("Copyright © 2011, Grid Protection Alliance.  All Rights Reserved.")]
[assembly: AssemblyProduct("openHistorian")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDefaultAlias("HistorianPlaybackUtility")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("ddd2158e-a37b-425f-8be5-c86c5a99a31a")]