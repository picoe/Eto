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
				this.Child.Allocation = allocation;
			}
			base.OnSizeAllocated (allocation);
		}

		protected override void OnSizeRequested (ref Requisition requisition)
		{
			if (this.Child != null) {
				requisition = this.Child.SizeRequest ();
			}
		}
	}
}