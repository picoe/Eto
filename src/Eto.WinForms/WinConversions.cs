using System;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Drawing;
using sd = System.Drawing;
using sdp = System.Drawing.Printing;
using sd2 = System.Drawing.Drawing2D;
using swf = System.Windows.Forms;
using sdi = System.Drawing.Imaging;
using Eto.WinForms.Forms.Printing;
using Eto.WinForms.Forms;

namespace Eto.WinForms
{
	public static partial class WinConversions
	{

		public static DialogResult ToEto(this swf.DialogResult result)
		{
			switch (result)
			{
				case swf.DialogResult.OK:
					return DialogResult.Ok;
				case swf.DialogResult.Cancel:
					return DialogResult.Cancel;
				case swf.DialogResult.Yes:
					return DialogResult.Yes;
				case swf.DialogResult.No:
					return DialogResult.No;
				case swf.DialogResult.Abort:
					return DialogResult.Cancel;
				case swf.DialogResult.Ignore:
					return DialogResult.Ignore;
				case swf.DialogResult.Retry:
					return DialogResult.Retry;
				case swf.DialogResult.None:
					return DialogResult.None;
				default:
					return DialogResult.None;
			}
		}

		public static sd.Imaging.ImageFormat ToSD(this ImageFormat format)
		{
			switch (format)
			{
				case ImageFormat.Jpeg:
					return sd.Imaging.ImageFormat.Jpeg;
				case ImageFormat.Bitmap:
					return sd.Imaging.ImageFormat.Bmp;
				case ImageFormat.Gif:
					return sd.Imaging.ImageFormat.Gif;
				case ImageFormat.Tiff:
					return sd.Imaging.ImageFormat.Tiff;
				case ImageFormat.Png:
					return sd.Imaging.ImageFormat.Png;
				default:
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid format specified"));
			}
		}

