using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Cells
{
	public class ProgressCellHandler : SingleCellHandler<Gtk.CellRendererProgress, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		class Renderer : Gtk.CellRendererProgress
		{
			WeakReference handler;
			public ProgressCellHandler Handler { get { return (ProgressCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			[GLib.Property("item")]
			public object Item { get; set; }

			[GLib.Property("row")]
			public int Row { get; set; }
			#if GTK2
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void Render(Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (Handler.FormattingEnabled)
					Handler.Format(new GtkGridCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Item, Row));

				// calling base crashes on windows
				GtkCell.gtksharp_cellrenderer_invoke_render(Gtk.CellRendererProgress.GType.Val, Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
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

		public ProgressCellHandler()
		{
			Control = new Renderer { Handler = this };
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "value", dataIndex++);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
				Widget.Binding.SetValue(dataItem, value is int ? (int)value : 0);
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			if (Widget.Binding != null)
				return new GLib.Value(Widget.Binding.GetValue(dataItem));

			return new GLib.Value(0);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

