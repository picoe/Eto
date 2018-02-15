using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Cells
{
	public class ImageViewCellHandler : SingleCellHandler<Gtk.CellRendererPixbuf, ImageViewCell, ImageViewCell.ICallback>, ImageViewCell.IHandler
	{
		class Renderer : Gtk.CellRendererPixbuf
		{
			WeakReference handler;
			public ImageViewCellHandler Handler { get { return (ImageViewCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			int row;
			[GLib.Property("row")]
			public int Row
			{
				get { return row; }
				set {
					row = value;
					if (Handler.FormattingEnabled)
						Handler.Format(new GtkGridCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Handler.Source.GetItem(Row), Row));
				}
			}

#if GTK2
			public override void GetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}
#else
			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				natural_size = Handler.Source.RowHeight;
			}
#endif
		}


		public ImageViewCellHandler ()
		{
			Control = new Renderer { Handler = this };
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.Control.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.Control.AddAttribute (Control, "pixbuf", dataIndex++);
			base.BindCell(ref dataIndex);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
		}
		
		public override void SetValue (object dataItem, object value)
		{
			// can't set
		}
		
		protected override GLib.Value GetValueInternal (object dataItem, int dataColumn, int row)
		{
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (dataItem);
				var image = ret as Image;
				if (image != null)
					return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16), ImageInterpolation.ToGdk()));
			}
			return new GLib.Value((Gdk.Pixbuf)null);
		}
		
		public override void AttachEvent (string id)
		{
			switch (id) {
			case Grid.CellEditedEvent:
				// no editing here
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public ImageInterpolation ImageInterpolation { get; set; }
	}
}

