using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android.Drawing
{
	class FontTypefaceHandler : WidgetHandler<ag.TypefaceStyle, FontTypeface>, FontTypeface.IHandler
	{
		public FontTypefaceHandler(ag.TypefaceStyle style)
		{
			this.Control = style;
			Name = this.FontStyle.ToString().Replace(',', ' ');
		}

		public string Name { get; set; }

		public FontStyle FontStyle
		{
			get { return Control.ToEto(); }
		}

		public String LocalizedName => Name;

		// TODO: Eto.Android.Drawing.FontTypeFaceHandler.IsSymbol not implemented
		public Boolean IsSymbol => false;

		// TODO: Eto.Android.Drawing.FontTypeFaceHandler.HasCharacterRanges not implemented
		public Boolean HasCharacterRanges(IEnumerable<Range<Int32>> ranges) => false;

		public FontFamily Family { get; private set; }

		public void Create(FontFamily family)
		{
			Family = family;
		}

		public void Create(Stream stream)
		{
			throw new NotImplementedException();
		}

		public void Create(string fileName)
		{
			throw new NotImplementedException();
		}
	}
}