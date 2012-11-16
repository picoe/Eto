using System;
using Gtk;
using Gdk;

namespace Eto.Platform.GtkSharp.CustomControls
{
	public class SizableBin : Gtk.Bin
	{
		protected override void OnSizeAllocated (Rectangle allocation)
		{
			if (this.Child != null) {
				this.Child.SetAllocation (allocation);
			}
			base.OnSizeAllocated (allocation);
		}

#if GTK2
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			if (this.Child != null) {
				requisition = this.Child.SizeRequest ();
			}
		}
#endif
	}
}