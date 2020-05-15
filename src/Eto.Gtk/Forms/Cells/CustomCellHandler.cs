using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Runtime.InteropServices;

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
			int editingRow = -1;

			public CustomCellHandler Handler { get { return (CustomCellHandler)handler.Target; } set { handler = new WeakReference(value); } }

			protected override void OnEditingStarted(IGtkCellEditable editable, string path)
			{
				//base.OnEditingStarted(editable, path);
				editable.EditingDone += Editable_EditingDone;
				editingRow = row;
			}

			private void Editable_EditingDone(object sender, EventArgs e)
			{
				editingRow = -1;
				var editable = sender as IGtkCellEditable;
				if (editable == null)
					return;
				editable.EditingDone -= Editable_EditingDone;
			}

			protected override void OnEditingCanceled()
			{
				//base.OnEditingCanceled();
				editingRow = -1;
			}

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
				int column = -1;
				var args = new CellEventArgs(null, Handler.Widget, Row, column, item, CellStates.Editing, null);

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
			public override void GetSize(Gtk.Widget widget, ref Gdk.Rectangle cell_area, out int x_offset, out int y_offset, out int width, out int height)
			{
				base.GetSize(widget, ref cell_area, out x_offset, out y_offset, out width, out height);
				var h = Handler;
				if (h == null)
					return;
				height = Math.Max(height, h.Source.RowHeight);
			}

			public override Gtk.CellEditable StartEditing(Gdk.Event evnt, Gtk.Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				return CreateEditable(cell_area);
			}

			protected override void Render(Gdk.Drawable window, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gdk.Rectangle expose_area, Gtk.CellRendererState flags)
			{
				var h = Handler;
				if (h == null)
					return;

				if (editingRow == row)
				{
					return;
				}
				using (var graphics = new Graphics(new GraphicsHandler(widget, window)))
				{
					var item = h.Source.GetItem(Row);
					var args = new CellPaintEventArgs(graphics, cell_area.ToEto(), flags.ToEto(), item);
					h.Callback.OnPaint(h.Widget, args);
				}
			}
#else
			protected override void OnGetPreferredHeight(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredHeight(widget, out minimum_size, out natural_size);
				var h = Handler;
				if (h == null)
					return;
				natural_size = h.Source.RowHeight;
			}

			protected override void OnGetPreferredWidth(Gtk.Widget widget, out int minimum_size, out int natural_size)
			{
				base.OnGetPreferredWidth(widget, out minimum_size, out natural_size);
				var h = Handler;
				if (h == null)
					return;
				var item = h.Source?.GetItem(Row);
				int column = -1;
				var args = new CellEventArgs(null, h.Widget, Row, column, item, CellStates.Editing, null);

				natural_size = (int)h.Callback.OnGetPreferredWidth(h.Widget, args);
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void RenderNativeDelegate(IntPtr inst, IntPtr cr, IntPtr widget, IntPtr background_area, IntPtr cell_area, int flags);

			protected override unsafe void OnRender(Cairo.Context cr, Gtk.Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
#if GTKCORE
				RenderNativeDelegate renderNativeDelegate = null;
				IntPtr* ptr = (IntPtr*)((long)LookupGType().GetThresholdType().GetClassPtr() + class_abi.GetFieldOffset("render"));
				if (*ptr != IntPtr.Zero)
				{
					renderNativeDelegate = (RenderNativeDelegate)Marshal.GetDelegateForFunctionPointer(*ptr, typeof(RenderNativeDelegate));
					if (renderNativeDelegate != null)
					{
						IntPtr intPtr = GLib.Marshaller.StructureToPtrAlloc(background_area);
						IntPtr intPtr2 = GLib.Marshaller.StructureToPtrAlloc(cell_area);
						renderNativeDelegate(base.Handle, cr?.Handle ?? IntPtr.Zero, widget?.Handle ?? IntPtr.Zero, intPtr, intPtr2, (int)flags);
						Marshal.FreeHGlobal(intPtr);
						Marshal.FreeHGlobal(intPtr2);
					}
				}
#endif

				if (editingRow == row)
				{
					return;
				}
				using (var graphics = new Graphics(new GraphicsHandler(widget, cr)))
				{
					var item = Handler.Source.GetItem(Row);
					var args = new CellPaintEventArgs(graphics, cell_area.ToEto(), flags.ToEto(), item);
					Handler.Callback.OnPaint(Handler.Widget, args);
				}
			}

			protected override IGtkCellEditable OnStartEditing(Gdk.Event evnt, Gtk.Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, Gtk.CellRendererState flags)
			{
				return CreateEditable(cell_area);
			}

			/*
			protected override void OnEditingStarted(IGtkCellEditable editable, string path)
			{
				//base.OnEditingStarted(editable, path);
			}*/
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

