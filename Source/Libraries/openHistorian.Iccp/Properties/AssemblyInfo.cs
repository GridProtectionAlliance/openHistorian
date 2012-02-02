using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("4.0.9.0")]
[assembly: AssemblyFileVersion("4.0.9.0")]

// Informational attributes.
[assembly: AssemblyCompany("TVA")]
[assembly: AssemblyCopyright("No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.")]
[assembly: AssemblyProduct("openHistorian ICCP Adapters")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDefaultAlias("openHistorian.Iccp")]
[assembly: AssemblyDescription("Library of openHistorian adapters for ICCP protocol.")]
[assembly: AssemblyTitle("openHistorian.Iccp")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("66c2952e-2bfe-422b-abf9-e82890174370")]
