using System.Reflection;
using System.Windows.Markup;

[assembly: AssemblyTitle("Eto.Forms Xaml serializer")]
[assembly: AssemblyDescription("Eto.Forms Xaml serializer")]

[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms", AssemblyName="Eto")]
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Xaml.Extensions")]
[assembly: XmlnsPrefix(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
