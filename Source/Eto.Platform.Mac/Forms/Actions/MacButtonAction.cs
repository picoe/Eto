using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Actions
{
	public class MacButtonAction : ButtonAction
	{
		public Selector Selector { get; set; }
		
		public MacButtonAction(string id, string text, string selector)
			: base(id, text)
		{
			this.Selector = new Selector(selector);
		}
		public override MenuItem GenerateMenuItem(Eto.Generator generator)
		{
			var item = base.GenerateMenuItem(generator) as ImageMenuItem;
			var menuItem = (NSMenuItem)item.ControlObject;
			menuItem.Target = null;
			menuItem.Action = Selector;
			
			return item;
		}

		public override ToolBarItem GenerateToolBarItem(ActionItem actionItem, Eto.Generator generator, ToolBarTextAlign textAlign)
		{
			var item = base.GenerateToolBarItem (actionItem, generator, textAlign);
			var tb = (NSToolbarItem)item.ControlObject;
			tb.Target = null;
			tb.Action = Selector;
			return item;
		}
	}
}

