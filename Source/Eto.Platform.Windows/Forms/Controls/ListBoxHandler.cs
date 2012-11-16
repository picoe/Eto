using System;
using System.Reflection;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class ListBoxHandler : WindowsControl<swf.ListBox, ListBox>, IListBox
	{
		ContextMenu contextMenu;
		CollectionHandler collection;

		class MyListBox : swf.ListBox
		{
			public MyListBox ()
			{
				DrawMode = swf.DrawMode.OwnerDrawFixed;
			}

			public override sd.Font Font
			{
				get
				{
					return base.Font;
				}
				set
				{
					base.Font = value;
					this.ItemHeight = value.Height;
				}
			}

			protected override void OnDrawItem (swf.DrawItemEventArgs e)
			{
				e.DrawBackground ();
				e.DrawFocusRectangle ();

				if (e.Index == -1)
					return;
				using (var foreBrush = new sd.SolidBrush (this.ForeColor)) {
					var bounds = e.Bounds;
					var imageitem = Items[e.Index] as IImageListItem;
					if (imageitem != null) {
						int offset = bounds.Left;
						if (imageitem.Image != null) {
							var img = imageitem.Image.Handler as IWindowsImage;
							if (img != null)
								e.Graphics.DrawImage (img.GetImageWithSize (bounds.Height), bounds.Left, bounds.Top, bounds.Height, bounds.Height);
							offset += bounds.Height + 2;
						}
						e.Graphics.DrawString (imageitem.Text, e.Font, foreBrush, offset, bounds.Top);
					} else {
						var item = Items[e.Index] as IListItem;
						e.Graphics.DrawString (item.Text, e.Font, foreBrush, bounds.Left, bounds.Top);
					}
				}
				base.OnDrawItem (e);
			}
		}
		
		public ListBoxHandler ()
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
					this.Control.ContextMenuStrip = contextMenu.ControlObject as swf.ContextMenuStrip;
				else
					this.Control.ContextMenuStrip = null;
			}
		}

		public int SelectedIndex {
			get	{ return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		private void control_SelectedIndexChanged (object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged (e);
		}

		private void control_DoubleClick (object sender, EventArgs e)
		{
			Widget.OnActivated (EventArgs.Empty);
		}

		private void control_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyData == swf.Keys.Return) {
				Widget.OnActivated (EventArgs.Empty);
				e.Handled = true;
			}
		}

		class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ListBoxHandler Handler { get; set; }
			
			public override int IndexOf (IListItem item)
			{
				return Handler.Control.Items.IndexOf (item);
			}
			
			public override void AddRange (IEnumerable<IListItem> items)
			{
				Handler.Control.Items.AddRange (items.ToArray ());
			}
			
			public override void AddItem (IListItem item)
			{
				Handler.Control.Items.Add (item);
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.Control.Items.Insert (index, item);
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Items.RemoveAt (index);
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Items.Clear ();
			}
		}

		public IListStore DataStore {
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value); 
			}
		}
	}
}
