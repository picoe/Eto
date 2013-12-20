using sd = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Mac.Drawing;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class DrawableHandler : MacDockContainer<DrawableHandler.EtoDrawableView, Drawable>, IDrawable
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

			public override void DrawRect(sd.RectangleF dirtyRect)
			{
				ApplicationHandler.QueueResizing = true;
				var drawable = Drawable;
				if (drawable == null)
					return;
				if (!IsFlipped)
					dirtyRect.Y = Frame.Height - dirtyRect.Y - dirtyRect.Height;
				if (dirtyRect.X % 1.0f > 0f)
					dirtyRect.Width += 1;
				if (dirtyRect.Y % 1.0f > 0f)
					dirtyRect.Height += 1;
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
			return new Graphics(Widget.Generator, new GraphicsHandler(Control));
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
					backgroundBrush = backgroundColor.A > 0 ? new SolidBrush(backgroundColor, Widget.Generator) : null;
					Invalidate();
				}
			}
		}

		public void Create()
		{
			Enabled = true;
			Control = new EtoDrawableView { Handler = this };
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
				var handler = new GraphicsHandler(Control, context, Control.Frame.Height, Control.IsFlipped);
				using (var graphics = new Graphics(Widget.Generator, handler))
				{
					if (backgroundBrush != null)
						graphics.FillRectangle(backgroundBrush, rect);

					var widget = Widget;
					if (widget != null)
						widget.OnPaint(new PaintEventArgs(graphics, rect));
				}
			}
		}
	}
}
