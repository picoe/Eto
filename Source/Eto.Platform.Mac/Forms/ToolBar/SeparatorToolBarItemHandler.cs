using System;
using System.Collections;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class SeparatorToolBarItemHandler : WidgetHandler<NSToolbarItem, SeparatorToolBarItem>, ISeparatorToolBarItem, IToolBarBaseItemHandler
	{
		public SeparatorToolBarItemHandler()
		{
			Type = SeparatorToolBarItemType.Divider;
		}
		
		public virtual string Identifier
		{
			get { 
				switch (Type) {
				default:
				case SeparatorToolBarItemType.Divider: return NSToolbar.NSToolbarSeparatorItemIdentifier;
				case SeparatorToolBarItemType.Space: return NSToolbar.NSToolbarSpaceItemIdentifier;
				case SeparatorToolBarItemType.FlexibleSpace: return NSToolbar.NSToolbarFlexibleSpaceItemIdentifier;
				}
				
			}
		}

		public bool Selectable {
			get { return false; }
		}
		
		public SeparatorToolBarItemType Type { get; set; }

		public void ControlAdded (ToolBarHandler toolbar)
		{
		}

	}
}
