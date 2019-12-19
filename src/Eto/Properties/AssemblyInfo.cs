using System.Reflection;
using System;

[assembly: CLSCompliant(true)]
#if !NETSTANDARD
[assembly: System.Runtime.InteropServices.ComVisible(false)]
#endif