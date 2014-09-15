using System;
using System.Linq;
using Eto.Drawing;
using SD = System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
using MonoMac;
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

#if OSX
using Eto.Mac.Forms;

#if XAMMAC2
using GraphicsBase = Eto.Mac.Forms.MacBase<CoreGraphics.CGContext, Eto.Drawing.Graphics, Eto.Drawing.Graphics.ICallback>;
#else
using GraphicsBase = Eto.Mac.Forms.MacBase<MonoMac.CoreGraphics.CGContext, Eto.Drawing.Graphics, Eto.Drawing.Graphics.ICallback>;
#endif

namespace Eto.Mac.Drawing
#elif IOS
using Eto.iOS.Forms;
using Eto.Mac;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using NSView = MonoTouch.UIKit.UIView;
using GraphicsBase = Eto.WidgetHandler<MonoTouch.CoreGraphics.CGContext, Eto.Drawing.Graphics, Eto.Drawing.Graphics.ICallback>;

namespace Eto.iOS.Drawing
#endif
{
	#if OSX
	public static class GraphicsExtensions
	{
		[DllImport(Constants.AppKitLibrary, EntryPoint = "NSSetFocusRingStyle")]
		public extern static void SetFocusRingStyle(NSFocusRingPlacement placement);
	}
	#endif

	/// <summary>
	/// Handler for the <see cref="Graphics.IHandler"/>
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class GraphicsHandler : GraphicsBase, Graphics.IHandler
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
		CGRect? clipBounds;
		IGraphicsPath clipPath;
		int transformSaveCount;
		Stack<CGAffineTransform> transforms;
		CGAffineTransform currentTransform = CGAffineTransform.MakeIdentity();
		static readonly CGColorSpace patternColorSpace = CGColorSpace.CreatePattern(null);

		public float Offset { get { return offset; } }

		public float InverseOffset { get { return inverseoffset; } }

		public NSView DisplayView { get; private set; }

		public CGAffineTransform CurrentTransform { get { return currentTransform; } }

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

		#if OSX

		public GraphicsHandler(NSView view)
		{
			this.view = view;
			graphicsContext = NSGraphicsContext.FromWindow(view.Window);
			graphicsContext = graphicsContext.IsFlipped ? graphicsContext : NSGraphicsContext.FromGraphicsPort(graphicsContext.GraphicsPortHandle, true);
			disposeContext = true;
			Control = graphicsContext.GraphicsPort;

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

			SetDefaults();
			InitializeContext(view.IsFlipped);
		}

		static void FrameDidChange(ObserverActionEventArgs e)
		{
			var h = e.Handler as GraphicsHandler;
			if (h != null && h.Control != null)
			{
				h.RewindAll();
				
				h.Control.RestoreState();
				h.Control.SaveState();
				h.InitializeContext(h.view.IsFlipped);
				
				h.ReplayAll();
			}
		}

		public GraphicsHandler(NSView view, NSGraphicsContext graphicsContext, float height, bool flipped)
		{
			DisplayView = view;
			this.height = height;
			this.graphicsContext = flipped != graphicsContext.IsFlipped ? graphicsContext : NSGraphicsContext.FromGraphicsPort(graphicsContext.GraphicsPortHandle, true);
			Control = this.graphicsContext.GraphicsPort;
			SetDefaults();
			InitializeContext(!flipped);
		}

		#elif IOS

		public GraphicsHandler(NSView view, CGContext context, float height)
		{
			this.DisplayView = view;
			this.height = height;
			this.Control = context;

			SetDefaults();
			InitializeContext(false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		#endif
		protected override bool DisposeControl { get { return false; } }

		public bool IsRetained { get { return false; } }

		bool antialias;

		public bool AntiAlias
		{
			get { return antialias; }
			set
			{
				antialias = value;
				Control.SetShouldAntialias(value);
			}
		}

		float scale = 1f;
		public float PointsPerPixel { get { return scale; } }

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
			graphicsContext = graphicsContext.IsFlipped ? graphicsContext : NSGraphicsContext.FromGraphicsPort(graphicsContext.GraphicsPortHandle, true);
			disposeContext = true;
			Control = graphicsContext.GraphicsPort;
			scale = (float)(rep.PixelsWide / handler.Control.Size.Width);
#elif IOS
			var cgimage = handler.Control.CGImage;
			Control = new CGBitmapContext(handler.Data.MutableBytes, cgimage.Width, cgimage.Height, cgimage.BitsPerComponent, cgimage.BytesPerRow, cgimage.ColorSpace, cgimage.BitmapInfo);
			scale = cgimage.Width / handler.Control.Size.Width;
#endif

			height = image.Size.Height;
			SetDefaults();
			InitializeContext(true);
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
			get { return (float)(view != null ? view.Bounds.Height : height); }
		}

		void SetDefaults()
		{
			Control.InterpolationQuality = CGInterpolationQuality.High;
			Control.SetAllowsSubpixelPositioning(false);
		}

		void InitializeContext(bool viewFlipped)
		{
			Control.SaveState();

			#if OSX
			// os x has different flipped states (depending on layers, etc), so compensate to make 0,0 at the top-left
			if (view != null)
			{
				// we have a view (drawing directly to the screen), so adjust to where it is
				Control.ClipToRect(view.ConvertRectToView(view.VisibleRect(), null));
				var pos = view.ConvertPointToView(CGPoint.Empty, null);
				if (!viewFlipped)
					pos.Y += view.Frame.Height;
				currentTransform = new CGAffineTransform(1, 0, 0, -1, (float)pos.X, (float)pos.Y);
				Control.ConcatCTM(currentTransform);
			}
			else
			{
				// drawing to a bitmap or during a drawRect operation
				currentTransform = new CGAffineTransform(1, 0, 0, -1, 0, viewFlipped ? ViewHeight : 0);
				if (viewFlipped)
					Control.ConcatCTM(currentTransform);
			}
			#elif IOS
			if (viewFlipped)
			{
				// on ios, we flip the context if we're drawing on a bitmap otherwise we don't need to
				currentTransform = new CGAffineTransform(1, 0, 0, -1, 0, ViewHeight);
				Control.ConcatCTM(currentTransform);
			}
			#endif
		}

		public CGPoint TranslateView(CGPoint point, bool halfers = false, bool inverse = false)
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

		public CGRect TranslateView(CGRect rect, bool halfers = false, bool inverse = false)
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

		void StartDrawing()
		{
#if OSX
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSGraphicsContext.CurrentContext = graphicsContext;
#elif IOS
			UIGraphics.PushContext(Control);
#endif
			Control.SaveState();
		}

		void EndDrawing()
		{
			Control.RestoreState();
#if OSX
			NSGraphicsContext.GlobalRestoreGraphicsState();
#elif IOS
			UIGraphics.PopContext();
#endif
		}

		public void DrawLine(Pen pen, float startx, float starty, float endx, float endy)
		{
			StartDrawing();
			pen.Apply(this);
			Control.StrokeLineSegments(new []
			{
				TranslateView(new CGPoint(startx, starty), true),
				TranslateView(new CGPoint(endx, endy), true)
			});
			EndDrawing();
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			StartDrawing();
			var rect = TranslateView(new CGRect(x, y, width, height), true);
			pen.Apply(this);
			Control.StrokeRect(rect);
			EndDrawing();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			StartDrawing();
			brush.Apply(this);
			Control.FillRect(TranslateView(new CGRect(x, y, width, height), width > 1 || height > 1, true));
			EndDrawing();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			StartDrawing();
			var rect = TranslateView(new CGRect(x, y, width, height), true);
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
			Control.FillEllipseInRect(TranslateView(new CGRect(x, y, width, height), true, true));
			EndDrawing();
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			StartDrawing();

			var rect = TranslateView(new CGRect(x, y, width, height), true);
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

			var rect = TranslateView(new CGRect(x, y, width, height), true, true);
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
			FontExtensions.DrawString(text, new PointF(x, y), brush.Color, font);
			EndDrawing();
		}

		public SizeF MeasureString(Font font, string text)
		{
			StartDrawing();
			var size = FontExtensions.MeasureString(text, font);
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
			currentTransform = CGAffineTransform.Multiply(CGAffineTransform.MakeTranslation(offsetX, offsetY), currentTransform);
		}

		public void RotateTransform(float angle)
		{
			angle = Conversions.DegreesToRadians(angle);
			Control.RotateCTM(angle);
			currentTransform = CGAffineTransform.Multiply(CGAffineTransform.MakeRotation(angle), currentTransform);
		}

		public void ScaleTransform(float scaleX, float scaleY)
		{
			Control.ScaleCTM(scaleX, scaleY);
			currentTransform = CGAffineTransform.Multiply(CGAffineTransform.MakeScale(scaleX, scaleY), currentTransform);
		}

		public void MultiplyTransform(IMatrix matrix)
		{
			var m = matrix.ToCG();
			Control.ConcatCTM(m);
			currentTransform = CGAffineTransform.Multiply(m, currentTransform);
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
			clipBounds = rectangle.ToNS();
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
				FillRectangle(brush, (float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
		}

		public void SetFillColorSpace()
		{
			Control.SetFillColorSpace(patternColorSpace);
		}
	}
}
