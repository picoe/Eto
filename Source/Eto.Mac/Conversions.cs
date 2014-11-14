using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Drawing;
using sd = System.Drawing;
using Eto.Mac.Forms.Printing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
using ImageIO;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
using MonoMac.ImageIO;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac
{
	public static partial class Conversions
	{
		public static NSColor ToNSUI(this Color color)
		{
			return NSColor.FromDeviceRgba(color.R, color.G, color.B, color.A);
		}

		public static Color ToEto(this NSColor color)
		{
			if (color == null)
				return Colors.Black;
			//if (color.ColorSpace.ColorSpaceModel != NSColorSpaceModel.RGB)
			color = color.UsingColorSpace(NSColorSpace.CalibratedRGB);
			nfloat red, green, blue, alpha;
			color.GetRgba(out red, out green, out blue, out alpha);
			return new Color((float)red, (float)green, (float)blue, (float)alpha);
		}

		public static NSRange ToNS(this Range<int> range)
		{
			return new NSRange(range.Start, range.End - range.Start + 1);
		}

		public static Range<int> ToEto(this NSRange range)
		{
			return new Range<int>((int)range.Location, (int)(range.Location + range.Length - 1));
		}

		public static NSImageInterpolation ToNS(this ImageInterpolation value)
		{
			switch (value)
			{
				case ImageInterpolation.Default:
					return NSImageInterpolation.Default;
				case ImageInterpolation.None:
					return NSImageInterpolation.None;
				case ImageInterpolation.Low:
					return NSImageInterpolation.Low;
				case ImageInterpolation.Medium:
					return NSImageInterpolation.Medium;
				case ImageInterpolation.High:
					return NSImageInterpolation.High;
				default:
					throw new NotSupportedException();
			}
		}

		public static ImageInterpolation ToEto(this NSImageInterpolation value)
		{
			switch (value)
			{
				case NSImageInterpolation.None:
					return ImageInterpolation.None;
				case NSImageInterpolation.Low:
					return ImageInterpolation.Low;
				case NSImageInterpolation.Medium:
					return ImageInterpolation.Medium;
				case NSImageInterpolation.Default:
				case NSImageInterpolation.High:
					return ImageInterpolation.High;
				default:
					throw new NotSupportedException();
			}
		}

		public static NSFontTraitMask ToNS(this FontStyle style)
		{
			var traits = (NSFontTraitMask)0;
			traits |= style.HasFlag(FontStyle.Bold) ? NSFontTraitMask.Bold : NSFontTraitMask.Unbold;
			traits |= style.HasFlag(FontStyle.Italic) ? NSFontTraitMask.Italic : NSFontTraitMask.Unitalic;
			//if (style.HasFlag (FontStyle.Condensed)) traits |= NSFontTraitMask.Condensed;
			//if (style.HasFlag (FontStyle.Light)) traits |= NSFontTraitMask.Narrow;
			return traits;
		}

		public static FontStyle ToEto(this NSFontTraitMask traits)
		{
			var style = FontStyle.None;
			if (traits.HasFlag(NSFontTraitMask.Bold))
				style |= FontStyle.Bold;
			if (traits.HasFlag(NSFontTraitMask.Italic))
				style |= FontStyle.Italic;
			//if (traits.HasFlag (NSFontTraitMask.Condensed))
			//	style |= FontStyle.Condensed;
			//if (traits.HasFlag (NSFontTraitMask.Narrow))
			//	style |= FontStyle.Light;
			return style;
		}

		public static NSPrintingOrientation ToNS(this PageOrientation value)
		{
			switch (value)
			{
				case PageOrientation.Landscape:
					return NSPrintingOrientation.Landscape;
				case PageOrientation.Portrait:
					return NSPrintingOrientation.Portrait;
				default:
					throw new NotSupportedException();
			}
		}

		public static PageOrientation ToEto(this NSPrintingOrientation value)
		{
			switch (value)
			{
				case NSPrintingOrientation.Landscape:
					return PageOrientation.Landscape;
				case NSPrintingOrientation.Portrait:
					return PageOrientation.Portrait;
				default:
					throw new NotSupportedException();
			}
		}

		public static PointF GetLocation(NSView view, NSEvent theEvent)
		{
			var loc = view.ConvertPointFromView(theEvent.LocationInWindow, null);
			if (!view.IsFlipped)
				loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEto();
		}

		public static MouseEventArgs GetMouseEvent(NSView view, NSEvent theEvent, bool includeWheel)
		{
			var pt = Conversions.GetLocation(view, theEvent);
			Keys modifiers = KeyMap.GetModifiers(theEvent);
			MouseButtons buttons = theEvent.GetMouseButtons();
			SizeF? delta = null;
			if (includeWheel)
				delta = new SizeF((float)theEvent.DeltaX, (float)theEvent.DeltaY);
			return new MouseEventArgs(buttons, modifiers, pt, delta);
		}

		public static MouseButtons GetMouseButtons(this NSEvent theEvent)
		{
			MouseButtons buttons = MouseButtons.None;
			
			switch (theEvent.Type)
			{
				case NSEventType.LeftMouseUp:
				case NSEventType.LeftMouseDown:
				case NSEventType.LeftMouseDragged:
					if ((theEvent.ModifierFlags & NSEventModifierMask.ControlKeyMask) > 0)
						buttons |= MouseButtons.Alternate;
					else
						buttons |= MouseButtons.Primary;
					break;
				case NSEventType.RightMouseUp:
				case NSEventType.RightMouseDown:
				case NSEventType.RightMouseDragged:
					buttons |= MouseButtons.Alternate;
					break;
				case NSEventType.OtherMouseUp:
				case NSEventType.OtherMouseDown:
				case NSEventType.OtherMouseDragged:
					buttons |= MouseButtons.Middle;
					break;
			}
			return buttons;
		}

		public static CGImage ToCG(this Image image)
		{
			using (var imageSource = CGImageSource.FromData(image.ToNS().AsTiff()))
			{
				return imageSource.CreateImage(0, null);
			}
		}

		public static NSImage ToNS(this Image image, int? size = null)
		{
			if (image == null)
				return null;
			var source = image.Handler as IImageSource;
			if (source == null)
				return null;
			var nsimage = source.GetImage();

			if (size != null)
			{
				var rep = nsimage.BestRepresentation(new CGRect(0, 0, size.Value, size.Value), null, null);
				if (rep.PixelsWide > size.Value || rep.PixelsHigh > size.Value)
				{
					var max = Math.Max(nsimage.Size.Width, nsimage.Size.Height);
					var newsize = new CGSize((int)(size.Value * nsimage.Size.Width / max), (int)(size.Value * nsimage.Size.Height / max));
					nsimage = nsimage.Resize(newsize);
				}
				else
				{
					nsimage = new NSImage();
					nsimage.AddRepresentation(rep);
				}
			}
			return nsimage;
		}

		public static NSCellImagePosition ToNS(this ButtonImagePosition value)
		{
			switch (value)
			{
				case ButtonImagePosition.Below:
					return NSCellImagePosition.ImageBelow;
				case ButtonImagePosition.Overlay:
					return NSCellImagePosition.ImageOverlaps;
				case ButtonImagePosition.Left:
					return NSCellImagePosition.ImageLeft;
				case ButtonImagePosition.Right:
					return NSCellImagePosition.ImageRight;
				case ButtonImagePosition.Above:
					return NSCellImagePosition.ImageAbove;
				default:
					throw new NotSupportedException();
			}
		}

		public static ButtonImagePosition ToEto(this NSCellImagePosition value)
		{
			switch (value)
			{
				case NSCellImagePosition.ImageBelow:
					return ButtonImagePosition.Below;
				case NSCellImagePosition.ImageOverlaps:
					return ButtonImagePosition.Overlay;
				case NSCellImagePosition.ImageLeft:
					return ButtonImagePosition.Left;
				case NSCellImagePosition.ImageRight:
					return ButtonImagePosition.Right;
				case NSCellImagePosition.ImageAbove:
					return ButtonImagePosition.Above;
				default:
					throw new NotSupportedException();
			}
		}

		public static WindowStyle ToEtoWindowStyle(this NSWindowStyle style)
		{
			return style.HasFlag(NSWindowStyle.Borderless) ? WindowStyle.None : WindowStyle.Default;
		}

		public static NSWindowStyle ToNS(this WindowStyle style, NSWindowStyle existing)
		{
			const NSWindowStyle NONE_STYLE = NSWindowStyle.Borderless;
			const NSWindowStyle DEFAULT_STYLE = NSWindowStyle.Titled;
			switch (style)
			{
				case WindowStyle.Default:
					return (existing & ~NONE_STYLE) | DEFAULT_STYLE;
				case WindowStyle.None:
					return (existing & ~DEFAULT_STYLE) | NONE_STYLE;
				default:
					throw new NotSupportedException();
			}
		}

		public static KeyEventArgs ToEtoKeyEventArgs(this NSEvent theEvent)
		{
			char keyChar = !string.IsNullOrEmpty(theEvent.Characters) ? theEvent.Characters[0] : '\0';
			Keys key = KeyMap.MapKey(theEvent.KeyCode);
			KeyEventArgs kpea;
			Keys modifiers = KeyMap.GetModifiers(theEvent);
			key |= modifiers;
			if (key != Keys.None)
			{
				if (((modifiers & ~(Keys.Shift | Keys.Alt)) == 0))
					kpea = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
				else
					kpea = new KeyEventArgs(key, KeyEventType.KeyDown);
			}
			else
			{
				kpea = new KeyEventArgs(key, KeyEventType.KeyDown, keyChar);
			}
			return kpea;
		}

		public static PrintSettings ToEto(this NSPrintInfo value)
		{
			return value == null ? null : new PrintSettings(new PrintSettingsHandler(value));
		}

		public static NSPrintInfo ToNS(this PrintSettings settings)
		{
			return settings == null ? null : ((PrintSettingsHandler)settings.Handler).Control;
		}

		public static SizeF ToEtoSize(this NSEdgeInsets insets)
		{
			return new SizeF((float)(insets.Left + insets.Right), (float)(insets.Top + insets.Bottom));
		}

		public static CalendarMode ToEto(this NSDatePickerMode mode)
		{
			switch (mode)
			{
				case NSDatePickerMode.Single:
					return CalendarMode.Single;
				case NSDatePickerMode.Range:
					return CalendarMode.Range;
				default:
					throw new NotSupportedException();
			}
		}

		public static NSDatePickerMode ToNS(this CalendarMode mode)
		{
			switch (mode)
			{
				case CalendarMode.Single:
					return NSDatePickerMode.Single;
				case CalendarMode.Range:
					return NSDatePickerMode.Range;
				default:
					throw new NotSupportedException();
			}
		}

		public static HorizontalAlign ToEto(this NSTextAlignment align)
		{
			switch (align)
			{
				default:
				case NSTextAlignment.Left:
					return HorizontalAlign.Left;
				case NSTextAlignment.Right:
					return HorizontalAlign.Right;
				case NSTextAlignment.Center:
					return HorizontalAlign.Center;
			}
		}

		public static NSTextAlignment ToNS(this HorizontalAlign align)
		{
			switch (align)
			{
				case HorizontalAlign.Left:
					return NSTextAlignment.Left;
				case HorizontalAlign.Center:
					return NSTextAlignment.Center;
				case HorizontalAlign.Right:
					return NSTextAlignment.Right;
				default:
					throw new NotSupportedException();
			}
		}
	}
}

