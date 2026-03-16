using System;
using System.Globalization;
using swd = System.Windows.Data;

namespace Eto.Wpf.Forms.Controls
{
	/// <summary>
	/// Builds a rounded rectangle <see cref="swm.Geometry"/> covering an element's own bounds, so it can be
	/// bound to that element's <see cref="sw.UIElement.Clip"/> to clip its children to a corner radius.
	/// Bind [ActualWidth, ActualHeight, CornerRadius] (the radius may also be a scalar double).
	/// </summary>
	/// <remarks>
	/// WPF's Border does not clip its children to its CornerRadius, so the usual workaround is an
	/// OpacityMask holding a VisualBrush with a rounded Border sized to the masked element. That visual is
	/// not part of any visual tree, so it has no namescope to resolve an ElementName binding against, and
	/// sizing it logs "System.Windows.Data Error: 4 : Cannot find source for binding with reference
	/// 'ElementName=...'" at runtime. Clip is set on the element itself, so it can bind to its own
	/// ActualWidth/ActualHeight with RelativeSource Self - and it avoids the intermediate render target
	/// an opacity mask requires.
	/// </remarks>
	public class RoundedClipConverter : swd.IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 2)
				return null;

			var width = values[0] as double? ?? 0;
			var height = values[1] as double? ?? 0;
			if (!(width > 0) || !(height > 0)) // also excludes NaN
				return null;

			var radius = values.Length > 2
				? values[2] switch
				{
					sw.CornerRadius cr => cr,
					double d => new sw.CornerRadius(d),
					_ => new sw.CornerRadius(0)
				}
				: new sw.CornerRadius(0);

			// A corner can never take up more than half of either dimension.
			var max = Math.Min(width, height) / 2;
			var tl = Clamp(radius.TopLeft, max);
			var tr = Clamp(radius.TopRight, max);
			var br = Clamp(radius.BottomRight, max);
			var bl = Clamp(radius.BottomLeft, max);

			var rect = new sw.Rect(0, 0, width, height);
			swm.Geometry geometry;
			if (tl == tr && tr == br && br == bl)
			{
				geometry = new swm.RectangleGeometry(rect, tl, tl);
			}
			else
			{
				var stream = new swm.StreamGeometry();
				using (var context = stream.Open())
				{
					context.BeginFigure(new sw.Point(tl, 0), true, true);
					ArcTo(context, new sw.Point(width - tr, 0), new sw.Point(width, tr), tr);
					ArcTo(context, new sw.Point(width, height - br), new sw.Point(width - br, height), br);
					ArcTo(context, new sw.Point(bl, height), new sw.Point(0, height - bl), bl);
					ArcTo(context, new sw.Point(0, tl), new sw.Point(tl, 0), tl);
				}
				geometry = stream;
			}
			geometry.Freeze();
			return geometry;
		}

		static double Clamp(double value, double max) => value <= 0 ? 0 : value > max ? max : value;

		/// <summary>
		/// Draws the straight edge up to <paramref name="corner"/>, then its arc round to <paramref name="next"/>.
		/// </summary>
		static void ArcTo(swm.StreamGeometryContext context, sw.Point corner, sw.Point next, double radius)
		{
			context.LineTo(corner, false, false);
			if (radius > 0)
				context.ArcTo(next, new sw.Size(radius, radius), 0, false, swm.SweepDirection.Clockwise, false, false);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
