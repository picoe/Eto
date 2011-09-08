using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Actions
{
	public class MacButtonAction : ButtonAction
	{
		public string Selector { get; set; }
		
		public MacButtonAction(string id, string text, string selector)
			: base(id, text)
		{
			this.Selector = selector;
		}

		public override MenuItem Generate (ActionItem actionItem, ISubMenuWidget menu)
		{
			var item = base.Generate (actionItem, menu) as ImageMenuItem;
			var menuItem = (NSMenuItem)item.ControlObject;
			menuItem.Target = null;
			menuItem.Action = new MonoMac.ObjCRuntime.Selector(Selector);
			
			return item;
		}
		
		public override ToolBarItem Generate (ActionItem actionItem, ToolBar toolBar)
		{
			var item = base.Generate (actionItem, toolBar);
			var tb = (NSToolbarItem)item.ControlObject;
			tb.Target = null;
			tb.Action = new MonoMac.ObjCRuntime.Selector(Selector);
			return item;
		}
	}
}

