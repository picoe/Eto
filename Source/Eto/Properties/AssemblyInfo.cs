using System.Reflection;
using System.Runtime.CompilerServices;
#if XAML
using System.Windows.Markup;
#endif

[assembly: AssemblyTitle ("Eto.Forms class libraries")]
[assembly: AssemblyDescription ("Eto.Forms class libraries")]

#if XAML
[assembly: XmlnsDefinition (Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms")]
[assembly: XmlnsDefinition (Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Xaml.Extensions")]
[assembly: XmlnsPrefix (Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
#endif