using System;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class PanelHandler : WindowsContainer<SWF.Panel, Panel>, IPanel
	{
		public PanelHandler()
		{
			Control = new SWF.Panel();
			Control.Size = SD.Size.Empty;
			this.Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			Control.AutoSize = true;
			this.Control.MinimumSize = SD.Size.Empty;
			//this.Control.Margin = new SWF.Padding(0);
		}
	}
}
