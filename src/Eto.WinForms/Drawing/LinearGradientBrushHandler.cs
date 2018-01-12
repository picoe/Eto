using System.Linq;
using Eto.Drawing;
using sd = System.Drawing;
using sd2 = System.Drawing.Drawing2D;
using System;

namespace Eto.WinForms.Drawing
{
	/// <summary>
	/// Handler for <see cref="LinearGradientBrush"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LinearGradientBrushHandler : BrushHandler, LinearGradientBrush.IHandler
	{
		class BrushObject
		{
			sd2.LinearGradientBrush brush;
			GradientWrapMode wrapMode;
			IMatrix transform;
			float lastStartPos;
			float lastEndPos;

			public PointF StartPoint { get; set; }
			public PointF EndPoint { get; set; }

			public sd.Color StartColor { get; set; }
			public sd.Color EndColor { get; set; }

			public GradientWrapMode WrapMode
			{
				get { return wrapMode; }
				set
				{
					wrapMode = value;
					Reset();
				}
			}

			public IMatrix Transform
			{
				get { return transform; }
				set
				{
					transform = value;
					Reset();
				}
			}

			void Reset()
			{
				if (brush != null)
					brush.Dispose();
				brush = null;
			}

			public sd2.LinearGradientBrush GetBrush(RectangleF rect)
			{
				var start = StartPoint;
				var end = EndPoint;
				if (wrapMode == GradientWrapMode.Pad)
				{
					// winforms does not support pad, so extend to fill entire drawing region
					if (transform != null)
					{
						start = transform.TransformPoint(start);
						end = transform.TransformPoint(end);
					}
					PointF min, max;
					GradientHelper.GetLinearMinMax(start, end, rect, out min, out max, true);
					var len = max.LengthTo(min);
					// find start/end pos based on entire position
					var startpos = min.LengthTo(start) / len;
					var endpos = min.LengthTo(end) / len;
					if (brush == null || lastStartPos != startpos || lastEndPos > endpos)
					{
						lastStartPos = startpos;
						lastEndPos = endpos;
						start = min;
						end = max;
						var diff = end - start;
						// account for innacuracies in system.drawing when nearing horizontal or vertical
						if (Math.Abs(diff.X) < 0.0001)
							end.X = start.X;
						if (Math.Abs(diff.Y) < 0.0001)
							end.Y = start.Y;

						brush = new sd2.LinearGradientBrush(start.ToSD(), end.ToSD(), StartColor, EndColor);
						brush.WrapMode = sd2.WrapMode.Tile;
						brush.InterpolationColors = new sd2.ColorBlend
						{
							Colors = new[]
						{
							StartColor,
							StartColor,
							EndColor,
							EndColor
						},
							Positions = new[]
						{
							0f,
							startpos,
							endpos,
							1f,
						}
						};
					}
				}
				else if (brush == null)
				{
					brush = new sd2.LinearGradientBrush(StartPoint.ToSD(), EndPoint.ToSD(), StartColor, EndColor);
					brush.WrapMode = wrapMode.ToSD();
					if (transform != null)
						brush.MultiplyTransform(transform.ToSD());
				}
				return brush;
			}
		}

		public object Create(Color startColor, Color endColor, PointF startPoint, PointF endPoint)
		{
			return new BrushObject
			{
				StartPoint = startPoint,
				EndPoint = endPoint,
				StartColor = startColor.ToSD(),
				EndColor = endColor.ToSD()
			};
		}

		public object Create(RectangleF rectangle, Color startColor, Color endColor, float angle)
		{
			return null;
		}

		public IMatrix GetTransform(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).Transform;
		}

		public void SetTransform(LinearGradientBrush widget, IMatrix transform)
		{
			((BrushObject)widget.ControlObject).Transform = transform;
		}

		public GradientWrapMode GetGradientWrap(LinearGradientBrush widget)
		{
			return ((BrushObject)widget.ControlObject).WrapMode;
		}

		public void SetGradientWrap(LinearGradientBrush widget, GradientWrapMode gradientWrap)
		{
			((BrushObject)widget.ControlObject).WrapMode = gradientWrap;
		}

		public override sd.Brush GetBrush(Brush brush, RectangleF rect)
		{
			return ((BrushObject)brush.ControlObject).GetBrush(rect);
		}
	}
}