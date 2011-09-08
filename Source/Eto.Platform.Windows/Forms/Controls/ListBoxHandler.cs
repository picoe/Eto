using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Windows
{
	public class ListBoxHandler : WindowsControl<SWF.ListBox, ListBox>, IListBox
	{
		ContextMenu contextMenu;
		
		public ListBoxHandler()
		{
			Control = new SWF.ListBox();
			this.Control.ValueMember = "Key";
			this.Control.FormattingEnabled = true;
			this.Control.Format += delegate(object sender, SWF.ListControlConvertEventArgs e) {
				var item = e.ListItem as IListItem;
				e.Value = item.Text;
			};
			Control.SelectedIndexChanged += control_SelectedIndexChanged;
			Control.IntegralHeight = false;
			Control.DoubleClick += control_DoubleClick;
			Control.KeyDown += control_KeyDown;
		}

		public ContextMenu ContextMenu {
			get {
				return contextMenu;
			}
			set {
				contextMenu = value;
				if (contextMenu != null)
					this.Control.ContextMenuStrip = contextMenu.ControlObject as SWF.ContextMenuStrip;
				else
					this.Control.ContextMenuStrip = null;
			}
		}

		public void AddRange (IEnumerable<IListItem> collection)
		{
			this.Control.SuspendLayout();
			this.Control.Items.AddRange(collection.ToArray());
			this.Control.ResumeLayout();
		}

		public void AddItem(IListItem item)
		{
			Control.Items.Add(item);
		}

		public void RemoveItem(IListItem item)
		{
			Control.Items.Remove(item);
		}

		public int SelectedIndex
		{
			get	{ return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public void RemoveAll()
		{
			Control.Items.Clear();
		}

		private void control_SelectedIndexChanged(object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged(e);
		}

		private void control_DoubleClick(object sender, EventArgs e)
		{
			Widget.OnActivated(EventArgs.Empty);
		}

		private void control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == SWF.Keys.Return)
			{
				Widget.OnActivated(EventArgs.Empty);
				e.Handled = true;
			}
		}
	}
}
