using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Forms.Cells
{
	public class GtkGridCellFormatEventArgs<T> : GridCellFormatEventArgs
			where T: Gtk.CellRenderer
	{
		public T Renderer { get; private set; }

		public GtkGridCellFormatEventArgs (T renderer, GridColumn column, object item, int row)
				: base(column, item, row)
		{
			this.Renderer = renderer;
		}

		public override Eto.Drawing.Font Font {
			get;
			set;
		}

		public override Eto.Drawing.Color BackgroundColor {
			get { return Generator.Convert (Renderer.CellBackgroundGdk); }
			set { Renderer.CellBackgroundGdk = Generator.Convert (value); }
		}

		public override Eto.Drawing.Color ForegroundColor {
			get;
			set;
		}
	}

	public class GtkTextCellFormatEventArgs<T> : GtkGridCellFormatEventArgs<T>
		where T: Gtk.CellRendererText
	{
		Font font;

		public GtkTextCellFormatEventArgs (T renderer, GridColumn column, object item, int row)
					: base(renderer, column, item, row)
		{
		}

		public override Eto.Drawing.Color ForegroundColor {
			get { return Generator.Convert (Renderer.ForegroundGdk); }
			set { Renderer.ForegroundGdk = Generator.Convert (value); }
		}

		public override Eto.Drawing.Font Font {
			get { return font; }
			set {
				font = value;
				if (Font != null)
					Renderer.FontDesc = font.ControlObject as Pango.FontDescription;
				else
					Renderer.FontDesc = null;
			}
		}

	}
}

