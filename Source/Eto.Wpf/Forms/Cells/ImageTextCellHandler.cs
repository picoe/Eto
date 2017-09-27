using System;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Wpf.Drawing;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Wpf.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<ImageTextCellHandler.Column, ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler
	{
		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set { Control.TextAlignment = value.ToWpfTextAlignment(); }
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Control.VerticalAlignment.ToEto(); }
			set { Control.VerticalAlignment = value.ToWpf(); }
		}

		object GetImageValue(object dataItem)
		{
			if (Widget.ImageBinding != null)
			{
				var image = Widget.ImageBinding.GetValue(dataItem) as Image;
				if (image != null)
					return image.ToWpf();
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

		public class Column : swc.DataGridTextColumn, INotifyPropertyChanged
		{
			public ImageTextCellHandler Handler { get; set; }

			sw.TextAlignment _textAlignment;
			public sw.TextAlignment TextAlignment
			{
				get { return _textAlignment; }
				set
				{
					_textAlignment = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextAlignment)));
				}
			}

			sw.VerticalAlignment _verticalAlignment = sw.VerticalAlignment.Center;
			public sw.VerticalAlignment VerticalAlignment
			{
				get { return _verticalAlignment; }
				set
				{
					_verticalAlignment = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerticalAlignment)));
				}
			}

			swm.BitmapScalingMode scalingMode;

			public event PropertyChangedEventHandler PropertyChanged;

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
				var image = new swc.Image { StretchDirection = swc.StretchDirection.DownOnly, Margin = new sw.Thickness(0, 2, 2, 2) };
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
				element.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
				var container = new swc.DockPanel();
				container.Children.Add(CreateImage());
				container.Children.Add(element);
				return Handler.SetupCell(container);
			}

			swd.Binding CreateBinding(string property)
			{
				var binding = new swd.Binding();
				binding.Source = this;
				binding.Path = new sw.PropertyPath(property);
				binding.Mode = swd.BindingMode.OneWay;
				binding.UpdateSourceTrigger = swd.UpdateSourceTrigger.PropertyChanged;
				return binding;
			}

			protected override sw.FrameworkElement GenerateElement(swc.DataGridCell cell, object dataItem)
			{
				var element = (swc.TextBlock)base.GenerateElement(cell, dataItem);
				element.SetBinding(swc.TextBlock.TextAlignmentProperty, CreateBinding(nameof(TextAlignment)));
				element.SetBinding(swc.TextBlock.VerticalAlignmentProperty, CreateBinding(nameof(VerticalAlignment)));
				element.Text = Handler.GetTextValue(dataItem);
				Handler.FormatCell(element, cell, dataItem);
				element.DataContextChanged += (sender, e) =>
				{
					var control = (swc.TextBlock)sender;
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
				element.SetBinding(swc.TextBlock.TextAlignmentProperty, CreateBinding(nameof(TextAlignment)));
				element.SetBinding(swc.TextBlock.VerticalAlignmentProperty, CreateBinding(nameof(VerticalAlignment)));
				element.Name = "control";
				element.Text = Handler.GetTextValue(dataItem);
				Handler.FormatCell(element, cell, dataItem);
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
				Handler.ContainerHandler.CellEdited(Handler, editingElement);
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
