using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Mac.Drawing;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ImageViewCellHandler : CellHandler<NSImageCell, ImageViewCell>, IImageViewCell
	{
		public class EtoCell : NSImageCell, IMacControl
		{
			public object Handler { get; set; }
			
			public EtoCell ()
			{
			}
			
			public EtoCell (IntPtr handle) : base(handle)
			{
			}

			public Color BackgroundColor { get; set; }

			public bool DrawsBackground { get; set; }
			
			[Export("copyWithZone:")]
			NSObject CopyWithZone (IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr (
					SuperHandle,
					MacCommon.selCopyWithZone.Handle,
					zone
				);
				return new EtoCell (ptr) { Handler = this.Handler };
			}

			public override void DrawInteriorWithFrame (System.Drawing.RectangleF cellFrame, NSView inView)
			{

				if (DrawsBackground) {
					var nscontext = NSGraphicsContext.CurrentContext;
					var context = nscontext.GraphicsPort;
					context.SetFillColor (Generator.Convert (BackgroundColor));
					context.FillRect (cellFrame);
				}

				base.DrawInteriorWithFrame (cellFrame, inView);
			}
		}

		public override bool Editable {
			get { return base.Editable; }
			set { Control.Editable = value; }
		}

		public ImageViewCellHandler ()
		{
			Control = new EtoCell { Handler = this, Enabled = true };
		}

		public override void SetBackgroundColor (NSCell cell, Color color)
		{
			var c = cell as EtoCell;
			c.BackgroundColor = color;
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor (NSCell cell)
		{
			var c = cell as EtoCell;
			return c.BackgroundColor;
		}

		public override void SetForegroundColor (NSCell cell, Color color)
		{
		}

		public override Color GetForegroundColor (NSCell cell)
		{
			return Colors.Transparent;
		}

		public override NSObject GetObjectValue (object dataItem)
		{
			if (Widget.Binding != null) {
				var img = Widget.Binding.GetValue (dataItem) as Image;
				if (img != null) {
					var imgHandler = ((IImageSource)img.Handler);
					return imgHandler.GetImage ();
				}
			}
			return new NSImage ();
		}

		public override void SetObjectValue (object dataItem, NSObject val)
		{
		}
		
		public override float GetPreferredSize (object value, System.Drawing.SizeF cellSize, NSCell cell)
		{
			var img = value as Image;
			if (img != null) {
				return cellSize.Height / (float)img.Size.Height * (float)img.Size.Width;
			}
			return 16;
		}
	}
}

