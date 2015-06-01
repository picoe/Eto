using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace Eto.Serialization.Xaml.Extensions
{
	[MarkupExtensionReturnType(typeof(object))]
	public class ResourceExtension : MarkupExtension
	{
		public NamespaceInfo Resource { get; set; }

		public ResourceExtension()
		{
		}

		public ResourceExtension(string resourceName)
		{
			Resource = new NamespaceInfo(resourceName);
		}

		public ResourceExtension(string resourceName, string assemblyName)
		{
			if (!string.IsNullOrEmpty(assemblyName))
				Resource = new NamespaceInfo(resourceName, Assembly.Load(assemblyName));
			else
				Resource = new NamespaceInfo(resourceName);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValue != null)
			{
				var propertyInfo = provideValue.TargetProperty as PropertyInfo;
				if (propertyInfo != null && !propertyInfo.PropertyType.IsAssignableFrom(typeof(Stream)))
				{
					var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
					if (converter != null)
					{
						if (converter.CanConvertFrom(typeof(NamespaceInfo)))
							return converter.ConvertFrom(Resource);
						if (converter.CanConvertFrom(typeof(Stream)))
							return converter.ConvertFrom(Resource.FindResource());
					}
					var constructor = propertyInfo.PropertyType.GetConstructor(new[] { typeof(Stream) });
					if (constructor != null)
					{
						return constructor.Invoke(new[] { Resource.FindResource() });
					}
				}
			}
			Stream stream = null;
			if (Resource != null && Resource.Assembly != null)
				stream = Resource.FindResource();
			return stream;
		}
	}
}