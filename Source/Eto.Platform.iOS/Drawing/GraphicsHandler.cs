using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Drawing
{
	public class GraphicsHandler : WidgetHandler<object, Graphics>, IGraphics
	{
		CGContext context;
		UIView view;
		float height;
		bool clean;
		
		public CGContext Context { get { return context; } }
		
		public bool Flipped {
			get;
			private set;
		}

		public GraphicsHandler ()
		{
		}
		
		bool antialias = true;
		
		public bool Antialias {
			get {
				return antialias;
			}
			set {
				antialias = value;
				context.SetShouldAntialias (antialias);
			}
		}

		public ImageInterpolation ImageInterpolation {
			get { return Generator.ConvertCG (context.InterpolationQuality); }
			set { context.InterpolationQuality = Generator.ConvertCG (value); }
		}

		public GraphicsHandler (UIView view)
		{
			this.view = view;
			//this.Control = view;
			this.context = UIGraphics.GetCurrentContext ();
			this.Flipped = !view.Layer.GeometryFlipped;
			context.InterpolationQuality = CGInterpolationQuality.High;
			context.SetAllowsSubpixelPositioning (false);
		}

		public GraphicsHandler (object gc, float height, bool flipped)
		{
			this.height = height;
			if (height > 0) {
				this.Flipped = flipped;
			}
			//this.Control = gc;
			this.context = UIGraphics.GetCurrentContext ();
			context.InterpolationQuality = CGInterpolationQuality.High;
			//context.ScaleCTM(1, -1);
			context.SetAllowsSubpixelPositioning (false);
		}

		public void CreateFromImage (Bitmap image)
		{
			var handler = image.Handler as BitmapHandler;
			var uiimage = (UIImage)image.ControlObject;
			var cgimage = uiimage.CGImage;
			//var rep = nsimage.Representations().OfType<NSBitmapImageRep>().FirstOrDefault();
			/*adjust = true;
			Flipped = true;
			this.height = cgimage.Height;*/
			context = new CGBitmapContext (handler.Data.MutableBytes, cgimage.Width, cgimage.Height, cgimage.BitsPerComponent, cgimage.BytesPerRow, cgimage.ColorSpace, cgimage.BitmapInfo);
			clean = true;
			context.InterpolationQuality = CGInterpolationQuality.High;
			context.SetAllowsSubpixelPositioning (false);
		}

		public void Flush ()
		{
			context.Flush ();
		}

		public SD.PointF TranslateView (SD.PointF point, bool halfers = false)
		{
			if (halfers) {
				point.X += 0.5F;
				point.Y += 0.5F;
			}
			if (view != null) {
				if (Flipped)
					point.Y = view.Bounds.Height - point.Y - 1;
				//point = view.ConvertPointToView(point);
			} else if (Flipped)
				point.Y = this.height - point.Y - 1;
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
				if (Flipped)
					rect.Y = view.Bounds.Height - rect.Y - rect.Height;
				//rect = view.ConvertRectToView(rect);	
			} else if (Flipped)
				rect.Y = this.height - rect.Y - rect.Height;
			return rect;
		}

		public SD.RectangleF Translate (SD.RectangleF rect, float height)
		{
			rect.Y = height - rect.Y - rect.Height;
			return rect;
		}
		
		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			UIGraphics.PushContext (this.context);
			context.SetStrokeColor (Generator.Convert (color));
			//context.SetShouldAntialias(false);
			context.SetLineCap (CGLineCap.Square);
			context.SetLineWidth (1.0F);
			context.StrokeLineSegments (new SD.PointF[] { TranslateView (new SD.PointF (startx, starty)), TranslateView (new SD.PointF (endx, endy)) });
			UIGraphics.PopContext ();
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
			UIGraphics.PushContext (this.context);
			var rect = new System.Drawing.RectangleF (x, y, width, height);
			context.SetStrokeColor (Generator.Convert (color));
			//context.SetShouldAntialias(false);
			context.SetLineWidth (1.0F);
			context.StrokeRect (TranslateView (rect));
			UIGraphics.PopContext ();
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/
			
			UIGraphics.PushContext (this.context);
			//this.Control.CompositingOperation = NSComposite.SourceOver;
			//this.Control.ColorRenderingIntent = NSColorRenderingIntent.Default;
			//this.context.SetFillColorSpace(CGColorSpace.CreateCalibratedRGB(new float[] { 1.0F, 1.0, 1.0 }, new float[] { 0, 0, 0 }, new float[] { 1.0F, 1.0, 1.0 }, ));
			//this.context.SetStrokeColorSpace(CGColorSpace.CreateDeviceCMYK());
			//this.context.SetAlpha(1.0F);
			context.SetFillColor (Generator.Convert (color));
			//context.SetShouldAntialias(false);
			context.FillRect (TranslateView (new SD.RectangleF (x, y, width, height)));
			UIGraphics.PopContext ();
		}

		public void DrawEllipse (Color color, int x, int y, int width, int height)
		{
			UIGraphics.PushContext (this.context);
			var rect = new System.Drawing.RectangleF (x, y, width, height);
			context.SetStrokeColor (Generator.Convert (color));
			//context.SetShouldAntialias(false);
			context.SetLineWidth (1.0F);
			context.StrokeEllipseInRect (TranslateView (rect));
			UIGraphics.PopContext ();
		}
		
		public void FillEllipse (Color color, int x, int y, int width, int height)
		{
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/
			
			UIGraphics.PushContext (this.context);
			//this.Control.CompositingOperation = NSComposite.SourceOver;
			//this.Control.ColorRenderingIntent = NSColorRenderingIntent.Default;
			//this.context.SetFillColorSpace(CGColorSpace.CreateCalibratedRGB(new float[] { 1.0F, 1.0, 1.0 }, new float[] { 0, 0, 0 }, new float[] { 1.0F, 1.0, 1.0 }, ));
			//this.context.SetStrokeColorSpace(CGColorSpace.CreateDeviceCMYK());
			//this.context.SetAlpha(1.0F);
			context.SetFillColor (Generator.Convert (color));
			//context.SetShouldAntialias(false);
			context.FillEllipseInRect (TranslateView (new SD.RectangleF (x, y, width, height)));
			UIGraphics.PopContext ();
		}

		public void FillPath (Color color, GraphicsPath path)
		{
			throw new NotImplementedException ();
		}
		
		public void DrawPath (Color color, GraphicsPath path)
		{
			throw new NotImplementedException ();
		}

		public void DrawImage (Image image, int x, int y)
		{
			UIGraphics.PushContext (this.context);

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, x, y);
			UIGraphics.PopContext ();
		}

		public void DrawImage (Image image, int x, int y, int width, int height)
		{
			UIGraphics.PushContext (this.context);

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, x, y, width, height);
			UIGraphics.PopContext ();
		}

		public void DrawImage (Image image, Rectangle source, Rectangle destination)
		{
			UIGraphics.PushContext (this.context);

			var handler = image.Handler as IImageHandler;
			handler.DrawImage (this, source, destination);
			UIGraphics.PopContext ();
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			UIGraphics.PushContext (this.context);

			var nsimage = icon.ControlObject as UIImage;
			var destRect = this.TranslateView (new SD.RectangleF (x, y, width, height), false);
			nsimage.Draw (destRect, CGBlendMode.Copy, 1);
			
			UIGraphics.PopContext ();
		}

		public Region ClipRegion {
			get { return null; } //new RegionHandler(drawable.ClipRegion); }
			set {
				//gc.ClipRegion = (Gdk.Region)((RegionHandler)value).ControlObject;
			}
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			UIGraphics.PushContext (this.context);
			
			var str = new NSString (text);
			var size = str.StringSize (font.ControlObject as UIFont);
			//context.SetShouldAntialias(true);
			str.DrawString (new SD.PointF (x, height - y - size.Height), font.ControlObject as UIFont);
			UIGraphics.PopContext ();
		}

		public SizeF MeasureString (Font font, string text)
		{
			UIGraphics.PushContext (this.context);
			var str = new NSString (text);
			var size = str.StringSize (font.ControlObject as UIFont);
			UIGraphics.PopContext ();
			return new SizeF (size.Width, size.Height);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				//if (Control != null) Control.FlushGraphics();
			}
			if (clean && context != null) {
				context.Dispose ();
				context = null;
			}
			base.Dispose (disposing);
		}

	}
}
