using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Cells
{
	public class TextBoxCellHandler : SingleCellHandler<Gtk.CellRendererText, TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler
	{
		class Renderer : Gtk.CellRendererText
		{
			WeakReference handler;
			public TextBoxCellHandler Handler { get { return (TextBoxCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			int row;
			[GLib.Property("row")]
			public int Row
			{
				get { return row; }
				set {
					row = value;
					if (Handler.FormattingEnabled)
						Handler.Format(new GtkTextCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Handler.Source.GetItem(Row), Row));
				}
			}

			#if GTK2
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}
			#else
			protected override void OnGetSize (Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.OnGetSize (widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
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

		protected class TextBoxCellEventConnector : CellConnector
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
			base.BindCell(ref dataIndex);
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
			Control.Editable = editable;
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.Binding != null)
			{
				Widget.Binding.SetValue(dataItem, Convert.ToString(value));
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

