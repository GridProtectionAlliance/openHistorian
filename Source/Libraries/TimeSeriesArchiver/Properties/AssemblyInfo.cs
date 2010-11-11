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
[assembly: AssemblyDefaultAlias("openHistorian")]
[assembly: AssemblyDescription("Core historian specific components.")]
[assembly: AssemblyTitle("TimeSeriesArchiver")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("b058b8eb-e73e-414d-a5b2-461181db0de2")]
