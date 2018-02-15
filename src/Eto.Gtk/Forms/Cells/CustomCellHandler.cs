using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

#if GTK2
using IGtkCellEditable = Gtk.CellEditable;
using IGtkCellEditableImplementor = Gtk.CellEditableImplementor;
#elif GTK3
using IGtkCellEditable = Gtk.ICellEditable;
using IGtkCellEditableImplementor = Gtk.ICellEditableImplementor;
#endif

namespace Eto.GtkSharp.Forms.Cells
{
	class EtoCellEditable : Gtk.Alignment, IGtkCellEditable, IGtkCellEditableImplementor
	{
		static GLib.GType gtype = GLib.GType.Invalid;

		public Control Content { get; set; }

		public EtoCellEditable()// : base(GType)
			: base(0, 0, 1, 1)
		{
		}

		public static new GLib.GType GType
		{
			get
			{
				gtype = RegisterGType(typeof(EtoCellEditable));
				return gtype;
			}
		}

#pragma warning disable 67
		public event EventHandler EditingDone;
		public event EventHandler WidgetRemoved;
#pragma warning restore 67

		public void RemoveWidget()
		{
		}

		public void FinishEditing()
		{
		}

		public void StartEditing(Gdk.Event evnt)
		{
		}

		[GLib.Property("editing-canceled")]
		public bool EditingCancelled
		{
			get;set;
		}
	}

	public class CustomCellHandler : SingleCellHandler<Gtk.CellRenderer, CustomCell, CustomCell.ICallback>, CustomCell.IHandler
	{
		class Renderer : Gtk.CellRenderer
		{
			WeakReference handler;

			public CustomCellHandler Handler { get { return (CustomCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			#if GTK2
			public Renderer()
			{
				this.EditingStarted += (o, args) => editingRow = row;
				this.EditingCanceled += (o, args) => editingRow = -1;
				//this.Edited += (o, args) => editingRow = -1;
			}
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
						Handler.Format(new GtkGridCellFormatEventArgs<Renderer>(this, Handler.Column.Widget, Handler.Source.GetItem(Row), Row));
				}
			}

			IGtkCellEditable CreateEditable(Gdk.Rectangle cellArea)
			{
				var item = Handler.Source.GetItem(Row);
				var args = new CellEventArgs(Row, item, CellStates.Editing);

				var ed = new EtoCellEditable();
				ed.Content = Handler.Callback.OnCreateCell(Handler.Widget, args);
				ed.Child = ed.Content.ToNative(true);
				ed.ShowAll();

				ed.SetSizeRequest(cellArea.Width, cellArea.Height);

				Handler.Callback.OnConfigureCell(Handler.Widget, args, ed.Content);
				//ed.PackStart(c.ToNative(true), true, true, 0);
				return ed;
			}


			#if GTK2
			int editingRow = -1;
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				height = Math.Max(height, Handler.Source.RowHeight);
			}

			public override Gtk.CellEditable StartEditing(Gdk.Event evnt, Gtk.Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				return CreateEditable(cell_area);
			}

			protected override void Render(Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				if (editingRow == row)
				{
					return;
				}
				using (var graphics = new Graphics(new GraphicsHandler(widget, window)))
				{
					var item = Handler.Source.GetItem(Row);
					var args = new CellPaintEventArgs(graphics, cell_area.ToEto(), flags.ToEto(), item);
					Handler.Callback.OnPaint(Handler.Widget, args);
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
			}


			protected override IGtkCellEditable OnStartEditing(Gdk.Event evnt, Gtk.Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				return CreateEditable(cell_area);
			}

			protected override void OnEditingStarted(IGtkCellEditable editable, string path)
			{
				base.OnEditingStarted(editable, path);
			}
			#endif
		}


		public CustomCellHandler()
		{
			Control = new Renderer { Handler = this, Mode = Gtk.CellRendererMode.Editable };
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
			return new GLib.Value(row);
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

