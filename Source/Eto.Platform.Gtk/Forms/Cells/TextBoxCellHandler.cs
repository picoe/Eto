using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public class TextBoxCellHandler : SingleCellHandler<Gtk.CellRendererText, TextBoxCell>, ITextBoxCell
	{
		class Renderer : Gtk.CellRendererText
		{
			WeakReference handler;
			public TextBoxCellHandler Handler { get { return (TextBoxCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

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
					Handler.Format(new GtkTextCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Item, Row));

				// calling base crashes on windows
				GtkCell.gtksharp_cellrenderer_invoke_render(Gtk.CellRendererText.GType.Val, Handle, window.Handle, widget.Handle, ref background_area, ref  cell_area, ref expose_area, flags);
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

		public TextBoxCellHandler()
		{
			Control = new Renderer { Handler = this };
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.Control.Edited += Connector.HandleEdited;
		}

		protected new TextBoxCellEventConnector Connector { get { return (TextBoxCellEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TextBoxCellEventConnector();
		}

		protected class TextBoxCellEventConnector : SingleCellConnector
		{
			public new TextBoxCellHandler Handler { get { return (TextBoxCellHandler)base.Handler; } }

			public void HandleEdited(object o, Gtk.EditedArgs args)
			{
				Handler.SetValue(args.Path, args.NewText);
			}

			public void HandleEndEditing(object o, Gtk.EditedArgs args)
			{
				Handler.Source.EndCellEditing(new Gtk.TreePath(args.Path), Handler.ColumnIndex);
			}
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "text", dataIndex++);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
			Control.Editable = editable;
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, value);
			}
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			if (Widget.Binding != null)
			{
				var ret = Widget.Binding.GetValue(dataItem);
				if (ret != null)
					return new GLib.Value(Convert.ToString(ret));
			}
			return new GLib.Value((string)null);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Grid.CellEditedEvent:
					Control.Edited += Connector.HandleEndEditing;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}

