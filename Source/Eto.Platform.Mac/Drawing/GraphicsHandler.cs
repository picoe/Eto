using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
#if OSX
using Eto.Platform.Mac.Forms;
using MonoMac.CoreGraphics;
using MonoMac.AppKit;
using MonoMac.Foundation;
#elif IOS
using Eto.Platform.iOS.Forms;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using NSView = MonoTouch.UIKit.UIView;
#endif

#if OSX
namespace Eto.Platform.Mac.Drawing
#elif IOS
namespace Eto.Platform.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for the <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : 
#if OSX
		MacBase<CGContext, Graphics>, IGraphics
#else
		WidgetHandler<CGContext, Graphics>, IGraphics
#endif
	{
#if OSX
		NSGraphicsContext graphicsContext;
#endif
		NSView view;
		float height;
		bool needsLock;
		PixelOffsetMode pixelOffsetMode = PixelOffsetMode.None;
		float offset = 0.5f;
		float inverseoffset = 0f;
		SD.RectangleF? clipBounds;
		IGraphicsPath clipPath;

		public float Offset { get { return offset; } }
		public float InverseOffset { get { return inverseoffset; } }
		
		public bool Flipped {
			get;
			set;
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get { return pixelOffsetMode; }
			set {
				pixelOffsetMode = value;
				offset = value == PixelOffsetMode.None ? 0.5f : 0f;
				inverseoffset = value == PixelOffsetMode.None ? 0f : 0.5f;
			}
		}

		public GraphicsHandler ()
		{
		}

		public GraphicsHandler (NSView view)
		{
			this.view = view;
#if OSX
			needsLock = true;
			graphicsContext = NSGraphicsContext.FromWindow (view.Window);
			Control = graphicsContext.GraphicsPort;
			this.Flipped = view.IsFlipped;
#elif IOS
			this.Control = UIGraphics.GetCurrentContext ();
			this.Flipped = !view.Layer.GeometryFlipped;
#endif
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning (false);
			if (!Flipped)
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));

			Control.SaveState ();
#if OSX
			Control.ClipToRect (TranslateView (view.VisibleRect ()));
#endif
		}

#if OSX
		public GraphicsHandler (NSGraphicsContext graphicsContext, float height, bool flipped)
		{ 
			this.height = height;
			this.graphicsContext = graphicsContext;
			this.Control = graphicsContext.GraphicsPort;
			this.Flipped = flipped;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning (false);
			if (!Flipped)
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));
		}
#elif IOS
		public GraphicsHandler (CGContext context, float height, bool flipped)
		{
			this.height = height;
			if (height > 0) {
				this.Flipped = flipped;
			}
			//this.Control = gc;
			this.Control = context;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			//context.ScaleCTM(1, -1);
			Control.SetAllowsSubpixelPositioning (false);
			if (!Flipped)
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));
		}

