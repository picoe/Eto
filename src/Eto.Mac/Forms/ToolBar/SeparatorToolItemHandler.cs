using Eto.Forms;
using System;
using Eto.Drawing;
using Eto.Mac.Drawing;
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
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.ToolBar
{
	public class SeparatorToolItemHandler : ToolItemHandler<NSToolbarItem, SeparatorToolItem>, SeparatorToolItem.IHandler, IToolBarBaseItemHandler
	{
		SeparatorToolItemType type = SeparatorToolItemType.Divider;

		ToolBarHandler ParentHandler => Widget.Parent?.Handler as ToolBarHandler;

		protected override bool IsButton => false;

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
