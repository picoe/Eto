using System;
using Eto.Forms;

using aw = Android.Widget;

namespace Eto.Android.Forms.ToolBar
{
	public class ButtonToolItemHandler : ToolItemHandler<aw.Button, ButtonToolItem>, ButtonToolItem.IHandler
	{
		protected override aw.Button GetInnerControl(ToolBarHandler handler)
		{
			if (!HasControl)
			{
				Control = new aw.Button(Platform.AppContextThemed);
				Control.Click += control_Click;
			}

			return Control;
		}

		void control_Click(object sender, EventArgs e)
		{
			Widget.OnClick(EventArgs.Empty);
		}
	}
}
