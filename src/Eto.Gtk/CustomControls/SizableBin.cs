using Gtk;
using Gdk;

namespace Eto.GtkSharp.CustomControls
{
	public class SizableBin : Bin
	{
		protected override void OnSizeAllocated (Rectangle allocation)
		{
			if (Child != null) {
				Child.SizeAllocate (allocation);
			}
			base.OnSizeAllocated (allocation);
		}

#if GTK2
		protected override void OnSizeRequested (ref Requisition requisition)
		{
			if (Child != null) {
				requisition = Child.SizeRequest ();
			}
		}
#else
		protected override void OnAdjustSizeRequest(Orientation orientation, out int minimum_size, out int natural_size)
		{
			if (this.Child != null)
			{
				Requisition minimum, natural;
				this.Child.GetPreferredSize(out minimum, out natural);
				if (orientation == Orientation.Horizontal)
				{
					minimum_size = minimum.Width;
					natural_size = natural.Width;
				}
				else if (orientation == Orientation.Vertical)
				{
					minimum_size = minimum.Height;
					natural_size = natural.Height;
				}
				else
					base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
			}
			else
			{
				base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
			}
		}

#endif
	}
}