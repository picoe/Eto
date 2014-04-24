using System.Reflection;
using System;
#if DESKTOP
using System.Runtime.InteropServices;
#endif

[assembly: AssemblyTitle ("Eto.Forms")]
[assembly: AssemblyDescription ("Eto.Forms UI Framework")]
[assembly: CLSCompliant(true)]
#if !PCL
[assembly: ComVisible(false)]
#endif