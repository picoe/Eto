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
	/// Handler for <see cref="IPen"/>
	/// </summary>
	/// <copyright>(c) 2013 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : IPen
	{
		public object Create(Color color, float thickness)
		{
			var paint = new ag.Paint 
			{ 
				Color = color.ToAndroid(), 
				StrokeWidth = thickness,
				StrokeCap = ag.Paint.Cap.Square,
				StrokeMiter = 10f
			};
			return paint;
		}

		public Color GetColor(Pen widget)
		{
			return widget.ToAndroid().Color.ToEto();
		}

		public void SetColor(Pen widget, Color color)
		{
			widget.ToAndroid().Color = color.ToAndroid();
		}

		public float GetThickness(Pen widget)
		{
			return widget.ToAndroid().StrokeWidth;
		}

		public void SetThickness(Pen widget, float thickness)
		{
			widget.ToAndroid().StrokeWidth = thickness;
		}

		public PenLineJoin GetLineJoin(Pen widget)
		{
			return widget.ToAndroid().StrokeJoin.ToEto();
		}

		public void SetLineJoin(Pen widget, PenLineJoin lineJoin)
		{
			widget.ToAndroid().StrokeJoin = lineJoin.ToAndroid();
		}

		public PenLineCap GetLineCap(Pen widget)
		{
			return widget.ToAndroid().StrokeCap.ToEto();
		}

		public void SetLineCap(Pen widget, PenLineCap lineCap)
		{
			var pen = widget.ToAndroid();
			pen.StrokeCap = lineCap.ToSD();
			SetDashStyle(widget, widget.DashStyle);
		}

		public float GetMiterLimit(Pen widget)
		{
			return widget.ToAndroid().StrokeMiter;
		}

		public void SetMiterLimit(Pen widget, float miterLimit)
		{
			widget.ToAndroid().StrokeMiter = miterLimit;
		}

		public void SetDashStyle(Pen widget, DashStyle dashStyle)
		{
			var pen = widget.ToAndroid();

			if (dashStyle == null || dashStyle.IsSolid)
				pen.SetPathEffect(null);
			else
			{
				// TODO: create a new ag.DashPathEffect with the appropriate intervals
				throw new NotImplementedException();
			}
		}
	}
}