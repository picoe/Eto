using System.Reflection;

#if XAMMAC2
[assembly: AssemblyTitle("Eto.Forms - Xamarin.Mac v2.0 Platform")]
[assembly: AssemblyDescription("OS X Platform for the Eto.Forms UI Framework using Xamarin.Mac v2.0")]
#elif XAMMAC
[assembly: AssemblyTitle("Eto.Forms - Xamarin.Mac Platform")]
[assembly: AssemblyDescription("OS X Platform for the Eto.Forms UI Framework using Xamarin.Mac")]
#elif Mac64
[assembly: AssemblyTitle("Eto.Forms - MonoMac 64-bit Platform")]
[assembly: AssemblyDescription("OS X Platform for the Eto.Forms UI Framework using the open-source MonoMac with 64-bit mono")]
#else
[assembly: AssemblyTitle("Eto.Forms - MonoMac Platform")]
[assembly: AssemblyDescription("OS X Platform for the Eto.Forms UI Framework using the open-source MonoMac")]
#endif

