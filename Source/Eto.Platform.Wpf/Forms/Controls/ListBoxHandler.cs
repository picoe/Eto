using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using Eto.Platform.Wpf.Drawing;
using System.Collections;
using System.Collections.ObjectModel;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ListBoxHandler : WpfControl<swc.ListBox, ListBox>, IListBox
	{
		IListStore store;
		ContextMenu contextMenu;

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			return base.GetPreferredSize(sw.Size.Empty);
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

		public override bool UseMousePreview { get { return true; } }

		public IListStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				var source = store as ObservableCollection<IListItem>; 
				if (source != null)
					Control.ItemsSource = source;
				else
					Control.ItemsSource = new ObservableCollection<IListItem>(store.AsEnumerable());
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
				if (contextMenu != null)
					Control.ContextMenu = contextMenu.ControlObject as System.Windows.Controls.ContextMenu;
				else
					Control.ContextMenu = null;
			}
		}
	}
}
