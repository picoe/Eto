using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms
{
	public class CustomLayoutHandler : WindowsContainer<swf.Panel, CustomLayout, CustomLayout.ICallback>, CustomLayout.IHandler
	{
		public CustomLayoutHandler()
		{
			Control = new swf.Panel
			{
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				AutoSize = false,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			// do not call base class - pixel layout never scales the content
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			return Size.Max(base.GetPreferredSize(availableSize, useCache), Control.PreferredSize.ToEto());
		}

		public void Add(Control child)
		{
			var childHandler = child.GetWindowsHandler();
			var childControl = childHandler.ContainerControl;
			childControl.Dock = swf.DockStyle.None;
			childHandler.BeforeAddControl(Widget.Loaded);
			Control.Controls.Add(childControl);
			childControl.BringToFront();
		}

		public void Move(Control child, Rectangle location)
		{
			var ctl = child.GetContainerControl();
			ctl.AutoSize = false;
			ctl.SetBounds(location.X, location.Y, location.Width, location.Height, swf.BoundsSpecified.All);
		}

		public void Remove(Control child)
		{
			var ctl = child.GetContainerControl();
			if (ctl.Parent == Control)
				ctl.Parent = null;
		}

		public void RemoveAll()
		{
			Control.Controls.Clear();
		}

		public void Update()
		{
			Control.PerformLayout();
		}
	}
}