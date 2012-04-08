using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using Eto.Forms;
using System.Windows.Controls;

namespace Eto.Platform.Wpf.CustomControls.TreeGridView
{
	public class TreeToggleButton : ToggleButton
	{
		public const int LEVEL_WIDTH = 16;

		public TreeController Controller { get; set; }

		public ITreeItem Item { get; private set; }

		static TreeToggleButton ()
		{
			DefaultStyleKeyProperty.OverrideMetadata (typeof (TreeToggleButton), new FrameworkPropertyMetadata (typeof (TreeToggleButton)));
		}

		public static FrameworkElement Create (FrameworkElement content, TreeController controller)
		{
			var panel = new StackPanel { Orientation = Orientation.Horizontal };
			var button = new TreeToggleButton { Controller = controller };
			panel.Children.Add (button);
			panel.DataContextChanged += (sender, e) => {
				button.Configure (panel.DataContext as ITreeItem);
			};
			panel.Children.Add (content);
			return panel;
		}

		protected override void OnClick ()
		{
			base.OnClick ();
			var index = Controller.IndexOf ((ITreeItem)this.DataContext);
			if (index >= 0) {
				if (!Controller.IsExpanded (index))
					Controller.ExpandRow (index);
				else
					Controller.CollapseRow (index);
			}
		}

		public void Configure (ITreeItem item)
		{
			this.Item = item;
			var index = Controller.IndexOf (item);
			this.IsChecked = Controller.IsExpanded (index);
			this.Visibility = item != null && item.Expandable ? Visibility.Visible : Visibility.Hidden;
			this.Margin = new Thickness (Controller.LevelAtRow (index) * LEVEL_WIDTH, 0, 0, 0);
		}
	}
}
