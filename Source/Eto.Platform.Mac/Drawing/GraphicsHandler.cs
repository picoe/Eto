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
		
		public bool Flipped {
			get;
			set;
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
			Control.SaveState ();
			Control.ClipToRect (view.ConvertRectToView (view.VisibleRect (), null));
			AddObserver (NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) { 
				var handler = e.Widget.Handler as GraphicsHandler;
				var innerview = handler.view;
				var innercontext = handler.Control;
				innercontext.RestoreState ();
				innercontext.ClipToRect (innerview.ConvertRectToView (innerview.VisibleRect (), null));
				innercontext.SaveState ();
			}, view);
			this.Flipped = view.IsFlipped;
#elif IOS
			this.Control = UIGraphics.GetCurrentContext ();
			this.Flipped = !view.Layer.GeometryFlipped;
#endif
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning (false);
		}

#if OSX
		public GraphicsHandler (NSGraphicsContext graphicsContext, float height)
		{ 
			this.height = height;
			this.Flipped = flipped;
			this.graphicsContext = graphicsContext;
			this.Control = graphicsContext.GraphicsPort;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning (false);
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
		}

#endif
		
        public bool IsRetainedMode { get { return false; } }

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
		
		float ViewHeight {
			get {
				if (view != null)
					return view.Bounds.Height;
				else
					return this.height;
			}
		}

		public SD.PointF TranslateView (SD.PointF point, bool halfers = false)
		{
			if (halfers) {
				point.X += 0.5F;
				point.Y += 0.5F;
			}
			if (view != null) {
				if (!Flipped)
					point.Y = view.Bounds.Height - point.Y;
				point = view.ConvertPointToView (point, null);
			} else if (!Flipped)
				point.Y = this.height - point.Y;
			return point;
		}
		
		public SD.RectangleF TranslateView (SD.RectangleF rect, bool halfers = false)
		{
			if (halfers) {
				rect.X += 0.5F;
				rect.Y += 0.5F;
				rect.Width -= 0.5F;
				rect.Height -= 0.5F;
			}

			if (view != null) {
				if (!Flipped)
					rect.Y = view.Bounds.Height - rect.Y - rect.Height;
				rect = view.ConvertRectToView (rect, null);
			} else if (!Flipped)
				rect.Y = this.height - rect.Y - rect.Height;
			return rect;
		}

		public SD.RectangleF Translate (SD.RectangleF rect, float height)
		{
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
#endif
		}
		
		void EndDrawing ()
		{
#if OSX
			NSGraphicsContext.GlobalRestoreGraphicsState ();
#elif IOS
			UIGraphics.PopContext ();
#endif
		}
		
		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			StartDrawing ();
			if (startx == endx && starty == endy) {
				// drawing a one pixel line in retina display draws more than just one pixel
				DrawRectangle (color, startx, starty, 1, 1);
				return;
			}
			Control.SetStrokeColor (color.ToCGColor ());
			Control.SetLineCap (CGLineCap.Square);
			Control.SetLineWidth (1.0F);
			Control.StrokeLineSegments (new SD.PointF[] { TranslateView (new SD.PointF (startx, starty), true), TranslateView (new SD.PointF (endx, endy), true) });
			EndDrawing ();
		}

        public void DrawLine(Pen pen, PointF pt1, PointF pt2)
        {
            if (pen != null)
            {
                DrawLine(
                    pen.Color,
                    (int)pt1.X,
                    (int)pt1.Y,
                    (int)pt2.X,
                    (int)pt2.Y); // BUGBUG: fix floats, pen width, brush
            }                
        }

        public void DrawRectangle(Color color, int x, int y, int width, int height)
        {
            DrawRectangle(color, (float)x, (float)y, (float)width, (float)height);
        }

		public void DrawRectangle (Color color, float x, float y, float width, float height)
		{
			StartDrawing ();
			System.Drawing.RectangleF rect = TranslateView (new System.Drawing.RectangleF (x, y, width, height));
			rect.Offset (0.5f, 0.5f);
			rect.Width -= 1f;
			rect.Height -= 1f;
			Control.SetStrokeColor (color.ToCGColor ());
			Control.SetLineCap (CGLineCap.Square);
			Control.SetLineWidth (1.0F);
			Control.StrokeRect (rect);
			EndDrawing ();
		}

        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            /* TODO: other pen attributes */
            if (pen != null)
                DrawRectangle(pen.Color, x, y, width, height);
        }

		public void FillRectangle (Color color, float x, float y, float width, float height)
		{
			StartDrawing ();
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/
			
			Control.SetFillColor (color.ToCGColor ());
			Control.FillRect (TranslateView (new SD.RectangleF (x, y, width, height)));
			EndDrawing ();
		}

		public void DrawEllipse (Color color, int x, int y, int width, int height)
		{
			StartDrawing ();
			System.Drawing.RectangleF rect = TranslateView (new System.Drawing.RectangleF (x, y, width, height));
			rect.Offset (0.5f, 0.5f);
			rect.Width -= 1f;
			rect.Height -= 1f;
			Control.SetStrokeColor (color.ToCGColor ());
			Control.SetLineCap (CGLineCap.Square);
			Control.SetLineWidth (1.0F);
			Control.StrokeEllipseInRect (rect);
			EndDrawing ();
		}

		public void FillEllipse (Color color, int x, int y, int width, int height)
		{
			StartDrawing ();
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/

			Control.SetFillColor (color.ToCGColor ());
			Control.FillEllipseInRect (TranslateView (new SD.RectangleF (x, y, width, height)));
			EndDrawing ();
		}

        public void FillRectangle(Brush brush, RectangleF Rectangle)
        {
            /* TODO */
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            /* TODO */
        }

		public void FillPath (Color color, GraphicsPath path)
		{
			StartDrawing ();

			if (!Flipped)
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));
			Control.BeginPath ();
			Control.AddPath (path.ControlObject as CGPath);
			Control.ClosePath ();
			Control.SetFillColor (color.ToCGColor ());
			Control.FillPath ();
			EndDrawing ();
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			StartDrawing ();
			
			if (!Flipped)
				Control.ConcatCTM (new CGAffineTransform (1, 0, 0, -1, 0, ViewHeight));
			Control.SetLineCap (CGLineCap.Square);
			Control.SetLineWidth (1.0F);
			Control.BeginPath ();
			Control.AddPath (((GraphicsPathHandler)path.Handler).Control);
			Control.SetStrokeColor (color.ToCGColor ());
			Control.StrokePath ();
			
			EndDrawing ();
		}

        public void DrawPath(Pen pen, GraphicsPath path)
        {
            if (pen != null &&
                path != null)
            {
                DrawPath(
                    pen.Color,
                    path); /* TODO: Width, brush, etc*/
            }
        }

		public void DrawImage (Image image, int x, int y)
		{
			StartDrawing ();

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, x, y);
			EndDrawing ();
		}

		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			StartDrawing ();

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, x, y, width, height);
			EndDrawing ();
		}

		public void DrawImage (Image image, Rectangle source, Rectangle destination)
		{
			StartDrawing ();

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, source, destination);
			EndDrawing ();
		}

        public void DrawImage(Image image, PointF p)
        {
            StartDrawing();

            var handler = image.Handler as IImageHandler;
            handler.DrawImage(this, (int)p.X, (int)p.Y);
            EndDrawing();
        }

        public void DrawImage(Image image, RectangleF rect)
        {
            StartDrawing();

            var r = rect.ToRectangle();

            var handler = image.Handler as IImageHandler;
            handler.DrawImage(this, r.X, r.Y, r.Width, r.Height);
            EndDrawing();
        }

        public void DrawImage(Image image, float x, float y, float width, float height)
        {
            DrawImage(image, new RectangleF(x, y, width, height));
        }

        public void DrawImage(Image image, RectangleF source, RectangleF destination)
        {
            // BUGBUG: Fix integer conversion
            DrawImage(
                image,
                source.ToRectangle(),
                destination.ToRectangle());
        }

        public void DrawIcon(Icon icon, int x, int y, int width, int height)
		{
			StartDrawing ();

#if OSX
			var nsimage = icon.ControlObject as NSImage;
			var sourceRect = Translate (new SD.RectangleF (0, 0, nsimage.Size.Width, nsimage.Size.Height), nsimage.Size.Height);
			var destRect = TranslateView (new SD.RectangleF (x, y, width, height), false);
			nsimage.Draw (destRect, sourceRect, NSCompositingOperation.Copy, 1);
#elif IOS
			var nsimage = icon.ControlObject as UIImage;
			var destRect = this.TranslateView (new SD.RectangleF (x, y, width, height), false);
			nsimage.Draw (destRect, CGBlendMode.Copy, 1);
#endif
			
			EndDrawing ();
		}

        public void DrawText(Font font, Color color, float x, float y, string text)
		{
			StartDrawing ();

#if OSX
			var str = new NSString (text);
			var fontHandler = font.Handler as FontHandler;
			var dic = new NSMutableDictionary ();
			dic.Add (NSAttributedString.ForegroundColorAttributeName, color.ToNS ());
			dic.Add (NSAttributedString.FontAttributeName, fontHandler.Control);
			var size = str.StringSize (dic);
			//context.SetShouldAntialias(true);
			str.DrawString (new SD.PointF (x, height - y - size.Height), dic);
			//context.SetShouldAntialias(antialias);
#elif IOS
			var uifont = font.ToUI ();
			var str = new NSString (text);
			var size = str.StringSize (uifont);
			//context.SetShouldAntialias(true);
			Control.SetFillColor(color.ToCGColor ());
			var pt = !Flipped ? new SD.PointF (x, height - y - size.Height) : new SD.PointF(x, y);
			str.DrawString (pt, uifont);
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
				if (graphicsContext != null)
					graphicsContext.FlushGraphics ();
			}
			base.Dispose (disposing);
		}
