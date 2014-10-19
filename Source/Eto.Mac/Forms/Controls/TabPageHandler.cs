using System;
using Eto.Forms;
using SD = System.Drawing;
using Eto.Drawing;

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
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#elif !XAMMAC2
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TabPageHandler : MacPanel<NSView, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		const int ICON_PADDING = 2;
		Image image;
		static readonly IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");

		public override NSView ContainerControl { get { return Control; } }

		public NSTabViewItem TabViewItem { get; private set; }

		class MyTabViewItem : NSTabViewItem
		{
			WeakReference handler;

			public TabPageHandler Handler { get { return (TabPageHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void DrawLabel(bool shouldTruncateLabel, CGRect labelRect)
			{
				if (Handler.image != null)
				{
					var nsimage = (NSImage)Handler.image.ControlObject;

					if (nsimage.RespondsToSelector(new Selector(selDrawInRectFromRectOperationFractionRespectFlippedHints)))
						nsimage.Draw(new CGRect(labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new CGRect(CGPoint.Empty, nsimage.Size), NSCompositingOperation.SourceOver, 1, true, null);
					else
					{
						#pragma warning disable 618
						nsimage.Flipped = View.IsFlipped;
						#pragma warning restore 618
						nsimage.Draw(new CGRect(labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new CGRect(CGPoint.Empty, nsimage.Size), NSCompositingOperation.SourceOver, 1);
					}
					
					labelRect.X += labelRect.Height + ICON_PADDING;
					labelRect.Width -= labelRect.Height + ICON_PADDING;
					base.DrawLabel(shouldTruncateLabel, labelRect);
				}
				base.DrawLabel(shouldTruncateLabel, labelRect);
			}

			// TODO: Mac64
			#if !Mac64 && !XAMMAC2
			public override CGSize SizeOfLabel (bool computeMin)
			{
				var size = base.SizeOfLabel (computeMin);
				if (Handler.image != null) {
					size.Width += size.Height + ICON_PADDING;
				}
				return size;
			}
			#endif
		}

		public TabPageHandler()
		{
			TabViewItem = new MyTabViewItem
			{
				Handler = this,
				Identifier = new NSString(Guid.NewGuid().ToString()),
				View = new MacEventView { Handler = this }
			};
			Control = TabViewItem.View;
			Enabled = true;
		}

		public string Text
		{
			get { return TabViewItem.Label; }
			set { TabViewItem.Label = value; }
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (image != null)
				{
				}
			}
		}

		public override NSView ContentControl
		{
			get { return TabViewItem.View; }
		}

		public override bool Enabled { get; set; }
	}
}
