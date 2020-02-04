using System;
using System.Windows.Controls.Primitives;
using System.Windows;
using Eto.Forms;
using System.Windows.Controls;
using Eto.CustomControls;
using swc = System.Windows.Controls;
using swcp = System.Windows.Controls.Primitives;
using swm = System.Windows.Media;
using System.Windows.Input;

namespace Eto.Wpf.CustomControls.TreeGridView
{
	public class TreeTogglePanel : DockPanel
	{
		public const int LevelWidth = 16;
		readonly TreeToggleButton button;
		readonly FrameworkElement content;

		public TreeTogglePanel(FrameworkElement content, TreeController controller)
		{
			Background = swm.Brushes.Transparent; // needed?
			button = new TreeToggleButton { Controller = controller, Width = 16 };
			SetDock(button, Dock.Left);
			Children.Add(button);
			Children.Add(content);
			this.content = content;

			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext is ITreeGridItem item)
			{
				button.Item = item;
				var index = button.Controller.IndexOf(item);
				button.IsChecked = button.Controller.IsExpanded(index);
				button.Visibility = item != null && item.Expandable ? Visibility.Visible : Visibility.Hidden;
				button.Margin = new Thickness(button.Controller.LevelAtRow(index) * LevelWidth, 0, 0, 0);
			}
		}

		public static bool? IsOverContent(DependencyObject hitTestResult)
		{
			if (hitTestResult is TreeTogglePanel)
				return false;
			var panel = hitTestResult.GetVisualParent<TreeTogglePanel>();
			if (panel == null)
				return null;

			while (hitTestResult != null && !ReferenceEquals(hitTestResult, panel))
			{
				if (ReferenceEquals(hitTestResult, panel.content))
					return true;
				hitTestResult = hitTestResult.GetVisualParent<DependencyObject>();
			}
			return false;
		}
	}

	public class TreeToggleButton : swcp.ToggleButton
	{
		public TreeController Controller { get; set; }

		public ITreeGridItem Item { get; set; }

		static TreeToggleButton ()
		{
			DefaultStyleKeyProperty.OverrideMetadata (typeof (TreeToggleButton), new FrameworkPropertyMetadata (typeof (TreeToggleButton)));
		}

		public static FrameworkElement Create (FrameworkElement content, TreeController controller)
		{
			return new TreeTogglePanel(content, controller);
		}


		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonUp(e);

			// only activate if the mouse wasn't moved outside the toggle button area
			var position = e.GetPosition(this);
			var size = this.GetSize();
			if (position.X >= 0 && position.Y >= 0 && position.X < size.Width && position.Y < size.Height)
			{
				Dispatcher.BeginInvoke(new Action(ToggleExpandCollapse));
				e.Handled = true;
			}
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown(e);
			e.Handled = true;
		}

		void ToggleExpandCollapse()
		{
			if (DataContext is ITreeGridItem item)
			{
				var index = Controller.IndexOf(item);
				if (index >= 0)
				{
					if (IsChecked ?? false)
					{
						if (Controller.CollapseRow(index))
						{
							IsChecked = false;
						}
					}
					else if (Controller.ExpandRow(index))
					{
						IsChecked = true;
					}
				}
			}
		}
	}
}
