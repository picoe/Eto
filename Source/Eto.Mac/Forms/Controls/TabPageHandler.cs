using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using MonoMac.Foundation;
using Eto.Drawing;
using MonoMac.ObjCRuntime;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class TabPageHandler : MacPanel<NSView, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		const int ICON_PADDING = 2;
		Image image;
		static readonly IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");

		public override NSView ContainerControl { get { return Control; } }

		public NSTabViewItem TabViewItem { get; private set; }
		
		class MyTabViewItem : NSTabViewItem
		{
			WeakReference handler;
			public TabPageHandler Handler { get { return (TabPageHandler)handler.Target; } set { handler = new WeakReference(value); } }
			
			public override void DrawLabel (bool shouldTruncateLabel, NSRect labelRect)
			{
				if (Handler.image != null) {
					var nsimage = (NSImage)Handler.image.ControlObject;

					if (nsimage.RespondsToSelector(new Selector(selDrawInRectFromRectOperationFractionRespectFlippedHints)))
						nsimage.Draw (new NSRect (labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new NSRect (new NSPoint(), nsimage.Size), NSCompositingOperation.SourceOver, 1, true, null);
					else {
						#pragma warning disable 618
						nsimage.Flipped = View.IsFlipped;
						#pragma warning restore 618
						nsimage.Draw (new NSRect (labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new NSRect (new NSPoint(), nsimage.Size), NSCompositingOperation.SourceOver, 1);
					}
					
					labelRect.X += labelRect.Height + ICON_PADDING;
					labelRect.Width -= labelRect.Height + ICON_PADDING;
					base.DrawLabel (shouldTruncateLabel, labelRect);
				}
				base.DrawLabel (shouldTruncateLabel, labelRect);
			}

			// TODO: Mac64
			#if !Mac64
			public override NSSize SizeOfLabel (bool computeMin)
			{
				var size = base.SizeOfLabel (computeMin);
				if (Handler.image != null) {
					size.Width += size.Height + ICON_PADDING;
				}
				return size;
			}
			#endif
		}
		
		public TabPageHandler ()
		{
			TabViewItem = new MyTabViewItem {
				Handler = this,
				Identifier = new NSString (Guid.NewGuid ().ToString ()),
				View = new MacEventView { Handler = this }
			};
			Control = TabViewItem.View;
			Enabled = true;
		}

		public string Text {
			get { return TabViewItem.Label; }
			set { TabViewItem.Label = value; }
		}
		
		public Image Image {
			get { return image; }
			set {
				image = value;
				if (image != null) {
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
