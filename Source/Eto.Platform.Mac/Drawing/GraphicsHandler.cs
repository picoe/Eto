using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;


#if OSX
using Eto.Platform.Mac.Forms;
using MonoMac.CoreGraphics;
using MonoMac.AppKit;
using GraphicsBase = Eto.Platform.Mac.Forms.MacBase<MonoMac.CoreGraphics.CGContext, Eto.Drawing.Graphics>;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using Eto.Platform.iOS.Forms;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using NSView = MonoTouch.UIKit.UIView;
using GraphicsBase = Eto.WidgetHandler<MonoTouch.CoreGraphics.CGContext, Eto.Drawing.Graphics>;

namespace Eto.Platform.iOS.Drawing
#endif
{
	/// <summary>
	/// Handler for the <see cref="IGraphics"/>
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : GraphicsBase, IGraphics
	{
		#if OSX
		NSGraphicsContext graphicsContext;
		bool disposeContext;
		#endif
		readonly NSView view;
		float height;
		PixelOffsetMode pixelOffsetMode = PixelOffsetMode.None;
		float offset = 0.5f;
		float inverseoffset;
		SD.RectangleF? clipBounds;
		IGraphicsPath clipPath;
		int transformSaveCount;
		Stack<CGAffineTransform> transforms;
		CGAffineTransform currentTransform = CGAffineTransform.MakeIdentity();

		public float Offset { get { return offset; } }

		public float InverseOffset { get { return inverseoffset; } }

		public NSView DisplayView { get; private set; }

		public CGAffineTransform CurrentTransform { get { return currentTransform; } }

		public bool Flipped
		{
			get;
			set;
		}

		public PixelOffsetMode PixelOffsetMode
		{
			get { return pixelOffsetMode; }
			set
			{
				pixelOffsetMode = value;
				offset = value == PixelOffsetMode.None ? 0.5f : 0f;
				inverseoffset = value == PixelOffsetMode.None ? 0f : 0.5f;
			}
		}

		public GraphicsHandler()
		{
		}

		public GraphicsHandler(NSView view)
		{
			this.view = view;
#if OSX
			graphicsContext = NSGraphicsContext.FromWindow(view.Window);
			graphicsContext = view.IsFlipped ? graphicsContext : NSGraphicsContext.FromGraphicsPort(graphicsContext.GraphicsPortHandle, true);
			disposeContext = true;
			Control = graphicsContext.GraphicsPort;
			Flipped = view.IsFlipped;

#elif IOS
			this.Control = UIGraphics.GetCurrentContext ();
			this.Flipped = view != null && view.Layer != null &&!view.Layer.GeometryFlipped;
#endif
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning(false);
			Control.SaveState();

#if OSX

			view.PostsFrameChangedNotifications = true;
			AddObserver(NSView.FrameChangedNotification, FrameDidChange, view);

			// if control is in a scrollview, we need to trap when it's scrolled as well
			var parent = view.Superview;
			while (parent != null)
			{
				var scroll = parent as NSScrollView;
				if (scroll != null)
				{
					scroll.ContentView.PostsBoundsChangedNotifications = true;
					AddObserver(NSView.BoundsChangedNotification, FrameDidChange, scroll.ContentView);
				}
				parent = parent.Superview;
			}

			SetViewClip();
#endif
		}
		#if OSX
		static void FrameDidChange(ObserverActionEventArgs e)
		{
			//Console.WriteLine ("Woooo!");
			var h = e.Handler as GraphicsHandler;
			if (h != null && h.Control != null)
			{
				h.RewindAll();
				
				h.Control.RestoreState();
				h.Control.SaveState();
				h.SetViewClip();
				
				h.ReplayAll();
			}
		}

		void SetViewClip()
		{
			Control.ClipToRect(view.ConvertRectToView(view.VisibleRect(), null));
			var pos = view.ConvertPointToView(SD.PointF.Empty, null);
			if (!Flipped)
				pos.Y += view.Frame.Height;
			currentTransform = new CGAffineTransform(1, 0, 0, -1, pos.X, pos.Y);
			Control.ConcatCTM(currentTransform);
		}

		public GraphicsHandler(NSView view, NSGraphicsContext graphicsContext, float height, bool flipped)
		{
			this.DisplayView = view;
			this.height = height;
			this.graphicsContext = flipped ? graphicsContext : NSGraphicsContext.FromGraphicsPort(graphicsContext.GraphicsPortHandle, true);
			this.Control = this.graphicsContext.GraphicsPort;
			//this.Flipped = flipped;
			Flipped = flipped;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning(false);
			Control.SaveState();
			if (!Flipped)
				FlipDrawing();
		}
		#elif IOS
		public GraphicsHandler (NSView view, CGContext context, float height, bool flipped)
		{
			this.DisplayView = view;
			this.height = height;
			if (height > 0) {
				this.Flipped = flipped;
			}
			this.Control = context;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SaveState ();
			Control.SetAllowsSubpixelPositioning (false);
			if (!Flipped)
				FlipDrawing ();
		}

#endif
		protected override bool DisposeControl { get { return false; } }

		public bool IsRetained { get { return false; } }

		bool antialias;

		public bool AntiAlias
		{
			get
			{
				return antialias;
			}
			set
			{
				antialias = value;
				Control.SetShouldAntialias(value);
			}
		}

		public ImageInterpolation ImageInterpolation
		{
			get { return Control.InterpolationQuality.ToEto(); }
			set { Control.InterpolationQuality = value.ToCG(); }
		}

		public void CreateFromImage(Bitmap image)
		{
			var handler = image.Handler as BitmapHandler;
#if OSX
			var rep = handler.Control.Representations().OfType<NSBitmapImageRep>().FirstOrDefault();
			graphicsContext = NSGraphicsContext.FromBitmap(rep);
			disposeContext = true;
			Control = graphicsContext.GraphicsPort;
#elif IOS
			var cgimage = handler.Control.CGImage;
			Control = new CGBitmapContext (handler.Data.MutableBytes, cgimage.Width, cgimage.Height, cgimage.BitsPerComponent, cgimage.BytesPerRow, cgimage.ColorSpace, cgimage.BitmapInfo);
#endif

			Flipped = false;
			height = image.Size.Height;
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning(false);
			Control.SaveState();
			if (!Flipped)
				FlipDrawing();
		}

		public void Reset()
		{
			// unwind all SaveState's
			ResetClip();
			while (transformSaveCount > 0)
			{
				RestoreTransform();
			}
			// initial save state
			Control.RestoreState();
		}

		public void Flush()
		{
#if OSX
			graphicsContext.FlushGraphics();
#endif
		}

		public float ViewHeight
		{
			get
			{
				return view != null ? view.Bounds.Height : height;
			}
		}

		public void FlipDrawing()
		{
			var m = new CGAffineTransform(1, 0, 0, -1, 0, ViewHeight);
			Control.ConcatCTM(m);
			currentTransform.Multiply(m);
		}

		public SD.PointF TranslateView(SD.PointF point, bool halfers = false, bool inverse = false)
		{
			if (halfers)
			{
				if (inverse)
				{
					point.X += inverseoffset;
					point.Y += inverseoffset;
				}
				else
				{
					point.X += offset;
					point.Y += offset;
				}
			}
			return point;
		}

		public SD.RectangleF TranslateView(SD.RectangleF rect, bool halfers = false, bool inverse = false)
		{
			if (halfers)
			{
				if (inverse)
				{
					rect.X += inverseoffset;
					rect.Y += inverseoffset;
				}
				else
				{
					rect.X += offset;
					rect.Y += offset;
				}
			}
			return rect;
		}

		public SD.RectangleF Translate(SD.RectangleF rect, float height)
		{
			if (!Flipped)
				rect.Y = height - rect.Y - rect.Height;
			return rect;
		}

		void StartDrawing()
		{
#if OSX
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphicsContext;
			Control.SaveState();
#elif IOS
			UIGraphics.PushContext (this.Control);
			this.Control.SaveState ();
#endif
		}

		void EndDrawing()
		{
#if OSX
			Control.RestoreState();
			NSGraphicsContext.GlobalRestoreGraphicsState();
#elif IOS
			this.Control.RestoreState ();
			UIGraphics.PopContext ();
#endif
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			StartDrawing();
			pen.Apply(this);
			Control.StrokeLineSegments(new SD.PointF[]
			{
				TranslateView(new SD.PointF(startx, starty), true),
				TranslateView(new SD.PointF(endx, endy), true)
			});
			EndDrawing();
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			StartDrawing();
			var rect = TranslateView(new SD.RectangleF(x, y, width, height), true);
			pen.Apply(this);
			Control.StrokeRect(rect);
			EndDrawing();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			StartDrawing();
			brush.Apply(this);
			Control.FillRect(TranslateView(new SD.RectangleF(x, y, width, height), width > 1 || height > 1, true));
			EndDrawing();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			StartDrawing();
			System.Drawing.RectangleF rect = TranslateView(new System.Drawing.RectangleF(x, y, width, height), true);
			pen.Apply(this);
			Control.StrokeEllipseInRect(rect);
			EndDrawing();
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			StartDrawing();
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/

			brush.Apply(this);
			Control.FillEllipseInRect(TranslateView(new SD.RectangleF(x, y, width, height), true, true));
			EndDrawing();
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			StartDrawing();

			var rect = TranslateView(new System.Drawing.RectangleF(x, y, width, height), true);
			pen.Apply(this);
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			var centerX = rect.GetMidX();
			Control.ConcatCTM(new CGAffineTransform(1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.AddArc(centerX, centerY, rect.Width / 2, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle), sweepAngle < 0);
			Control.StrokePath();
			EndDrawing();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			StartDrawing();

			var rect = TranslateView(new System.Drawing.RectangleF(x, y, width, height), true, true);
			brush.Apply(this);
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			var centerX = rect.GetMidX();
			Control.ConcatCTM(new CGAffineTransform(1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.MoveTo(centerX, centerY);
			Control.AddArc(centerX, centerY, rect.Width / 2, Conversions.DegreesToRadians(startAngle), Conversions.DegreesToRadians(startAngle + sweepAngle), sweepAngle < 0);
			Control.AddLineToPoint(centerX, centerY);
			Control.ClosePath();
			Control.FillPath();
			EndDrawing();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			StartDrawing();

			Control.TranslateCTM(inverseoffset, inverseoffset);
			Control.BeginPath();
			Control.AddPath(path.ToCG());
			Control.ClosePath();
			brush.Apply(this);
			switch (path.FillMode)
			{
				case FillMode.Alternate:
					Control.EOFillPath();
					break;
				case FillMode.Winding:
					Control.FillPath();
					break;
				default:
					throw new NotSupportedException();
			}
			EndDrawing();
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			StartDrawing();
			
			Control.TranslateCTM(offset, offset);
			pen.Apply(this);
			Control.BeginPath();
			Control.AddPath(path.ToCG());
			Control.StrokePath();
			
			EndDrawing();
		}

		public void DrawImage(Image image, float x, float y)
		{
			StartDrawing();

			var handler = (IImageHandler)image.Handler;
			handler.DrawImage(this, x, y);
			EndDrawing();
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			StartDrawing();

			var handler = (IImageHandler)image.Handler;
			handler.DrawImage(this, x, y, width, height);
			EndDrawing();
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			StartDrawing();

			var handler = (IImageHandler)image.Handler;
			handler.DrawImage(this, source, destination);
			EndDrawing();
		}

		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			StartDrawing();
#if OSX
			FontExtensions.DrawString(text, new PointF(x, y), brush.Color, font);
#elif IOS
			var uifont = font.ToUI ();
			var str = new NSString (text);
			var size = str.StringSize (uifont);
			//context.SetShouldAntialias(true);
			Control.SetFillColor(brush.Color.ToCGColor());
			str.DrawString (TranslateView (new SD.PointF(x, y)), uifont);
#endif

			EndDrawing();
		}

		public SizeF MeasureString(Font font, string text)
		{
			StartDrawing();
#if OSX
			var size = FontExtensions.MeasureString(text, font);
#elif IOS
			var str = new NSString (text);
			var size = str.StringSize (font.ToUI ()).ToEto();
#endif
			EndDrawing();
			return size;
		}
		#if OSX
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (disposeContext && graphicsContext != null)
					graphicsContext.FlushGraphics();
				Reset();
				if (disposeContext && graphicsContext != null)
					graphicsContext.Dispose();
			}
			base.Dispose(disposing);
		}
		#endif
		public void TranslateTransform(float offsetX, float offsetY)
		{
			Control.TranslateCTM(offsetX, offsetY);
			currentTransform.Translate(offsetX, offsetY);
		}

		public void RotateTransform(float angle)
		{
			angle = Conversions.DegreesToRadians(angle);
			Control.RotateCTM(angle);
			currentTransform.Rotate(angle);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.ScaleCTM(scaleX, scaleY);
			currentTransform.Scale(scaleX, scaleY);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			var m = matrix.ToCG();
			Control.ConcatCTM(m);
		}

		public void SaveTransform()
		{
			RewindClip();
			Control.SaveState();
			transformSaveCount++;
			ReplayClip();
			if (transforms == null)
				transforms = new Stack<CGAffineTransform>();
			transforms.Push(currentTransform);
		}

		public void RestoreTransform()
		{
			if (transformSaveCount <= 0)
				throw new InvalidOperationException("No saved transform");
			RewindClip();
			transformSaveCount--;
			Control.RestoreState();
			ReplayClip();
			currentTransform = transforms.Pop();
		}

		public RectangleF ClipBounds
		{
			get { return Control.GetClipBoundingBox().ToEto(); }
		}

		public void SetClip(RectangleF rectangle)
		{
			ResetClip();
			clipBounds = TranslateView(rectangle.ToSD());
			ReplayClip();
		}

		public void SetClip(IGraphicsPath path)
		{
			ResetClip();
			clipPath = path.Clone();
			ReplayClip();
		}

		void ReplayClip()
		{
			if (clipPath != null)
			{
				Control.SaveState();
				Control.AddPath(clipPath.ToCG());
				switch (clipPath.FillMode)
				{
					case FillMode.Alternate:
						Control.EOClip();
						break;
					case FillMode.Winding:
						Control.Clip();
						break;
					default:
						throw new NotSupportedException();
				}
			}
			else if (clipBounds != null)
			{
				Control.SaveState();
				Control.ClipToRect(clipBounds.Value);
			}
		}

		void RewindClip()
		{
			if (clipBounds != null || clipPath != null)
			{
				Control.RestoreState();
			}
		}

		void RewindTransform()
		{
			for (int i = 0; i < transformSaveCount; i++)
				Control.RestoreState();
		}

		void ReplayTransform()
		{
			// replay transforms
			if (transforms != null)
			{
				foreach (var transform in transforms)
				{
					Control.ConcatCTM(transform);
					Control.SaveState();
				}
			}
			//Control.ConcatCTM (currentTransform);
		}

		void RewindAll()
		{
			RewindClip();
			RewindTransform();
		}

		void ReplayAll()
		{
			ReplayTransform();
			ReplayClip();
		}

		public void ResetClip()
		{
			RewindClip();
			clipBounds = null;
			clipPath = null;
		}

		public void Clear(SolidBrush brush)
		{
			var rect = Control.GetClipBoundingBox();
			Control.ClearRect(rect);
			if (brush != null)
				FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
		}
	}
}
