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
			this.Control.SuspendLayout ();
			this.Control.Size = SD.Size.Empty;
			this.Control.MinimumSize = SD.Size.Empty;
			this.Control.AutoSize = true;
			this.Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			//this.Control.Margin = new SWF.Padding(0);
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.ResumeLayout ();
		}
	}
}
