using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class ListBoxHandler : WindowsControl<SWF.ListBox, ListBox>, IListBox
	{
		ContextMenu contextMenu;

		class MyListBox : SWF.ListBox
		{
			public MyListBox ()
			{
				DrawMode = SWF.DrawMode.OwnerDrawFixed;
			}

			protected override void OnDrawItem (SWF.DrawItemEventArgs e)
			{
				e.DrawBackground ();
				e.DrawFocusRectangle ();

				if (e.Index == -1) return;
				using (var foreBrush = new SD.SolidBrush (e.ForeColor)) {
					var bounds = e.Bounds;
					var imageitem = Items[e.Index] as IImageListItem;
					if (imageitem != null) {
						int offset = bounds.Left;
						if (imageitem.Image != null) {
							var img = imageitem.Image.Handler as IWindowsImage;
							if (img != null)
								e.Graphics.DrawImage (img.GetImageWithSize(bounds.Height), bounds.Left, bounds.Top, bounds.Height, bounds.Height);
							offset += bounds.Height + 2;
						}
						e.Graphics.DrawString (imageitem.Text, e.Font, foreBrush, offset, bounds.Top);
					}
					else {
						var item = Items[e.Index] as IListItem;
						e.Graphics.DrawString (item.Text, e.Font, foreBrush, bounds.Left, bounds.Top);
					}
				}
				base.OnDrawItem (e);
			}
		}
		
		public ListBoxHandler()
		{
			Control = new MyListBox ();
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
