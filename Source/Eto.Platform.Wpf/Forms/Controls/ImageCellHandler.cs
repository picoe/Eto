using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Platform.Wpf.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ImageCellHandler : CellHandler<swc.DataGridTemplateColumn, ImageCell>, IImageCell
	{
		swd.Binding binding;

		class Converter : swd.IValueConverter
		{
			public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var image = value as Image;
				if (image == null) return null;
				return ((IWpfImage)image.Handler).GetIconClosestToSize (16);
			}

			public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				throw new NotImplementedException ();
			}
		}

		public ImageCellHandler ()
		{
			Control = new swc.DataGridTemplateColumn ();
			var template = new sw.DataTemplate ();

			var factory = new sw.FrameworkElementFactory (typeof (swc.Image));
			factory.SetValue (swc.Image.MaxHeightProperty, 16.0);
			factory.SetValue (swc.Image.MaxWidthProperty, 16.0);
			factory.SetValue (swc.Image.StretchDirectionProperty, swc.StretchDirection.DownOnly);
			factory.SetValue (swc.Image.MarginProperty, new sw.Thickness (0, 2, 2, 2));
			factory.SetBinding (swc.Image.SourceProperty, binding = new sw.Data.Binding { Converter = new Converter() });
			
			template.VisualTree = factory;
			Control.CellTemplate = template;
		}

		public override void Bind (int column)
		{
			binding.Path = new sw.PropertyPath (string.Format (".[{0}]", column));
		}
	}
}
