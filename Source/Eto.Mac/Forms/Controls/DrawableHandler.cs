using sd = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class DrawableHandler : MacPanel<DrawableHandler.EtoDrawableView, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		Brush backgroundBrush;
		Color backgroundColor;

		public bool SupportsCreateGraphics { get { return true; } }

		public override NSView ContainerControl { get { return Control; } }

		public class EtoDrawableView : MacEventView
		{
			Drawable Drawable
			{
				get { return Widget as Drawable; }
			}

			public override void DrawRect(NSRect dirtyRect)
			{
				var drawable = Drawable;
				if (drawable == null)
					return;
				if (!IsFlipped)
					dirtyRect.Y = Frame.Height - dirtyRect.Y - dirtyRect.Height;
				if (dirtyRect.X % 1.0f > 0f)
					dirtyRect.Width += 1;
				if (dirtyRect.Y % 1.0f > 0f)
					dirtyRect.Height += 1;
				ApplicationHandler.QueueResizing = true;
				drawable.Update(Rectangle.Ceiling(dirtyRect.ToEto()));
				ApplicationHandler.QueueResizing = false;
			}

			public bool CanFocus { get; set; }

			public override bool AcceptsFirstResponder()
			{
				return CanFocus;
			}

			public override bool AcceptsFirstMouse(NSEvent theEvent)
			{
				return CanFocus;
			}
		}

		public Graphics CreateGraphics()
		{
			return new Graphics(new GraphicsHandler(Control));
		}

		public override bool Enabled { get; set; }

		public override Color BackgroundColor
		{
			get { return backgroundColor; }
			set
			{
				if (backgroundColor != value)
				{
					backgroundColor = value;
					backgroundBrush = backgroundColor.A > 0 ? new SolidBrush(backgroundColor) : null;
					Invalidate();
				}
			}
		}

		public void Create()
		{
			Enabled = true;
			Control = new EtoDrawableView { Handler = this };
		}

		public void Create(bool largeCanvas)
		{
			Create();
		}

		public bool CanFocus
		{
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}

		public override void Invalidate()
		{
			if (!NeedsQueue(Invalidate))
				base.Invalidate();
		}

		public override void Invalidate(Rectangle rect)
		{
			if (!NeedsQueue(() => Invalidate(rect)))
				base.Invalidate(rect);
		}

		public void Update(Rectangle rect)
		{
			var context = NSGraphicsContext.CurrentContext;
			if (context != null)
			{
				var handler = new GraphicsHandler(Control, context, (float)Control.Frame.Height, Control.IsFlipped);
				using (var graphics = new Graphics(handler))
				{
					if (backgroundBrush != null)
						graphics.FillRectangle(backgroundBrush, rect);

					var widget = Widget;
					if (widget != null)
						Callback.OnPaint(widget, new PaintEventArgs(graphics, rect));
				}
			}
		}
	}
}
