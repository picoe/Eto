using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Eto.Drawing;
using System.Linq;


#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml.Extensions
{
	[MarkupExtensionReturnType(typeof(Font))]
	public class FontExtension : MarkupExtension
	{
		[ConstructorArgument("family")]
		public string Family { get; set; }

		[ConstructorArgument("size")]
		public float? Size { get; set; }

		[ConstructorArgument("typeface")]
		public string Typeface { get; set; }

		[ConstructorArgument("style")]
		public FontStyle Style { get; set; }

		[ConstructorArgument("decoration")]
		public FontDecoration Decoration { get; set; }

		public SystemFont? SystemFont { get; set; }

		public FontExtension()
		{
		}

		public FontExtension(string family, float? size = null, string typeface = null, FontStyle style = FontStyle.None, FontDecoration decoration = FontDecoration.None)
		{
			Family = family;
			Typeface = typeface;
			Size = size;
			Style = style;
			Decoration = decoration;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (SystemFont != null)
			{
				return new Font(SystemFont.Value, Size, Decoration);
			}
			var size = Size ?? SystemFonts.Default().Size;
			var familyName = Family ?? SystemFonts.Default().FamilyName;
			var family = new FontFamily(familyName);

			if (!string.IsNullOrEmpty(Typeface))
			{
				var typeface = family.Typefaces.FirstOrDefault(r => string.Equals(r.Name, Typeface, StringComparison.OrdinalIgnoreCase));
				if (typeface != null)
					return new Font(typeface, size, Decoration);
			}
			return new Font(family, size, Style, Decoration);
		}
	}
}