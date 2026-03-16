using System;
using System.Globalization;

namespace Eto.Wpf.Forms.Controls
{
	/// <summary>
	/// Converts a uniform <see cref="sw.CornerRadius"/> (or a scalar double) into a CornerRadius that
	/// rounds only the two OUTER corners for a given tab strip side, so themed tabs follow the palette
	/// corner radius on their outer edge while staying square where they meet the content area.
	/// </summary>
	/// <remarks>
	/// The palette exposes a single scalar corner radius (Eto.Palette.CornerRadius); a partial
	/// CornerRadius can't be derived from it in XAML, so the tab template feeds that value through this
	/// converter with the side as the ConverterParameter ("Top", "Bottom", "Left" or "Right").
	/// CornerRadius order is (TopLeft, TopRight, BottomRight, BottomLeft).
	/// </remarks>
	public class TabCornerRadiusConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double r = value switch
			{
				sw.CornerRadius cr => Math.Max(Math.Max(cr.TopLeft, cr.TopRight), Math.Max(cr.BottomLeft, cr.BottomRight)),
				double d => d,
				_ => 0
			};

			return (parameter as string) switch
			{
				"Bottom" => new sw.CornerRadius(0, 0, r, r),
				"Left" => new sw.CornerRadius(r, 0, 0, r),
				"Right" => new sw.CornerRadius(0, r, r, 0),
				_ => new sw.CornerRadius(r, r, 0, 0), // Top
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
