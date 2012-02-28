using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ImageCellHandler : CellHandler<Gtk.CellRendererPixbuf, ImageCell>, IImageCell
	{
		class EtoCellRendererPixbuf : Gtk.CellRendererPixbuf
		{
		}
		
		public ImageCellHandler ()
		{
			Control = new EtoCellRendererPixbuf ();
		}
		
		protected override void BindCell ()
		{
			Column.ClearAttributes (Control);
			Column.AddAttribute (Control, "pixbuf", ColumnIndex);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
		}
		
		public override GLib.Value GetValue (IGridItem item, int column)
		{
			if (item == null) return new GLib.Value((Gdk.Pixbuf)null);
			var ret = item.GetValue (column);
			var image = ret as Image;
			if (image == null) return new GLib.Value((Gdk.Pixbuf)null);
			return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16)));
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

