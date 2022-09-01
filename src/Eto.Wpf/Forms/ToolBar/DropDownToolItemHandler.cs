using System;
using Eto.Drawing;
using Eto.Forms;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using System.Windows;

namespace Eto.Wpf.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<swc.Menu, DropDownToolItem>, DropDownToolItem.IHandler
	{
		Image image;
		readonly swc.Image swcImage;
		readonly swc.TextBlock label;
		readonly swc.MenuItem root;
		readonly sw.Shapes.Path arrow;

		public DropDownToolItemHandler ()
		{
			root = new swc.MenuItem();
			Control = new swc.Menu();
			Control.Items.Add(root);
			swcImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock();
			arrow = new sw.Shapes.Path { Data = swm.Geometry.Parse("M 0 0 L 3 3 L 6 0 Z"), VerticalAlignment = sw.VerticalAlignment.Center, Margin = new Thickness(8, 2, 0, 0), Fill = swm.Brushes.Black };
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal, Children = { swcImage, label, arrow } };

			root.Header = panel;
			root.Click += Control_Click;
			root.SubmenuOpened += Control_Click;
			sw.Automation.AutomationProperties.SetLabeledBy(Control, label);
		}

		private void Control_Click(object sender, RoutedEventArgs e)
		{
			// WPF raises this event for all child items as well as the root menu, so check sender
			if (e.OriginalSource != root)
				return;

			Widget.OnClick(EventArgs.Empty);
		}

		public override string Text
		{
			get { return label.Text.ToEtoMnemonic(); }
			set { label.Text = value.ToPlatformMnemonic(); }
		}

		public override string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public override Image Image
		{
			get { return image; }
			set
			{
				image = value;
				swcImage.Source = image.ToWpf(Screen.PrimaryScreen.LogicalPixelSize, swcImage.GetMaxSize().ToEtoSize());
			}
		}

		public override bool Enabled
		{
			get { return Control.IsEnabled; }
			set
			{
				Control.IsEnabled = value;
				swcImage.IsEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the drop arrow is shown on the button.
		/// </summary>
		public bool ShowDropArrow
		{
			get { return arrow.Visibility == Visibility.Visible; }
			set { arrow.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
		}

		public void AddMenu(int index, MenuItem item)
		{
			root.Items.Insert(index, (swc.MenuItem)item.ControlObject);
		}

		public void RemoveMenu(MenuItem item)
		{
			root.Items.Remove((swc.MenuItem)item.ControlObject);
		}

		public void Clear()
		{
			root.Items.Clear();
		}
	}
}