#endif
		
		public bool IsRetained { get { return false; } }

		bool antialias;

		public bool Antialias {
			get {
				return antialias;
			}
			set {
				antialias = value;
				Control.SetShouldAntialias (value);
			}
		}

		public ImageInterpolation ImageInterpolation {
			get { return Control.InterpolationQuality.ToEto (); }
			set { Control.InterpolationQuality = value.ToCG (); }
		}

		public void CreateFromImage (Bitmap image)
		{
			var handler = image.Handler as BitmapHandler;
#if OSX
			var rep = handler.Control.Representations ().OfType<NSBitmapImageRep> ().FirstOrDefault ();
			graphicsContext = NSGraphicsContext.FromBitmap (rep);
			Control = graphicsContext.GraphicsPort;
#elif IOS
			var cgimage = handler.Control.CGImage;
			Control = new CGBitmapContext (handler.Data.MutableBytes, cgimage.Width, cgimage.Height, cgimage.BitsPerComponent, cgimage.BytesPerRow, cgimage.ColorSpace, cgimage.BitmapInfo);
#endif

			Flipped = false;
			this.height = image.Size.Height;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning (false);
			if (!Flipped)
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));
		}

		public void Commit ()
		{
			/*if (image != null)
			{
				Gdk.Pixbuf pb = (Gdk.Pixbuf)image.ControlObject;
				pb.GetFromDrawable(drawable, drawable.Colormap, 0, 0, 0, 0, image.Size.Width, image.Size.Height);
				gc.Dispose();
				gc = null;
				drawable.Dispose();
				drawable = null;
			}*/
		}
		
		public void Flush ()
		{
			if (view != null && !needsLock) {
				//view.UnlockFocus ();
				needsLock = true;
			}
#if OSX
			graphicsContext.FlushGraphics ();
#endif
		}
		
		public float ViewHeight {
			get {
				if (view != null)
					return view.Bounds.Height;
				else
					return this.height;
			}
		}

		public SD.PointF TranslateView (SD.PointF point, bool halfers = false, bool inverse = false, float elementHeight = 0)
		{
			if (halfers) {
				if (inverse) {
					point.X += inverseoffset;
					point.Y += inverseoffset;
				}
				else {
					point.X += offset;
					point.Y += offset;
				}
			}
			if (view != null) {
				point = view.ConvertPointToView (point, null);
			}
			return point;
		}
		
		public SD.RectangleF TranslateView (SD.RectangleF rect, bool halfers = false, bool inverse = false)
		{
			if (halfers) {
				if (inverse) {
					rect.X += inverseoffset;
					rect.Y += inverseoffset;
				}
				else {
					rect.X += offset;
					rect.Y += offset;
				}
			}

			if (view != null) {
				rect = view.ConvertRectToView (rect, null);
			}
			return rect;
		}

		public SD.RectangleF Translate (SD.RectangleF rect, float height)
		{
			if (!Flipped)
				rect.Y = height - rect.Y - rect.Height;
			return rect;
		}

		void Lock ()
		{
			if (needsLock && view != null) {
				//view.LockFocus();
				needsLock = false;
			}
		}
		
		void StartDrawing ()
		{
			Lock ();
#if OSX
			NSGraphicsContext.GlobalSaveGraphicsState ();
			NSGraphicsContext.CurrentContext = this.graphicsContext;
#elif IOS
			UIGraphics.PushContext (this.Control);
			this.Control.SaveState ();
#endif
		}
		
		void EndDrawing ()
		{
#if OSX
			NSGraphicsContext.GlobalRestoreGraphicsState ();
#elif IOS
			this.Control.RestoreState ();
			UIGraphics.PopContext ();
#endif
		}

		public void DrawLine (Pen pen, float startx, float starty, float endx, float endy)
		{
			StartDrawing ();
			pen.Apply (this);
			Control.StrokeLineSegments (new SD.PointF[] { TranslateView (new SD.PointF (startx, starty), true), TranslateView (new SD.PointF (endx, endy), true) });
			EndDrawing ();
		}

		public void DrawRectangle (Pen pen, float x, float y, float width, float height)
		{
			StartDrawing ();
			System.Drawing.RectangleF rect = TranslateView (new System.Drawing.RectangleF (x, y, width, height), true);
			pen.Apply (this);
			Control.StrokeRect (rect);
			EndDrawing ();
		}

		public void FillRectangle (Brush brush, float x, float y, float width, float height)
		{
			StartDrawing ();
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/

			brush.Apply (this);
			Control.FillRect (TranslateView (new SD.RectangleF (x, y, width, height), true, true));
			EndDrawing ();
		}

		public void DrawEllipse (Pen pen, float x, float y, float width, float height)
		{
			StartDrawing ();
			System.Drawing.RectangleF rect = TranslateView (new System.Drawing.RectangleF (x, y, width, height), true);
			pen.Apply (this);
			Control.StrokeEllipseInRect (rect);
			EndDrawing ();
		}

		public void FillEllipse (Brush brush, float x, float y, float width, float height)
		{
			StartDrawing ();
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/

			brush.Apply (this);
			Control.FillEllipseInRect (TranslateView (new SD.RectangleF (x, y, width, height), true, true));
			EndDrawing ();
		}

		public void DrawArc (Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			StartDrawing ();

			var rect = TranslateView (new System.Drawing.RectangleF (x, y, width, height), true);
			pen.Apply (this);
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			Control.ConcatCTM (new CGAffineTransform (1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.AddArc (rect.GetMidX(), centerY, rect.Width / 2, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle), sweepAngle < 0);
			Control.StrokePath ();
			EndDrawing ();
		}

		public void FillPie (Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			StartDrawing ();

			var rect = TranslateView (new System.Drawing.RectangleF (x, y, width, height), true, true);
			brush.Apply (this);
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			Control.ConcatCTM (new CGAffineTransform (1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.MoveTo (rect.GetMidX(), centerY);
			Control.AddArc (rect.GetMidX(), centerY, rect.Width / 2, Conversions.DegreesToRadians (startAngle), Conversions.DegreesToRadians (startAngle + sweepAngle), sweepAngle < 0);
			Control.AddLineToPoint (rect.GetMidX(), centerY);
			Control.ClosePath ();
			Control.FillPath ();
			EndDrawing ();
		}

		public void FillPath (Brush brush, IGraphicsPath path)
		{
			StartDrawing ();

			Control.TranslateCTM (inverseoffset, inverseoffset);
			Control.BeginPath ();
			Control.AddPath (path.ToCG ());
			Control.ClosePath ();
			brush.Apply (this);
			switch (path.FillMode)
			{
			case FillMode.Alternate:
				Control.EOFillPath ();
				break;
			case FillMode.Winding:
				Control.FillPath ();
				break;
			default:
				throw new NotSupportedException ();
			}
			EndDrawing ();
		}

		public void DrawPath (Pen pen, IGraphicsPath path)
		{
			StartDrawing ();
			
			Control.TranslateCTM (offset, offset);
			pen.Apply (this);
			Control.BeginPath ();
			Control.AddPath (path.ToCG ());
			Control.StrokePath ();
			
			EndDrawing ();
		}

		public void DrawImage (Image image, float x, float y)
		{
			StartDrawing ();

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, x, y);
			EndDrawing ();
		}

		public void DrawImage (Image image, float x, float y, float width, float height)
		{
			StartDrawing ();

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, x, y, width, height);
			EndDrawing ();
		}

		public void DrawImage (Image image, RectangleF source, RectangleF destination)
		{
			StartDrawing ();

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, source, destination);
			EndDrawing ();
		}

		public void DrawText(Font font, Color color, float x, float y, string text)
		{
			if (string.IsNullOrEmpty(text)) return;

			StartDrawing ();
#if OSX
			var nsfont = FontHandler.GetControl (font);
			var str = new NSString (text);
			var dic = new NSMutableDictionary ();
			dic.Add (NSAttributedString.ForegroundColorAttributeName, color.ToNS ());
			dic.Add (NSAttributedString.FontAttributeName, nsfont);
			//context.SetShouldAntialias(true);
			if (!Flipped) {
				var size = str.StringSize (dic);
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));
				y = ViewHeight - y - size.Height;
			}
			str.DrawString (TranslateView (new SD.PointF(x, y)), dic);
			//context.SetShouldAntialias(antialias);
