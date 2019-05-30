using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Collections;

namespace Eto.GtkSharp.Forms.Cells
{
	public class ComboBoxCellHandler : SingleCellHandler<Gtk.CellRendererCombo, ComboBoxCell, ComboBoxCell.ICallback>, ComboBoxCell.IHandler
	{
		CollectionHandler collection;
		readonly Gtk.ListStore listStore;

		class Renderer : Gtk.CellRendererCombo, IEtoCellRenderer
		{
			WeakReference handler;
			public ComboBoxCellHandler Handler { get { return (ComboBoxCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

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
			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				natural_size = Handler.Source.RowHeight;
			}
#endif
		}

		public ComboBoxCellHandler()
		{
			listStore = new Gtk.ListStore(typeof(string), typeof(string));
			Control = new Renderer
			{
				Handler = this,
				Model = listStore,
				TextColumn = 0,
				HasEntry = false
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.Control.Edited += Connector.HandleEdited;
		}

		protected new ComboBoxCellEventConnector Connector { get { return (ComboBoxCellEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ComboBoxCellEventConnector();
		}

		protected class ComboBoxCellEventConnector : CellConnector
		{
			public new ComboBoxCellHandler Handler { get { return (ComboBoxCellHandler)base.Handler; } }

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
				Widget.Binding.SetValue(dataItem, value);
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

		public class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ComboBoxCellHandler Handler { get; set; }

			public override void AddItem(object item)
			{
				Handler.listStore.AppendValues(Handler.Widget.ComboTextBinding.GetValue(item), Handler.Widget.ComboKeyBinding.GetValue(item));
			}

			public override void InsertItem(int index, object item)
			{
				Handler.listStore.InsertWithValues(index, Handler.Widget.ComboTextBinding.GetValue(item), Handler.Widget.ComboKeyBinding.GetValue(item));
			}

			public override void RemoveItem(int index)
			{
				Gtk.TreeIter iter;
				if (Handler.listStore.IterNthChild(out iter, index))
					Handler.listStore.Remove(ref iter);
			}

			public override void RemoveAllItems()
			{
				Handler.listStore.Clear();
			}
		}

		public IEnumerable<object> DataStore
		{
			get { return collection != null ? collection.Collection : null; }
			set
			{
				if (collection != null)
					collection.Unregister();
				collection = new CollectionHandler{ Handler = this };
				collection.Register(value);
			}
		}
	}
}

