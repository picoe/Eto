using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Handlers.Drawing
{
	/// <summary>
	/// A mock IFont implementation.
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TestFontHandler : WidgetHandler<Font>, Font.IHandler
	{
		const float PointsToPixels = 96f / 72f;

		public float XHeight { get; set; }
		public float Ascent { get; set; }
		public float Descent { get; set; }
		public float LineHeight { get; private set; }
		public float Leading { get; set; }
		public float Baseline { get; set; }
		float size;
		public float Size
		{
			get { return size; }
			set
			{
				size = value;
				LineHeight = size * PointsToPixels;
			}
		}
		public string FamilyName { get; set; }
		public FontStyle FontStyle { get; set; }
		public FontDecoration FontDecoration { get; set; }
		public FontFamily Family { get; set; }
		public FontTypeface Typeface { get; set; }
		public object ControlObject { get; set; }

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			Family = family;
			Size = size;
			FontStyle = style;
			FontDecoration = decoration;
		}

		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			Size = size ?? 12f;
			FontDecoration = decoration;
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			Typeface = typeface;
			Size = size;
			FontDecoration = decoration;
		}

		public SizeF MeasureString(string text)
		{
			throw new NotImplementedException();
		}
	}
}
