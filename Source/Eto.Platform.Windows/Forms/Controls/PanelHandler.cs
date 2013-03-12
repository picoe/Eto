using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class PanelHandler : WindowsContainer<PanelHandler.EtoPanel, Panel>, IPanel
	{
		public class EtoPanel : swf.Panel
		{
			// Need to override IsInputKey to capture 
			// the arrow keys.
			protected override bool IsInputKey (swf.Keys keyData)
			{
				switch (keyData & swf.Keys.KeyCode) {
				case swf.Keys.Up:
				case swf.Keys.Down:
				case swf.Keys.Left:
				case swf.Keys.Right:
				case swf.Keys.Back:
					return true;
				default:
					return base.IsInputKey (keyData);
				}
			}
		}

		public PanelHandler ()
		{
			Control = new EtoPanel ();
			this.Control.SuspendLayout ();
			this.Control.Size = sd.Size.Empty;
			this.Control.MinimumSize = sd.Size.Empty;
			this.Control.AutoSize = true;
			this.Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.ResumeLayout ();
		}
	}
}
