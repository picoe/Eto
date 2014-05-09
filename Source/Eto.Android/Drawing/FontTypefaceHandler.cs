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
	}
}