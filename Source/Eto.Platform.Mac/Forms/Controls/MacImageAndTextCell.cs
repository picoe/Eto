using System;
using MonoMac.AppKit;
using SD = System.Drawing;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

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
		public NSString Description {
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
				this.Image = imgitem.Image.ControlObject as NSImage;
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
		const int IMAGE_PADDING = 2;
		
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
				size.Width += bounds.Height + IMAGE_PADDING;
			}
			size.Width = Math.Min (size.Width, bounds.Width);
			return size;
		}
		
		public override void DrawInteriorWithFrame (SD.RectangleF cellFrame, NSView inView)
		{
			var data = ObjectValue as MacImageData;
			if (data != null) {
					
				if (data.Image != null) {
					var imageRect = new SD.RectangleF (cellFrame.X, cellFrame.Y, cellFrame.Height, cellFrame.Height);
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
					cellFrame.Width -= cellFrame.Height + IMAGE_PADDING;
					cellFrame.X += cellFrame.Height + IMAGE_PADDING;
				}
			}
			
			base.DrawInteriorWithFrame (cellFrame, inView);
		}
	}
}

