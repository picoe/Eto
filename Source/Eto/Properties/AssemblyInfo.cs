using System.Reflection;
#if XAML
using System.Windows.Markup;
#endif

[assembly: AssemblyTitle ("Eto.Forms")]
[assembly: AssemblyDescription ("Eto.Forms UI Framework")]

#if XAML
[assembly: XmlnsDefinition (Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms")]
[assembly: XmlnsDefinition (Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Xaml.Extensions")]
[assembly: XmlnsPrefix (Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
#endif