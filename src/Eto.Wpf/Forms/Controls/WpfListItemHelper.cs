using swd = System.Windows.Data;
using Eto.Wpf.CustomControls;
namespace Eto.Wpf.Forms.Controls
{

	public class WpfActionValueConverter : swd.IValueConverter
	{
		public delegate object ConvertDelegate(object value, Type targetType, object parameter, CultureInfo culture);

		readonly ConvertDelegate _convert;
		readonly ConvertDelegate _convertBack;

		public WpfActionValueConverter(ConvertDelegate convert, ConvertDelegate convertBack = null)
		{
			_convert = convert ?? throw new ArgumentNullException(nameof(convert));
			_convertBack = convertBack;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return _convert(value, targetType, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (_convertBack != null)
				return _convertBack.Invoke(value, targetType, parameter, culture);
			throw new NotImplementedException();
		}
	}

	public class WpfTextBindingBlock : sw.FrameworkElementFactory, swd.IValueConverter
	{
		Func<IIndirectBinding<string>> Binding { get; set; }

		public WpfTextBindingBlock(Func<IIndirectBinding<string>> binding, bool setMargin = true)
			: base(typeof(swc.TextBlock))
		{
			Binding = binding;
			SetBinding(swc.TextBlock.TextProperty, new sw.Data.Binding { Converter = this });
			SetValue(sw.FrameworkElement.HorizontalAlignmentProperty, sw.HorizontalAlignment.Stretch);
			if (setMargin)
				SetValue(sw.FrameworkElement.MarginProperty, new sw.Thickness(2,0, 2, 0));
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Binding().GetValue(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	public class WpfImageBindingBlock : sw.FrameworkElementFactory, swd.IValueConverter
	{
		Func<IIndirectBinding<Image>> Binding { get; set; }

		public WpfImageBindingBlock(Func<IIndirectBinding<Image>> binding, bool setMargin = true)
			: base(typeof(swc.Image))
		{
			Binding = binding;
			SetValue(swc.Image.StretchDirectionProperty, swc.StretchDirection.DownOnly);
			SetValue(sw.FrameworkElement.MarginProperty, new sw.Thickness(0, 0, 2, 0));
			SetBinding(swc.Image.SourceProperty, new sw.Data.Binding { Converter = this });
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var binding = Binding();
			if (binding == null)
				return null;
			return binding.GetValue(value).ToWpf();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	class WpfImageTextGrid : swc.Grid
	{
		public WpfImageTextGrid()
		{
			ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(0, sw.GridUnitType.Auto)});
			ColumnDefinitions.Add(new swc.ColumnDefinition { Width = new sw.GridLength(1, sw.GridUnitType.Star) });
		}
	}

	public class WpfImageTextBindingBlock : sw.FrameworkElementFactory
	{
		public WpfImageTextBindingBlock(Func<IIndirectBinding<string>> textBinding, Func<IIndirectBinding<Image>> imageBinding, swd.RelativeSource relativeSource = null)
			: base(typeof(WpfImageTextGrid))
		{
			SetValue(sw.FrameworkElement.HorizontalAlignmentProperty, sw.HorizontalAlignment.Stretch);

			var image = new WpfImageBindingBlock(imageBinding);
			image.SetValue(swc.Grid.ColumnProperty, 0);
			AppendChild(image);

			var text = new WpfTextBindingBlock(textBinding);
			text.SetValue(sw.FrameworkElement.VerticalAlignmentProperty, sw.VerticalAlignment.Center);
			text.SetValue(swc.Grid.ColumnProperty, 1);
			AppendChild(text);
		}
	}
}
