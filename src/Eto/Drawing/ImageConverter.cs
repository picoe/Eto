using System;
using sc = System.ComponentModel;
using System.IO;

namespace Eto.Drawing
{
	class IconConverter : ImageConverterInternal
	{
		protected override bool IsIcon(string fileName)
		{
			return true;
		}
	}

	class BitmapConverter : ImageConverterInternal
	{
		protected override bool IsIcon(string fileName)
		{
			return false;
		}
	}

	/// <summary>
	/// Converter to convert a string to an <see cref="Image"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	class ImageConverterInternal : sc.TypeConverter
	{
		/// <summary>
		/// Prefix to use to load an image from a resource of an assembly
		/// </summary>
		public const string ResourcePrefix = "resource:";

		/// <summary>
		/// Prefix to use to load an image from a file path
		/// </summary>
		public const string FilePrefix = "file:";

		/// <summary>
		/// Determines whether the specified fileName is an icon (ends with .ico)
		/// </summary>
		/// <returns><c>true</c> if the fileName is an icon; otherwise, <c>false</c>.</returns>
		/// <param name="fileName">File name.</param>
		protected virtual bool IsIcon (string fileName)
		{
			return fileName.EndsWith (".ico", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets a value indicating that this converter can convert from the source type to an image
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can handle converting from the specified <paramref name="sourceType"/> to an image</returns>
		public override bool CanConvertFrom (sc.ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || typeof(NamespaceInfo).IsAssignableFrom(sourceType) || typeof(Stream).IsAssignableFrom(sourceType);
		}

		/// <summary>
		/// Gets a value indicating that this converter can convert to the specified type.
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter can convert to the specified <paramref name="destinationType"/>, otherwise false.</returns>
		public override bool CanConvertTo(sc.ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		static Image LoadImage (Stream stream)
		{
			return stream == null ? null : new Bitmap(stream);
		}

		Image LoadImage (NamespaceInfo ns)
		{
			var isIcon = IsIcon(ns.Namespace);
			if (isIcon)
				return Icon.FromResource(ns.Namespace, ns.Assembly);
			return Bitmap.FromResource(ns.Namespace, ns.Assembly);
		}

		Image LoadImage (string resourceName)
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
		public override object ConvertFrom (sc.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is NamespaceInfo ns)
				return LoadImage(ns);

			if (value is Stream stream)
				return LoadImage(stream);

			if (value is string val) 
				return LoadImage(val);

			return null;
		}
	}


	/// <summary>
	/// Converter to convert a string to an <see cref="Image"/>
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Obsolete("Since 2.5. Use TypeDescriptor.GetConverter instead")]
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

		/// <summary>
		/// Determines whether the specified fileName is an icon (ends with .ico)
		/// </summary>
		/// <returns><c>true</c> if the fileName is an icon; otherwise, <c>false</c>.</returns>
		/// <param name="fileName">File name.</param>
		protected virtual bool IsIcon (string fileName)
		{
			return fileName.EndsWith (".ico", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets a value indicating that this converter can convert from the source type to an image
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="sourceType">Type to convert from</param>
		/// <returns>True if this converter can handle converting from the specified <paramref name="sourceType"/> to an image</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || typeof(NamespaceInfo).IsAssignableFrom(sourceType) || typeof(Stream).IsAssignableFrom(sourceType);
		}

		/// <summary>
		/// Gets a value indicating that this converter can convert to the specified type.
		/// </summary>
		/// <param name="context">Conversion context</param>
		/// <param name="destinationType">Type to convert to</param>
		/// <returns>True if this converter can convert to the specified <paramref name="destinationType"/>, otherwise false.</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return false;
		}

		static Image LoadImage (Stream stream)
		{
			return stream == null ? null : new Bitmap(stream);
		}

		Image LoadImage (NamespaceInfo ns)
		{
			var isIcon = IsIcon(ns.Namespace);
			if (isIcon)
				return Icon.FromResource(ns.Namespace, ns.Assembly);
			return Bitmap.FromResource(ns.Namespace, ns.Assembly);
		}

		Image LoadImage (string resourceName)
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
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is NamespaceInfo ns)
				return LoadImage(ns);

			if (value is Stream stream)
				return LoadImage(stream);

			if (value is string val)
				return LoadImage(val);

			return null;
		}
	}
}
