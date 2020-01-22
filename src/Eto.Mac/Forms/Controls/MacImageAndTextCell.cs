using System;
using Eto.Forms;
using Eto.Mac.Drawing;
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
	public class MacImageData : NSObject, ICloneable
	{
		public  MacImageData()
		{
		}

		public  MacImageData(IListItem item)
		{
			SetItem(item);
		}

		public MacImageData(IntPtr handle)
			: base(handle)
		{
		}

		public MacImageData(MacImageData value)
		{
			this.Text = value.Text;
			this.Image = value.Image;
		}

		public override string Description
		{
			get { return (string)Text ?? string.Empty; }
		}

		public NSImage Image { get; set; }

		public NSString Text { get; set; }

		
		public void SetItem(IListItem value)
		{
			var imgitem = value as IImageListItem;
			if (imgitem != null && imgitem.Image != null)
				Image = ((IImageSource)imgitem.Image.Handler).GetImage();
			Text = (NSString)value.Text;
		}

		[Export("copyWithZone:")]
		public virtual NSObject CopyWithZone(IntPtr zone)
		{
			var clone = (MacImageData)Clone();
			clone.DangerousRetain();
			return clone;
		}

		#region ICloneable implementation

		public virtual object Clone()
		{
			return new MacImageData(this);
		}

		#endregion
	}

	public class MacImageListItemCell : EtoLabelFieldCell
	{
		public const int ImagePadding = 2;
		NSShadow textShadow;
		NSShadow textHighlightShadow;
		NSColor groupColor = NSColor.FromCalibratedRgba(0x6F / (float)0xFF, 0x7E / (float)0xFF, 0x8B / (float)0xFF, 1.0F);
		//light shade: NSColor.FromCalibratedRgba (0x82 / (float)0xFF, 0x90 / (float)0xFF, 0x9D / (float)0xFF, 1.0F);
		

		public MacImageListItemCell()
		{
		}

		public override void SelectWithFrame(CGRect aRect, NSView inView, NSText editor, NSObject delegateObject, nint selStart, nint selLength)
		{
			aRect = AdjustBoundsForImage(aRect);
			base.SelectWithFrame(aRect, inView, editor, delegateObject, selStart, selLength);
		}

		public NSImageInterpolation ImageInterpolation { get; set; }

		public NSColor GroupColor
		{
			get { return groupColor; }
			set { groupColor = value; }
		}

		public bool UseTextShadow
		{
			get;
			set;
		}

		public void SetGroupItem(bool isGroupItem, NSTableView tableView, float? groupSize = null, float? normalSize = null)
		{
			if (isGroupItem)
				Font = NSFont.BoldSystemFontOfSize(groupSize ?? (float)NSFont.SystemFontSize);
			else if (Highlighted)
				Font = NSFont.BoldSystemFontOfSize(normalSize ?? (float)NSFont.SystemFontSize);
			else
				Font = NSFont.SystemFontOfSize(normalSize ?? (float)NSFont.SystemFontSize);
			
			if (Highlighted)
				TextColor = NSColor.Highlight;
			else if (!tableView.Window.IsKeyWindow)
				TextColor = NSColor.DisabledControlText;
			else if (isGroupItem)
				TextColor = GroupColor;
			else
				TextColor = NSColor.ControlText;
				
		}

		public NSShadow TextShadow
		{
			get
			{
				if (textShadow == null)
				{
					textShadow = new NSShadow();
					textShadow.ShadowColor = NSColor.FromDeviceWhite(1F, 0.5F);
					textShadow.ShadowOffset = new CGSize(0F, -1.0F);
					textShadow.ShadowBlurRadius = 0F;
				}
				return textShadow;
			}
			set { textShadow = value; }
		}

		public NSShadow TextHighlightShadow
		{
			get
			{
				if (textHighlightShadow == null)
				{
					textHighlightShadow = new NSShadow();
					textHighlightShadow.ShadowColor = NSColor.FromDeviceWhite(0F, 0.5F);
					textHighlightShadow.ShadowOffset = new CGSize(0F, -1.0F);
					textHighlightShadow.ShadowBlurRadius = 2F;
				}
				return textHighlightShadow;
			}
			set { textShadow = value; }
		}

		public MacImageListItemCell(IntPtr handle)
			: base(handle)
		{
		}

		CGSize GetImageSize(CGRect bounds)
		{
			if (ObjectValue is MacImageData data && data.Image != null)
			{
				var imageSize = data.Image.Size;
				if (imageSize.Width > 0)
				{
					var newHeight = Math.Min(imageSize.Height, bounds.Height);
					var newWidth = imageSize.Width * newHeight / imageSize.Height;
					newWidth += ImagePadding;
					return new CGSize(newWidth, newHeight);
				}

			}
			return CGSize.Empty;
		}

		CGRect AdjustBoundsForImage(CGRect bounds)
		{
			var imageSize = GetImageSize(bounds);
			if (imageSize.Width > 0)
			{
				bounds.X += imageSize.Width;
				bounds.Width -= imageSize.Width;
			}
			return bounds;
		}

		public override void DrawFocusRing(CGRect cellFrameMask, NSView inControlView)
		{
			cellFrameMask = AdjustBoundsForImage(cellFrameMask);
			base.DrawFocusRing(cellFrameMask, inControlView);
		}

		public override CGRect GetFocusRingMaskBounds(CGRect cellFrame, NSView controlView)
		{
			var rect = base.GetFocusRingMaskBounds(cellFrame, controlView);
			rect = AdjustBoundsForImage(rect);
			return rect;
		}

		public override CGSize CellSizeForBounds(CGRect bounds)
		{
			var size = base.CellSizeForBounds(bounds);
			var imageSize = GetImageSize(bounds);
			if (imageSize.Width > 0)
			{
				size.Width += (nfloat)(imageSize.Width + ImagePadding);
				size.Height = (nfloat)Math.Max(imageSize.Height, size.Height);
			}
			size.Width = (nfloat)Math.Min(size.Width, bounds.Width);
			return size;
		}

		public override CGRect TitleRectForBounds(CGRect theRect)
		{
			var result = base.TitleRectForBounds(theRect);
			result = AdjustBoundsForImage(result);
			return result;
		}

		public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
		{
			var frame = cellFrame;
			var data = ObjectValue as MacImageData;
			if (data != null)
			{
				if (data.Image != null)
				{
					var imageSize = data.Image.Size;
					if (imageSize.Width > 0 && imageSize.Height > 0)
					{
						var newHeight = (nfloat)Math.Min(imageSize.Height, frame.Height);
						var newWidth = (nfloat)(imageSize.Width * newHeight / imageSize.Height);

						var context = NSGraphicsContext.CurrentContext;
						var g = context.GraphicsPort;

						if (DrawsBackground && !Highlighted)
						{
							BackgroundColor.SetFill();
							g.FillRect(new CGRect(frame.X, frame.Y, newWidth + ImagePadding, frame.Height));
						}

						var imageRect = new CGRect(frame.X, frame.Y, newWidth, newHeight);
						imageRect.Y += (frame.Height - newHeight) / 2;

						const float alpha = 1; //Enabled ? 1 : (nfloat)0.5;

						context.ImageInterpolation = ImageInterpolation;

						data.Image.Draw(imageRect, new CGRect(CGPoint.Empty, data.Image.Size), NSCompositingOperation.SourceOver, alpha, true, null);
						frame.Width -= newWidth + ImagePadding;
						frame.X += newWidth + ImagePadding;
					}
				}
			}

			var titleSize = AttributedStringValue.Size;
			
			// test to see if the text height is bigger then the cell, if it is,
			// don't try to center it or it will be pushed up out of the cell!
			if (titleSize.Height < frame.Size.Height)
			{
				//frame.Y = frame.Y + (frame.Size.Height - titleSize.Height) / 2;
			}
			
			if (UseTextShadow)
			{
				var str = new NSMutableAttributedString(StringValue);
				str.AddAttribute(NSStringAttributeKey.Shadow, Highlighted ? TextHighlightShadow : TextShadow, new NSRange(0, (int)str.Length));
				AttributedStringValue = str;
			}
			
			base.DrawInteriorWithFrame(frame, inView);
		}
	}
}

