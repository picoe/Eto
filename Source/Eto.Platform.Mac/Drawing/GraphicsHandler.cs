using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using MonoMac.CoreGraphics;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Drawing
{

	public class RegionHandler : Region
	{
		//Gdk.Region region;
		//Gdk.Region original;
		public RegionHandler()
		{
			//this.original = region;
			//this.region = region.Copy();
		}

		public override object ControlObject
		{
			get { return null; }
		}

		public override void Exclude(Rectangle rect)
		{
			//Gdk.Region r = new Gdk.Region();
			//r.UnionWithRect(Generator.Convert(rect));
			//region.Subtract(r);
		}

		public override void Reset()
		{
			//region = original;
		}

		public override void Set(Rectangle rect)
		{
			//region.Empty();
			//region.UnionWithRect(Generator.Convert(rect));
		}
	}

	public class GraphicsHandler : MacObject<NSGraphicsContext, Graphics>, IGraphics
	{
		CGContext context;
		NSView view;
		float height;
		bool needsLock;
		
		bool Flipped
		{
			get;
			set;
		}

		public GraphicsHandler()
		{
		}

		public GraphicsHandler(NSView view)
		{
			this.view = view;
			this.needsLock = true;
			this.Control = NSGraphicsContext.FromWindow(view.Window);
			this.context = this.Control.GraphicsPort;
			context.SaveState();
			context.ClipToRect(view.ConvertRectToBase(view.VisibleRect ()));
			AddObserver(NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) { 
				var handler = e.Widget.Handler as GraphicsHandler;
				var innerview = handler.view;
				var innercontext = handler.Control.GraphicsPort;
				innercontext.RestoreState();
				innercontext.ClipToRect(innerview.ConvertRectToBase(innerview.VisibleRect ()));
				innercontext.SaveState();
			}, view);
			this.Flipped = view.IsFlipped;
			context.InterpolationQuality = CGInterpolationQuality.High;
			context.SetAllowsSubpixelPositioning(false);
		}

		public GraphicsHandler(NSGraphicsContext gc, float height)
		{
			this.height = height;
			this.Flipped = false;
			this.Control = gc;
			this.context = gc.GraphicsPort;
			context.InterpolationQuality = CGInterpolationQuality.High;
			context.SetAllowsSubpixelPositioning(false);
		}
		
		bool antialias;
		public bool Antialias {
			get {
				return antialias;
			}
			set {
				antialias = value;
				context.SetShouldAntialias(value);
			}
		}


		public void CreateFromImage(Bitmap image)
		{
			NSImage nsimage = (NSImage)image.ControlObject;
			
			var rep = nsimage.Representations().OfType<NSBitmapImageRep>().FirstOrDefault();
			Control = NSGraphicsContext.FromBitmap(rep);
			context = Control.GraphicsPort;
			this.Flipped = false;
			this.height = image.Size.Height;
			context.InterpolationQuality = CGInterpolationQuality.High;
			context.SetAllowsSubpixelPositioning(false);
		}

		public void Commit()
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
		
		public void Flush()
		{
			if (view != null && !needsLock)
			{
				//view.UnlockFocus ();
				needsLock = true;
			}
			Control.FlushGraphics();
		}
		
		float ViewHeight
		{
			get
			{
				if (view != null) return view.Bounds.Height;
				else return this.height;
			}
		}

		public SD.PointF TranslateView(SD.PointF point, bool halfers = false)
		{
			if (halfers) {
				point.X += 0.5F;
				point.Y += 0.5F;
			}
			if (view != null) 
			{
				if (!Flipped) point.Y = view.Bounds.Height - point.Y;
				point = view.ConvertPointToBase(point);
			}
			else if (!Flipped) point.Y = this.height - point.Y;
			return point;
		}
		
		public SD.RectangleF TranslateView(SD.RectangleF rect, bool halfers = false)
		{
			if (halfers) {
				rect.X += 0.5F;
				rect.Y += 0.5F;
				rect.Width -= 0.5F;
				rect.Height -= 0.5F;
			}

			if (view != null) {
				if (!Flipped) rect.Y = view.Bounds.Height - rect.Y - rect.Height;
				rect = view.ConvertRectToBase(rect);	
			}
			else if (!Flipped) rect.Y = this.height - rect.Y - rect.Height;
			return rect;
		}

		public SD.RectangleF Translate(SD.RectangleF rect, float height)
		{
			rect.Y = height - rect.Y - rect.Height;
			return rect;
		}
		void Lock()
		{
			if (needsLock && view != null)
			{
				//view.LockFocus();
				needsLock = false;
			}
		}
		
		
		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;
			context.SetStrokeColor(Generator.Convert(color));
			context.SetLineCap(CGLineCap.Square);
			context.SetLineWidth(1.0F);
			context.StrokeLineSegments(new SD.PointF[] { TranslateView(new SD.PointF(startx, starty), true), TranslateView(new SD.PointF(endx, endy), true) });
		}

		public void DrawRectangle(Color color, int x, int y, int width, int height)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;
			System.Drawing.RectangleF rect = TranslateView(new System.Drawing.RectangleF(x, y, width, height));
			rect.Offset (0.5f, 0.5f);
			rect.Width -= 1f;
			rect.Height -= 1f;
			context.SetStrokeColor(Generator.Convert(color));
			context.SetLineCap(CGLineCap.Square);
			context.SetLineWidth(1.0F);
			context.StrokeRect(rect);
		}

		public void FillRectangle(Color color, int x, int y, int width, int height)
		{
			Lock ();
		/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/
			
			NSGraphicsContext.CurrentContext = this.Control;
			context.SetFillColor(Generator.Convert(color));
			context.FillRect(TranslateView(new SD.RectangleF(x, y, width, height)));
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;

			if (!Flipped) context.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, ViewHeight));
			context.BeginPath ();
			context.AddPath (path.ControlObject as CGPath);
			context.ClosePath ();
			context.SetFillColor(Generator.Convert(color));
			context.FillPath ();
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;
			
			if (!Flipped) context.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, ViewHeight));
			context.BeginPath ();
			context.AddPath (((GraphicsPathHandler)path.Handler).Control);
			context.SetStrokeColor(Generator.Convert(color));
			context.StrokePath ();
		}
		
		public void DrawImage(IImage image, int x, int y)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;

			var handler = image.Handler as IImageHandler;
			handler.DrawImage(this, x, y);
		}

		public void DrawImage(IImage image, int x, int y, int width, int height)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;

			var handler = image.Handler as IImageHandler;
			handler.DrawImage(this, x, y, width, height);
		}

		public void DrawImage(IImage image, Rectangle source, Rectangle destination)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;

			var handler = image.Handler as IImageHandler;
			handler.DrawImage(this, source, destination);
		}

		public void DrawIcon(Icon icon, int x, int y, int width, int height)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = this.Control;

			var nsimage = icon.ControlObject as NSImage;
			var sourceRect = Translate(new SD.RectangleF(0, 0, nsimage.Size.Width, nsimage.Size.Height), nsimage.Size.Height);
			var destRect = TranslateView(new SD.RectangleF(x, y, width, height), false);
			nsimage.Draw(destRect, sourceRect, NSCompositingOperation.Copy, 1);
		}

		public Region ClipRegion
		{
			get { return null; } //new RegionHandler(drawable.ClipRegion); }
			set
			{
				//gc.ClipRegion = (Gdk.Region)((RegionHandler)value).ControlObject;
			}
		}

		public void DrawText(Font font, Color color, int x, int y, string text)
		{
			Lock ();
			NSGraphicsContext.CurrentContext = Control;
			
			var str = new NSString(text);
			var fontHandler = font.Handler as FontHandler;
			var dic = new NSMutableDictionary();
			dic.Add(NSAttributedString.ForegroundColorAttributeName, Generator.ConvertNS(color));
			dic.Add(NSAttributedString.FontAttributeName, fontHandler.Control);
			var size = str.StringSize(dic);
			//context.SetShouldAntialias(true);
			str.DrawString(new SD.PointF(x, height - y - size.Height), dic);
			//context.SetShouldAntialias(antialias);
		}

		public SizeF MeasureString(Font font, string text)
		{
			NSGraphicsContext.CurrentContext = Control;
			var fontHandler = font.Handler as FontHandler;
			var dic = new NSMutableDictionary();
			dic.Add(NSAttributedString.FontAttributeName, fontHandler.Control);
			var str = new NSString(text);
			var size = str.StringSize(dic);
			return new SizeF(size.Width, size.Height);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (Control != null) Control.FlushGraphics();
			}
			base.Dispose (disposing);
		}

	}
}
