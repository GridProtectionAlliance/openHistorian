using System.Reflection;
using System.Runtime.InteropServices;

// Informational attributes.

[assembly: AssemblyCompany("Grid Protection Alliance")]
[assembly: AssemblyCopyright("Copyright © 2016.  All Rights Reserved.")]
[assembly: AssemblyProduct("openHistorian")]

// Assembly manifest attributes.
#if DEBUG

[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif

[assembly: AssemblyDescription("Windows service provides archive management and routing services for streaming time-series data in real-time for process applications.")]
[assembly: AssemblyTitle("openHistorian Iaon Host")]

// Other configuration attributes.

[assembly: ComVisible(false)]
[assembly: Guid("f65126e5-e27a-49df-8188-1cde74fe15f3")]

// Assembly identity attributes.

[assembly: AssemblyVersion("2.6.30.0")]
[assembly: AssemblyFileVersion("2.6.30.0")]