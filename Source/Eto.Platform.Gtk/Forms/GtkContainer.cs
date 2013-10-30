using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public abstract class GtkContainer<T, W> : GtkControl<T, W>, IContainer
		where T: Gtk.Widget
		where W: Container
	{

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		public override void SetBackgroundColor()
		{
			base.SetBackgroundColor();
			foreach (var child in Widget.Controls.Select(r => r.GetGtkControlHandler()).Where(r => r != null))
			{
				child.SetBackgroundColor();
			}
		}
	}
}
