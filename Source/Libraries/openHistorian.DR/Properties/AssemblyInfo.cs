using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("4.0.9.0")]
[assembly: AssemblyFileVersion("4.0.9.0")]

// Informational attributes.
[assembly: AssemblyCompany("TVA")]
[assembly: AssemblyCopyright("No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.")]
[assembly: AssemblyProduct("openHistorian Demand Response Adapters")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDefaultAlias("openHistorian.DR")]
[assembly: AssemblyDescription("Library of openHistorian adapters for demand response.")]
[assembly: AssemblyTitle("openHistorian.DR")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("48638d90-e50d-4f4d-979d-05ab66c026d8")]
