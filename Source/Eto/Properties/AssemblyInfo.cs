using System.Reflection;
using System;
#if DESKTOP
using System.Runtime.InteropServices;
#endif
#if XAML
using System.Windows.Markup;

[assembly: XmlnsDefinition(Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Forms")]
[assembly: XmlnsDefinition(Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "Eto.Xaml.Extensions")]
[assembly: XmlnsPrefix(Eto.Xaml.EtoXamlSchemaContext.EtoFormsNamespace, "eto")]
#endif

[assembly: AssemblyTitle ("Eto.Forms")]
[assembly: AssemblyDescription ("Eto.Forms UI Framework")]
[assembly: CLSCompliant(true)]
#if DESKTOP
[assembly: ComVisible(false)]
#endif