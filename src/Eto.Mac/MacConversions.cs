using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms;
using Eto.Mac.Drawing;
using Eto.Mac.Forms.Printing;
using System.Linq;
using Eto.Mac.Forms.Menu;

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

namespace Eto.Mac
{
	public static partial class MacConversions
	{
		public static NSColor ToNSUI(this Color color)
		{
			if (color.ControlObject is NSColor nscolor)
				return nscolor;
			if (color.ControlObject is CGColor cgcolor && MacVersion.IsAtLeast(10, 8))
				return NSColor.FromCGColor(cgcolor);
			return NSColor.FromDeviceRgba(color.R, color.G, color.B, color.A);
		}

		public static NSColor ToNSUI(this Color color, bool calibrated)
		{
			if (color.ControlObject is NSColor nscolor)
				return nscolor;
			if (color.ControlObject is CGColor cgcolor && MacVersion.IsAtLeast(10, 8))
				return NSColor.FromCGColor(cgcolor);
			return calibrated
				? NSColor.FromCalibratedRgba(color.R, color.G, color.B, color.A)
				: NSColor.FromDeviceRgba(color.R, color.G, color.B, color.A);
		}

		public static Color ToEtoWithAppearance(this NSColor color, bool calibrated = true)
		{
			if (color == null)
				return Colors.Transparent;
			if (!MacVersion.IsAtLeast(10, 9))
				return color.ToEto(calibrated);

			// use the current appearance to get the proper RGB values (it can be different than when the application started).
			NSAppearance saved = NSAppearance.CurrentAppearance;
			var appearance = NSApplication.SharedApplication.MainWindow?.EffectiveAppearance;
			if (appearance != null)
				NSAppearance.CurrentAppearance = appearance;

			var result = color.ToEto(calibrated);
			NSAppearance.CurrentAppearance = saved;
			return result;
		}

		public static Color ToEto(this NSColor color, bool calibrated = false)
		{
			if (color == null)
				return Colors.Transparent;

			var colorspace = calibrated ? NSColorSpace.CalibratedRGB : NSColorSpace.DeviceRGB;
			var converted = color.UsingColorSpaceFast(colorspace);
			if (converted == null)
			{
				// Convert named (e.g. system) colors to RGB using its CGColor
				converted = color.ToCG().ToNS().UsingColorSpaceFast(colorspace);

				if (converted == null)
					return new Color(color, 0, 0, 0, 1f);
			}
			nfloat red, green, blue, alpha;
			converted.GetRgba(out red, out green, out blue, out alpha);
			return new Color(color, (float)red, (float)green, (float)blue, (float)alpha);
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

		public static PointF ToEto(this CGPoint locationInWindow, NSView view)
		{
			var loc = view.ConvertPointFromView(locationInWindow, null);
			if (!view.IsFlipped)
				loc.Y = view.Frame.Height - loc.Y;
			return loc.ToEto();
		}

		public static GridViewCellEventArgs CreateCellEventArgs(GridColumn column, NSView tableView, int row, int col, object item)
		{
			return new GridViewCellEventArgs(column, row, col, item);
		}

		public static GridCellMouseEventArgs CreateCellMouseEventArgs(GridColumn column, NSView view, int row, int col, object item, NSEvent theEvent = null)
		{
			var ev = theEvent ?? NSApplication.SharedApplication.CurrentEvent;
			var buttons = ev.GetMouseButtons();
			var modifiers = ev.ModifierFlags.ToEto();
			var location = ev.LocationInWindow.ToEto(view);
			return new GridCellMouseEventArgs(column, row, col, item, buttons, modifiers, location);
		}

		public static MouseEventArgs GetMouseEvent(IMacViewHandler handler, NSEvent theEvent, bool includeWheel)
		{
			var view = handler.ContainerControl;
			var pt = theEvent.LocationInWindow;
			pt = handler.GetAlignmentPointForFramePoint(pt);
			Keys modifiers = theEvent.ModifierFlags.ToEto();
			MouseButtons buttons = theEvent.GetMouseButtons();
			SizeF? delta = null;
			if (includeWheel)
				delta = new SizeF((float)theEvent.DeltaX, (float)theEvent.DeltaY);
			return new MouseEventArgs(buttons, modifiers, pt.ToEto(view), delta);
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
					var buttonNumber = (int)theEvent.ButtonNumber;
                    switch (buttonNumber)
                    {
						case 0:
							buttons |= MouseButtons.Primary;
							break;
						case 1:
							buttons |= MouseButtons.Alternate;
							break;
						case 2:
							buttons |= MouseButtons.Middle;
							break;
					}
					break;
			}
			return buttons;
		}

