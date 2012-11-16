using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public class CheckBoxCellHandler : SingleCellHandler<Gtk.CellRendererToggle, CheckBoxCell>, ICheckBoxCell
	{
		class Renderer : Gtk.CellRendererToggle
		{
			public CheckBoxCellHandler Handler { get; set; }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }

#if GTK2
			public override void GetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void Render (Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<Renderer> (this, Handler.Column.Widget, Item, Row));

				// calling base crashes on windows
				GtkCell.gtksharp_cellrenderer_invoke_render (Gtk.CellRendererToggle.GType.Val, this.Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
				//base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
#else
			protected override void OnGetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.OnGetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void OnRender (Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<Renderer> (this, Handler.Column.Widget, Item, Row));
				base.OnRender (cr, widget, background_area, cell_area, flags);
			}
#endif
		}

		public CheckBoxCellHandler ()
		{
			Control = new Renderer { Handler = this };
			this.Control.Toggled += delegate(object o, Gtk.ToggledArgs args) {
				SetValue (args.Path, !Control.Active);
			};
		}
		
		protected override void BindCell (ref int dataIndex)
		{
			Column.Control.ClearAttributes (Control);
			SetColumnMap (dataIndex);
			Column.Control.AddAttribute (Control, "active", dataIndex++);
		}
		
		public override void SetEditable (Gtk.TreeViewColumn column, bool editable)
		{
			this.Control.Activatable = editable;
		}

		public override void SetValue (object dataItem, object value)
		{
			if (Widget.Binding != null) {
				Widget.Binding.SetValue (dataItem, value);
			}
		}
		
		protected override GLib.Value GetValueInternal (object item, int column, int row)
		{
			if (Widget.Binding != null) {
				var ret = Widget.Binding.GetValue (item);
				if (ret != null)
					return new GLib.Value (ret);
			}
			return new GLib.Value ((bool)false);
		}

		public override void AttachEvent (string eventHandler)
		{
			switch (eventHandler) {
			case GridView.EndCellEditEvent:
				Control.Toggled += (sender, e) => {
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

