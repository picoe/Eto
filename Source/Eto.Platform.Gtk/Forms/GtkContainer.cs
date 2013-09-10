using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public abstract class GtkContainer<T, W> : GtkControl<T, W>, IContainer
		where T: Gtk.Widget
		where W: Container
	{

		public virtual Size ClientSize { get; set; }
	}
}
