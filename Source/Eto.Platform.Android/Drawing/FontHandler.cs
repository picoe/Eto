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
	/// <summary>
	/// Handler for <see cref="IFont"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontHandler : WidgetHandler<ag.Typeface, Font>, IFont
	{
		/// <summary>
		/// Used by GraphicsHandler.MeasureText
		/// </summary>
		ag.Paint paint;
		public ag.Paint Paint
		{
			get
			{
				if (paint == null)
				{
					paint = new ag.Paint();
					paint.SetTypeface(this.Control);
				}
				return paint;
			}
		}

		public void Create(FontFamily family, float size, FontStyle style)
		{
			throw new NotImplementedException();
		}

		public void Create(SystemFont systemFont, float? size)
		{
			throw new NotImplementedException();
		}

		public void Create(FontTypeface typeface, float size)
		{
			throw new NotImplementedException();
		}

		public float XHeight
		{
			get { throw new NotImplementedException(); }
		}

		public float Ascent
		{
			get { throw new NotImplementedException(); }
		}

		public float Descent
		{
			get { throw new NotImplementedException(); }
		}

		public float LineHeight
		{
			get { throw new NotImplementedException(); }
		}

		public float Leading
		{
			get { throw new NotImplementedException(); }
		}

		public float Baseline
		{
			get { throw new NotImplementedException(); }
		}

		public float Size
		{
			get { throw new NotImplementedException(); }
		}

		public string FamilyName
		{
			get { throw new NotImplementedException(); }
		}

		public FontStyle FontStyle
		{
			get { throw new NotImplementedException(); }
		}

		public FontFamily Family
		{
			get { throw new NotImplementedException(); }
		}

		public FontTypeface Typeface
		{
			get { throw new NotImplementedException(); }
		}
	}
}