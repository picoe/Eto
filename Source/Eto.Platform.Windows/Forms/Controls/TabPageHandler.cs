using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class TabPageHandler : WindowsContainer<SWF.TabPage, TabPage>, ITabPage
	{

		public TabPageHandler()
		{
			Control = new SWF.TabPage();
			//control.Click += control_Click;
		}


		/*
		private void control_Click(object sender, EventArgs e)
		{
			//base.OnClick(e);
		}
		 */

	}
}
