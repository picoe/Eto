using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp
{
	public abstract class GtkContainer<TControl, TWidget> : GtkControl<TControl, TWidget>, IContainer
		where TControl: Gtk.Widget
		where TWidget: Container
	{
		public bool RecurseToChildren { get { return true; } }

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
