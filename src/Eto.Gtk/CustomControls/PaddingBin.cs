#if GTK3
namespace Eto.GtkSharp.CustomControls
{
	/// <summary>
	/// Container that fills its child into its allocation, inset by <see cref="Padding"/>.
	/// </summary>
	public class PaddingBin : Gtk.Bin
	{
		Padding padding;

		public PaddingBin()
		{
			HasWindow = false;
		}

		public PaddingBin(IntPtr raw) : base(raw)
		{
		}

		public Padding Padding
		{
			get => padding;
			set
			{
				if (padding == value)
					return;
				padding = value;
				QueueResize();
			}
		}

		bool HasVisibleChild => Child != null && Child.Visible;

		protected override Gtk.SizeRequestMode OnGetRequestMode() => HasVisibleChild ? Child.RequestMode : Gtk.SizeRequestMode.ConstantSize;

		protected override void OnGetPreferredWidth(out int minimum_size, out int natural_size)
		{
			minimum_size = natural_size = 0;
			if (HasVisibleChild)
				Child.GetPreferredWidth(out minimum_size, out natural_size);
			minimum_size += padding.Horizontal;
			natural_size += padding.Horizontal;
		}

		protected override void OnGetPreferredHeight(out int minimum_size, out int natural_size)
		{
			minimum_size = natural_size = 0;
			if (HasVisibleChild)
				Child.GetPreferredHeight(out minimum_size, out natural_size);
			minimum_size += padding.Vertical;
			natural_size += padding.Vertical;
		}

		protected override void OnGetPreferredWidthForHeight(int height, out int minimum_width, out int natural_width)
		{
			minimum_width = natural_width = 0;
			if (HasVisibleChild)
				Child.GetPreferredWidthForHeight(Math.Max(0, height - padding.Vertical), out minimum_width, out natural_width);
			minimum_width += padding.Horizontal;
			natural_width += padding.Horizontal;
		}

		protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
		{
			minimum_height = natural_height = 0;
			if (HasVisibleChild)
				Child.GetPreferredHeightForWidth(Math.Max(0, width - padding.Horizontal), out minimum_height, out natural_height);
			minimum_height += padding.Vertical;
			natural_height += padding.Vertical;
		}

		protected override void OnSizeAllocated(Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated(allocation);
			if (HasVisibleChild)
			{
				Child.SizeAllocate(new Gdk.Rectangle(
					allocation.X + padding.Left,
					allocation.Y + padding.Top,
					Math.Max(1, allocation.Width - padding.Horizontal),
					Math.Max(1, allocation.Height - padding.Vertical)));
			}
		}
	}
}
#endif
