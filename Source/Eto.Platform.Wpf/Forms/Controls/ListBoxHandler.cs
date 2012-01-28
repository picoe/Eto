using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using swc = System.Windows.Controls;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ListBoxHandler : WpfControl<swc.ListBox, ListBox>, IListBox
	{
		ContextMenu contextMenu;

		public ListBoxHandler ()
		{
			Control = new swc.ListBox ();
			Control.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			Control.DisplayMemberPath = "Text";
			Control.SelectionChanged += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
			Control.MouseDoubleClick += delegate {
				Widget.OnActivated (EventArgs.Empty);
			};
			Control.KeyDown += (sender, e) => {
				if (e.Key == sw.Input.Key.Return) {
					Widget.OnActivated (EventArgs.Empty);
					e.Handled = true;
				}
			};
			
		}

		public void AddRange (IEnumerable<IListItem> collection)
		{
			foreach (var item in collection)
				AddItem (item);
		}

		public void AddItem (IListItem item)
		{
			Control.Items.Add (item);
		}

		public void RemoveItem (IListItem item)
		{
			Control.Items.Remove (item);
		}

		public void RemoveAll ()
		{
			Control.Items.Clear ();
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
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
