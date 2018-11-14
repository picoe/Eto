using System;
using Eto.Forms;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

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
