using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Platform.Wpf.Drawing;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<ImageTextCellHandler.Column, ImageTextCell>, IImageTextCell
	{
		object GetImageValue (object dataItem)
		{
			if (Widget.ImageBinding != null) {
				var image = Widget.ImageBinding.GetValue (dataItem) as Image;
				if (image != null)
					return ((IWpfImage)image.Handler).GetIconClosestToSize (16);
			}
			return null;
		}

		string GetTextValue (object dataItem)
		{
			if (Widget.TextBinding != null) {
				return Convert.ToString(Widget.TextBinding.GetValue (dataItem));
			}
			return null;
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
				swc.StackPanel container = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
				container.Children.Add (Image ());
				container.Children.Add (element);
				return Handler.SetupCell (container);
			}

			protected override sw.FrameworkElement GenerateElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement (cell, dataItem);
				element.DataContextChanged += (sender, e) => {
					var text = sender as swc.TextBlock;
					text.Text = Handler.GetTextValue (text.DataContext);
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
				var element = base.GenerateEditingElement (cell, dataItem) as swc.TextBox;
				element.Name = "control";
				element.DataContextChanged += (sender, e) => {
					var text = sender as swc.TextBox;
					text.Text = Handler.GetTextValue (text.DataContext);
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
