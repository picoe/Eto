using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Controls
{
	public class PanelHandler : WindowsPanel<PanelHandler.EtoPanel, Panel, Panel.ICallback>, Panel.IHandler
	{
		public class EtoPanel : swf.Panel
		{
			public PanelHandler Handler { get; set; }

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var size = base.GetPreferredSize(proposedSize);
				var userSize = Handler.UserDesiredSize;
				if (userSize.Width >= 0)
					size.Width = Math.Max(userSize.Width, MinimumSize.Width);
				if (userSize.Height >= 0)
					size.Height = Math.Max(userSize.Height, MinimumSize.Height);
				return size;
			}

			// Need to override IsInputKey to capture 
			// the arrow keys.
			protected override bool IsInputKey(swf.Keys keyData)
			{
				switch (keyData & swf.Keys.KeyCode)
				{
					case swf.Keys.Up:
					case swf.Keys.Down:
					case swf.Keys.Left:
					case swf.Keys.Right:
					case swf.Keys.Back:
						return true;
					default:
						return base.IsInputKey(keyData);
				}
			}
		}

		public PanelHandler()
		{
			Control = new EtoPanel
			{
				Handler = this,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink
			};
		}
	}
}
