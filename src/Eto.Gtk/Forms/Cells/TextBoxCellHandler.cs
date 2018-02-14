using System;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms.Cells
{
	interface ITextBoxCellHandler
	{
		bool FormattingEnabled { get; }
		void Format(GridCellFormatEventArgs args);
		AutoSelectMode AutoSelectMode { get; }
		GridColumnHandler Column { get; }
		ICellDataSource Source { get; }
	}

	public class TextBoxCellHandler : SingleCellHandler<Gtk.CellRendererText, TextBoxCell, TextBoxCell.ICallback>, TextBoxCell.IHandler, ITextBoxCellHandler
	{
		internal class Renderer : Gtk.CellRendererText, IEtoCellRenderer
		{
			Gdk.Rectangle cell_area;
			WeakReference handler;
			public ITextBoxCellHandler Handler { get { return (ITextBoxCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

#if GTK2
			public bool Editing => (bool)GetProperty("editing").Val;
#endif

			int row;
			[GLib.Property("row")]
			public int Row
			{
				get { return row; }
				set
				{
					row = value;
					if (Handler.FormattingEnabled)
						Handler.Format(new GtkTextCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Handler.Source.GetItem(Row), Row));
				}
			}

			void HandleFocusInEventHandler(object o, Gtk.FocusInEventArgs args)
			{
				var entry = o as Gtk.Entry;
				var h = Handler;
				if (Mouse.Buttons == MouseButtons.Primary)
				{
					// translate mouse cursor to location in text
					int x, y, idx, trailing;
					entry.GetWindow().GetOrigin(out x, out y);
					x = (int)Math.Round(Mouse.Position.X) - x - cell_area.X;
					if (entry.Layout.XyToIndex(Pango.Units.FromPixels(x), 0, out idx, out trailing))
					{
						Application.Instance.AsyncInvoke(() => entry.SelectRegion(idx, idx));
						return;
					}
				}

				if (h.AutoSelectMode == AutoSelectMode.Never)
					Application.Instance.AsyncInvoke(() => entry.SelectRegion(entry.Text.Length, entry.Text.Length));
			}

			void ConfigureEntry(Gtk.Entry entry)
			{
				if (entry != null && Handler.AutoSelectMode != AutoSelectMode.Always)
				{
					entry.FocusInEvent += HandleFocusInEventHandler;
					entry.EditingDone += (sender, e) =>
					{
						entry.FocusInEvent -= HandleFocusInEventHandler;
					};
				}
			}

#if GTK2
			public override Gtk.CellEditable StartEditing(Gdk.Event evnt, Gtk.Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				this.cell_area = cell_area;
				var result = base.StartEditing(evnt ?? new Gdk.Event(IntPtr.Zero), widget, path, background_area, cell_area, flags);
				ConfigureEntry(result as Gtk.Entry);
				return result;
			}

			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}
#else
			protected override Gtk.ICellEditable OnStartEditing(Gdk.Event evnt, Gtk.Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				this.cell_area = cell_area;
				var result = base.OnStartEditing(evnt, widget, path, background_area, cell_area, flags);
				ConfigureEntry(result as Gtk.Entry);
				return result;
			}

			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				natural_size = Handler.Source.RowHeight;
			}
#endif
		}

		public TextBoxCellHandler()
		{
			Control = new Renderer { Handler = this };
			VerticalAlignment = VerticalAlignment.Center;
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.Control.Edited += Connector.HandleEdited;
		}

		protected new TextBoxCellEventConnector Connector { get { return (TextBoxCellEventConnector)base.Connector; } }

		public TextAlignment TextAlignment
		{
			get { return Control.Alignment.ToEto(); }
			set
			{
				Control.Alignment = value.ToPango();
				Control.Xalign = value.ToAlignment();
				Column?.Control?.TreeView?.QueueDraw();
			}
		}

		VerticalAlignment verticalAlignment = VerticalAlignment.Center;
		public VerticalAlignment VerticalAlignment
		{
			get { return verticalAlignment; }
			set
			{
				verticalAlignment = value;
				Control.Yalign = value.ToAlignment();
				Column?.Control?.TreeView?.QueueDraw();
			}
		}

		public AutoSelectMode AutoSelectMode { get; set; }

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

