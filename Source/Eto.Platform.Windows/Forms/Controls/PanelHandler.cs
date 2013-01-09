using System;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class PanelHandler : WindowsContainer<MyPanel, Panel>, IPanel
	{
		public PanelHandler()
		{
			Control = new MyPanel();
			this.Control.SuspendLayout ();
			this.Control.Size = SD.Size.Empty;
			this.Control.MinimumSize = SD.Size.Empty;
			this.Control.AutoSize = true;
			this.Control.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.ResumeLayout ();
		}
	}

	public class MyPanel : SWF.Panel
	{
		// Need to override IsInputKey to capture 
		// the arrow keys.
		protected override bool IsInputKey(SWF.Keys keyData)
		{
			switch (keyData & SWF.Keys.KeyCode)
			{
				case SWF.Keys.Up:
				case SWF.Keys.Down:
				case SWF.Keys.Left:
				case SWF.Keys.Right:
				case SWF.Keys.Back:
					return true;
				default:
					return base.IsInputKey(keyData);
			}
		}
	}
}
