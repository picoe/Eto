using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Eto.Drawing
{
	public class ImageConverter : TypeConverter
	{
		public const string ResourcePrefix = "resource:";
		public const string FilePrefix = "file:";

		static bool IsIcon (string fileName)
		{
			return fileName.EndsWith (".ico");
		}

		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string) || sourceType == typeof(NamespaceInfo) || sourceType == typeof(Stream) || base.CanConvertFrom (context, sourceType);
		}

		static Image LoadImage (Stream stream)
		{
			if (stream != null)
				return new Bitmap (stream);
			else
				return null;
		}

		static Image LoadImage (NamespaceInfo ns)
		{
			var isIcon = IsIcon (ns.Namespace);
			if (isIcon)
				return Icon.FromResource (ns.Assembly, ns.Namespace);
			else
				return Bitmap.FromResource (ns.Assembly, ns.Namespace);
		}

		static Image LoadImage (string resourceName)
		{
			if (resourceName.StartsWith (ResourcePrefix)) {
				resourceName = resourceName.Substring (ResourcePrefix.Length);
				return LoadImage (new NamespaceInfo (resourceName));
			}
			else
			{
				var fileName = resourceName;
				if (fileName.StartsWith(FilePrefix))
					fileName = fileName.Substring (FilePrefix.Length);
				if (!Path.IsPathRooted (fileName))
					fileName = Path.Combine (EtoEnvironment.GetFolderPath (EtoSpecialFolder.ApplicationResources), fileName);
				if (IsIcon (fileName))
					return new Icon (fileName);
				else
					return new Bitmap (fileName);
			}
		}

		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var ns = value as NamespaceInfo;
			if (ns != null)
				return LoadImage (ns);

			var val = value as string;
			if (val != null)
				return LoadImage (val);
			return null;
		}
	}
}
