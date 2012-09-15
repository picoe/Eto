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

		public override string Description {
			get { return (string)Text ?? string.Empty; }
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
		NSShadow textShadow;
		NSShadow textHighlightShadow;
		
		static IntPtr selDrawInRectFromRectOperationFractionRespectFlippedHints = Selector.GetHandle ("drawInRect:fromRect:operation:fraction:respectFlipped:hints:");

		
		public MacImageListItemCell ()
		{
		}
		
		public bool UseTextShadow
		{
			get; set;
		}
		
		public void SetGroupItem (bool isGroupItem, NSTableView tableView, float? groupSize = null, float? normalSize = null)
		{
			if (isGroupItem)
				this.Font = NSFont.BoldSystemFontOfSize (groupSize ?? NSFont.SystemFontSize);
			else if (Highlighted)
				Font = NSFont.BoldSystemFontOfSize (normalSize ?? NSFont.SystemFontSize);
			else
				Font = NSFont.SystemFontOfSize (normalSize ?? NSFont.SystemFontSize);
			
			if (Highlighted)
				TextColor = NSColor.Highlight;
			else if (!tableView.Window.IsKeyWindow)
				TextColor = NSColor.DisabledControlText;
			else if (isGroupItem)
				TextColor = NSColor.FromCalibratedRgba (0x82 / (float)0xFF, 0x90 / (float)0xFF, 0x9D / (float)0xFF, 1.0F);
			else
				TextColor = NSColor.ControlText;
				
		}
		
		public NSShadow TextShadow
		{
			get {
				if (textShadow == null) {
					textShadow = new NSShadow();
					textShadow.ShadowColor = NSColor.FromDeviceWhite (1F, 0.5F);
					textShadow.ShadowOffset = new SD.SizeF(0F, -1.0F);
					textShadow.ShadowBlurRadius = 0F;
				}
				return textShadow;
			}
			set { textShadow = value; }
		}

		public NSShadow TextHighlightShadow
		{
			get {
				if (textHighlightShadow == null) {
					textHighlightShadow = new NSShadow();
					textHighlightShadow.ShadowColor = NSColor.FromDeviceWhite (0F, 0.5F);
					textHighlightShadow.ShadowOffset = new SD.SizeF(0F, -1.0F);
					textHighlightShadow.ShadowBlurRadius = 2F;
				}
				return textHighlightShadow;
			}
			set { textShadow = value; }
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
					if (imageSize.Width > 0 && imageSize.Height > 0) {
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
			}
			
			if (UseTextShadow) {
				var str = new NSMutableAttributedString(this.StringValue);
				str.AddAttribute (NSAttributedString.ShadowAttributeName, this.Highlighted ? TextHighlightShadow : TextShadow, new NSRange(0, str.Length));
				this.AttributedStringValue = str;
			}
			
			base.DrawInteriorWithFrame (cellFrame, inView);
		}
	}
}

