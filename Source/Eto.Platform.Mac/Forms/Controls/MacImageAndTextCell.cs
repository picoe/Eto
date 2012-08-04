using System;
using MonoMac.AppKit;
using SD = System.Drawing;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Platform.Mac.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class MacImageData : NSObject, ICloneable
	{
		bool nodispose;

		public  MacImageData ()
		{
		}

		public  MacImageData (IListItem item)
		{
			SetItem (item);
		}

		public MacImageData (IntPtr handle)
			: base(handle)
		{
		}
		
		public MacImageData (MacImageData value)
		{
			this.Text = value.Text;
			this.Image = value.Image;
		}
		
		[Export("description")]
		public new NSString Description {
			get { return Text ?? new NSString (string.Empty); }
		}
			
		public NSImage Image { get; set; }

		public NSString Text { get; set; }
		
		[Export("dealloc")]
		public void Dealloc ()
		{
			if (nodispose)
				Handle = IntPtr.Zero;
		}

		public void SetItem (IListItem value)
		{
			var imgitem = value as IImageListItem;
			if (imgitem != null && imgitem.Image != null)
				this.Image = ((IImageSource)imgitem.Image.Handler).GetImage ();
			this.Text = (NSString)value.Text;
		}
		
		[Export("copyWithZone:")]
		public virtual NSObject CopyWithZone (IntPtr zone)
		{
			var clone = (MacImageData)this.Clone ();
			clone.nodispose = true;
			return clone;
		}
		
		#region ICloneable implementation
		
		public virtual object Clone ()
		{
			return new MacImageData (this);
		}
		
		#endregion
	}

	public class MacImageListItemCell : NSTextFieldCell
	{
		public const int ImagePadding = 2;
		
		static IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");

		
		public MacImageListItemCell ()
		{
		}
		
		public MacImageListItemCell (IntPtr handle)
			: base(handle)
		{
		}
		
		public override SD.SizeF CellSizeForBounds (SD.RectangleF bounds)
		{
			var size = base.CellSizeForBounds (bounds);
			var data = ObjectValue as MacImageData;
			if (data != null && data.Image != null) {
				var imageSize = data.Image.Size;
				var newHeight = Math.Min (imageSize.Height, size.Height);
				var newWidth = imageSize.Width * newHeight / imageSize.Height;
				size.Width += newWidth + ImagePadding;
			}
			size.Width = Math.Min (size.Width, bounds.Width);
			return size;
		}
		
		public override void DrawInteriorWithFrame (SD.RectangleF cellFrame, NSView inView)
		{
			var data = ObjectValue as MacImageData;
			if (data != null) {
					
				if (data.Image != null) {
					var imageSize = data.Image.Size;
					var newHeight = Math.Min (imageSize.Height, cellFrame.Height);
					var newWidth = imageSize.Width * newHeight / imageSize.Height;
					
					var imageRect = new SD.RectangleF (cellFrame.X, cellFrame.Y, newWidth, newHeight);
					imageRect.Y += (cellFrame.Height - newHeight) / 2;
					
					if (data.Image.RespondsToSelector (new Selector (selDrawInRectFromRectOperationFractionRespectFlippedHints)))
						// 10.6+
						data.Image.Draw (imageRect, new SD.RectangleF (SD.PointF.Empty, data.Image.Size), NSCompositingOperation.SourceOver, 1, true, null);
					else {
						// 10.5-
						#pragma warning disable 618
						data.Image.Flipped = this.ControlView.IsFlipped; 
						#pragma warning restore 618
						data.Image.Draw (imageRect, new SD.RectangleF (SD.PointF.Empty, data.Image.Size), NSCompositingOperation.SourceOver, 1);
					}
					cellFrame.Width -= newWidth + ImagePadding;
					cellFrame.X += newWidth + ImagePadding;
				}
			}
			
			base.DrawInteriorWithFrame (cellFrame, inView);
		}
	}
}

