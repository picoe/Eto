using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms
{
	public abstract class GtkContainer<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, Container.IHandler
		where TControl: Gtk.Widget
		where TWidget: Container
		where TCallback: Container.ICallback
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
