#if TODO_XAML
using System;
using System.Linq;
using Eto.Forms;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using swd = Windows.UI.Xaml.Data;
using System.Collections.ObjectModel;

namespace Eto.WinRT.Forms.Controls
{
	public class ListBoxHandler : WpfControl<swc.ListBox, ListBox>, IListBox
	{
		IListStore store;
		ContextMenu contextMenu;

		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			return base.GetPreferredSize(wf.Size.Empty);
		}

		public ListBoxHandler()
		{
			Control = new swc.ListBox();
			Control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			//Control.DisplayMemberPath = "Text";
			var template = new sw.DataTemplate(typeof(IListItem));

			template.VisualTree = WpfListItemHelper.ItemTemplate(false);
			Control.ItemTemplate = template;
			Control.SelectionChanged += delegate
			{
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};
			Control.MouseDoubleClick += delegate
			{
				if (SelectedIndex >= 0)
					Widget.OnActivated(EventArgs.Empty);
			};
			Control.KeyDown += (sender, e) =>
			{
				if (e.Key == sw.Input.Key.Return)
				{
					if (SelectedIndex >= 0)
					{
						Widget.OnActivated(EventArgs.Empty);
						e.Handled = true;
					}
				}
			};
		}

		public override void Focus()
		{
			if (Control.IsLoaded)
			{
				var item = Control.ItemContainerGenerator.ContainerFromIndex(Math.Max(0, SelectedIndex)) as sw.FrameworkElement;
				if (item != null)
					item.Focus();
				else
					Control.Focus();
			}
			else
			{
				Control.Loaded += Control_Loaded;
			}
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			Focus();
			Control.Loaded -= Control_Loaded;
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public IListStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				var source = store as ObservableCollection<IListItem>; 
				Control.ItemsSource = source ?? new ObservableCollection<IListItem>(store.AsEnumerable());
			}
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set
			{
				Control.SelectedIndex = value;
				if (value >= 0)
				{
					var item = store.AsEnumerable().Skip(value).FirstOrDefault();
					Control.ScrollIntoView(item);
				}
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.ContextMenu = contextMenu != null ? contextMenu.ControlObject as sw.Controls.ContextMenu : null;
			}
		}
	}
}
#endif