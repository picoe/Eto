using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class NativeControlHandler : GtkControl<Gtk.Widget, Control, Control.ICallback>
	{
		public NativeControlHandler(Gtk.Widget nativeControl)
		{
			Control = nativeControl;
		}
	}
}

