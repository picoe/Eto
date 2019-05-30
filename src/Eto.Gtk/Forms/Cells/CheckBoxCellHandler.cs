using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Cells
{
	public class CheckBoxCellHandler : SingleCellHandler<Gtk.CellRendererToggle, CheckBoxCell, CheckBoxCell.ICallback>, CheckBoxCell.IHandler
	{
		class Renderer : Gtk.CellRendererToggle, IEtoCellRenderer
		{
			WeakReference handler;
			public CheckBoxCellHandler Handler { get { return (CheckBoxCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

#if GTK2
			public bool Editing => (bool)GetProperty("editing").Val;
#endif

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
#else
			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				natural_size = Handler.Source.RowHeight;
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

		protected class CheckBoxCellEventConnector : CellConnector
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
			base.BindCell(ref dataIndex);
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

