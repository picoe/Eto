using Eto.Drawing;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using System;

namespace Eto.GtkSharp
{
	public class DrawableHandler : GtkPanel<Gtk.EventBox, Drawable>, IDrawable
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
				Gdk.EventExpose ev = args.Event;
				using (var graphics = new Graphics(h.Widget.Platform, new GraphicsHandler(h.Control, ev.Window)))
				{
					Rectangle rect = ev.Region.Clipbox.ToEto();
					h.Widget.OnPaint(new PaintEventArgs(graphics, rect));
				}
			}
#else
			public void HandleDrawn(object o, Gtk.DrawnArgs args)
			{
				var h = Handler;
				using (var graphics = new Graphics(h.Widget.Platform, new GraphicsHandler(args.Cr, h.Control.CreatePangoContext(), false)))
				{
					h.Widget.OnPaint (new PaintEventArgs (graphics, new Rectangle(h.Size)));
				}
			}
#endif
		}

		public void Update(Rectangle rect)
		{
			using (var graphics = new Graphics(Widget.Platform, new GraphicsHandler(Control, Control.GdkWindow)))
			{
				Widget.OnPaint(new PaintEventArgs(graphics, rect));
			}
		}

		public Graphics CreateGraphics()
		{
			return new Graphics(Widget.Platform, new GraphicsHandler(Control, Control.GdkWindow));
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			this.content.Add(content);
		}
	}
}