		public static ImageInterpolation ToEto(this sd2.InterpolationMode value)
		{
			switch (value)
			{
				case sd2.InterpolationMode.NearestNeighbor:
					return ImageInterpolation.None;
				case sd2.InterpolationMode.Low:
					return ImageInterpolation.Low;
				case sd2.InterpolationMode.High:
					return ImageInterpolation.Medium;
				case sd2.InterpolationMode.HighQualityBilinear:
					return ImageInterpolation.High;
				case sd2.InterpolationMode.Default:
					return ImageInterpolation.Default;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd2.InterpolationMode ToSD(this ImageInterpolation value)
		{
			switch (value)
			{
				case ImageInterpolation.Default:
					return sd2.InterpolationMode.High;
				case ImageInterpolation.None:
					return sd2.InterpolationMode.NearestNeighbor;
				case ImageInterpolation.Low:
					return sd2.InterpolationMode.Low;
				case ImageInterpolation.Medium:
					return sd2.InterpolationMode.High;
				case ImageInterpolation.High:
					return sd2.InterpolationMode.HighQualityBilinear;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd.FontStyle ToSD(this FontStyle style)
		{
			sd.FontStyle ret = sd.FontStyle.Regular;
			if (style.HasFlag(FontStyle.Bold))
				ret |= sd.FontStyle.Bold;
			if (style.HasFlag(FontStyle.Italic))
				ret |= sd.FontStyle.Italic;
			return ret;
		}
		public static sd.FontStyle ToSD(this FontDecoration decoration)
		{
			sd.FontStyle ret = sd.FontStyle.Regular;
			if (decoration.HasFlag(FontDecoration.Underline))
				ret |= sd.FontStyle.Underline;
			if (decoration.HasFlag(FontDecoration.Strikethrough))
				ret |= sd.FontStyle.Strikeout;
			return ret;
		}

		public static sdp.PrintRange ToSDP(this PrintSelection value)
		{
			switch (value)
			{
				case PrintSelection.AllPages:
					return sdp.PrintRange.AllPages;
				case PrintSelection.SelectedPages:
					return sdp.PrintRange.SomePages;
				case PrintSelection.Selection:
					return sdp.PrintRange.Selection;
				default:
					throw new NotSupportedException();
			}
		}

		public static PrintSelection ToEto(this sdp.PrintRange value)
		{
			switch (value)
			{
				case sdp.PrintRange.AllPages:
					return PrintSelection.AllPages;
				case sdp.PrintRange.SomePages:
					return PrintSelection.SelectedPages;
				case sdp.PrintRange.Selection:
					return PrintSelection.Selection;
				default:
					throw new NotSupportedException();
			}
		}

		public static FontStyle ToEtoStyle(this sd.FontStyle style)
		{
			var ret = FontStyle.None;
			if (style.HasFlag(sd.FontStyle.Bold))
				ret |= FontStyle.Bold;
			if (style.HasFlag(sd.FontStyle.Italic))
				ret |= FontStyle.Italic;
			return ret;
		}

		public static FontDecoration ToEtoDecoration(this sd.FontStyle style)
		{
			var ret = FontDecoration.None;
			if (style.HasFlag(sd.FontStyle.Underline))
				ret |= FontDecoration.Underline;
			if (style.HasFlag(sd.FontStyle.Strikeout))
				ret |= FontDecoration.Strikethrough;
			return ret;
		}


		public static sd.Graphics ToSD(this Graphics graphics)
		{
			var h = (GraphicsHandler)graphics.Handler;
			return h.Control;
		}

		public static sd.Image ToSD(this Image image, int? size = null)
		{
			if (image == null)
				return null;
			var h = (IWindowsImageSource)image.Handler;
			return h.GetImageWithSize(size);
		}

		public static Icon ToEto(this sd.Icon icon)
		{
			return new Icon(new IconHandler(icon));
		}

		public static sd.Image ToSD(this Image image, Size? size)
		{
			if (image == null)
				return null;
			var h = (IWindowsImageSource)image.Handler;
			return h.GetImageWithSize(size);
		}

		public static sd.Font ToSD(this Font font)
		{
			if (font == null)
				return null;
			return ((IWindowsFontSource)font.Handler).GetFont();
		}

		public static sd.FontFamily ToSD(this FontFamily family)
		{
			if (family == null)
				return null;
			return ((FontFamilyHandler)family.Handler).Control;
		}

		public static sd.FontStyle ToSD(this FontTypeface typeface)
		{
			return FontTypefaceHandler.GetControl(typeface);
		}

		public static sd.Font ToSDFont(this FontTypeface typeface, float size)
		{
			return new sd.Font(typeface.Family.ToSD(), size, typeface.ToSD());
		}

		public static sd.Font ToSD(this SystemFont systemFont)
		{
			switch (systemFont)
			{
				case SystemFont.Default:
					return sd.SystemFonts.DefaultFont;
				case SystemFont.User:
					return sd.SystemFonts.DefaultFont;
				case SystemFont.Bold:
					return new sd.Font(sd.SystemFonts.DefaultFont, sd.FontStyle.Bold);
				case SystemFont.TitleBar:
					return sd.SystemFonts.CaptionFont;
				case SystemFont.ToolTip:
					return sd.SystemFonts.DefaultFont;
				case SystemFont.Label:
					return sd.SystemFonts.DialogFont;
				case SystemFont.MenuBar:
					return sd.SystemFonts.MenuFont;
				case SystemFont.Menu:
					return sd.SystemFonts.MenuFont;
				case SystemFont.Message:
					return sd.SystemFonts.MessageBoxFont;
				case SystemFont.Palette:
					return sd.SystemFonts.DialogFont;
				case SystemFont.StatusBar:
					return sd.SystemFonts.StatusFont;
				default:
					throw new NotSupportedException();
			}
		}

		public static Font ToEto(this sd.Font font)
		{
			return font == null ? null : new Font(new FontHandler(font));
		}

		public static FontFamily ToEto(this sd.FontFamily family)
		{
			return family == null ? null : new FontFamily(new FontFamilyHandler(family));
		}

		/// <summary>
		/// Convert native graphics to eto wrapper
		/// </summary>
		/// <param name="g">Native graphics</param>
		/// <param name="dispose">Pass ownership and dispose native graphics in Dispose</param>
		/// <returns>The wrapper</returns>
		public static Graphics ToEto(this sd.Graphics g, bool dispose)
		{
			return new Graphics(new GraphicsHandler(g, dispose));
		}

		public static sd2.PixelOffsetMode ToSD(this PixelOffsetMode mode)
		{
			switch (mode)
			{
				case PixelOffsetMode.None:
					return sd2.PixelOffsetMode.None;
				case PixelOffsetMode.Half:
					return sd2.PixelOffsetMode.Half;
				default:
					throw new NotSupportedException();
			}
		}

		public static PixelOffsetMode ToEto(this sd2.PixelOffsetMode mode)
		{
			switch (mode)
			{
				case sd2.PixelOffsetMode.None:
					return PixelOffsetMode.None;
				case sd2.PixelOffsetMode.Half:
					return PixelOffsetMode.Half;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd2.Matrix ToSD(this IMatrix m)
		{
			return (sd2.Matrix)m.ControlObject;
		}

		public static IMatrix ToEto(this sd2.Matrix matrix)
		{
			return new MatrixHandler(matrix);
		}

		public static ITreeItem ToEto(this swf.TreeNode treeNode)
		{
			return
				treeNode != null
				? treeNode.Tag as ITreeItem
				: null;
		}

		public static sd.Pen ToSD(this Pen pen, RectangleF bounds)
		{
			return ((PenHandler)pen.Handler).GetPen(pen, bounds);
		}

		public static sd.Brush ToSD(this Brush brush, RectangleF bounds)
		{
			return ((BrushHandler)brush.Handler).GetBrush(brush, bounds);
		}

		public static sd2.LineJoin ToSD(this PenLineJoin value)
		{
			switch (value)
			{
				case PenLineJoin.Miter:
					return sd2.LineJoin.MiterClipped;
				case PenLineJoin.Bevel:
					return sd2.LineJoin.Bevel;
				case PenLineJoin.Round:
					return sd2.LineJoin.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static PenLineJoin ToEto(this sd2.LineJoin value)
		{
			switch (value)
			{
				case sd2.LineJoin.Bevel:
					return PenLineJoin.Bevel;
				case sd2.LineJoin.Miter:
					return PenLineJoin.Miter;
				case sd2.LineJoin.Round:
					return PenLineJoin.Round;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd2.LineCap ToSD(this PenLineCap value)
		{
			switch (value)
			{
				case PenLineCap.Butt:
					return sd2.LineCap.Flat;
				case PenLineCap.Round:
					return sd2.LineCap.Round;
				case PenLineCap.Square:
					return sd2.LineCap.Square;
				default:
					throw new NotSupportedException();
			}
		}

		public static PenLineCap ToEto(this sd2.LineCap value)
		{
			switch (value)
			{
				case sd2.LineCap.Flat:
					return PenLineCap.Butt;
				case sd2.LineCap.Round:
					return PenLineCap.Round;
				case sd2.LineCap.Square:
					return PenLineCap.Square;
				default:
					throw new NotSupportedException();
			}
		}

		public static sd2.GraphicsPath ToSD(this IGraphicsPath path)
		{
			return (sd2.GraphicsPath)path.ControlObject;
		}

		public static sd2.WrapMode ToSD(this GradientWrapMode wrap)
		{
			switch (wrap)
			{
				case GradientWrapMode.Reflect:
					return sd2.WrapMode.TileFlipXY;
				case GradientWrapMode.Repeat:
					return sd2.WrapMode.Tile;
				case GradientWrapMode.Pad:
					return sd2.WrapMode.Clamp;
				default:
					throw new NotSupportedException();
			}
		}

		public static GradientWrapMode ToEto(this sd2.WrapMode wrapMode)
		{
			switch (wrapMode)
			{
				case sd2.WrapMode.TileFlipXY:
					return GradientWrapMode.Reflect;
				case sd2.WrapMode.Tile:
					return GradientWrapMode.Repeat;
				case sd2.WrapMode.Clamp:
					return GradientWrapMode.Pad;
				default:
					throw new NotSupportedException();
			}
		}

		public static int BitsPerPixel(this sdi.PixelFormat format)
		{
			switch (format)
			{
				case sdi.PixelFormat.Format1bppIndexed:
					return 1;
				case sdi.PixelFormat.Format4bppIndexed:
					return 4;
				case sdi.PixelFormat.Format8bppIndexed:
					return 8;
				case sdi.PixelFormat.Format24bppRgb:
					return 24;
				case sdi.PixelFormat.Format32bppArgb:
				case sdi.PixelFormat.Format32bppPArgb:
				case sdi.PixelFormat.Format32bppRgb:
					return 32;
				default:
					throw new NotSupportedException();
			}
		}

		public static swf.TextImageRelation ToSD(this ButtonImagePosition value)
		{
			switch (value)
			{
				case ButtonImagePosition.Left:
					return swf.TextImageRelation.ImageBeforeText;
				case ButtonImagePosition.Right:
					return swf.TextImageRelation.TextBeforeImage;
				case ButtonImagePosition.Above:
					return swf.TextImageRelation.ImageAboveText;
				case ButtonImagePosition.Below:
					return swf.TextImageRelation.TextAboveImage;
				case ButtonImagePosition.Overlay:
					return swf.TextImageRelation.Overlay;
				default:
					throw new NotSupportedException();
			}
		}

		public static ButtonImagePosition ToEto(this swf.TextImageRelation value)
		{
			switch (value)
			{
				case swf.TextImageRelation.ImageAboveText:
					return ButtonImagePosition.Above;
				case swf.TextImageRelation.ImageBeforeText:
					return ButtonImagePosition.Left;
				case swf.TextImageRelation.Overlay:
					return ButtonImagePosition.Overlay;
				case swf.TextImageRelation.TextAboveImage:
					return ButtonImagePosition.Below;
				case swf.TextImageRelation.TextBeforeImage:
					return ButtonImagePosition.Left;
				default:
					throw new NotSupportedException();
			}
		}

		public static bool IsResizable(this swf.FormBorderStyle style)
		{
			switch (style)
			{
				case swf.FormBorderStyle.Fixed3D:
				case swf.FormBorderStyle.FixedDialog:
				case swf.FormBorderStyle.FixedSingle:
				case swf.FormBorderStyle.FixedToolWindow:
				case swf.FormBorderStyle.None:
					return false;
				case swf.FormBorderStyle.Sizable:
				case swf.FormBorderStyle.SizableToolWindow:
					return true;
				default:
					throw new NotSupportedException();
			}
		}

		public static WindowStyle ToEto(this swf.FormBorderStyle style)
		{
			switch (style)
			{
				case swf.FormBorderStyle.Fixed3D:
				case swf.FormBorderStyle.Sizable:
				case swf.FormBorderStyle.SizableToolWindow:
				case swf.FormBorderStyle.FixedDialog:
					return WindowStyle.Default;
				case swf.FormBorderStyle.None:
					return WindowStyle.None;
				default:
					throw new NotSupportedException();
			}
		}

		public static swf.FormBorderStyle ToSWF(this WindowStyle style, bool resizable, swf.FormBorderStyle defaultStyle)
		{
			switch (style)
			{
				case WindowStyle.Default:
					return resizable ? swf.FormBorderStyle.Sizable : defaultStyle;
				case WindowStyle.None:
					return swf.FormBorderStyle.None;
				default:
					throw new NotSupportedException();
			}
		}

		public static CellStates ToEto(this swf.DataGridViewElementStates state)
		{
			if (state.HasFlag(swf.DataGridViewElementStates.Selected))
				return CellStates.Selected;

			return CellStates.None;
		}

		public static PrintSettings ToEto(this sdp.PrinterSettings settings)
		{
			return settings == null ? null : new PrintSettings(new PrintSettingsHandler(settings));
		}

		public static sdp.PrinterSettings ToSD(this PrintSettings settings)
		{
			if (settings == null)
				return PrintSettingsHandler.DefaultSettings();
			return ((PrintSettingsHandler)settings.Handler).Control;
		}

		public static sdp.PrintDocument ToSD(this PrintDocument document)
		{
			return document == null ? null : ((PrintDocumentHandler)document.Handler).Control;
		}

		public static Range<DateTime> ToEto(this swf.SelectionRange range)
		{
			return new Range<DateTime>(range.Start, range.End);
		}

		public static swf.SelectionRange ToSWF(this Range<DateTime> range)
		{
			return new swf.SelectionRange(range.Start, range.End);
		}

		public static swf.HorizontalAlignment ToSWF(this TextAlignment align)
		{
			switch (align)
			{
				case TextAlignment.Left:
					return swf.HorizontalAlignment.Left;
				case TextAlignment.Center:
					return swf.HorizontalAlignment.Center;
				case TextAlignment.Right:
					return swf.HorizontalAlignment.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static TextAlignment ToEtoTextAlignment(this swf.DataGridViewContentAlignment existing)
		{
			switch (existing)
			{
				default:
				case swf.DataGridViewContentAlignment.NotSet:
				case swf.DataGridViewContentAlignment.TopLeft:
				case swf.DataGridViewContentAlignment.MiddleLeft:
				case swf.DataGridViewContentAlignment.BottomLeft:
					return TextAlignment.Left;
				case swf.DataGridViewContentAlignment.TopCenter:
				case swf.DataGridViewContentAlignment.MiddleCenter:
				case swf.DataGridViewContentAlignment.BottomCenter:
					return TextAlignment.Center;
				case swf.DataGridViewContentAlignment.TopRight:
				case swf.DataGridViewContentAlignment.MiddleRight:
				case swf.DataGridViewContentAlignment.BottomRight:
					return TextAlignment.Right;
			}
		}

		public static VerticalAlignment ToEtoVerticalAlignment(this swf.DataGridViewContentAlignment existing)
		{
			switch (existing)
			{
				default:
				case swf.DataGridViewContentAlignment.NotSet:
				case swf.DataGridViewContentAlignment.TopLeft:
				case swf.DataGridViewContentAlignment.MiddleLeft:
				case swf.DataGridViewContentAlignment.BottomLeft:
					return VerticalAlignment.Top;
				case swf.DataGridViewContentAlignment.TopCenter:
				case swf.DataGridViewContentAlignment.MiddleCenter:
				case swf.DataGridViewContentAlignment.BottomCenter:
					return VerticalAlignment.Center;
				case swf.DataGridViewContentAlignment.TopRight:
				case swf.DataGridViewContentAlignment.MiddleRight:
				case swf.DataGridViewContentAlignment.BottomRight:
					return VerticalAlignment.Bottom;
			}
		}

		public static swf.DataGridViewContentAlignment ToSWF(TextAlignment textAlignment, VerticalAlignment verticalAlignment)
		{
			switch (verticalAlignment)
			{
				case VerticalAlignment.Stretch:
				case VerticalAlignment.Top:
					switch (textAlignment)
					{
						case TextAlignment.Left:
							return swf.DataGridViewContentAlignment.TopLeft;
						case TextAlignment.Center:
							return swf.DataGridViewContentAlignment.TopCenter;
						case TextAlignment.Right:
							return swf.DataGridViewContentAlignment.TopRight;
						default:
							throw new ArgumentOutOfRangeException();
					}
				case VerticalAlignment.Center:
					switch (textAlignment)
					{
						case TextAlignment.Left:
							return swf.DataGridViewContentAlignment.MiddleLeft;
						case TextAlignment.Center:
							return swf.DataGridViewContentAlignment.MiddleCenter;
						case TextAlignment.Right:
							return swf.DataGridViewContentAlignment.MiddleRight;
						default:
							throw new ArgumentOutOfRangeException();
					}
				case VerticalAlignment.Bottom:
					switch (textAlignment)
					{
						case TextAlignment.Left:
							return swf.DataGridViewContentAlignment.BottomLeft;
						case TextAlignment.Center:
							return swf.DataGridViewContentAlignment.BottomCenter;
						case TextAlignment.Right:
							return swf.DataGridViewContentAlignment.BottomRight;
						default:
							throw new ArgumentOutOfRangeException();
					}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static TextAlignment ToEto(this swf.HorizontalAlignment align)
		{
			switch (align)
			{
				case swf.HorizontalAlignment.Center:
					return TextAlignment.Center;
				case swf.HorizontalAlignment.Left:
					return TextAlignment.Left;
				case swf.HorizontalAlignment.Right:
					return TextAlignment.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static swf.Form ToSWF(this Window window)
		{
			if (window == null)
				return null;
			var handler = window.Handler as Forms.IWindowHandler;
			if (handler != null)
				return handler.Win32Window as swf.Form;
			return null;
		}

		public static swf.TabAlignment ToSWF(this DockPosition position)
		{
			switch (position)
			{
				case DockPosition.Top:
					return swf.TabAlignment.Top;
				case DockPosition.Left:
					return swf.TabAlignment.Left;
				case DockPosition.Right:
					return swf.TabAlignment.Right;
				case DockPosition.Bottom:
					return swf.TabAlignment.Bottom;
				default:
					throw new NotSupportedException();
			}
		}

		public static DockPosition ToEto(this swf.TabAlignment alignment)
		{
			switch (alignment)
			{
				case swf.TabAlignment.Top:
					return DockPosition.Top;
				case swf.TabAlignment.Bottom:
					return DockPosition.Bottom;
				case swf.TabAlignment.Left:
					return DockPosition.Left;
				case swf.TabAlignment.Right:
					return DockPosition.Right;
				default:
					throw new NotSupportedException();
			}
		}

		public static Screen ToEto(this swf.Screen screen)
		{
			if (screen == null)
				return null;
			return new Screen(new ScreenHandler(screen));
		}

		public static BorderType ToEto(this swf.BorderStyle border)
		{
			switch (border)
			{
				case swf.BorderStyle.FixedSingle:
					return BorderType.Line;
				case swf.BorderStyle.None:
					return BorderType.None;
				case swf.BorderStyle.Fixed3D:
					return BorderType.Bezel;
				default:
					throw new NotSupportedException();
			}
		}

		public static swf.BorderStyle ToSWF(this BorderType border)
		{
			switch (border)
			{
				case BorderType.Bezel:
					return swf.BorderStyle.Fixed3D;
				case BorderType.Line:
					return swf.BorderStyle.FixedSingle;
				case BorderType.None:
					return swf.BorderStyle.None;
				default:
					throw new NotSupportedException();
			}
		}

		public static Bitmap ToEto(this sd.Bitmap sdbitmap)
		{
			if (sdbitmap == null)
				return null;
			return new Bitmap(new BitmapHandler(sdbitmap));
		}

		public static swf.DragDropEffects ToSwf(this DragEffects action)
		{
			var resultAction = swf.DragDropEffects.None;

			if (action.HasFlag(DragEffects.Copy))
				resultAction |= swf.DragDropEffects.Copy;

			if (action.HasFlag(DragEffects.Move))
				resultAction |= swf.DragDropEffects.Move;

			if (action.HasFlag(DragEffects.Link))
				resultAction |= swf.DragDropEffects.Link;

			return resultAction;
		}

		public static DragEffects ToEto(this swf.DragDropEffects effects)
		{
			var action = DragEffects.None;

			if (effects.HasFlag(swf.DragDropEffects.Copy))
				action |= DragEffects.Copy;

			if (effects.HasFlag(swf.DragDropEffects.Move))
				action |= DragEffects.Move;

			if (effects.HasFlag(swf.DragDropEffects.Link))
				action |= DragEffects.Link;

			return action;
		}
		public static swf.DataObject ToSwf(this DataObject data) => DataObjectHandler.GetControl(data);

		public static DataObject ToEto(this swf.IDataObject data) => new DataObject(new DataObjectHandler(data));

		public static Keys GetEtoModifiers(this swf.DragEventArgs e)
		{
			var modifiers = Keys.None;
			if ((e.KeyState & 4) == 4)
				modifiers |= Keys.Shift;
			if ((e.KeyState & 8) == 8)
				modifiers |= Keys.Control;
			if ((e.KeyState & 32) == 32)
				modifiers |= Keys.Alt;
			return modifiers;
		}

		public static MouseButtons GetEtoButtons(this swf.DragEventArgs e)
		{
			var buttons = MouseButtons.None;
			if ((e.KeyState & 1) == 1)
				buttons |= MouseButtons.Primary;
			if ((e.KeyState & 2) == 2)
				buttons |= MouseButtons.Primary;
			if ((e.KeyState & 16) == 16)
				buttons |= MouseButtons.Middle;
			return buttons;
		}

		public static swf.Cursor ToSwf(this Cursor cursor) => CursorHandler.GetControl(cursor);

	}
}