#endif

        public void SetClip(RectangleF rect)
        {
            /* TODO */
        }

        public void FillPath(Brush brush, GraphicsPath path)
        {
            if (brush != null &&
                path != null)
            {
                var h = brush.Handler as BrushHandler;
                if (h != null)
                    FillPath(
                        h.Color,
                        path);
            }
        }

        public RectangleF ClipBounds
        {
            get { return Generator.Convert(context.GetClipBoundingBox()); }
        }

        public Matrix Transform
        {
            get
            {
                return Generator.Convert(context.GetCTM());
            }
            set
            {
                // BUGBUG: TODO: very inefficient!
                // see http://stackoverflow.com/questions/469505
                // get the current CTM


                var temp = context.GetCTM();

                var inverse = temp.Invert();

                context.ConcatCTM(inverse);

                var v = Generator.Convert(value);

                context.ConcatCTM(v);
            }
        }

        public void TranslateTransform(float dx, float dy)
        {
            context.TranslateCTM(
                dx, 
                // TODO: is this correct?
                Flipped
                ? dy
                : -dy); 
        }

        public void RotateTransform(float angle)
        {
            context.RotateCTM(angle);
        }

        public void ScaleTransform(float sx, float sy)
        {
            context.ScaleCTM(sx, sy);
        }

        public void MultiplyTransform(Matrix matrix)
        {
            context.ConcatCTM((CGAffineTransform)matrix.ControlObject);
        }


        public void SaveTransform()
        {
            Control.SaveGraphicsState();
        }

        public void RestoreTransform()
        {
            Control.RestoreGraphicsState();
        }
    }
}
