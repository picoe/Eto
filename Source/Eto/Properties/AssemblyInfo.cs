using System.Reflection;
using System;

#if XAML
using System.Windows.Markup;
using System.Runtime.InteropServices;

[assembly: XmlnsDefinition(Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms")]
[assembly: XmlnsDefinition(Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Xaml.Extensions")]
[assembly: XmlnsPrefix(Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
#endif

[assembly: AssemblyTitle ("Eto.Forms")]
[assembly: AssemblyDescription ("Eto.Forms UI Framework")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]