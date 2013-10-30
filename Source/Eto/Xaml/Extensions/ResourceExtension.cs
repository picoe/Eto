#if XAML
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace Eto.Xaml.Extensions
{
	[MarkupExtensionReturnType (typeof (object))]
	public class ResourceExtension : MarkupExtension
	{
		public NamespaceInfo Resource { get; set; }

		public ResourceExtension ()
		{
		}

		public ResourceExtension (string resourceName)
		{
			this.Resource = new NamespaceInfo (resourceName);
		}

		public ResourceExtension (string resourceName, string assemblyName)
		{
			if (!string.IsNullOrEmpty (assemblyName))
				this.Resource = new NamespaceInfo (resourceName, Assembly.Load (assemblyName));
			else
				this.Resource = new NamespaceInfo (resourceName);
		}

		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			var provideValue = serviceProvider.GetService (typeof (IProvideValueTarget)) as IProvideValueTarget;
			if (provideValue != null)
			{
				var propertyInfo = provideValue.TargetProperty as PropertyInfo;
				if (propertyInfo != null && !propertyInfo.PropertyType.IsAssignableFrom (typeof(Stream)))
				{
					var converter = TypeDescriptor.GetConverter (propertyInfo.PropertyType);
					if (converter != null)
					{
						if (converter.CanConvertFrom (typeof (NamespaceInfo)))
							return converter.ConvertFrom (Resource);
						if (converter.CanConvertFrom (typeof (Stream)))
							return converter.ConvertFrom (Resource.FindResource ());
					}
				}
			}
			Stream stream = null;
			if (Resource != null && Resource.Assembly != null)
				stream = Resource.FindResource ();
			return stream;
		}
	}
}

#endif