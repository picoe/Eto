using System.Windows;

namespace Eto.Wpf.Forms.ToolBar
{
	public class DropDownToolItemHandler : ToolItemHandler<swc.Menu, DropDownToolItem>, DropDownToolItem.IHandler
	{
		readonly swc.MenuItem root;
		readonly sw.Shapes.Path arrow;

		public DropDownToolItemHandler ()
		{
			root = new swc.MenuItem();
			Control = new swc.Menu();
			Control.Background = swm.Brushes.Transparent;
			Control.Items.Add(root);
			arrow = new sw.Shapes.Path { Data = swm.Geometry.Parse("M 0 0 L 3 3 L 6 0 Z"), VerticalAlignment = sw.VerticalAlignment.Center, Margin = new Thickness(2, 2, 0, 0), Fill = swm.Brushes.Black };

			root.Click += Control_Click;
			root.SubmenuOpened += Control_Click;
		}

		protected override void Initialize()
		{
			base.Initialize();
			root.Header = CreateContent(arrow);
		}

		private void Control_Click(object sender, RoutedEventArgs e)
		{
			// WPF raises this event for all child items as well as the root menu, so check sender
			if (e.OriginalSource != root)
				return;

			Widget.OnClick(EventArgs.Empty);
		}

		public override string ToolTip
		{
			get { return Control.ToolTip as string; }
			set { Control.ToolTip = value; }
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
