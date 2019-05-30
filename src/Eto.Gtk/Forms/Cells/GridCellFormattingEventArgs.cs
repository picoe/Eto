using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Cells
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
				return base.Font ?? (base.Font = Renderer.FontDesc.ToEto());
			}
			set {
				base.Font = value;
				Renderer.FontDesc = value.ToPango();
			}
		}

	}
}

