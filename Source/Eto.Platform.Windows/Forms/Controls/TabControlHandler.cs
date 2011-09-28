using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class TabControlHandler : WindowsControl<SWF.TabControl, TabControl>, ITabControl
	{
		public TabControlHandler()
		{
			this.Control = new SWF.TabControl();
			this.Control.ImageList = new SWF.ImageList{ ColorDepth = SWF.ColorDepth.Depth32Bit };
			this.Control.SelectedIndexChanged += control_SelectedIndexChanged;
		}

		private void control_SelectedIndexChanged(object sender, EventArgs e)
		{
			Widget.OnSelectedIndexChanged(e);
		}
		#region ITabControl Members

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		public void AddTab(TabPage page)
		{
			Control.TabPages.Add((SWF.TabPage)page.ControlObject);
		}

		public void RemoveTab(TabPage page)
		{
			Control.TabPages.Remove((SWF.TabPage)page.ControlObject);
		}

		#endregion
	}
}
