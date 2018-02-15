using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Cells
{
	public class ProgressCellHandler : SingleCellHandler<Gtk.CellRendererProgress, ProgressCell, ProgressCell.ICallback>, ProgressCell.IHandler
	{
		int valueCol;
		class Renderer : Gtk.CellRendererProgress
		{
			WeakReference handler;
			public ProgressCellHandler Handler { get { return (ProgressCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			[GLib.Property("hasValue")]
			public bool HasValue { get; set; }

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
				if (!HasValue)
					return;

				// calling base crashes on windows
				GtkCell.gtksharp_cellrenderer_invoke_render(Gtk.CellRendererProgress.GType.Val, Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
				//base.Render (window, widget, background_area, cell_area, expose_area, flags);
			}
			#else
			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				natural_size = Handler.Source.RowHeight;
			}

			protected override void OnRender (Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				if (!HasValue)
					return;

				base.OnRender (cr, widget, background_area, cell_area, flags);
			}
			#endif
		}

		public ProgressCellHandler()
		{
			// We attach the cell formatting event to set FormattingEnabled to true so we can get the progress value in the renderer.
			AttachEvent(Grid.CellFormattingEvent);
			Control = new Renderer { Handler = this };
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			valueCol = SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "value", dataIndex++);
			SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "hasValue", dataIndex++);
			base.BindCell(ref dataIndex);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				float? progress = value as float?;
				if (progress.HasValue)
					progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
				Widget.Binding.SetValue(dataItem, progress);
			}
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			if (Widget.Binding != null)
			{
				float? progress = Widget.Binding.GetValue(dataItem);
				if (progress.HasValue)
				{
					if (dataColumn == valueCol)
					{
						progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
						return new GLib.Value((int)(progress * 100f));
					}
					return new GLib.Value(true);
				}
			}

			if (dataColumn == valueCol)
				return new GLib.Value(0);
			else
				return new GLib.Value(false);
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

