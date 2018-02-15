#if TODO_XAML
using System;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using swd = Windows.UI.Xaml.Data;
using swm = Windows.UI.Xaml.Media;
using Eto.WinRT.Drawing;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<ImageTextCellHandler.Column, ImageTextCell>, IImageTextCell
	{
		object GetImageValue (object dataItem)
		{
			if (Widget.ImageBinding != null) {
				var image = Widget.ImageBinding.GetValue (dataItem) as Image;
				if (image != null)
					return ((IWpfImage)image.Handler).GetImageClosestToSize (16);
			}
			return null;
		}

		string GetTextValue (object dataItem)
		{
			return Widget.TextBinding != null ? Convert.ToString(Widget.TextBinding.GetValue(dataItem)) : null;
		}

		void SetTextValue (object dataItem, string value)
		{
			if (Widget.TextBinding != null) {
				Widget.TextBinding.SetValue (dataItem, value);
			}
		}

		public class Column : swc.DataGridTextColumn
		{
			public ImageTextCellHandler Handler { get; set; }

			swc.Image Image ()
			{
				var image = new swc.Image { MaxWidth = 16, MaxHeight = 16, StretchDirection = swc.StretchDirection.DownOnly, Margin = new sw.Thickness (0, 2, 2, 2) };
				image.DataContextChanged += (sender, e) => {
					var img = sender as swc.Image;
					img.Source = Handler.GetImageValue (img.DataContext) as swm.ImageSource;
				};
				return image;
			}

			sw.FrameworkElement SetupCell (sw.FrameworkElement element)
			{
				var container = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
				container.Children.Add (Image ());
				container.Children.Add (element);
				return Handler.SetupCell (container);
			}

			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.TextBlock;
					control.Text = Handler.GetTextValue (control.DataContext);
					Handler.FormatCell (control, cell, control.DataContext);
				};
				return SetupCell (element);
			}

			protected override object PrepareCellForEdit (sw.FrameworkElement editingElement, sw.RoutedEventArgs editingEventArgs)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox> ("control");
				return base.PrepareCellForEdit (control, editingEventArgs);
			}

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.TextBox)base.GenerateEditingElement (cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) => {
					var control = sender as swc.TextBox;
					control.Text = Handler.GetTextValue (control.DataContext);
					Handler.FormatCell (control, cell, control.DataContext);
				};
				return SetupCell (element);
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox> ("control");
				Handler.SetTextValue (control.DataContext, control.Text);
				return true;
			}

		}


		public ImageTextCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}
#endif