using System;
using System.Runtime.InteropServices;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto
{
	static class GradientHelper
	{
		static PointF GetPerpendicular(PointF l1start, PointF l1end, PointF l2start)
		{
			var k = ((l1end.Y - l1start.Y) * (l2start.X - l1start.X) - (l1end.X - l1start.X) * (l2start.Y - l1start.Y)) / (Math.Pow(l1end.Y - l1start.Y, 2) + Math.Pow(l1end.X - l1start.X, 2));
			var x4 = l2start.X - k * (l1end.Y - l1start.Y);
			var y4 = l2start.Y + k * (l1end.X - l1start.X);
			return new PointF((float)x4, (float)y4);
		}

		public static float GetLinearScale(ref PointF startPoint, ref PointF endPoint, RectangleF rect, float lastScale, float gradientScale)
		{
			var diff = endPoint - startPoint;
			var gradientLength = diff.Length;
			var swap = diff.X < 0;

			PointF min;
			PointF max;
			GetLinearMinMax(startPoint, endPoint, rect, out min, out max);

			var overlapRatio = (((min - startPoint).Length % (gradientLength * gradientScale)) / gradientLength);
			if (swap ? min.X > startPoint.X : min.X < startPoint.X)
				overlapRatio = gradientScale - overlapRatio;
			startPoint = min - diff * overlapRatio;

			var scale = Math.Max((float)Math.Ceiling((max - startPoint).Length / gradientLength), lastScale);
			endPoint = startPoint + diff * scale;

			return scale;
		}

		public static void GetLinearFromRectangle(RectangleF rectangle, float angle, out PointF min, out PointF max)
		{
			PointF startPoint;
			PointF endPoint;
			if (angle >= 0 && angle < 90)
			{
				startPoint = rectangle.TopLeft;
				endPoint = rectangle.TopRight;
			}
			else if (angle >= 90 && angle < 180)
			{
				startPoint = rectangle.TopRight;
				endPoint = rectangle.BottomRight;
				angle -= 90;
			}
			else if (angle >= 180 && angle < 270)
			{
				startPoint = rectangle.BottomRight;
				endPoint = rectangle.BottomLeft;
				angle -= 180;
			}
			else
			{
				startPoint = rectangle.BottomLeft;
				endPoint = rectangle.TopLeft;
				angle -= 270;
			}
			var matrix = Matrix.Create();
			matrix.RotateAt(angle, startPoint);
			endPoint = matrix.TransformPoint(endPoint);
			GradientHelper.GetLinearMinMax(startPoint, endPoint, rectangle, out min, out max);
		}

		public static void GetLinearMinMax(PointF startPoint, PointF endPoint, RectangleF rect, out PointF min, out PointF max, bool includeStartEnd = false)
		{
			var points = new List<PointF>
			{
				GetPerpendicular(startPoint, endPoint, rect.TopLeft),
				GetPerpendicular(startPoint, endPoint, rect.TopRight),
				GetPerpendicular(startPoint, endPoint, rect.BottomLeft),
				GetPerpendicular(startPoint, endPoint, rect.BottomRight),
			};
			if (includeStartEnd)
				points.AddRange(new[] { startPoint, endPoint });

			var diff = endPoint - startPoint;
			if (Math.Abs(diff.X) > 0.1f)
			{
				points.Sort((a, b) => a.X.CompareTo(b.X));

				var swap = diff.X < 0;
				min = swap ? points.Last() : points.First();
				max = swap ? points.First() : points.Last();
			}
			else
			{
				points.Sort((a, b) => a.Y.CompareTo(b.Y));

				var swap = diff.Y < 0;
				min = swap ? points.Last() : points.First();
				max = swap ? points.First() : points.Last();
			}
		}


		public static float GetRadialScale(PointF center, SizeF radius, PointF gradientOrigin, RectangleF bounds)
		{
			var sbr = GetRadialScale(center, radius, gradientOrigin, bounds.BottomRight);
			var sbl = GetRadialScale(center, radius, gradientOrigin, bounds.BottomLeft);
			var str = GetRadialScale(center, radius, gradientOrigin, bounds.TopRight);
			var stl = GetRadialScale(center, radius, gradientOrigin, bounds.TopLeft);
			return Math.Max(1f, Math.Max(sbr, Math.Max(sbl, Math.Max(str, stl))));
		}

		public static float GetRadialScale(PointF center, SizeF radius, PointF gorigin, PointF bouter)
		{
			var gouter = FindEllipseIntersection(center, radius, gorigin, bouter);
			if (gouter != null)
			{
				var glength = gorigin.LengthTo(gouter.Value);
				var olength = gorigin.LengthTo(bouter);
				return (float)Math.Ceiling(olength / glength);
			}
			return 1f;
		}

		public static IEnumerable<Tuple<float, TColor>> GetGradientStops<TColor>(TColor startColor, TColor endColor, float scale, GradientWrapMode wrap)
		{
			yield return new Tuple<float, TColor>(0f, startColor);
			var inc = 1f / scale;
			var pos = inc;
			switch (wrap)
			{
				case GradientWrapMode.Repeat:
					while (inc > 0f && pos < 1f)
					{
						yield return new Tuple<float, TColor>(pos, endColor);
						yield return new Tuple<float, TColor>(pos, startColor);
						pos += inc;
					}
					yield return new Tuple<float, TColor>(1f, endColor);
					break;
				case GradientWrapMode.Reflect:
					var factor = true;
					while (inc > 0f && pos < 1f)
					{
						yield return new Tuple<float, TColor>(pos, factor ? endColor : startColor);
						factor = !factor;
						pos += inc;
					}
					yield return new Tuple<float, TColor>(1f, factor ? endColor : startColor);
					break;
				case GradientWrapMode.Pad:
					yield return new Tuple<float, TColor>(pos, endColor);
					if (pos < 1f)
					{
						yield return new Tuple<float, TColor>(1f, endColor);
					}
					break;
			}
		}

		// modified from: http://csharphelper.com/blog/2012/09/calculate-where-a-line-segment-and-an-ellipse-intersect-in-c/
		static PointF? FindEllipseIntersection(PointF center, SizeF radius, PointF start, PointF end)
		{
			// Center ellipse around origin for calculation, so adjust line points to match
			start -= center;
			end -= center;

			var rx = radius.Width;
			var ry = radius.Height;

			// Calculate the quadratic parameters
			var a = (end.X - start.X) * (end.X - start.X) / rx / rx + (end.Y - start.Y) * (end.Y - start.Y) / ry / ry;

			if (a == 0) // when the start & end are at the same point, there is no line to intersect with.
				return null;

			var b = 2 * start.X * (end.X - start.X) / rx / rx + 2 * start.Y * (end.Y - start.Y) / ry / ry;
			float c = start.X * start.X / rx / rx + start.Y * start.Y / ry / ry - 1;

			// Calculate the discriminant
			float discriminant = b * b - 4 * a * c;
			float t;
			if (Math.Abs(discriminant) < float.Epsilon)
			{
				t = -b / 2 / a;
			}
			else if (discriminant > 0)
			{
				// Two possible values
				var t1 = (float)((-b + Math.Sqrt(discriminant)) / 2 / a);
				var t2 = (float)((-b - Math.Sqrt(discriminant)) / 2 / a);
				if (t1 < 0f || t1 > 1f)
					t = t2;
				else if (t2 < 0f || t2 > 1f)
					t = t1;
				else
					t = Math.Max(t1, t2);
			}
			else
				return null;

			// check that the value falls between the line points
			if (t < 0f || t > 1f)
				return null;
			return start + (end - start) * t + center;
		}
	}
}