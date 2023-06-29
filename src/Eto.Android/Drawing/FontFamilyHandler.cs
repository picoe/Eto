using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using at = Android.Text;
using ag = Android.Graphics;

namespace Eto.Android.Drawing
{
	public class FontFamilyHandler : WidgetHandler<ag.Typeface, FontFamily>, FontFamily.IHandler
	{
		private string name;

		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(string familyName)
		{
			name = familyName;
		}

		public string Name => name;

		public string LocalizedName => Name;
		
		public IEnumerable<FontTypeface> Typefaces
		{
			get 
				{
				var Styles = new[]
					{
					ag.TypefaceStyle.Normal,
					ag.TypefaceStyle.Bold,
					ag.TypefaceStyle.Italic,
					ag.TypefaceStyle.BoldItalic
					};

				return Styles
					.Select(s => new FontTypeface(Widget, new FontTypefaceHandler(s)))
					.ToArray();
				}
		}

		public void Create(string familyName)
		{
			name = familyName;
			Control = ag.Typeface.Create(familyName, ag.TypefaceStyle.Normal); // the style doesn't matter
		}

		public void CreateFromFiles(IEnumerable<string> fileNames)
		{
			throw new NotImplementedException();
		}

		public void CreateFromStreams(IEnumerable<System.IO.Stream> streams)
		{
			throw new NotImplementedException();
		}
	}
}
