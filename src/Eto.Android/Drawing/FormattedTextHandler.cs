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
using at = Android.Text;
using Android.Text;
using Javax.Net.Ssl;
using Org.Apache.Http.Conn;
using static System.Net.Mime.MediaTypeNames;

namespace Eto.Android.Drawing
{
	/// <summary>
	/// Handler for <see cref="IGraphicsPath"/>
	/// </summary>
	/// <copyright>(c) 2020 by Vecsoft (UK) Ltd</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FormattedTextHandler : WidgetHandler<StaticLayout, FormattedText>, FormattedText.IHandler
	{

		ag.PointF position;

		public FormattedTextHandler()
		{
		}

		public FormattedTextWrapMode Wrap
		{
			get;
			set;
		}

		public FormattedTextTrimming Trimming
		{
			get;
			set;
		}

		public FormattedTextAlignment Alignment
		{
			get;
			set;
		}

		public Font Font
		{
			get;
			set;
		}

		public String Text
		{
			get;
			set;
		}

		public SizeF MaximumSize
		{
			get;
			set;
		}

		public Brush ForegroundBrush
		{
			get;
			set;
		}

		public SizeF Measure()
		{
			var FontH = (FontHandler)Font.Handler;
			var Paint = FontH.Paint;

			var Lay = new StaticLayout(Text, Paint, (int)MaximumSize.Width, Translate(Alignment), 1, 0, true);

			return new SizeF(Lay.Width, Lay.Height);
		}

		private Layout.Alignment Translate(FormattedTextAlignment alignment)
		{
			if (alignment == FormattedTextAlignment.Left)
				return Layout.Alignment.AlignNormal;

			if (alignment == FormattedTextAlignment.Right)
				return Layout.Alignment.AlignOpposite;

			if (alignment == FormattedTextAlignment.Center)
				return Layout.Alignment.AlignCenter;
			return Layout.Alignment.AlignNormal;
		}
	}
}