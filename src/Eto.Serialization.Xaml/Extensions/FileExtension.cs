using System;
using sc = System.ComponentModel;
using System.IO;
using System.Reflection;
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
	public class FileExtension : MarkupExtension
	{
		[ConstructorArgument("fileName")]
		public string FileName { get; set; }

		public FileExtension()
		{
		}

		public FileExtension(string fileName)
		{
			this.FileName = fileName;
		}

		Stream GetStream()
		{
			var fileName = FileName;
			if (!Path.IsPathRooted(fileName))
				fileName = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), fileName);
			return null;//*PCL File.OpenRead(fileName);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (!string.IsNullOrEmpty(FileName))
			{
				var provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
				if (provideValue != null)
				{
					var propertyInfo = provideValue.TargetProperty as PropertyInfo;
					if (propertyInfo != null && !propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(typeof(Stream).GetTypeInfo()))
					{
						var converter = sc.TypeDescriptor.GetConverter(propertyInfo.PropertyType);
						if (converter != null)
						{
							if (converter.CanConvertFrom(typeof(string)))
								return converter.ConvertFrom(FileName);
							if (converter.CanConvertFrom(typeof(Stream)))
								return converter.ConvertFrom(GetStream());
						}

#pragma warning disable 618
						var etoConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
						if (etoConverter != null)
						{
							if (etoConverter.CanConvertFrom(typeof(string)))
								return etoConverter.ConvertFrom(FileName);
							if (etoConverter.CanConvertFrom(typeof(Stream)))
								return etoConverter.ConvertFrom(GetStream());
						}
#pragma warning restore 618
					}
				}
				return GetStream();
			}
			return null;
		}
	}
}