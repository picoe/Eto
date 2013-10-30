using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

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

		public override Font Font {
			get;
			set;
		}

		public override Color BackgroundColor {
			get { return Renderer.CellBackgroundGdk.ToEto (); }
			set { Renderer.CellBackgroundGdk = value.ToGdk (); }
		}

		public override Color ForegroundColor {
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

		public override Color ForegroundColor {
			get { return Renderer.ForegroundGdk.ToEto (); }
			set { Renderer.ForegroundGdk = value.ToGdk (); }
		}

		public override Font Font {
			get {
				if (font == null)
					return new Font (Column.Generator, new FontHandler (Renderer.FontDesc));
				return font;
			}
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

