#if TODO_XAML
using System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Eto.Forms;
using Windows.UI.Xaml.Controls;
using Eto.Platform.CustomControls;

namespace Eto.WinRT.CustomControls.TreeGridView
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
			var panel = new StackPanel { Orientation = Orientation.Horizontal };
			var button = new TreeToggleButton { Controller = controller, Width = 16 };
			panel.Children.Add (button);
			panel.DataContextChanged += (sender, e) => button.Configure(panel.DataContext as ITreeGridItem);
			panel.Children.Add (content);
			return panel;
		}

		protected override void OnPreviewMouseLeftButtonDown (Windows.UI.Xaml.Input.MouseButtonEventArgs e)
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
#endif