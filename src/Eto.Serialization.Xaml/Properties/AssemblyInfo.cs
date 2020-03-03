using System.Reflection;
#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

//#if !PCL
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Serialization.Xaml.Extensions")]
[assembly: XmlnsDefinition(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms", AssemblyName="Eto")]
[assembly: XmlnsPrefix(Eto.Serialization.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
//#endif