		public static CGImage ToCG(this Image image)
		{
			return image.ToNS().CGImage;
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
				var mainScale = Screen.PrimaryScreen.RealScale;
				var scales = new[] { 1f, 2f }; // generate both retina and non-retina representations
				var sz = (float)Math.Ceiling(size.Value / mainScale);
				var rep = nsimage.BestRepresentation(new CGRect(0, 0, sz, sz), null, null);
				sz = size.Value;
				var imgsize = image.Size;
				var max = Math.Max(imgsize.Width, imgsize.Height);
				var newimagesize = new CGSize((nint)(sz * imgsize.Width / max), (nint)(sz * imgsize.Height / max));

				var newimage = new NSImage(newimagesize);
				foreach (var scale in scales)
				{
					sz = (float)Math.Ceiling(size.Value * scale / mainScale);
					rep = nsimage.BestRepresentation(new CGRect(0, 0, sz, sz), null, null);
					max = (int)Math.Max(rep.PixelsWide, rep.PixelsHigh);
					sz = (float)Math.Ceiling(size.Value * scale);
					var newsize = new CGSize((nint)(sz * rep.PixelsWide / max), (nint)(sz * rep.PixelsHigh / max));
					newimage.AddRepresentation(rep.Resize(newsize, imageSize: newimagesize));
				}
				nsimage = newimage;
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
			return style.HasFlag(NSWindowStyle.Titled) ? WindowStyle.Default : WindowStyle.None;
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
			Keys modifiers = theEvent.ModifierFlags.ToEto();
			key |= modifiers;

			KeyEventType keyEventType = theEvent.Type == NSEventType.KeyUp ? KeyEventType.KeyUp : KeyEventType.KeyDown;

			if (key != Keys.None)
			{
				if (((modifiers & ~(Keys.Shift | Keys.Alt)) == 0))
					kpea = new KeyEventArgs(key, keyEventType, keyChar);
				else
					kpea = new KeyEventArgs(key, keyEventType);
			}
			else
			{
				kpea = new KeyEventArgs(key, keyEventType, keyChar);
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

		public static Padding ToEto(this NSEdgeInsets insets)
		{
			return new Padding((int)insets.Left, (int)insets.Top, (int)insets.Right, (int)insets.Bottom);
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

		public static TextAlignment ToEto(this NSTextAlignment align)
		{
			switch (align)
			{
				default:
				case NSTextAlignment.Left:
					return TextAlignment.Left;
				case NSTextAlignment.Right:
					return TextAlignment.Right;
				case NSTextAlignment.Center:
					return TextAlignment.Center;
			}
		}

		public static NSTextAlignment ToNS(this TextAlignment align)
		{
			switch (align)
			{
				case TextAlignment.Left:
					return NSTextAlignment.Left;
				case TextAlignment.Center:
					return NSTextAlignment.Center;
				case TextAlignment.Right:
					return NSTextAlignment.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static Font ToEto(this NSFont font)
		{
			if (font == null)
				return null;
			return new Font(new FontHandler(font));
		}

		public static DockPosition ToEto(this NSTabViewType type)
		{
			switch (type)
			{
				case NSTabViewType.NSTopTabsBezelBorder:
					return DockPosition.Top;
				case NSTabViewType.NSLeftTabsBezelBorder:
					return DockPosition.Left;
				case NSTabViewType.NSBottomTabsBezelBorder:
					return DockPosition.Bottom;
				case NSTabViewType.NSRightTabsBezelBorder:
					return DockPosition.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static NSTabViewType ToNS(this DockPosition position)
		{
			switch (position)
			{
				case DockPosition.Top:
					return NSTabViewType.NSTopTabsBezelBorder;
				case DockPosition.Left:
					return NSTabViewType.NSLeftTabsBezelBorder;
				case DockPosition.Right:
					return NSTabViewType.NSRightTabsBezelBorder;
				case DockPosition.Bottom:
					return NSTabViewType.NSBottomTabsBezelBorder;
				default:
					throw new NotSupportedException();
			}
		}

		public static NSFont ToNS(this Font font)
		{
			if (font == null)
				return null;
			return ((FontHandler)font.Handler).Control;
		}

		public static BorderType ToEto(this NSBorderType border)
		{
			switch (border)
			{
				case NSBorderType.BezelBorder:
					return BorderType.Bezel;
				case NSBorderType.LineBorder:
					return BorderType.Line;
				case NSBorderType.NoBorder:
					return BorderType.None;
				default:
					throw new NotSupportedException();
			}
		}

		public static NSBorderType ToNS(this BorderType border)
		{
			switch (border)
			{
				case BorderType.Bezel:
					return NSBorderType.BezelBorder;
				case BorderType.Line:
					return NSBorderType.LineBorder;
				case BorderType.None:
					return NSBorderType.NoBorder;
				default:
					throw new NotSupportedException();
			}
		}

		public static DataObject ToEto(this NSPasteboard pasteboard) => new DataObject(new DataObjectHandler(pasteboard));

		public static NSPasteboard ToNS(this DataObject data) => DataObjectHandler.GetControl(data);

		public static NSDragOperation ToNS(this DragEffects effects)
		{
			var op = NSDragOperation.None;
			if (effects.HasFlag(DragEffects.Copy))
				op |= NSDragOperation.Copy;
			if (effects.HasFlag(DragEffects.Link))
				op |= NSDragOperation.Link;
			if (effects.HasFlag(DragEffects.Move))
				op |= NSDragOperation.Move;
			return op;
		}
		public static DragEffects ToEto(this NSDragOperation operation)
		{
			var effects = DragEffects.None;
			if (operation.HasFlag(NSDragOperation.Copy))
				effects |= DragEffects.Copy;
			if (operation.HasFlag(NSDragOperation.Link))
				effects |= DragEffects.Link;
			if (operation.HasFlag(NSDragOperation.Move))
				effects |= DragEffects.Move;
			return effects;
		}

		public static NSMenu ToNS(this ContextMenu menu) => ContextMenuHandler.GetControl(menu);

		internal static string StripAmpersands(string text)
		{
			if (string.IsNullOrEmpty(text)) return text;

			text = text.Replace("&&", "\x01");
			text = text.Replace("&", "");
			text = text.Replace("\x01", "&");
			return text;
		}

		public static NSCursor ToNS(this Cursor cursor) => CursorHandler.GetControl(cursor);

		public static NSLineBreakMode ToNS(this FormattedTextTrimming trim)
		{
			switch (trim)
			{
				case FormattedTextTrimming.CharacterEllipsis:
				case FormattedTextTrimming.WordEllipsis:
					return NSLineBreakMode.TruncatingTail;
				default:
				case FormattedTextTrimming.None:
					return NSLineBreakMode.Clipping;
			}
		}

		public static NSLineBreakMode ToNS(this FormattedTextWrapMode wrap)
		{
			switch (wrap)
			{
				case FormattedTextWrapMode.Character:
					return NSLineBreakMode.CharWrapping;
				case FormattedTextWrapMode.Word:
					return NSLineBreakMode.ByWordWrapping;
				default:
				case FormattedTextWrapMode.None:
					return NSLineBreakMode.Clipping;
			}
		}

		public static NSLineBreakMode ToNS(this WrapMode wrap)
		{
			switch (wrap)
			{
				case WrapMode.Character:
					return NSLineBreakMode.CharWrapping;
				case WrapMode.Word:
					return NSLineBreakMode.ByWordWrapping;
				default:
				case WrapMode.None:
					return NSLineBreakMode.Clipping;
			}
		}
		public static NSTextAlignment ToNS(this FormattedTextAlignment align)
		{
			switch (align)
			{
				case FormattedTextAlignment.Left:
					return NSTextAlignment.Left;
				case FormattedTextAlignment.Center:
					return NSTextAlignment.Center;
				case FormattedTextAlignment.Right:
					return NSTextAlignment.Right;
				case FormattedTextAlignment.Justify:
					return NSTextAlignment.Justified;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
