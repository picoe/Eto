using System;
using System.Linq;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using sw = System.Windows;
using Eto.Forms;
using Eto.Drawing;
using System.Windows;
using System.Windows.Input;

namespace Eto.Wpf.Forms.ToolBar
{
	public class RadioToolItemHandler : ToolItemHandler<swc.Primitives.ToggleButton, RadioToolItem>, RadioToolItem.IHandler
	{
		public RadioToolItemHandler()
		{
			Control = new swc.Primitives.ToggleButton
			{
				IsThreeState = false
			};
			Control.Checked += Control_Checked;
			Control.Unchecked += Control_Unchecked;
			Control.PreviewMouseDown += Control_PreviewMouseDown;
			Control.Click += Control_Click;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Content = CreateContent();
		}

		private void Control_Click(object sender, RoutedEventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}

		private void Control_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Checked)
			{
				Widget.OnClick(EventArgs.Empty);
				e.Handled = true;
			}
		}

		private void Control_Unchecked(object sender, RoutedEventArgs e)
		{
			Widget.OnCheckedChanged(EventArgs.Empty);
		}

		private void Control_Checked(object sender, RoutedEventArgs e)
		{
			var toolbar = Control.GetVisualParent<swc.ToolBar>();
			if (toolbar != null)
			{
				var toolbarHandler = toolbar.Tag as ToolBarHandler;
				if (toolbarHandler != null)
				{
					foreach (var item in toolbarHandler.Widget.Items.OfType<RadioToolItem>().Where(r => r != Widget))
					{
						item.Checked = false;
					}
				}
			}
			Widget.OnCheckedChanged(EventArgs.Empty);
		}

		public bool Checked
		{
			get { return Control.IsChecked ?? false; }
			set { Control.IsChecked = value; }
		}

		public override string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
		}

		public override bool Enabled
		{
			get { return Control.IsEnabled; }
			set
			{
				Control.IsEnabled = value;
				Control.Opacity = value ? 1 : 0.5;
			}
		}
	}
}
