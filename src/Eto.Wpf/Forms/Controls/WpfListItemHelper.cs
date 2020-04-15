using System;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using Eto.Forms;
using Eto.Wpf.CustomControls;
using Eto.Drawing;
using System.Globalization;

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

	public class WpfEditableTextBindingBlock : sw.FrameworkElementFactory
	{
		public WpfEditableTextBindingBlock(Func<IIndirectBinding<string>> getBinding, swd.RelativeSource relativeSource)
			: base(typeof(EditableTextBlock))
		{
			//var binding = new sw.Data.Binding { Path = TextPath, RelativeSource = relativeSource, Mode = swd.BindingMode.TwoWay, UpdateSourceTrigger = swd.UpdateSourceTrigger.PropertyChanged };
			//SetBinding(EditableTextBlock.TextProperty, binding);
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

	public class WpfImageTextBindingBlock : sw.FrameworkElementFactory
	{
		public WpfImageTextBindingBlock(Func<IIndirectBinding<string>> textBinding, Func<IIndirectBinding<Image>> imageBinding, bool editable, swd.RelativeSource relativeSource = null)
			: base(typeof(swc.StackPanel))
		{
			SetValue(swc.StackPanel.OrientationProperty, swc.Orientation.Horizontal);
			AppendChild(new WpfImageBindingBlock(imageBinding));
			if (editable)
				AppendChild(new WpfEditableTextBindingBlock(textBinding, relativeSource));
			else
				AppendChild(new WpfTextBindingBlock(textBinding));
		}
	}

	public static class WpfListItemHelper
	{
		public static sw.FrameworkElementFactory ItemTemplate(bool editable, swd.RelativeSource relativeSource = null)
		{
			var factory = new sw.FrameworkElementFactory(typeof(swc.StackPanel));
			factory.SetValue(swc.StackPanel.OrientationProperty, swc.Orientation.Horizontal);
			factory.AppendChild(ImageBlock());
			factory.AppendChild(editable ? EditableBlock(relativeSource) : TextBlock());
			return factory;
		}

		class TextConverter : swd.IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var item = value as IListItem;
				return (item != null) ? item.Text : null;
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		public static sw.FrameworkElementFactory TextBlock(bool setMargin = true)
		{
			var factory = new sw.FrameworkElementFactory(typeof(swc.TextBlock));
			factory.SetBinding(swc.TextBlock.TextProperty, new sw.Data.Binding { Path = new sw.PropertyPath("Text") });
			if (setMargin)
				factory.SetValue(sw.FrameworkElement.MarginProperty, new sw.Thickness(2));
			return factory;
		}

		public static sw.FrameworkElementFactory EditableBlock(swd.RelativeSource relativeSource)
		{
			var factory = new sw.FrameworkElementFactory(typeof(EditableTextBlock));
			var binding = new sw.Data.Binding { Path = new sw.PropertyPath("Text"), RelativeSource = relativeSource, Mode = swd.BindingMode.TwoWay, UpdateSourceTrigger = swd.UpdateSourceTrigger.LostFocus };
			factory.SetBinding(EditableTextBlock.TextProperty, binding);
			return factory;
		}

		class ImageConverter : swd.IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var item = value as IImageListItem;
				if (item == null || item.Image == null) return null;
				return item.Image.ToWpf(16);
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		public static sw.FrameworkElementFactory ImageBlock()
		{
			var factory = new sw.FrameworkElementFactory(typeof(swc.Image));
			factory.SetValue(sw.FrameworkElement.MaxHeightProperty, 16.0);
			factory.SetValue(sw.FrameworkElement.MaxWidthProperty, 16.0);
			factory.SetValue(swc.Image.StretchDirectionProperty, swc.StretchDirection.DownOnly);
			factory.SetValue(sw.FrameworkElement.MarginProperty, new sw.Thickness(0, 2, 2, 2));
			factory.SetBinding(swc.Image.SourceProperty, new sw.Data.Binding { Converter = new ImageConverter() });
			return factory;
		}

	}
}
