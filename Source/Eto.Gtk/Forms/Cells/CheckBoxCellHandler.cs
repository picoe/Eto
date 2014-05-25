using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Cells
{
	public class CheckBoxCellHandler : SingleCellHandler<Gtk.CellRendererToggle, CheckBoxCell>, CheckBoxCell.IHandler
	{
		class Renderer : Gtk.CellRendererToggle
		{
			WeakReference handler;
			public CheckBoxCellHandler Handler { get { return (CheckBoxCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

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
				GtkCell.gtksharp_cellrenderer_invoke_render(Gtk.CellRendererToggle.GType.Val, Handle, window.Handle, widget.Handle, ref background_area, ref cell_area, ref expose_area, flags);
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

		public CheckBoxCellHandler()
		{
			Control = new Renderer { Handler = this };
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.Control.Toggled += Connector.HandleToggled;
		}

		protected new CheckBoxCellEventConnector Connector { get { return (CheckBoxCellEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new CheckBoxCellEventConnector();
		}

		protected class CheckBoxCellEventConnector : SingleCellConnector
		{
			public new CheckBoxCellHandler Handler { get { return (CheckBoxCellHandler)base.Handler; } }

			public void HandleToggled(object o, Gtk.ToggledArgs args)
			{
				Handler.SetValue(args.Path, !Handler.Control.Active);
			}

			public void HandleEndCellEditing(object o, Gtk.ToggledArgs args)
			{
				Handler.Source.EndCellEditing(new Gtk.TreePath(args.Path), Handler.ColumnIndex);
			}
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "active", dataIndex++);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
			Control.Activatable = editable;
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, value as bool?);
			}
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			if (Widget.Binding != null)
			{
				var ret = Widget.Binding.GetValue(dataItem);
				if (ret != null)
					return new GLib.Value(ret);
			}
			return new GLib.Value(false);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditedEvent:
					Control.Toggled += Connector.HandleEndCellEditing;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

