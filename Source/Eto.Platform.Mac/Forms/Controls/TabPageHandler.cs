using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using MonoMac.Foundation;
using Eto.Drawing;
using MonoMac.ObjCRuntime;
using Eto.Platform.Mac.Drawing;
using Eto.Platform.Mac.Forms.Printing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class TabPageHandler : MacDockContainer<NSView, TabPage>, ITabPage, IMacContainer
	{
		const int ICON_PADDING = 2;
		Image image;
		static IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");

		public override NSView ContainerControl
		{
			get { return Control; }
		}

		public NSTabViewItem TabViewItem { get; private set; }
		
		class MyTabViewItem : NSTabViewItem
		{
			WeakReference handler;
			public TabPageHandler Handler { get { return (TabPageHandler)handler.Target; } set { handler = new WeakReference(value); } }
			
			public override void DrawLabel (bool shouldTruncateLabel, SD.RectangleF labelRect)
			{
				if (Handler.image != null) {
					var nsimage = Handler.image.ControlObject as NSImage;

					if (nsimage.RespondsToSelector(new Selector(selDrawInRectFromRectOperationFractionRespectFlippedHints)))
						nsimage.Draw (new SD.RectangleF (labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new SD.RectangleF (SD.PointF.Empty, nsimage.Size), NSCompositingOperation.SourceOver, 1, true, null);
					else {
						#pragma warning disable 618
						nsimage.Flipped = this.View.IsFlipped;
						#pragma warning restore 618
						nsimage.Draw (new SD.RectangleF (labelRect.X, labelRect.Y, labelRect.Height, labelRect.Height), new SD.RectangleF (SD.PointF.Empty, nsimage.Size), NSCompositingOperation.SourceOver, 1);
					}
					
					labelRect.X += labelRect.Height + ICON_PADDING;
					labelRect.Width -= labelRect.Height + ICON_PADDING;
					base.DrawLabel (shouldTruncateLabel, labelRect);
				}
				base.DrawLabel (shouldTruncateLabel, labelRect);
			}
			
			public override SD.SizeF SizeOfLabel (bool computeMin)
			{
				var size = base.SizeOfLabel (computeMin);
				if (Handler.image != null) {
					size.Width += size.Height + ICON_PADDING;
				}
				return size;
			}
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
