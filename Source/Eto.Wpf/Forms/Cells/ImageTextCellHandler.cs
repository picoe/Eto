using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Wpf.Drawing;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<ImageTextCellHandler.Column, ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler
	{
		object GetImageValue(object dataItem)
		{
			if (Widget.ImageBinding != null)
			{
				var image = Widget.ImageBinding.GetValue(dataItem) as Image;
				if (image != null)
					return ((IWpfImage)image.Handler).GetImageClosestToSize(16);
			}
			return null;
		}

		string GetTextValue(object dataItem)
		{
			return Widget.TextBinding != null ? Convert.ToString(Widget.TextBinding.GetValue(dataItem)) : null;
		}

		void SetTextValue(object dataItem, string value)
		{
			if (Widget.TextBinding != null)
			{
				Widget.TextBinding.SetValue(dataItem, value);
			}
		}

		public class Column : swc.DataGridTextColumn
		{
			public ImageTextCellHandler Handler { get; set; }

			swm.BitmapScalingMode scalingMode;
			public swm.BitmapScalingMode ScalingMode
			{
				get { return scalingMode; }
				set
				{
					if (scalingMode != value)
					{
						scalingMode = value;
						if (DataGridOwner != null)
							DataGridOwner.UpdateLayout();
					}
				}
			}

			public Column()
			{
				ScalingMode = swm.BitmapScalingMode.HighQuality;
			}

			swc.Image CreateImage()
			{
				var image = new swc.Image { MaxWidth = 16, MaxHeight = 16, StretchDirection = swc.StretchDirection.DownOnly, Margin = new sw.Thickness(0, 2, 2, 2) };
				swm.RenderOptions.SetBitmapScalingMode(image, ScalingMode);
				image.DataContextChanged += (sender, e) =>
				{
					var img = sender as swc.Image;
					img.Source = Handler.GetImageValue(img.DataContext) as swm.ImageSource;
				};
				return image;
			}

			sw.FrameworkElement SetupCell(sw.FrameworkElement element)
			{
				var container = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
				container.Children.Add(CreateImage());
				container.Children.Add(element);
				return Handler.SetupCell(container);
			}

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = base.GenerateElement(cell, dataItem);
				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.TextBlock;
					control.Text = Handler.GetTextValue(control.DataContext);
					Handler.FormatCell(control, cell, control.DataContext);
				};
				return SetupCell(element);
			}

			protected override object PrepareCellForEdit(sw.FrameworkElement editingElement, sw.RoutedEventArgs editingEventArgs)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox>("control");
				return base.PrepareCellForEdit(control, editingEventArgs);
			}

			protected override sw.FrameworkElement GenerateEditingElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.TextBox)base.GenerateEditingElement(cell, dataItem);
				element.Name = "control";
				element.DataContextChanged += (sender, e) =>
				{
					var control = sender as swc.TextBox;
					control.Text = Handler.GetTextValue(control.DataContext);
					Handler.FormatCell(control, cell, control.DataContext);
				};
				return SetupCell(element);
			}

			protected override bool CommitCellEdit(sw.FrameworkElement editingElement)
			{
				var control = editingElement as swc.TextBox ?? editingElement.FindChild<swc.TextBox>("control");
				Handler.SetTextValue(control.DataContext, control.Text);
				return true;
			}
		}

		public ImageTextCellHandler()
		{
			Control = new Column { Handler = this };
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return Control.ScalingMode.ToEto(); }
			set { Control.ScalingMode = value.ToWpf(); }
		}
	}
}
