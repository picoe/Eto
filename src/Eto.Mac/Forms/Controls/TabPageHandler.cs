using System;
using Eto.Forms;
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

namespace Eto.Mac.Forms.Controls
{
	public class TabPageHandler : MacPanel<NSTabViewItem, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		const int ICON_PADDING = 2;
		Image image;
		static readonly IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");

		public override NSView ContainerControl { get { return Control.View; } }

		public class EtoTabViewItem : NSTabViewItem, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public TabPageHandler Handler { get { return (TabPageHandler)WeakHandler.Target; } set { WeakHandler = new WeakReference(value); } }

			public override void DrawLabel(bool shouldTruncateLabel, CGRect labelRect)
			{
				var h = Handler;
				if (h?.image != null)
				{
					var nsimage = (NSImage)h.image.ControlObject;

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

			public override CGSize SizeOfLabel(bool computeMin)
			{
				var size = base.SizeOfLabel(computeMin);
				if (Handler?.image != null)
				{
					size.Width += size.Height + ICON_PADDING;
				}
				return size;
			}

			public EtoTabViewItem(IMacViewHandler handler)
			{
				Identifier = new NSString(Guid.NewGuid().ToString());
				View = new MacPanelView { Handler = handler };
			}
		}


		protected override NSTabViewItem CreateControl()
		{
			return new EtoTabViewItem(this);
		}

		public string Text
		{
			get { return Control.Label; }
			set { Control.Label = value ?? string.Empty; }
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

		public override NSView ContentControl => Control.View;
	}
}
