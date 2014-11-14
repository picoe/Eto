using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class PanelHandler : WindowsPanel<PanelHandler.EtoPanel, Panel, Panel.ICallback>, Panel.IHandler
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
			Control = new EtoPanel
			{
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
		}
	}
}
