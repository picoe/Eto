using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using System;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DrawableHandler : GtkPanel<Gtk.EventBox, Drawable, Drawable.ICallback>, Drawable.IHandler
	{
		Gtk.VBox content;

		public bool SupportsCreateGraphics { get { return true; } }

		public void Create()
		{
			Control = new Gtk.EventBox();
			Control.Events |= Gdk.EventMask.ExposureMask;
			//Control.ModifyBg(Gtk.StateType.Normal, new Gdk.Color(0, 0, 0));
			//Control.DoubleBuffered = false;
			Control.CanFocus = false;
			Control.CanDefault = true;
			Control.Events |= Gdk.EventMask.ButtonPressMask;

			content = new Gtk.VBox();

			Control.Add(content);
		}

		protected override void Initialize()
		{
			base.Initialize();
#if GTK2
			Control.ExposeEvent += Connector.HandleExpose;
#else
			Control.Drawn += Connector.HandleDrawn;
#endif
			Control.ButtonPressEvent += Connector.HandleDrawableButtonPressEvent;
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

		protected new DrawableConnector Connector { get { return (DrawableConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DrawableConnector();
		}

		protected class DrawableConnector : GtkPanelEventConnector
		{
			public new DrawableHandler Handler { get { return (DrawableHandler)base.Handler; } }

			public void HandleDrawableButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
			{
				if (Handler.CanFocus)
					Handler.Control.GrabFocus();
			}

#if GTK2
			public void HandleExpose(object o, Gtk.ExposeEventArgs args)
			{
				var h = Handler;
				if (h == null) // can happen if expose event happens after window is closed
					return;
				Gdk.EventExpose ev = args.Event;
				using (var graphics = new Graphics(new GraphicsHandler(h.Control, ev.Window)))
				{
					Rectangle rect = ev.Region.Clipbox.ToEto();
					h.Callback.OnPaint(h.Widget, new PaintEventArgs(graphics, rect));
				}
			}
#else
			[GLib.ConnectBefore]
			public void HandleDrawn(object o, Gtk.DrawnArgs args)
			{
				var h = Handler;

				var allocation = h.Control.Allocation.Size;
				args.Cr.Rectangle(new Cairo.Rectangle(0, 0, allocation.Width, allocation.Height));
				args.Cr.Clip();
				Gdk.Rectangle rect = new Gdk.Rectangle();
				if (!GraphicsHandler.GetClipRectangle(args.Cr, ref rect))
					rect = new Gdk.Rectangle(Gdk.Point.Zero, allocation);

				using (var graphics = new Graphics(new GraphicsHandler(args.Cr, h.Control.CreatePangoContext(), false)))
				{
					if (h.SelectedBackgroundColor != null)
						graphics.Clear(h.SelectedBackgroundColor.Value);
					
					h.Callback.OnPaint(h.Widget, new PaintEventArgs (graphics, rect.ToEto()));
				}
			}
#endif
		}

		public void Update(Rectangle rect)
		{
			using (var graphics = new Graphics(new GraphicsHandler(Control, Control.GetWindow())))
			{
				Callback.OnPaint(Widget, new PaintEventArgs(graphics, rect));
			}
		}

		public Graphics CreateGraphics()
		{
			return new Graphics(new GraphicsHandler(Control, Control.GetWindow()));
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			this.content.Add(content);
		}

#if GTK3
		protected override void SetBackgroundColor(Color? color)
		{
			// we handle this ourselves
			//base.SetBackgroundColor(color);
			Invalidate(false);
		}
#endif
	}
}
