using Eto.Forms;
using System;
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
		public static string DividerIdentifier = "divider";

		public SeparatorToolItemHandler()
		{
			Type = SeparatorToolItemType.Divider;
		}

		public override string Identifier
		{
			get
			{ 
				switch (Type)
				{
					case SeparatorToolItemType.Divider:
						return DividerIdentifier;
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

		SeparatorToolItemType type;
		public SeparatorToolItemType Type
		{
			get { return type; }
			set
			{
				type = value;
				if (type == SeparatorToolItemType.Divider)
				{
					Control = new NSToolbarItem(SeparatorToolItemHandler.DividerIdentifier)
					{
						View = new NSView(),
						PaletteLabel = "Small Space"
					};
				}
				else
					Control = null;
			}
		}
	}
}
