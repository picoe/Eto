using System;
using System.ComponentModel;
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
			var contextProvider = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
			if (contextProvider != null)
			{
				var context = contextProvider.SchemaContext as EtoXamlSchemaContext;
				if (context != null && context.DesignMode)
					return new Bitmap(24, 24, PixelFormat.Format32bppRgba);
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

			var provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValue != null)
			{
				var propertyInfo = provideValue.TargetProperty as PropertyInfo;
				if (propertyInfo != null && !propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(typeof(Stream).GetTypeInfo()))
				{
					var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
					if (converter != null)
					{
						if (converter.CanConvertFrom(typeof(NamespaceInfo)))
							return converter.ConvertFrom(resource);
						if (converter.CanConvertFrom(typeof(Stream)))
							return converter.ConvertFrom(resource.FindResource());
					}
					var streamArgs = new [] { typeof(Stream) };
					#if NET40
					var constructor = propertyInfo.PropertyType.GetConstructor(streamArgs);
					#else
					var constructor = propertyInfo.PropertyType.GetTypeInfo().DeclaredConstructors.FirstOrDefault(r => r.GetParameters().Select(p => p.ParameterType).SequenceEqual(streamArgs));;
					#endif
					if (constructor != null)
					{
						return constructor.Invoke(new[] { resource.FindResource() });
					}
				}
			}
			Stream stream = null;
			if (resource != null && resource.Assembly != null)
				stream = resource.FindResource();
			return stream;
		}
	}
}