using System.Reflection;
using System.Runtime.CompilerServices;
#if DESKTOP
using System.Windows.Markup;
#endif

[assembly: AssemblyTitle("Eto.Forms class libraries")]
[assembly: AssemblyDescription("Eto.Forms class libraries")]

#if DESKTOP
[assembly: XmlnsDefinition("http://schema.picoe.ca/eto.forms", "Eto.Forms")]
[assembly: XmlnsPrefix("http://schema.picoe.ca/eto.forms", "eto")]
#endif
