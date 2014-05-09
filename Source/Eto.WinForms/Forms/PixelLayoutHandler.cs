using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms
{
	public class PixelLayoutHandler : WindowsContainer<swf.Panel, PixelLayout, PixelLayout.ICallback>, IPixelLayout
	{
		public PixelLayoutHandler()
		{
			Control = new swf.Panel
			{
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			// do not call base class - pixel layout never scales the content
		}

		public override Size GetPreferredSize(Size availableSize)
		{
			return Size.Max(base.GetPreferredSize(availableSize), Control.PreferredSize.ToEto());
		}

		public void Add(Control child, int x, int y)
		{
			var ctl = child.GetContainerControl();
			var pt = new sd.Point(x, y);
			ctl.Dock = swf.DockStyle.None;
			ctl.Location = pt;
			Control.Controls.Add(ctl);
			ctl.BringToFront();
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