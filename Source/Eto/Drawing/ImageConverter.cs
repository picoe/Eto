using System;
using System.ComponentModel;
using System.IO;

namespace Eto.Drawing
{
	/// <summary>
	/// Converter to convert a string to an <see cref="Image"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ImageConverter : TypeConverter
	{
		/// <summary>
		/// Prefix to use to load an image from a resource of an assembly
		/// </summary>
		public const string ResourcePrefix = "resource:";

		/// <summary>
		/// Prefix to use to load an image from a file path
		/// </summary>
		public const string FilePrefix = "file:";

		static bool IsIcon (string fileName)
		{
			return fileName.EndsWith (".ico", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets a value indicating that this converter can convert from the source type to an image
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can handle converting from the specified <paramref name="sourceType"/> to an image</returns>
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string) || sourceType == typeof(NamespaceInfo) || sourceType == typeof(Stream) || base.CanConvertFrom (context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		static Image LoadImage (Stream stream)
		{
			return stream == null ? null : new Bitmap(stream);
		}

		static Image LoadImage (NamespaceInfo ns)
		{
			var isIcon = IsIcon(ns.Namespace);
			if (isIcon)
				return Icon.FromResource(ns.Namespace, ns.Assembly);
			return Bitmap.FromResource(ns.Namespace, ns.Assembly);
		}

		static Image LoadImage (string resourceName)
		{
			if (resourceName.StartsWith (ResourcePrefix, StringComparison.OrdinalIgnoreCase)) {
				resourceName = resourceName.Substring (ResourcePrefix.Length);
				return LoadImage (new NamespaceInfo (resourceName));
			}
			else
			{
				var fileName = resourceName;
				if (fileName.StartsWith(FilePrefix, StringComparison.OrdinalIgnoreCase))
					fileName = fileName.Substring(FilePrefix.Length);
				if (!Path.IsPathRooted(fileName))
					fileName = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), fileName);
				if (IsIcon(fileName))
					return new Icon(fileName);
				return new Bitmap(fileName);
			}
		}

		/// <summary>
		/// Performs the conversion from the given <paramref name="value"/> to an <see cref="Image"/> object
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="culture">Culture to perform the conversion</param>
		/// <param name="value">Value to convert to an image</param>
		/// <returns>A new instance of an image, or null if it cannot be converted</returns>
		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var ns = value as NamespaceInfo;
			if (ns != null)
				return LoadImage (ns);

			var val = value as string;
			return val == null ? null : LoadImage(val);
		}
	}
}
