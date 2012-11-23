using System;
using System.Xml;

namespace Eto.Drawing
{
	/// <summary>
	/// Xml extensions to read/write Eto.Drawing structs to xml
	/// </summary>
	public static class XmlExtensions
	{
		/// <summary>
		/// Gets a <see cref="Size"/> struct as a set of attributes of the specified <paramref name="element"/>
		/// </summary>
		/// <remarks>
		/// This will read attributes with suffixes "-width" and "-height" prefixed by <paramref name="baseName"/>.
		/// For example, if you specify "myProperty" as the base name, then it will read attributes "myProperty-width" and "myProperty-height".
		/// 
		/// Both the width and height must be specified as attributes for this to return a value.
		/// </remarks>
		/// <param name="element">Element to read the width and height attributes from</param>
		/// <param name="baseName">Base attribute name prefix</param>
		/// <returns>A size struct if both the width and height attributes are specified, or null otherwise</returns>
		public static Size? GetSizeAttributes(this XmlElement element, string baseName)
		{
			var width = element.GetIntAttribute (baseName + "-width");
			var height = element.GetIntAttribute (baseName + "-height");
			if (width != null && height != null)
				return new Size(width.Value, height.Value);
			else
				return null;
		}

		/// <summary>
		/// Sets attributes on the specified <paramref name="element"/> with width and height attributes of the specified value
		/// </summary>
		/// <remarks>
		/// This will write attributes with suffixes "-width" and "-height" prefixed by <paramref name="baseName"/>.
		/// For example, if you specify "myProperty" as the base name, then it will write attributes "myProperty-width" and "myProperty-height".
		/// 
		/// Passing null as the size will not write either attribute value.
		/// </remarks>
		/// <param name="element">Element to write the width and height attributes on</param>
		/// <param name="baseName">Base attribute name prefix</param>
		/// <param name="value">Value to set the width and height attributes, if not null</param>
		public static void SetSizeAttributes (this XmlElement element, string baseName, Size? value)
		{
			if (value != null) {
				element.SetAttribute (baseName + "-width", value.Value.Width);
				element.SetAttribute (baseName + "-height", value.Value.Height);
			}
		}

		/// <summary>
		/// Writes the specified size <paramref name="value"/> to a child of the specified <paramref name="element"/> with the given name
		/// </summary>
		/// <remarks>
		/// The child element will contain "width" and "height" attributes for the value of the size.
		/// If the value is null, no child element will be written.
		/// </remarks>
		/// <param name="element">Element to append the child element to if <paramref name="value"/> is not null</param>
		/// <param name="elementName">Name of the element to append</param>
		/// <param name="value">Size value to write</param>
		public static void WriteChildSizeXml (this XmlElement element, string elementName, Size? value)
		{
			if (value != null)
				element.WriteChildXml (elementName, new SizeSaver { Size = value.Value });
		}

		/// <summary>
		/// Reads a child of the <paramref name="element"/> with the given <paramref name="elementName"/> as a <see cref="Size"/>
		/// </summary>
		/// <remarks>
		/// The child element must contain both "width" and "height" attributes for the value of the size.
		/// </remarks>
		/// <param name="element">Element to read from</param>
		/// <param name="elementName">Name of the element to read into the Size struct</param>
		/// <returns>A new Size struct if the element exists, or null if not</returns>
		public static Size? ReadChildSizeXml (this XmlElement element, string elementName)
		{
			var size = element.ReadChildXml<SizeSaver> (elementName);
			if (size != null)
				return size.Size;
			else
				return null;
		}

		class SizeSaver : IXmlReadable
		{
			public Size Size { get; set; }

			public void ReadXml (XmlElement element)
			{
				var width = element.GetIntAttribute ("width") ?? 0;
				var height = element.GetIntAttribute ("height") ?? 0;
				Size = new Size (width, height);
			}

			public void WriteXml (XmlElement element)
			{
				element.SetAttribute ("width", Size.Width);
				element.SetAttribute ("height", Size.Height);
			}
		}
	}
}