#elif IOS
			var uifont = font.ToUI ();
			var str = new NSString (text);
			var size = str.StringSize (uifont);
			//context.SetShouldAntialias(true);
			Control.SetFillColor(color.ToCGColor ());
			str.DrawString (TranslateView (new SD.PointF(x, y), elementHeight: size.Height), uifont);
#endif

			EndDrawing ();
		}

		public SizeF MeasureString (Font font, string text)
		{
			StartDrawing ();
#if OSX
			var fontHandler = font.Handler as FontHandler;
			var dic = new NSMutableDictionary ();
			dic.Add (NSAttributedString.FontAttributeName, fontHandler.Control);
			var str = new NSString (text);
			var size = str.StringSize (dic);
#elif IOS
			var str = new NSString (text);
			var size = str.StringSize (font.ToUI ());
#endif
			EndDrawing ();
			return Eto.Platform.Conversions.ToEto (size);
		}

#if OSX
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				ReverseClip ();
				if (graphicsContext != null)
					graphicsContext.FlushGraphics ();
			}
			base.Dispose (disposing);
		}
#endif
		public void TranslateTransform (float offsetX, float offsetY)
		{
			Control.TranslateCTM (offsetX, offsetY);
		}
		
		public void RotateTransform (float angle)
		{
			angle = Conversions.DegreesToRadians (angle);
			Control.RotateCTM (angle);
		}
		
		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.ScaleCTM(scaleX, scaleY);
		}
		
		public void MultiplyTransform (IMatrix matrix)
		{
			Control.ConcatCTM (matrix.ToCG ());
		}

		public void SaveTransform ()
		{
			ReverseClip ();
			Control.SaveState ();
			RestoreClip ();
		}
		
		public void RestoreTransform ()
		{
			ReverseClip ();
			Control.RestoreState ();
			RestoreClip ();
		}

		public RectangleF ClipBounds
		{
			get { return Platform.Conversions.ToEto (Control.GetClipBoundingBox ()); }
		}

		public void SetClip (RectangleF rectangle)
		{
			ResetClip ();
			clipBounds = TranslateView (rectangle.ToSD ());
			RestoreClip ();
		}

		public void SetClip (IGraphicsPath path)
		{
			ResetClip ();
			clipPath = path.Clone ();
			RestoreClip ();
		}

		void RestoreClip ()
		{
			if (clipPath != null) {
				Control.SaveState ();
				Control.AddPath (clipPath.ToCG ());
				switch (clipPath.FillMode)
				{
				case FillMode.Alternate:
					Control.EOClip ();
					break;
				case FillMode.Winding:
					Control.Clip ();
					break;
				default:
					throw new NotSupportedException ();
				}
			} else if (clipBounds != null) {
				Control.SaveState ();
				Control.ClipToRect (clipBounds.Value);
			}
		}

		void ReverseClip ()
		{
			if (clipBounds != null || clipPath != null) {
				Control.RestoreState ();
			}
		}

		public void ResetClip ()
		{
			ReverseClip ();
			clipBounds = null;
			clipPath = null;
		}

		public void Clear (SolidBrush brush)
		{
			var rect = Control.GetClipBoundingBox ();
			this.Control.ClearRect (rect);
			if (brush != null)
				this.FillRectangle (brush, rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}
