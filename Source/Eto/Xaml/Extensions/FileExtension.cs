#if XAML
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace Eto.Xaml.Extensions
{
	[MarkupExtensionReturnType(typeof(object))]
	public class FileExtension : MarkupExtension
	{
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
			return File.OpenRead(fileName);
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (!string.IsNullOrEmpty(FileName))
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
							if (converter.CanConvertFrom(typeof(string)))
								return converter.ConvertFrom(FileName);
							if (converter.CanConvertFrom(typeof(Stream)))
								return converter.ConvertFrom(GetStream());
						}
					}
				}
				return GetStream();
			}
			return null;
		}
	}
}

#endif