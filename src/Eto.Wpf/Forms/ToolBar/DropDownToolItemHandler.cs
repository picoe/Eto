using Eto.Drawing;
using Eto.Forms;
using System;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Wpf.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<swc.Button, DropDownToolItem>, DropDownToolItem.IHandler
	{
		Image image;
		readonly swc.Image swcImage;
		readonly swc.TextBlock label;
		ContextMenu contextMenu;

		public DropDownToolItemHandler()
		{
			Control = new swc.Button();
			swcImage = new swc.Image { MaxHeight = 16, MaxWidth = 16 };
			label = new swc.TextBlock();
			var panel = new swc.StackPanel { Orientation = swc.Orientation.Horizontal };
			panel.Children.Add(swcImage);
			panel.Children.Add(label);
			Control.Content = panel;
			Control.Click += Control_Click;
			sw.Automation.AutomationProperties.SetLabeledBy(Control, label);
		}

		private void Control_Click(object sender, sw.RoutedEventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);

			var ctxMenu = contextMenu.ControlObject as swc.ContextMenu;
			if (ctxMenu != null)
			{
				ctxMenu.PlacementTarget = Control;
				ctxMenu.Placement = swc.Primitives.PlacementMode.Bottom;
				ctxMenu.IsOpen = true;
			}
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

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; }
		}
	}
}
