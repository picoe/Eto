using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ImageTextCellHandler : CellHandler<Gtk.CellRendererText, ImageTextCell>, IImageTextCell
	{
		Gtk.CellRendererPixbuf imageCell;
		int imageDataIndex;
		int textDataIndex;
		
		public ImageTextCellHandler ()
		{
			imageCell = new Gtk.CellRendererPixbuf ();
			Control = new Gtk.CellRendererText ();
		}
		
		public override void AddCells (Gtk.TreeViewColumn column)
		{
			column.PackStart (imageCell, false);
			column.PackStart (Control, true);
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			imageDataIndex = dataIndex;
			Column.AddAttribute (imageCell, "pixbuf", dataIndex++);
			SetColumnMap (dataIndex);
			textDataIndex = dataIndex;
			Column.AddAttribute (Control, "text", dataIndex++);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
		}
		
		public override void SetValue (object dataItem, object value)
		{
			// can't set
		}
		
		public override GLib.Value GetValue (object item, int column)
		{
			if (column == imageDataIndex) {
				if (Widget.ImageBinding != null) {
					var ret = Widget.ImageBinding.GetValue (item);
					var image = ret as Image;
					if (image != null)
						return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16)));
				}
				return new GLib.Value((Gdk.Pixbuf)null);
			}
			else if (column == textDataIndex) {
				var ret = Widget.TextBinding.GetValue (item);
				if (ret != null)
					return new GLib.Value (Convert.ToString (ret));
			}
			return new GLib.Value((string)null);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case GridView.EndCellEditEvent:
				// no editing here
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
	}
}

