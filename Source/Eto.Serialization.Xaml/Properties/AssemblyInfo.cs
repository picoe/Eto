using System.Reflection;
#if !PCL
using System.Windows.Markup;
#endif

[assembly: AssemblyTitle("Eto.Forms Xaml serializer")]
[assembly: AssemblyDescription("Eto.Forms Xaml serializer")]

#if !PCL
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms", AssemblyName="Eto")]
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Serialization.Xaml.Extensions")]
[assembly: XmlnsPrefix(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
#endif
