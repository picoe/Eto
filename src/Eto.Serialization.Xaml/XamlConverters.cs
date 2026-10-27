#if PORTABLE
using Portable.Xaml.Markup;

// compiled xeto calls these directly instead of going through each type's TypeConverter
[assembly: XamlCodeGen(typeof(Color), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.Color))]
[assembly: XamlCodeGen(typeof(Size), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.Size))]
[assembly: XamlCodeGen(typeof(SizeF), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.SizeF))]
[assembly: XamlCodeGen(typeof(Point), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.Point))]
[assembly: XamlCodeGen(typeof(PointF), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.PointF))]
[assembly: XamlCodeGen(typeof(Rectangle), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.Rectangle))]
[assembly: XamlCodeGen(typeof(RectangleF), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.RectangleF))]
[assembly: XamlCodeGen(typeof(Padding), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.Padding))]
[assembly: XamlCodeGen(typeof(PaddingF), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.PaddingF))]
[assembly: XamlCodeGen(typeof(Keys), typeof(Eto.Serialization.Xaml.XamlConverters), nameof(Eto.Serialization.Xaml.XamlConverters.Keys))]

namespace Eto.Serialization.Xaml
{
	/// <summary>
	/// Parses attribute values for compiled xeto, giving the same results as each type's TypeConverter.
	/// </summary>
	public static class XamlConverters
	{
		static readonly char[] DimensionSplitter = { ',' };

		// matches the converters, which parse with the current culture
		static string[] Split(string text, string typeName, string form, params int[] counts)
		{
			var parts = text.Split(DimensionSplitter, StringSplitOptions.RemoveEmptyEntries);
			if (Array.IndexOf(counts, parts.Length) < 0)
				throw Invalid(text, typeName, form);
			return parts;
		}

		static ArgumentException Invalid(string text, string typeName, string form)
			=> new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot parse value '{0}' as {1}. Should be in the form of {2}", text, typeName, form));

		static T Parse<T>(string text, string typeName, string form, Func<T> parse)
		{
			try
			{
				return parse();
			}
			catch (Exception ex) when (!(ex is ArgumentException))
			{
				throw Invalid(text, typeName, form);
			}
		}

		public static Color Color(string text)
		{
			if (!Eto.Drawing.Color.TryParse(text, out var color))
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "{0} is not a valid color value.", text));
			return color;
		}

		public static Size Size(string text)
		{
			const string form = "'size' or 'width, height'";
			var p = Split(text, nameof(Size), form, 1, 2);
			return Parse(text, nameof(Size), form, () => p.Length == 1 ? new Size(int.Parse(p[0])) : new Size(int.Parse(p[0]), int.Parse(p[1])));
		}

		public static SizeF SizeF(string text)
		{
			const string form = "'size' or 'width, height'";
			var p = Split(text, nameof(SizeF), form, 1, 2);
			return Parse(text, nameof(SizeF), form, () => p.Length == 1 ? new SizeF(float.Parse(p[0])) : new SizeF(float.Parse(p[0]), float.Parse(p[1])));
		}

		public static Point Point(string text)
		{
			const string form = "'x, y'";
			var p = Split(text, nameof(Point), form, 2);
			return Parse(text, nameof(Point), form, () => new Point(int.Parse(p[0]), int.Parse(p[1])));
		}

		public static PointF PointF(string text)
		{
			const string form = "'x, y'";
			var p = Split(text, nameof(PointF), form, 2);
			return Parse(text, nameof(PointF), form, () => new PointF(float.Parse(p[0]), float.Parse(p[1])));
		}

		public static Rectangle Rectangle(string text)
		{
			const string form = "'x, y, width, height'";
			var p = Split(text, nameof(Rectangle), form, 4);
			return Parse(text, nameof(Rectangle), form, () => new Rectangle(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3])));
		}

		public static RectangleF RectangleF(string text)
		{
			const string form = "'x, y, width, height'";
			var p = Split(text, nameof(RectangleF), form, 4);
			return Parse(text, nameof(RectangleF), form, () => new RectangleF(float.Parse(p[0]), float.Parse(p[1]), float.Parse(p[2]), float.Parse(p[3])));
		}

		const string PaddingForm = "'all', 'horizontal, vertical', or 'left, top, right, bottom'";

		public static Padding Padding(string text)
		{
			var p = Split(text, nameof(Padding), PaddingForm, 1, 2, 4);
			return Parse(text, nameof(Padding), PaddingForm, () => p.Length switch
			{
				1 => new Padding(int.Parse(p[0])),
				2 => new Padding(int.Parse(p[0]), int.Parse(p[1])),
				_ => new Padding(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]))
			});
		}

		public static PaddingF PaddingF(string text)
		{
			var p = Split(text, nameof(PaddingF), PaddingForm, 1, 2, 4);
			return Parse(text, nameof(PaddingF), PaddingForm, () => p.Length switch
			{
				1 => new PaddingF(float.Parse(p[0])),
				2 => new PaddingF(float.Parse(p[0]), float.Parse(p[1])),
				_ => new PaddingF(float.Parse(p[0]), float.Parse(p[1]), float.Parse(p[2]), float.Parse(p[3]))
			});
		}

		public static Keys Keys(string text) => KeysExtensions.FromShortcutString(text);
	}
}
#endif
