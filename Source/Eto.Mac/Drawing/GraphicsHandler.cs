using System;
using System.Globalization;
using System.Linq;
using Eto.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
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
using CoreGraphics;
using UIKit;
using Foundation;
using NSView = UIKit.UIView;
using GraphicsBase = Eto.WidgetHandler<CoreGraphics.CGContext, Eto.Drawing.Graphics, Eto.Drawing.Graphics.ICallback>;

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
		readonly NSView view;
		#endif
		float height;
		bool isOffset;
		CGRect? clipBounds;
		IGraphicsPath clipPath;
		readonly Stack<CGAffineTransform> transforms = new Stack<CGAffineTransform>();
		CGAffineTransform currentTransform = CGAffineTransform.MakeIdentity();
		static readonly CGColorSpace patternColorSpace = CGColorSpace.CreatePattern(null);

		public NSView DisplayView { get; private set; }

		static readonly object PixelOffsetMode_Key = new object();

		public PixelOffsetMode PixelOffsetMode
		{
			get { return Widget != null ? Widget.Properties.Get<PixelOffsetMode>(PixelOffsetMode_Key) : default(PixelOffsetMode); }
			set { Widget.Properties.Set(PixelOffsetMode_Key, value); }
		}

		void SetOffset(bool fill)
		{
			var requiresOffset = !fill && PixelOffsetMode == PixelOffsetMode.None;
			if (requiresOffset != isOffset)
			{
				RewindAll();
				isOffset = requiresOffset;
				ApplyAll();
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

				h.InitializeContext(h.view.IsFlipped);
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

		public GraphicsHandler(NSView view, CGContext context, nfloat height)
		{
			this.DisplayView = view;
			this.height = (float)height;
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

		static readonly object AntiAlias_Key = new object();

		public bool AntiAlias
		{
			get { return Widget.Properties.Get<bool>(AntiAlias_Key, true); }
			set { Widget.Properties.Set(AntiAlias_Key, value, () => Control.SetShouldAntialias(value)); }
		}

		static readonly object PointsPerPixel_Key = new object();
		public float PointsPerPixel
		{ 
			get { return Widget.Properties.Get<float>(PointsPerPixel_Key, 1f); } 
			private set { Widget.Properties.Set(PointsPerPixel_Key, value); }
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
			graphicsContext = graphicsContext.IsFlipped ? graphicsContext : NSGraphicsContext.FromGraphicsPort(graphicsContext.GraphicsPortHandle, true);
			disposeContext = true;
			Control = graphicsContext.GraphicsPort;
			PointsPerPixel = (float)(rep.PixelsWide / handler.Control.Size.Width);
#elif IOS
			var cgimage = handler.Control.CGImage;
			Control = new CGBitmapContext(handler.Data.MutableBytes, cgimage.Width, cgimage.Height, cgimage.BitsPerComponent, cgimage.BytesPerRow, cgimage.ColorSpace, cgimage.BitmapInfo);
			PointsPerPixel = (float)(cgimage.Width / handler.Control.Size.Width);
#endif

			height = image.Size.Height;
			SetDefaults();
			InitializeContext(true);
		}

		public void Reset()
		{
			// unwind all SaveState's
			RewindAll();
			// initial save state
			Control.RestoreState();
		}

		public void Flush()
		{
#if OSX
			graphicsContext.FlushGraphics();
#endif
		}

		#if OSX
		public float ViewHeight
		{
			get { return (float)(view != null ? view.Bounds.Height : height); }
		}
		#elif IOS
		public float ViewHeight
		{
			get { return height; }
		}
		#endif

		void SetDefaults()
		{
			Control.InterpolationQuality = CGInterpolationQuality.Default;
			Control.SetAllowsSubpixelPositioning(false);
		}

		#if OSX
		CGSize? phase;
		public void SetPhase()
		{
			if (DisplayView == null)
				return;
			if (phase == null)
			{
				// find parent with layer, or goes up to window if none are found
				var layerView = DisplayView;
				while (layerView.Superview != null && layerView.Layer == null)
					layerView = layerView.Superview;

				// convert bottom-left point relative to layer or window
				var pos = DisplayView.ConvertPointToView(CGPoint.Empty, layerView);

				// phase should be based on position of control within closest layer or window.
				phase = new CGSize(pos.X, pos.Y);
			}

			Control.SetPatternPhase(phase.Value);
		}
		#endif

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
				Control.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, (float)pos.X, (float)pos.Y));
			}
			else
			{
				// drawing to a bitmap or during a drawRect operation
				if (viewFlipped)
					Control.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, ViewHeight));
			}

			phase = null;

			#elif IOS
			if (viewFlipped)
			{
				// on ios, we flip the context if we're drawing on a bitmap otherwise we don't need to
				Control.ConcatCTM(new CGAffineTransform(1, 0, 0, -1, 0, ViewHeight));
			}
			#endif

			ApplyAll();

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
			SetOffset(false);
			StartDrawing();
			pen.Apply(this);
			Control.StrokeLineSegments(new []
			{
				new CGPoint(startx, starty),
				new CGPoint(endx, endy)
			});
			EndDrawing();
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			pen.Apply(this);
			Control.StrokeRect(rect);
			EndDrawing();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			Control.AddRect(rect);
			Control.Clip();
			brush.Draw(this, rect.ToEto());
			EndDrawing();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			pen.Apply(this);
			Control.StrokeEllipseInRect(rect);
			EndDrawing();
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			StartDrawing();
			/*	if (width == 1 || height == 1)
			{
				DrawLine(color, x, y, x+width-1, y+height-1);
				return;
			}*/
			var rect = new CGRect(x, y, width, height);
			Control.AddEllipseInRect(rect);
			Control.Clip();
			brush.Draw(this, rect.ToEto());
			EndDrawing();
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(false);
			StartDrawing();

			var rect = new CGRect(x, y, width, height);
			pen.Apply(this);
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			var centerX = rect.GetMidX();
			Control.ConcatCTM(new CGAffineTransform(1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.AddArc(centerX, centerY, rect.Width / 2, CGConversions.DegreesToRadians(startAngle), CGConversions.DegreesToRadians(startAngle + sweepAngle), sweepAngle < 0);
			Control.StrokePath();
			EndDrawing();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(true);
			StartDrawing();

			var rect = new CGRect(x, y, width, height);
			Control.SaveState();
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			var centerX = rect.GetMidX();
			Control.ConcatCTM(new CGAffineTransform(1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.MoveTo(centerX, centerY);
			Control.AddArc(centerX, centerY, rect.Width / 2, CGConversions.DegreesToRadians(startAngle), CGConversions.DegreesToRadians(startAngle + sweepAngle), sweepAngle < 0);
			Control.AddLineToPoint(centerX, centerY);
			Control.ClosePath();
			Control.RestoreState();
			Control.Clip();
			brush.Draw(this, rect.ToEto());
			EndDrawing();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			SetOffset(true);
			StartDrawing();

			Control.BeginPath();
			Control.AddPath(path.ToCG());
			Control.ClosePath();
			switch (path.FillMode)
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
			brush.Draw(this, path.Bounds);
			EndDrawing();
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			SetOffset(false);
			StartDrawing();
			
			pen.Apply(this);
			Control.BeginPath();
			Control.AddPath(path.ToCG());
			Control.StrokePath();
			
			EndDrawing();
		}

		public void DrawImage(Image image, float x, float y)
		{
			SetOffset(true);
			StartDrawing();

			var handler = (IImageHandler)image.Handler;
			handler.DrawImage(this, x, y);
			EndDrawing();
		}

		public void DrawImage(Image image, float x, float y, float width, float height)
		{
			SetOffset(true);
			StartDrawing();

			var handler = (IImageHandler)image.Handler;
			handler.DrawImage(this, x, y, width, height);
			EndDrawing();
		}

		public void DrawImage(Image image, RectangleF source, RectangleF destination)
		{
			SetOffset(true);
			StartDrawing();

			var handler = (IImageHandler)image.Handler;
			handler.DrawImage(this, source, destination);
			EndDrawing();
		}

		public void DrawText(Font font, SolidBrush brush, float x, float y, string text)
		{
			SetOffset(true);
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
			angle = (float)CGConversions.DegreesToRadians(angle);
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
			transforms.Push(currentTransform);
		}

		public void RestoreTransform()
		{
			if (transforms.Count <= 0)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "No saved transform"));
			RewindTransform();
			currentTransform = transforms.Pop();
			ApplyTransform();
		}

		public IMatrix CurrentTransform
		{
			get { return currentTransform.ToEto(); }
		}

		public RectangleF ClipBounds
		{
			get { return Control.GetClipBoundingBox().ToEto(); }
		}

		public void SetClip(RectangleF rectangle)
		{
			RewindTransform();
			ResetClip();
			clipBounds = currentTransform.TransformRect(rectangle.ToNS());
			ApplyClip();
			ApplyTransform();
		}

		public void SetClip(IGraphicsPath path)
		{
			RewindTransform();
			ResetClip();
			clipPath = path.Clone();
			clipPath.Transform(currentTransform.ToEto());
			ApplyClip();
			ApplyTransform();
		}

		void ApplyClip()
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
			Control.RestoreState();
		}

		void ApplyTransform()
		{
			Control.SaveState();
			Control.ConcatCTM(currentTransform);
		}

		void RewindAll()
		{
			RewindTransform();
			RewindClip();
			if (isOffset)
			{
				Control.RestoreState();
			}
		}

		void ApplyAll()
		{
			if (isOffset)
			{
				Control.SaveState();
				Control.TranslateCTM(0.5f, 0.5f);
			}
			ApplyClip();
			ApplyTransform();
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
