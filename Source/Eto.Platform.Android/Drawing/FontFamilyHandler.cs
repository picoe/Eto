using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Platform.Android.Drawing
{
	public class FontFamilyHandler : WidgetHandler<ag.Typeface, FontFamily>, IFontFamily
	{
		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerable<FontTypeface> Typefaces
		{
			get { throw new NotImplementedException(); }
		}

		public void Create(string familyName)
		{
			Control = ag.Typeface.Create(familyName, ag.TypefaceStyle.Normal); // the style doesn't matter
		}
	}
}