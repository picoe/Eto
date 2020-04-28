using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.GtkSharp.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<Pango.FontFace, FontTypeface>, FontTypeface.IHandler
	{
		public FontTypefaceHandler(Pango.FontFace pangoFace)
		{
			Control = pangoFace;
		}

		public string Name => Control?.FaceName;

		public string LocalizedName => Name;

		public FontStyle FontStyle
		{
			get
			{
				var style = FontStyle.None;
				if (Control == null)
					return style;

				var description = Control.Describe();
				if (description.Style == Pango.Style.Italic || description.Style == Pango.Style.Oblique)
					style |= FontStyle.Italic;
				if ((int)description.Weight >= (int)Pango.Weight.Semibold)
					style |= FontStyle.Bold;
				return style;
			}
		}

		public bool IsSymbol => false; // todo, how do we get font info?

		static Pango.AttrList noFallbackAttributes;
		static object noFallbackLock = new object();

		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges)
		{
			var desc = Control.Describe();

			if (noFallbackAttributes == null)
			{
				lock (noFallbackLock)
				{
					if (noFallbackAttributes != null)
					{
						noFallbackAttributes = new Pango.AttrList();
						noFallbackAttributes.Change(new Pango.AttrFallback(false));
					}
				}
			}

			using (var layout = new Pango.Layout(FontsHandler.Context))
			{
				layout.FontDescription = desc;
				layout.Attributes = noFallbackAttributes;
				foreach (var range in ranges)
				{
					var text = new string(Enumerable.Range(range.Start, range.Length()).Select(c => (char)c).ToArray());
					layout.SetText(text);
					if (layout.UnknownGlyphsCount > 0)
						return false;
				}
			}
			return true;
		}
	}
}

