using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

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
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			// do not call base class - pixel layout never scales the content
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			return Size.Max(base.GetPreferredSize(availableSize, useCache), Control.PreferredSize.ToEto());
		}

		public void Add(Control child, int x, int y)
		{
			var childHandler = child.GetWindowsHandler();
			var childControl = childHandler.ContainerControl;
			childControl.Dock = swf.DockStyle.None;
			childControl.Location = new sd.Point(x, y);
			childHandler.BeforeAddControl(Widget.Loaded);
			Control.Controls.Add(childControl);
			childControl.BringToFront();
		}

		public void Move(Control child, int x, int y)
		{
			var ctl = child.GetContainerControl();
			ctl.Location = new sd.Point(x, y);
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