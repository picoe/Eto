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
	/// Handler for <see cref="SolidBrush"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class SolidBrushHandler : BrushHandler, SolidBrush.IHandler
	{
		public object Create(Color color)
		{
			var result = new ag.Paint
			{
				Color = color.ToAndroid(),
			};
			result.SetStyle(ag.Paint.Style.Fill);
			return result;
		}

		public Color GetColor(SolidBrush widget)
		{
			return ((ag.Paint)widget.ControlObject).Color.ToEto();
		}

		public void SetColor(SolidBrush widget, Color color)
		{
			throw new NotImplementedException();
		}

		public override ag.Paint GetPaint(Brush brush)
		{
			return (ag.Paint)brush.ControlObject;
		}
	}
}