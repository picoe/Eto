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
			Image image;

			[GLib.Property("image")]
			public Image Image {
				get { 
					return image; 
				}
				set {
					if (value == NullValue)
						image = null;
					else
						image = value;
					if (image != null) {
						this.Pixbuf = ((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16));
					} else
						this.Pixbuf = null;
				}
			}
		}
		
		public ImageCellHandler ()
		{
			Control = new EtoCellRendererPixbuf ();
		}
		
		protected override void BindCell ()
		{
			Column.ClearAttributes (Control);
			Column.AddAttribute (Control, "image", ColumnIndex);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
		}
		
		static Bitmap NullValue;
		
		public override void GetNullValue (ref GLib.Value val)
		{
			if (NullValue == null)
				NullValue = new Bitmap (Widget.Generator, new BitmapHandler ());
			val = new GLib.Value (NullValue);
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

