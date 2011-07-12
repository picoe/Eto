using System;
using System.Xml;

namespace Eto.Drawing
{
	public static class XmlExtensions
	{
		public static Size? GetSizeAttributes(this XmlElement element, string baseName)
		{
			var width = element.GetIntAttribute (baseName + "-width");
			var height = element.GetIntAttribute (baseName + "-height");
			if (width != null && height != null)
				return new Size(width.Value, height.Value);
			else
				return null;
		}
		
		public static void WriteChildXml(this XmlElement element, string elementName, Size? value)
		{
			if (value != null)
				element.WriteChildXml (elementName, new SizeSaver{ Size = value.Value });
		}

		public static void ReadChildXml(this XmlElement element, string elementName, Size value)
		{
			element.ReadChildXml (elementName, new SizeSaver{ Size = value });
		}

		public static Size? ReadChildSizeXml(this XmlElement element, string elementName)
		{
			var size = element.ReadChildXml<SizeSaver> (elementName);
			if (size != null) return size.Size;
			else return null;
		}
		
		class SizeSaver : IXmlReadable
		{
			public Size Size { get; set; }
			
			#region IXmlReadable implementation
			
			public void ReadXml (XmlElement element)
			{
				var width = element.GetIntAttribute ("width");
				var height = element.GetIntAttribute ("height");
				var size = Size;
				if (width != null) size.Width = width.Value;
				if (height != null) size.Height = height.Value;
				Size = size;
			}

			public void WriteXml (XmlElement element)
			{
				element.SetAttribute ("width", Size.Width);
				element.SetAttribute ("height", Size.Height);
			}
			
			#endregion
			
		}
	}
}

