using Eto.Forms;
using MonoMac.AppKit;
using System;

namespace Eto.Mac
{
	public class SeparatorToolItemHandler : WidgetHandler<NSToolbarItem, SeparatorToolItem>, SeparatorToolItem.IHandler, IToolBarBaseItemHandler
	{
		public SeparatorToolItemHandler()
		{
			Type = SeparatorToolItemType.Divider;
		}

		public virtual string Identifier
		{
			get
			{ 
				switch (Type)
				{
					default:
						return NSToolbar.NSToolbarSeparatorItemIdentifier;
					case SeparatorToolItemType.Space:
						return NSToolbar.NSToolbarSpaceItemIdentifier;
					case SeparatorToolItemType.FlexibleSpace:
						return NSToolbar.NSToolbarFlexibleSpaceItemIdentifier;
				}
				
			}
		}

		public bool Selectable
		{
			get { return false; }
		}

		public SeparatorToolItemType Type { get; set; }

		public void ControlAdded(ToolBarHandler toolbar)
		{
		}

		public void CreateFromCommand(Command command)
		{
		}

		public string Text
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public string ToolTip
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public Eto.Drawing.Image Image
		{
			get { return null; }
			set { throw new NotSupportedException(); }
		}

		public bool Enabled
		{
			get { return false; }
			set { throw new NotSupportedException(); }
		}
	}
}
