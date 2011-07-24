using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	public class RegionHandler : Region
	{
		Gdk.Region region;
		Gdk.Region original;

		public RegionHandler (Gdk.Region region)
		{
			this.original = region;
			this.region = region.Copy ();
		}

		public override object ControlObject {
			get { return region; }
		}

		public override void Exclude (Rectangle rect)
		{
			Gdk.Region r = new Gdk.Region ();
			r.UnionWithRect (Generator.Convert (rect));
			region.Subtract (r);
		}

		public override void Reset ()
		{
			region = original;
		}

		public override void Set (Rectangle rect)
		{
			region.Empty ();
			region.UnionWithRect (Generator.Convert (rect));
		}
	}

	public class GraphicsHandler : WidgetHandler<Gdk.Drawable, Graphics>, IGraphics
	{
		Gtk.Widget widget;
		Gdk.GC gc;
		Cairo.Context context;
		bool deletegc = true;
		IImage image;

		public GraphicsHandler ()
		{
		}
		
		public GraphicsHandler (Gtk.Widget widget, Gdk.Drawable drawable, Gdk.GC gc)
		{
			this.widget = widget;
			this.Control = drawable;
			this.DisposeControl = false;
			this.gc = gc;
			this.context = Gdk.CairoHelper.Create (drawable);
			deletegc = false;
		}

		public bool Antialias
		{
			get { return context.Antialias != Cairo.Antialias.None; }
			set {
				if (value) context.Antialias = Cairo.Antialias.Default;
				else context.Antialias = Cairo.Antialias.None;
			}
		}

		public Gdk.GC GC {
			get { return gc; }
		}
		
		public Cairo.Context Context {
			get { return context; }
		}

		public void CreateFromImage (Bitmap image)
		{
			this.image = image;
			var active = Gdk.Screen.Default.ActiveWindow;
			Control = new Gdk.Pixmap (active, image.Size.Width, image.Size.Height, active != null ? -1 : 24);
			if (Control.Colormap == null) 
				Control.Colormap = new Gdk.Colormap (Gdk.Visual.System, true);
			gc = new Gdk.GC (Control);
			this.context = Gdk.CairoHelper.Create (Control);
		}

		public void Flush ()
		{
			if (image != null) {
				Gdk.Pixbuf pb = (Gdk.Pixbuf)image.ControlObject;
				pb.GetFromDrawable (Control, Control.Colormap, 0, 0, 0, 0, image.Size.Width, image.Size.Height);
			}
			//context.Paint ();
			//GdkHandler.Global.Flush ();
		}

		public void DrawLine (Color color, int startx, int starty, int endx, int endy)
		{
#if CAIRO
			context.Save ();
			context.Color = Generator.ConvertC(color);
			if (startx != endx || starty != endy) {
				// to draw a line, it must move..
				context.MoveTo (startx+0.5, starty+0.5);
				context.LineTo (endx+0.5, endy+0.5);
				context.LineCap = Cairo.LineCap.Square;
				context.LineWidth = 1.0;
				context.Stroke ();
			}
			else {
				// to draw one pixel, we must fill it
				context.Rectangle (startx, starty, 1, 1);
				context.Fill();
			}
			context.Restore ();
#else
			gc.RgbFgColor = Generator.Convert (color);
			Control.DrawLine (gc, startx, starty, endx, endy);
#endif
		}

		public void DrawRectangle (Color color, int x, int y, int width, int height)
		{
#if CAIRO
			context.Save ();
			context.Color = Generator.ConvertC(color);
			context.Rectangle (x+0.5, y+0.5, width-1, height-1);
			context.LineWidth = 1.0;
			context.Stroke ();
			context.Restore ();
#else
			gc.RgbFgColor = Generator.Convert (color);
			Control.DrawRectangle (gc, false, x, y, width + 1, height + 1);
#endif
		}

		public void FillRectangle (Color color, int x, int y, int width, int height)
		{
#if CAIRO
			context.Save ();
			context.Color = Generator.ConvertC(color);
			context.Rectangle (x, y, width, height);
			context.Fill ();
			context.Restore ();
#else
			gc.RgbBgColor = Generator.Convert (color);
			gc.RgbFgColor = Generator.Convert (color);
			Control.DrawRectangle (gc, true, x, y, width, height);
#endif
		}
		
		public void FillPath (Color color, GraphicsPath path)
		{
#if CAIRO
			context.Save ();
			context.Color = Generator.ConvertC(color);
			var pathHandler = path.Handler as GraphicsPathHandler;
			pathHandler.Apply (this);
			context.Fill ();
			context.Restore ();
#endif
		}

		public void DrawPath (Color color, GraphicsPath path)
		{
#if CAIRO
			context.Save ();
			context.Color = Generator.ConvertC(color);
			var pathHandler = path.Handler as GraphicsPathHandler;
			pathHandler.Apply (this);
			context.Stroke ();
			context.Restore ();
#endif
		}
		
		public void DrawImage (IImage image, int x, int y)
		{
			((IImageHandler)image.Handler).DrawImage (this, x, y);
		}

		public void DrawImage (IImage image, int x, int y, int width, int height)
		{
			((IImageHandler)image.Handler).DrawImage (this, x, y, width, height);
		}

		public void DrawImage (IImage image, Rectangle source, Rectangle destination)
		{
			((IImageHandler)image.Handler).DrawImage (this, source, destination);
		}

		public void DrawIcon (Icon icon, int x, int y, int width, int height)
		{
			((Gdk.Pixbuf)icon.ControlObject).RenderToDrawableAlpha (Control, 0, 0, x, y, width, height, Gdk.PixbufAlphaMode.Bilevel, 0, Gdk.RgbDither.Normal, 0, 0);
		}

		public Region ClipRegion {
			get { return new RegionHandler (Control.ClipRegion); }
			set {
				gc.ClipRegion = (Gdk.Region)((RegionHandler)value).ControlObject;
			}
		}

		public void DrawText (Font font, Color color, int x, int y, string text)
		{
			if (widget != null) {
				Pango.Layout layout = new Pango.Layout (widget.PangoContext);
				layout.FontDescription = (Pango.FontDescription)font.ControlObject;
				layout.SetText (text);
				gc.RgbFgColor = Generator.Convert (color);
				//layout.Wrap = Pango.WrapMode.
				Control.DrawLayout (gc, x, y, layout);
				layout.Dispose ();
			}
		}

		public SizeF MeasureString (Font font, string text)
		{
			if (widget != null) {
				Pango.Layout layout = new Pango.Layout (widget.PangoContext);
				layout.FontDescription = (Pango.FontDescription)font.ControlObject;
				layout.SetText (text);
				int width, height;
				layout.GetPixelSize (out width, out height);
				layout.Dispose ();
				return new SizeF (width, height);
			}
			return new SizeF ();
		}

		protected override void Dispose (bool disposing)
		{
			if (image != null) Flush();

			if (disposing) {
				if (gc != null && deletegc) {
					gc.Dispose ();
					gc = null;
				}
				if (context != null) {
					((IDisposable) context).Dispose();
					context = null;
				}
			}
			
			base.Dispose (disposing);
		}
	}
}
