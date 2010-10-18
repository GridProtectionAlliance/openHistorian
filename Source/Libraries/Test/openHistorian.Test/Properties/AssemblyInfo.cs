using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("1.1.34.54206")]

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
[assembly: AssemblyDefaultAlias("openHistorian.Test")]
[assembly: AssemblyDescription("Unit tests for core historian specific components.")]
[assembly: AssemblyTitle("openHistorian.Test")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("f17ded3b-5667-4835-bbd1-dbc10ee2e1d0")]
