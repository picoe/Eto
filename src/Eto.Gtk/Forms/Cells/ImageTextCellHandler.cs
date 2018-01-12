using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Cells
{
	public class ImageTextCellHandler : CellHandler<Gtk.CellRendererText, ImageTextCell, ImageTextCell.ICallback>, ImageTextCell.IHandler, ITextBoxCellHandler
	{
		readonly Gtk.CellRendererPixbuf imageCell;
		int imageDataIndex;
		int textDataIndex;

		class ImageRenderer : Gtk.CellRendererPixbuf
		{
			public ImageTextCellHandler Handler { get; set; }

			[GLib.Property("item")]
			public object Item { get; set; }

			int row;
			[GLib.Property("row")]
			public int Row
			{
				get { return row; }
				set {
					row = value;
					if (Handler.FormattingEnabled)
						Handler.Format(new GtkGridCellFormatEventArgs<ImageRenderer>(this, Handler.Column.Widget, Handler.Source.GetItem(Row), Row));
				}
			}
		}

		public ImageTextCellHandler()
		{
			imageCell = new ImageRenderer { Handler = this };
			Control = new TextBoxCellHandler.Renderer { Handler = this };
			VerticalAlignment = VerticalAlignment.Center;
		}

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

		protected override void Initialize()
		{
			base.Initialize();
			this.Control.Edited += Connector.HandleEdited;
		}

		protected new ImageTextCellEventConnector Connector { get { return (ImageTextCellEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ImageTextCellEventConnector();
		}

		protected class ImageTextCellEventConnector : CellConnector
		{
			public new ImageTextCellHandler Handler { get { return (ImageTextCellHandler)base.Handler; } }

			public void HandleEdited(object o, Gtk.EditedArgs args)
			{
				Handler.SetValue(args.Path, args.NewText);
			}

			public void HandleEndCellEditing(object o, Gtk.EditedArgs args)
			{
				Handler.Source.EndCellEditing(new Gtk.TreePath(args.Path), Handler.ColumnIndex);
			}
		}

		public override void AddCells(Gtk.TreeViewColumn column)
		{
			column.PackStart(imageCell, false);
			column.PackStart(Control, true);
		}

		protected override void BindCell(ref int dataIndex)
		{
			Column.Control.ClearAttributes(Control);
			Column.Control.ClearAttributes(imageCell);
			imageDataIndex = SetColumnMap(dataIndex);
			Column.Control.AddAttribute(imageCell, "pixbuf", dataIndex++);
			textDataIndex = SetColumnMap(dataIndex);
			Column.Control.AddAttribute(Control, "text", dataIndex++);
			base.BindCell(ref dataIndex);

			if (FormattingEnabled)
			{
				Column.Control.AddAttribute(imageCell, "row", Source.RowDataColumn);
			}
		}

		public override void SetEditable(Gtk.TreeViewColumn column, bool editable)
		{
			Control.Editable = editable;
		}

		public override void SetValue(object dataItem, object value)
		{
			if (Widget.TextBinding != null)
			{
				Widget.TextBinding.SetValue(dataItem, Convert.ToString(value));
			}
		}

		protected override GLib.Value GetValueInternal(object dataItem, int dataColumn, int row)
		{
			if (dataColumn == imageDataIndex)
			{
				if (Widget.ImageBinding != null)
				{
					var ret = Widget.ImageBinding.GetValue(dataItem);
					var image = ret as Image;
					if (image != null)
						return new GLib.Value(((IGtkPixbuf)image.Handler).GetPixbuf(new Size(16, 16), ImageInterpolation.ToGdk()));
				}
				return new GLib.Value((Gdk.Pixbuf)null);
			}
			if (dataColumn == textDataIndex)
			{
				var ret = Widget.TextBinding.GetValue(dataItem);
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
					Control.Edited += Connector.HandleEndCellEditing;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public ImageInterpolation ImageInterpolation { get; set; }
		public AutoSelectMode AutoSelectMode { get; set; }
	}
}

