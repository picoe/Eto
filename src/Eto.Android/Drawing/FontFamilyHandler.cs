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

namespace Eto.Android.Drawing
{
	public class FontFamilyHandler : WidgetHandler<ag.Typeface, FontFamily>, FontFamily.IHandler
	{
		public string Name
		{
			get { return Control.Class.Name; }
		}

		public string LocalizedName
		{
			get { return Name; }
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