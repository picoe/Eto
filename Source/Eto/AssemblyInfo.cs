using System.Reflection;
using System.Runtime.CompilerServices;
#if XAML
using System.Windows.Markup;
#endif

[assembly: AssemblyTitle("Eto.Forms class libraries")]
[assembly: AssemblyDescription("Eto.Forms class libraries")]

#if XAML
[assembly: XmlnsDefinition ("http://schema.picoe.ca/eto.forms", "Eto.Forms")]
[assembly: XmlnsDefinition ("http://schema.picoe.ca/eto.forms", "Eto.Xaml.Extensions")]
[assembly: XmlnsPrefix ("http://schema.picoe.ca/eto.forms", "eto")]
#endif
