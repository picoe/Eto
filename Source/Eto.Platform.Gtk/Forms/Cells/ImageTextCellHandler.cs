using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<Gtk.CellRendererText, ImageTextCell>, IImageTextCell
	{
		Gtk.CellRendererPixbuf imageCell;
		int imageDataIndex;
		int textDataIndex;

		class Renderer : Gtk.CellRendererText
		{
			public ImageTextCellHandler Handler { get; set; }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }


			public override void GetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void Render (Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkTextCellFormatEventArgs<Renderer> (this, Handler.Column.Widget, Item, Row));
			
				// calling base crashes on windows /w gtk 2.12.9
				GtkCell.gtksharp_cellrenderer_invoke_render (Gtk.CellRendererText.GType.Val, this.Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
				//base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
		}

		class ImageRenderer : Gtk.CellRendererPixbuf
		{
			public ImageTextCellHandler Handler { get; set; }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }

			protected override void Render (Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<ImageRenderer> (this, Handler.Column.Widget, Item, Row));
				base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
		}


		public ImageTextCellHandler ()
		{
			imageCell = new ImageRenderer { Handler = this };
			Control = new Renderer { Handler = this };
			this.Control.Edited += delegate(object o, Gtk.EditedArgs args) {
				SetValue (args.Path, args.NewText);
			};
		}
		
		public override void AddCells (Gtk.TreeViewColumn column)
		{
			column.PackStart (imageCell, false);
			column.PackStart (Control, true);
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.Control.ClearAttributes (Control);
			imageDataIndex = SetColumnMap (dataIndex);
			Column.Control.AddAttribute (imageCell, "pixbuf", dataIndex++);
			textDataIndex = SetColumnMap (dataIndex);
			Column.Control.AddAttribute (Control, "text", dataIndex++);
			BindBase (imageCell, ref dataIndex);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Editable = editable;
		}
		
		public override void SetValue (object dataItem, object value)
		{
			if (Widget.TextBinding != null) {
				Widget.TextBinding.SetValue (dataItem, value as string);
			}
		}
		
		protected override GLib.Value GetValueInternal (object item, int column, int row)
		{
			if (column == imageDataIndex) {
				if (Widget.ImageBinding != null) {
					var ret = Widget.ImageBinding.GetValue (item);
					var image = ret as Image;
					if (image != null)
						return new GLib.Value (((IGtkPixbuf)image.Handler).GetPixbuf (new Size (16, 16)));
				}
				return new GLib.Value ((Gdk.Pixbuf)null);
			} else if (column == textDataIndex) {
				var ret = Widget.TextBinding.GetValue (item);
				if (ret != null)
					return new GLib.Value (Convert.ToString (ret));
			}
			return new GLib.Value ((string)null);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case GridView.EndCellEditEvent:
				Control.Edited += (sender, e) => {
					Source.EndCellEditing (new Gtk.TreePath (e.Path), this.ColumnIndex);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
	}
}

