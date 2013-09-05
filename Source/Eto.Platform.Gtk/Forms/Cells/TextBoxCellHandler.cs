using System;
using Eto.Forms;
using Eto.Drawing;
using System.Runtime.InteropServices;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public class TextBoxCellHandler : SingleCellHandler<Gtk.CellRendererText, TextBoxCell>, ITextBoxCell
	{
		class Renderer : Gtk.CellRendererText
		{
			public TextBoxCellHandler Handler { get; set; }

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

				// calling base crashes on windows
				GtkCell.gtksharp_cellrenderer_invoke_render (Gtk.CellRendererText.GType.Val, this.Handle, window.Handle, widget.Handle, ref background_area, ref  cell_area, ref expose_area, flags);
				//base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
		}

		public TextBoxCellHandler ()
		{
			Control = new Renderer { Handler = this };
			this.Control.Edited += delegate(object o, Gtk.EditedArgs args) {
				SetValue (args.Path, args.NewText);
			};
		}

		protected override void BindCell (ref int dataIndex)
		{
			Column.Control.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.Control.AddAttribute (Control, "text", dataIndex++);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Editable = editable;
		}
		
		public override void SetValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value as string);
			}
		}
		
		protected override GLib.Value GetValueInternal (object item, int column, int row)
		{
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (item);
				if (ret != null)
					return new GLib.Value (Convert.ToString (ret));
			}
			return new GLib.Value ((string)null);
		}
		
		public override void AttachEvent (string eventHandler)
		{
			switch (eventHandler) {
			case GridView.EndCellEditEvent:
				Control.Edited += (sender, e) => {
					Source.EndCellEditing (new Gtk.TreePath(e.Path), this.ColumnIndex);
				};
				break;
			default:
				base.AttachEvent (eventHandler);
				break;
			}
		}
	}
}

