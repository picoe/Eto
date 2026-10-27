#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Serialization.Xaml.Extensions")]
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms", AssemblyName="Eto")]
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto", AssemblyName="Eto")]
[assembly: XmlnsPrefix(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.DesignNamespace, "Eto.Serialization.Xaml.Design")]
[assembly: XmlnsPrefix(Eto.Serialization.Xaml.EtoXamlSchemaContext.DesignNamespace, "d")]
