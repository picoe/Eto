using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Cells
{
	public class GtkGridCellFormatEventArgs : GridCellFormatEventArgs
	{
		ICellDataSource _source;
		Font font;
		public Gtk.CellRenderer Renderer { get; private set; }
		public Gtk.CellRendererText TextRenderer => Renderer as Gtk.CellRendererText;

		public GtkGridCellFormatEventArgs(ICellDataSource source, Gtk.CellRenderer renderer, GridColumn column, object item, int row)
				: base(column, item, row)
		{
			Renderer = renderer;
			_source = source;
		}

		public override Font Font
		{
			get => font ??= TextRenderer?.FontDesc.ToEto();
			set
			{
				font = value;
				if (TextRenderer != null)
					TextRenderer.FontDesc = font.ToPango();
			}
		}

		public override Color BackgroundColor
		{
			get => Renderer.CellBackgroundRgba.ToEto();
			set => Renderer.CellBackgroundRgba = value.ToRGBA();
		}

		public override Color ForegroundColor
		{
			get => TextRenderer?.ForegroundRgba.ToEto() ?? Colors.Transparent;
			set
			{
				if (TextRenderer != null)
					TextRenderer.ForegroundRgba = value.ToRGBA();
			}
		}

		protected override int GetRow()
		{
			var row = base.GetRow();
			if (row == -1)
			{
				row = _source.GetRowOfItem(Item);
			}
			return row;
		}
	}
	public class GtkGridRowFormatEventArgs : GridRowFormatEventArgs
	{
		ICellDataSource _source;
		public Gtk.CellRenderer Renderer { get; private set; }
		public Gtk.CellRendererText TextRenderer => Renderer as Gtk.CellRendererText;

		public GtkGridRowFormatEventArgs(ICellDataSource source, Gtk.CellRenderer renderer, object item, int row)
				: base(item, row)
		{
			Renderer = renderer;
			_source = source;
		}

		public override Color BackgroundColor
		{
			get => Renderer.CellBackgroundRgba.ToEto();
			set => Renderer.CellBackgroundRgba = value.ToRGBA();
		}

		protected override int GetRow()
		{
			var row = base.GetRow();
			if (row == -1)
			{
				row = _source.GetRowOfItem(Item);
			}
			return row;
		}
	}
}

