using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using System;

namespace Eto.WinForms.Forms.Controls
{
	public class PanelHandler : WindowsPanel<PanelHandler.EtoPanel, Panel, Panel.ICallback>, Panel.IHandler
	{
		public class EtoPanel: EtoPanel<PanelHandler>
		{
			public EtoPanel(PanelHandler handler) : base(handler) { }
		}
		public PanelHandler()
		{
			Control = new EtoPanel(this);
		}
	}
}
