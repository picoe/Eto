using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;
using swc = System.Windows.Controls;
using swd = System.Windows.Data;
using Eto.Forms;
using Eto.Platform.Wpf.Drawing;
using Eto.Platform.Wpf.CustomControls;
using System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms.Controls
{
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

		static sw.PropertyPath TextPath = PropertyPathHelper.Create("(Eto.Forms.IListItem,Eto.Text)");

		public static sw.FrameworkElementFactory TextBlock()
		{
			var factory = new sw.FrameworkElementFactory(typeof(swc.TextBlock));
			factory.SetBinding(swc.TextBlock.TextProperty, new sw.Data.Binding { Path = TextPath });
			factory.SetValue(swc.TextBlock.MarginProperty, new sw.Thickness(2));
			return factory;
		}

		public static sw.FrameworkElementFactory EditableBlock(swd.RelativeSource relativeSource)
		{
			var factory = new sw.FrameworkElementFactory(typeof(EditableTextBlock));
			var binding = new sw.Data.Binding { Path = TextPath, RelativeSource = relativeSource, Mode = swd.BindingMode.TwoWay, UpdateSourceTrigger = swd.UpdateSourceTrigger.PropertyChanged };
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
			factory.SetValue(swc.Image.MaxHeightProperty, 16.0);
			factory.SetValue(swc.Image.MaxWidthProperty, 16.0);
			factory.SetValue(swc.Image.StretchDirectionProperty, swc.StretchDirection.DownOnly);
			factory.SetValue(swc.Image.MarginProperty, new sw.Thickness(0, 2, 2, 2));
			factory.SetBinding(swc.Image.SourceProperty, new sw.Data.Binding { Converter = new ImageConverter() });
			return factory;
		}

	}
}
