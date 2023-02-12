using Eto.Forms;
using System;
using Eto.Drawing;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.ToolBar
{
	public class SeparatorToolItemHandler : ToolItemHandler<NSToolbarItem, SeparatorToolItem>, SeparatorToolItem.IHandler, IToolBarBaseItemHandler
	{
		SeparatorToolItemType type = SeparatorToolItemType.Divider;

		ToolBarHandler ParentHandler => Widget.Parent?.Handler as ToolBarHandler;

		protected override bool IsButton => false;

		protected override bool UseButtonStyle => false;

		public override string Identifier
		{
			get
			{ 
				switch (Type)
				{
					case SeparatorToolItemType.Divider:
						return ToolBarHandler.DividerIdentifier;
					case SeparatorToolItemType.Space:
						return NSToolbar.NSToolbarSpaceItemIdentifier;
					case SeparatorToolItemType.FlexibleSpace:
						return NSToolbar.NSToolbarFlexibleSpaceItemIdentifier;
					default:
						throw new NotSupportedException();
				}
				
			}
			set { }
		}

		public SeparatorToolItemType Type
		{
			get => type;
			set
			{
				if (type != value)
				{
					type = value;
					ParentHandler?.ChangeIdentifier(Widget);
				}
			}
		}

		protected override NSToolbarItem CreateControl() => null;
	}
}
