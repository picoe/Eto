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
	/// <summary>
	/// Handler for <see cref="IFont"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FontHandler : WidgetHandler<ag.Typeface, Font>, Font.IHandler
	{
		float size;
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

		ag.Paint.FontMetrics fontMetrics;
		ag.Paint.FontMetrics FontMetrics { get { return fontMetrics = fontMetrics ?? Paint.GetFontMetrics(); } }

		public FontHandler()
		{
		}

		public FontHandler(ag.Typeface typeface)
		{
			Control = typeface;
		}

		public void Create(FontFamily family, float size, FontStyle style, FontDecoration decoration)
		{
			this.size = size;
			this.Control = ag.Typeface.Create(family.ControlObject as ag.Typeface, style.ToAndroid());
		}

		// TODO:
		public void Create(SystemFont systemFont, float? size, FontDecoration decoration)
		{
			var style = ag.TypefaceStyle.Normal;
			switch (systemFont)
			{
			case SystemFont.Default:
				break;
			case SystemFont.Bold:
				break;
			case SystemFont.Label:
				break;
			case SystemFont.TitleBar:
				break;
			case SystemFont.ToolTip:
				break;
			case SystemFont.MenuBar:
				break;
			case SystemFont.Menu:
				break;
			case SystemFont.Message:
				break;
			case SystemFont.Palette:
				break;
			case SystemFont.StatusBar:
				break;
				default:
					break;
			}

			Control = ag.Typeface.Create(ag.Typeface.Default, style); 
		}

		public void Create(FontTypeface typeface, float size, FontDecoration decoration)
		{
			this.size = size;
			this.Control = ag.Typeface.Create(typeface.Family.Name, typeface.FontStyle.ToAndroid());
		}

		public float XHeight
		{
			get { throw new NotImplementedException(); }
		}

		public float Ascent
		{
			get { return Math.Abs(FontMetrics.Ascent); } // TODO: does this need to be scaled?
		}

		public float Descent
		{
			get { return Math.Abs(FontMetrics.Descent); } // TODO: does this need to be scaled?
		}

		public float LineHeight
		{
			get { throw new NotImplementedException(); }
		}

		public float Leading
		{
			get { return Math.Abs(FontMetrics.Leading); } // TODO: does this need to be scaled?
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

		public FontDecoration FontDecoration
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

		public SizeF MeasureString(string text)
		{
			throw new NotImplementedException();
		}
	}
}