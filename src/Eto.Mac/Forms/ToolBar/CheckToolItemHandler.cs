using System;
using Eto.Forms;

namespace Eto.Mac.Forms.ToolBar
{
	public class CheckToolItemHandler : ToolItemHandler<NSToolbarItem, CheckToolItem>, CheckToolItem.IHandler
	{
		public bool Checked
		{
			get { return Button.State == NSCellStateValue.On; }
			set { 
				Button.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Button.SetButtonType(NSButtonType.PushOnPushOff);
		}

		public override void InvokeButton()
		{
			Widget.OnClick(EventArgs.Empty);
			Widget.OnCheckedChanged(EventArgs.Empty);
		}
	}
}
