using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.

[assembly: AssemblyVersion("2.6.30.0")]

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

[assembly: AssemblyDescription("Remote console application for windows service that hosts input, action and output adapters.")]
[assembly: AssemblyTitle("openHistorian Remote Console")]

// Other configuration attributes.

[assembly: ComVisible(false)]
[assembly: Guid("1db0288f-db88-45d5-9187-5ab60e4dba69")]