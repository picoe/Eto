using System;
using sc = System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Linq;
using Eto.Drawing;

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
	public class ResourceExtension : MarkupExtension
	{
		[ConstructorArgument("resourceName")]
		public string ResourceName { get; set; }

		[ConstructorArgument("assemblyName")]
		public string AssemblyName { get; set; }

		public ResourceExtension()
		{
		}

		public ResourceExtension(string resourceName)
		{
			ResourceName = resourceName;
		}

		public ResourceExtension(string resourceName, string assemblyName)
		{
			ResourceName = resourceName;
			AssemblyName = assemblyName;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			var propertyInfo = provideValue?.TargetProperty as PropertyInfo;

			var contextProvider = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
			if (contextProvider != null)
			{
				var context = contextProvider.SchemaContext as EtoXamlSchemaContext;
				if (context != null && context.DesignMode)
				{
					// TODO: Lookup resource file using ide service so we can actually show the image or use the resource
					if (propertyInfo != null)
					{
						if (typeof(Bitmap).IsAssignableFrom(propertyInfo.PropertyType))
							return new Bitmap(24, 24, PixelFormat.Format32bppRgba);
						
						if (typeof(Icon).IsAssignableFrom(propertyInfo.PropertyType))
							return new Icon(1, new Bitmap(24, 24, PixelFormat.Format32bppRgba));

						// return its current value if not a known type to handle
						return propertyInfo.GetValue(provideValue.TargetObject, null);
					}
					return null;
				}
			}

			Assembly assembly;
			NamespaceInfo resource;
			if (!string.IsNullOrEmpty(AssemblyName))
			{
				assembly = Assembly.Load(new AssemblyName(AssemblyName));
				resource = new NamespaceInfo(ResourceName, assembly);
			}
			else
			{
				try
				{
					resource = new NamespaceInfo(ResourceName);
				}
				catch (ArgumentException)
				{
					var rootProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
					if (rootProvider == null)
						throw;
					assembly = rootProvider.RootObject.GetType().GetTypeInfo().Assembly; 
					resource = new NamespaceInfo(ResourceName, assembly);
				}
			}

			if (propertyInfo != null && !propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(typeof(Stream).GetTypeInfo()))
			{
				var converter = sc.TypeDescriptor.GetConverter(propertyInfo.PropertyType);
				if (converter != null)
				{
					if (converter.CanConvertFrom(typeof(NamespaceInfo)))
						return converter.ConvertFrom(resource);
					if (converter.CanConvertFrom(typeof(Stream)))
						return converter.ConvertFrom(resource.FindResource());
				}
#pragma warning disable 618
				var etoConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
				if (etoConverter != null)
				{
					if (etoConverter.CanConvertFrom(typeof(NamespaceInfo)))
						return etoConverter.ConvertFrom(resource);
					if (etoConverter.CanConvertFrom(typeof(Stream)))
						return etoConverter.ConvertFrom(resource.FindResource());
				}
#pragma warning restore 618

				var streamArgs = new [] { typeof(Stream) };
				var constructor = propertyInfo.PropertyType.GetConstructor(streamArgs);
				if (constructor != null)
				{
					return constructor.Invoke(new[] { resource.FindResource() });
				}
			}
			Stream stream = null;
			if (resource != null && resource.Assembly != null)
				stream = resource.FindResource();
			return stream;
		}
	}
}