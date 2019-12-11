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
		FormattedText _formattedText;

		FormattedText SharedFormattedText => _formattedText ?? (_formattedText = new FormattedText());

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
			get { return Widget.Properties.Get(AntiAlias_Key, true); }
			set { Widget.Properties.Set(AntiAlias_Key, value, SetAntiAlias, true); }
		}

		void SetAntiAlias()
		{
			RewindAll();
			Control.SetShouldAntialias(AntiAlias);
			ApplyAll();
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

		static readonly object SourceImage_Key = new object();
		static readonly object DrawingImage_Key = new object();

		/// <summary>
		/// Source image we are drawing to. Do not dispose as we don't own it.
		/// </summary>
		Bitmap SourceImage
		{
			get { return Widget.Properties.Get<Bitmap>(SourceImage_Key); }
			set { Widget.Properties.Set(SourceImage_Key, value); }
		}

		/// <summary>
		/// Image we're drawing to, when the source image is not compatible. Disposed with Graphics object.
		/// </summary>
		Bitmap DrawingImage
		{
			get { return Widget.Properties.Get<Bitmap>(DrawingImage_Key); }
			set { Widget.Properties.Set(DrawingImage_Key, value); }
		}

		public void CreateFromImage(Bitmap image)
		{
			var handler = image.Handler as BitmapHandler;
			SourceImage = image;
#if OSX
			var rep = handler.Control.Representations().OfType<NSBitmapImageRep>().FirstOrDefault();
			if (rep.BitsPerPixel != 32)
			{
				// CoreGraphics only supports drawing to 32bpp, create a new 32-bpp image and copy back when disposed or flushed.
				DrawingImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgb);
				handler = DrawingImage.Handler as BitmapHandler;
				rep = handler.Control.Representations().OfType<NSBitmapImageRep>().FirstOrDefault();
			}
			graphicsContext = NSGraphicsContext.FromBitmap(rep);
			if (graphicsContext == null)
			{
				// invalid parameters for the rep
				DrawingImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppRgba);
				handler = DrawingImage.Handler as BitmapHandler;
				rep = handler.Control.Representations().OfType<NSBitmapImageRep>().FirstOrDefault();
				graphicsContext = NSGraphicsContext.FromBitmap(rep);
			}

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
			if (DrawingImage != null && SourceImage != null)
			{
				// draw source image onto context, when source is incompatible for CoreGraphics drawing.
				DrawImage(SourceImage, 0, 0);
			}
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
			CopyToOriginalImage();
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
			Control.MoveTo(startx, starty);
			Control.AddLineToPoint(endx, endy);
			pen.Finish(this);
			EndDrawing();
		}

		public void DrawLines(Pen pen, IEnumerable<PointF> points)
		{
			SetOffset(false);
			StartDrawing();
			pen.Apply(this);
			Control.AddLines(points.Select(r => r.ToNS()).ToArray());
			pen.Finish(this);
			EndDrawing();
		}

		public void DrawPolygon(Pen pen, IEnumerable<PointF> points)
		{
			SetOffset(false);
			StartDrawing();
			pen.Apply(this);
			Control.AddLines(points.Select(r => r.ToNS()).ToArray());
			Control.ClosePath();
			pen.Finish(this);
			EndDrawing();
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			pen.Apply(this);
			Control.AddRect(rect);
			pen.Finish(this);
			EndDrawing();
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			Control.AddRect(rect);
			brush.Draw(this, false, FillMode.Winding);
			EndDrawing();
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			SetOffset(false);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			pen.Apply(this);
			Control.AddEllipseInRect(rect);
			pen.Finish(this);
			EndDrawing();
		}

		public void FillEllipse(Brush brush, float x, float y, float width, float height)
		{
			SetOffset(true);
			StartDrawing();
			var rect = new CGRect(x, y, width, height);
			Control.AddEllipseInRect(rect);
			brush.Draw(this, false, FillMode.Winding);
			EndDrawing();
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(true);
			StartDrawing();

			var rect = new CGRect(x, y, width, height);
			pen.Apply(this);
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			var centerX = rect.GetMidX();
			Control.SaveState(); // save so the drawing of the pen isn't affected by the transform
			Control.ConcatCTM(new CGAffineTransform(1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.AddArc(centerX, centerY, rect.Width / 2, CGConversions.DegreesToRadians(startAngle), CGConversions.DegreesToRadians(startAngle + sweepAngle), sweepAngle < 0);
			Control.RestoreState();
			pen.Finish(this);
			EndDrawing();
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			SetOffset(true);
			StartDrawing();

			var rect = new CGRect(x, y, width, height);
			Control.SaveState(); // save so the drawing of the brush isn't affected by the transform
			var yscale = rect.Height / rect.Width;
			var centerY = rect.GetMidY();
			var centerX = rect.GetMidX();
			Control.ConcatCTM(new CGAffineTransform(1.0f, 0, 0, yscale, 0, centerY - centerY * yscale));
			Control.MoveTo(centerX, centerY);
			Control.AddArc(centerX, centerY, rect.Width / 2, CGConversions.DegreesToRadians(startAngle), CGConversions.DegreesToRadians(startAngle + sweepAngle), sweepAngle < 0);
			Control.AddLineToPoint(centerX, centerY);
			Control.ClosePath();
			Control.RestoreState();
			brush.Draw(this, false, FillMode.Winding);
			EndDrawing();
		}

		public void FillPath(Brush brush, IGraphicsPath path)
		{
			SetOffset(true);
			StartDrawing();

			Control.BeginPath();
			Control.AddPath(path.ToCG());
			Control.ClosePath();
			brush.Draw(this, false, FillMode.Winding);
			EndDrawing();
		}

		internal void Clip(FillMode mode)
		{
			switch (mode)
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

		internal void Fill(FillMode mode)
		{
			switch (mode)
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
		}

		public void DrawPath(Pen pen, IGraphicsPath path)
		{
			SetOffset(false);
			StartDrawing();
			
			pen.Apply(this);
			Control.BeginPath();
			Control.AddPath(path.ToCG());
			//Control.StrokePath();
			pen.Finish(this);

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

		public void DrawText(Font font, Brush brush, float x, float y, string text)
		{
			var formattedText = SharedFormattedText;
			formattedText.Text = text;
			formattedText.Font = font;
			formattedText.ForegroundBrush = brush;
			DrawText(formattedText, new PointF(x, y));
		}

		public void DrawText(FormattedText formattedText, PointF location)
		{
			SetOffset(true);
			StartDrawing();
			if (formattedText.Handler is FormattedTextHandler handler)
				handler.DrawText(this, location);
			EndDrawing();
		}

		public SizeF MeasureString(Font font, string text)
		{
			var formattedText = SharedFormattedText;
			formattedText.Text = text;
			formattedText.Font = font;
			return formattedText.Measure();
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
				CopyToOriginalImage();
				if (DrawingImage != null)
				{
					DrawingImage.Dispose();
					DrawingImage = null;
				}
			}
			base.Dispose(disposing);
		}

		void CopyToOriginalImage()
		{
			if (SourceImage != null && DrawingImage != null)
			{
				// copy back to original image when format is incompatible with CoreGraphics
				using (var bdNew = DrawingImage.Lock())
				using (var bdOrig = SourceImage.Lock())
				{
					var size = DrawingImage.Size;
					if (bdNew.BitsPerPixel == 32 && bdOrig.BitsPerPixel == 24)
					{
						unsafe {
							// assuming rgb is in same order as 32bpp bitmap, should be..
							var src = (byte*)bdNew.Data;
							var dest = (byte*)bdOrig.Data;
							var length = size.Width * size.Height;
							for (int i = 0; i < length; i++)
							{
								*(dest++) = *(src++);
								*(dest++) = *(src++);
								*(dest++) = *(src++);
								src++; // ignore alpha
							}
						}
					}
					else
					{
						for (int y = 0; y < size.Height; y++)
							for (int x = 0; x < size.Width; x++)
								bdOrig.SetPixel(x, y, bdNew.GetPixel(x, y));
					}
				}
			}
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
			RewindClip();
			clipPath = null;
			clipBounds = currentTransform.TransformRect(rectangle.ToNS());
			ApplyClip();
			ApplyTransform();
		}

		public void SetClip(IGraphicsPath path)
		{
			RewindTransform();
			RewindClip();
			clipBounds = null;
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
			RewindTransform();
			RewindClip();
			clipBounds = null;
			clipPath = null;
			ApplyTransform();
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
