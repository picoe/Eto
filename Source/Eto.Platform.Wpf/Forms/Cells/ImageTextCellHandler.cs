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
				var image = new swc.Image ();
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

			protected override sw.FrameworkElement GenerateEditingElement (swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateEditingElement (cell, dataItem) as swc.TextBox;
				element.Name = "text";
				element.DataContextChanged += (sender, e) => {
					var text = sender as swc.TextBox;
					text.Text = Handler.GetTextValue (text.DataContext);
				};
				return SetupCell (element);
			}

			protected override bool CommitCellEdit (sw.FrameworkElement editingElement)
			{
				var text = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox> ("text");
				Handler.SetTextValue (text.DataContext, text.Text);
				return true;
			}

		}


		public ImageTextCellHandler ()
		{
			Control = new Column { Handler = this };
		}
	}
}
