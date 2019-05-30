using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Cells
{
	public class DrawableCellHandler : SingleCellHandler<Gtk.CellRenderer, DrawableCell, DrawableCell.ICallback>, DrawableCell.IHandler
	{
		class Renderer : Gtk.CellRenderer
		{
			WeakReference handler;

			public DrawableCellHandler Handler { get { return (DrawableCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

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
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			protected override void Render(Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				using (var graphics = new Graphics(new GraphicsHandler(widget, window)))
				{
					var item = Handler.Source.GetItem(Row);
#pragma warning disable 618
					var args = new DrawableCellPaintEventArgs(graphics, cell_area.ToEto(), flags.ToEto(), item);
					Handler.Callback.OnPaint(Handler.Widget, args);
#pragma warning restore 618
				}
			}
#else
			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				natural_size = Handler.Source.RowHeight;
			}
			
			protected override void OnRender (Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				using (var graphics = new Graphics(new GraphicsHandler(cr, null, false)))
				{
					var item = Handler.Source.GetItem(Row);
#pragma warning disable 618
					var args = new DrawableCellPaintEventArgs(graphics, cell_area.ToEto(), flags.ToEto(), item);
					Handler.Callback.OnPaint(Handler.Widget, args);
#pragma warning restore 618
				}
			}
#endif
		}


		public DrawableCellHandler()
		{
			Control = new Renderer { Handler = this };
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			base.BindCell(ref dataIndex);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
		}

		public override void SetValue(object dataItem, object value)
		{
			// can't set
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			return new GLib.Value(dataItem);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditedEvent:
				// no editing here
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

