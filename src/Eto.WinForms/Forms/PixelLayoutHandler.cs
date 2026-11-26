namespace Eto.WinForms.Forms
{
	public class PixelLayoutHandler : WindowsContainer<PixelLayoutHandler.Panel, PixelLayout, PixelLayout.ICallback>, PixelLayout.IHandler
	{
		public class Panel: EtoPanel<PixelLayoutHandler>
		{
			public Panel(PixelLayoutHandler handler)
				: base(handler)
			{ }
		}
		public PixelLayoutHandler()
		{
			Control = new Panel(this);
			Control.Layout += (sender, e) =>
			{
				// positions are from the right edge, so they move when the width or a child's size changes
				if (IsRightToLeft)
					PositionChildren();
			};
		}

		bool positioning;

		void PositionChildren()
		{
			if (positioning)
				return;
			positioning = true;
			try
			{
				foreach (var child in Widget.Controls)
					PositionChild(child);
			}
			finally
			{
				positioning = false;
			}
		}

		void PositionChild(Control child)
		{
			var ctl = child.GetContainerControl();
			if (ctl == null)
				return;
			var location = LogicalToDeviceUnits(PixelLayout.GetLocation(child));
			if (IsRightToLeft)
				location.X = Control.ClientSize.Width - location.X - ctl.Width;
			if (ctl.Location != location)
				ctl.Location = location;
		}

		protected override void OnLayoutDirectionChanged()
		{
			base.OnLayoutDirectionChanged();
			PositionChildren();
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			// do not call base class - pixel layout never scales the content
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			var preferred = base.GetPreferredSize(availableSize, useCache);
			if (!IsRightToLeft)
				return Size.Max(preferred, DeviceUnitsToLogical(Control.PreferredSize));

			// children are placed from the right edge, so measure using their logical positions
			var size = Size.Empty;
			foreach (var child in Widget.Controls)
			{
				var ctl = child.GetContainerControl();
				if (ctl == null || !ctl.Visible)
					continue;
				var location = PixelLayout.GetLocation(child);
				size = Size.Max(size, new Size(location.X, location.Y) + DeviceUnitsToLogical(ctl.Size));
			}
			return Size.Max(preferred, size);
		}

		public void Add(Control child, int x, int y)
		{
			var childHandler = child.GetWindowsHandler();
			var childControl = childHandler.ContainerControl;
			childControl.Dock = swf.DockStyle.None;
			childHandler.BeforeAddControl(Widget.Loaded);
			Control.Controls.Add(childControl);
			PositionChild(child);
			childControl.BringToFront();
		}

		public void Move(Control child, int x, int y)
		{
			PositionChild(child);
		}

		public void Remove(Control child)
		{
			var ctl = child.GetContainerControl();
			if (ctl.Parent == Control)
				ctl.Parent = null;
		}

		public void Update()
		{
			Control.PerformLayout();
		}
	}
}