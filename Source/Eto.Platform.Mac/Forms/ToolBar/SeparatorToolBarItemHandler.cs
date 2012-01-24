using System;
using System.Collections;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class SeparatorToolBarItemHandler : WidgetHandler<NSToolbarItem, SeparatorToolBarItem>, ISeparatorToolBarItem
	{
		SeparatorToolBarItemType type;
		
		public SeparatorToolBarItemHandler()
		{
			
		}
		
		public override string ID
		{
			get { 
				switch (Type) {
				default:
				case SeparatorToolBarItemType.Divider: return NSToolbar.NSToolbarSeparatorItemIdentifier;
				case SeparatorToolBarItemType.Space: return NSToolbar.NSToolbarSpaceItemIdentifier;
				case SeparatorToolBarItemType.FlexibleSpace: return NSToolbar.NSToolbarFlexibleSpaceItemIdentifier;
				}
				
			}
			set { }
		}
		
		public SeparatorToolBarItemType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
		
	}
}
