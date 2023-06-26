using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Drawing;

namespace Eto.Mac.Forms.Controls
{
	public class DrawableHandler : MacPanel<DrawableHandler.EtoDrawableView, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		Brush backgroundBrush;
		Color backgroundColor;

		public bool SupportsCreateGraphics => false;

		public override NSView ContainerControl => Control;

		public class EtoDrawableView : MacPanelView
		{
			DrawableHandler Drawable => Handler as DrawableHandler;

			public override void DrawRect(CGRect dirtyRect)
			{
				var drawable = Drawable;
				if (drawable == null)
					return;
				// restrict drawing to the bounds of the drawable
				// macOS can give us dirty rects outside this range
				var bounds = Bounds.ToEto();
				var dirty = dirtyRect.ToEto();
				dirty.Restrict(bounds);
				
				if (!IsFlipped)
					dirty.Y = bounds.Height - dirty.Y - dirty.Height;
				if (dirty.X % 1.0f > 0f)
					dirty.Width += 1;
				if (dirty.Y % 1.0f > 0f)
					dirty.Height += 1;
				ApplicationHandler.QueueResizing = true;
				drawable.DrawRegion(Rectangle.Ceiling(dirty));
				ApplicationHandler.QueueResizing = false;
			}

			// public override bool IsFlipped => true;  // uncomment to test flipped views with GraphicsHandler.

			public bool CanFocus { get; set; }

			public override bool AcceptsFirstResponder() => CanFocus && Drawable?.Enabled == true;

			public override NSView HitTest(CGPoint aPoint)
			{
				var view = base.HitTest(aPoint);
				if (view == ContentView)
				{
					// forward all events to this view, not the content view (which covers the drawable)
					// the properly enables AcceptsFirstMouse above, since the ContentView returns false
					return this;
				}
				return view;
			}
		}

		public Graphics CreateGraphics()
		{
			throw new NotSupportedException();
		}

		public override Color BackgroundColor
		{
			get { return backgroundColor; }
			set
			{
				if (backgroundColor != value)
				{
					backgroundColor = value;
					backgroundBrush = backgroundColor.A > 0 ? new SolidBrush(backgroundColor) : null;
					Invalidate(false);
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

		public override void Invalidate(bool invalidateChildren)
		{
			if (NeedsQueue)
			{
				AsyncQueue.Add(() => Invalidate(invalidateChildren));
				return;
			}
			base.Invalidate(invalidateChildren);
		}

		public override void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			if (NeedsQueue)
			{
				AsyncQueue.Add(() => Invalidate(rect, invalidateChildren));
				return;
			}
			base.Invalidate(rect, invalidateChildren);
		}

		void DrawRegion(Rectangle rect)
		{
			var context = NSGraphicsContext.CurrentContext;
			if (context != null)
			{
				var handler = new GraphicsHandler(Control, context, (float)Control.Frame.Height);
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

		public void Update(Rectangle rect)
		{
			Control.DisplayRect(rect.ToNS());
		}

		protected override bool OnAcceptsFirstMouse(NSEvent theEvent)
		{
			if (CanFocus)
				return true;
			return base.OnAcceptsFirstMouse(theEvent);
		}
	}
}
