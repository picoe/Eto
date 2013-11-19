using System;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using Eto.Drawing;
using MonoMac.Foundation;
using Eto.Forms;
using Eto.Platform.Mac.Drawing;
using MonoMac.ImageIO;
using sd = System.Drawing;
using Eto.Platform.Mac.Forms.Printing;

namespace Eto.Platform.Mac
{
	public static partial class Conversions
	{
		public static NSColor ToNS(this Color color)
		{
			return NSColor.FromDeviceRgba(color.R, color.G, color.B, color.A);
		}

		public static Color ToEto(this NSColor color)
		{
			if (color == null)
				return Colors.Black;
			float red, green, blue, alpha;
			color.GetRgba(out red, out green, out blue, out alpha);
			return new Color(red, green, blue, alpha);
		}

		public static CGColor ToCG(this NSColor color)
		{
			var cs = NSColorSpace.DeviceRGBColorSpace;
			
			var devColor = color.UsingColorSpace(cs);
			float[] components;
			devColor.GetComponents(out components);
			return new CGColor(cs.ColorSpace, components);
		}

		public static NSRange ToNS(this Range range)
		{
			return new NSRange(range.Start, range.Length);
		}

		public static Range ToEto(this NSRange range)
		{
			return new Range(range.Location, range.Length);
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
				delta = new SizeF(theEvent.DeltaX, theEvent.DeltaY);
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
			using (var imageSource = CGImageSource.FromData (image.ToNS ().AsTiff ()))
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
				var rep = nsimage.BestRepresentation(new sd.RectangleF(0, 0, size.Value, size.Value), null, null);
				if (rep.PixelsWide > size.Value || rep.PixelsHigh > size.Value)
				{
					var max = Math.Max(nsimage.Size.Width, nsimage.Size.Height);
					var newsize = new sd.Size((int)(size.Value * nsimage.Size.Width / max), (int)(size.Value * nsimage.Size.Height / max));
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

		public static KeyEventArgs ToEtoKeyPressEventArgs(this NSEvent theEvent)
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

		public static PrintSettings ToEto(this NSPrintInfo value, Eto.Generator generator)
		{
			return value == null ? null : new PrintSettings(generator, new PrintSettingsHandler(value));
		}

		public static NSPrintInfo ToNS(this PrintSettings settings)
		{
			return settings == null ? null : ((PrintSettingsHandler)settings.Handler).Control;
		}

		public static SizeF ToEtoSize(this NSEdgeInsets insets)
		{
			return new SizeF(insets.Left + insets.Right, insets.Top + insets.Bottom);
		}
	}
}

