using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;


#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml.Extensions
{
	[MarkupExtensionReturnType(typeof(object))]
	public class StaticResourceExtension : MarkupExtension
	{
		[ConstructorArgument("resourceKey")]
		public string ResourceKey { get; set; }

		public StaticResourceExtension()
		{
		}

		public StaticResourceExtension(string resourceKey)
		{
			ResourceKey = resourceKey;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var schemaContextProvider = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
			if (schemaContextProvider == null)
				throw new InvalidOperationException("StaticResource requires a schema context");

			var schemaContext = schemaContextProvider.SchemaContext;

			var ambientProvider = serviceProvider.GetService(typeof(IAmbientProvider)) as IAmbientProvider;
			if (ambientProvider == null)
				throw new InvalidOperationException("StaticResource requires an ambient provider");
			var types = new []
			{
				schemaContext.GetXamlType(typeof(PropertyStore))
			};
			var members = new []
			{
				schemaContext.GetXamlType(typeof(Control)).GetMember("Properties")
			};
			var values = ambientProvider.GetAllAmbientValues(null, true, types, members);
			foreach (var dictionary in values.Select(r => r.Value).OfType<PropertyStore>())
			{
				object val;
				if (dictionary.TryGetValue(ResourceKey, out val))
					return val;
			}
			return null;
		}
	}
}