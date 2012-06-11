using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using Eto.Forms;
using System.Windows.Controls;
using Eto.Platform.CustomControls;

namespace Eto.Platform.Wpf.CustomControls.TreeGridView
{
	public class TreeToggleButton : ToggleButton
	{
		public const int LEVEL_WIDTH = 16;

		public TreeController Controller { get; set; }

		public ITreeGridItem Item { get; private set; }

		static TreeToggleButton ()
		{
			DefaultStyleKeyProperty.OverrideMetadata (typeof (TreeToggleButton), new FrameworkPropertyMetadata (typeof (TreeToggleButton)));
		}

		public static FrameworkElement Create (FrameworkElement content, TreeController controller)
		{
			var panel = new StackPanel { Orientation = Orientation.Horizontal };
			var button = new TreeToggleButton { Controller = controller, Width = 16 };
			panel.Children.Add (button);
			panel.DataContextChanged += (sender, e) => {
				button.Configure (panel.DataContext as ITreeGridItem);
			};
			panel.Children.Add (content);
			return panel;
		}

		protected override void OnPreviewMouseLeftButtonDown (System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown (e);
			
			var index = Controller.IndexOf ((ITreeGridItem)this.DataContext);
			if (index >= 0) {
				Dispatcher.BeginInvoke (new Action (delegate {
					if (this.IsChecked ?? false) {
						if (Controller.CollapseRow (index)) {
							this.IsChecked = false;
						}
					}
					else if (Controller.ExpandRow (index)) {
						this.IsChecked = true;
					}
				}));
			}
			e.Handled = true;
		}

		public void Configure (ITreeGridItem item)
		{
			this.Item = item;
			var index = Controller.IndexOf (item);
			this.IsChecked = Controller.IsExpanded (index);
			this.Visibility = item != null && item.Expandable ? Visibility.Visible : Visibility.Hidden;
			this.Margin = new Thickness (Controller.LevelAtRow (index) * LEVEL_WIDTH, 0, 0, 0);
		}
	}
}
