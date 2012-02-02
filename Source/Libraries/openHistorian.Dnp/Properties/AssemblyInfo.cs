using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("4.0.9.0")]
[assembly: AssemblyFileVersion("4.0.9.0")]

// Informational attributes.
[assembly: AssemblyCompany("TVA")]
[assembly: AssemblyCopyright("No copyright is claimed pursuant to 17 USC § 105.  All Other Rights Reserved.")]
[assembly: AssemblyProduct("openHistorian DNP Adapters")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDefaultAlias("openHistorian.Dnp")]
[assembly: AssemblyDescription("Library of openHistorian adapters for DNP protocol.")]
[assembly: AssemblyTitle("openHistorian.Dnp")]

// Other configuration attributes.
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("3ffebffd-8440-4de2-8b20-607ab7d35d33")]
