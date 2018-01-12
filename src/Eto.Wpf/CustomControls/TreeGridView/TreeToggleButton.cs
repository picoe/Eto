using System;
using System.Windows.Controls.Primitives;
using System.Windows;
using Eto.Forms;
using System.Windows.Controls;
using Eto.CustomControls;
using swc = System.Windows.Controls;

namespace Eto.Wpf.CustomControls.TreeGridView
{
	public class TreeToggleButton : ToggleButton
	{
		public const int LevelWidth = 16;

		public TreeController Controller { get; set; }

		public ITreeGridItem Item { get; private set; }

		static TreeToggleButton ()
		{
			DefaultStyleKeyProperty.OverrideMetadata (typeof (TreeToggleButton), new FrameworkPropertyMetadata (typeof (TreeToggleButton)));
		}

		public static FrameworkElement Create (FrameworkElement content, TreeController controller)
		{
			var dock = new DockPanel();
			var button = new TreeToggleButton { Controller = controller, Width = 16 };
			DockPanel.SetDock(button, Dock.Left);
			dock.Children.Add (button);
			dock.DataContextChanged += (sender, e) => button.Configure(dock.DataContext as ITreeGridItem);
			dock.Children.Add (content);
			return dock;
		}

		protected override void OnPreviewMouseLeftButtonDown (System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown (e);
			
			var index = Controller.IndexOf ((ITreeGridItem)DataContext);
			if (index >= 0) {
				Dispatcher.BeginInvoke (new Action (delegate {
					if (IsChecked ?? false) {
						if (Controller.CollapseRow (index)) {
							IsChecked = false;
						}
					}
					else if (Controller.ExpandRow (index)) {
						IsChecked = true;
					}
				}));
			}
			e.Handled = true;
		}

		public void Configure (ITreeGridItem item)
		{
			Item = item;
			var index = Controller.IndexOf (item);
			IsChecked = Controller.IsExpanded (index);
			Visibility = item != null && item.Expandable ? Visibility.Visible : Visibility.Hidden;
			Margin = new Thickness (Controller.LevelAtRow (index) * LevelWidth, 0, 0, 0);
		}
	}
}
