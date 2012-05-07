using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Controls
{
	public class ImageViewCellHandler : SingleCellHandler<Gtk.CellRendererPixbuf, ImageViewCell>, IImageViewCell
	{
		class EtoCellRendererPixbuf : Gtk.CellRendererPixbuf
		{
		}
		
		public ImageViewCellHandler ()
		{
			Control = new EtoCellRendererPixbuf ();
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.AddAttribute (Control, "pixbuf", dataIndex++);
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
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (item);
				var image = ret as Image;
				if (image != null)
					return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16)));
			}
			return new GLib.Value((Gdk.Pixbuf)null);
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

