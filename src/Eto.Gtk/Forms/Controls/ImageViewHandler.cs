using System;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ImageViewHandler : GtkControl<Gtk.DrawingArea, ImageView, ImageView.ICallback>, ImageView.IHandler
	{
		Image image;
		bool widthSet;
		bool heightSet;

		public ImageViewHandler()
		{
			Control = new Gtk.DrawingArea
			{
				CanFocus = false,
				CanDefault = true
			};
			Control.Events |= Gdk.EventMask.ExposureMask;
			#if GTK3
			// set to transparent by default
			Control.OverrideBackgroundColor(Gtk.StateFlags.Normal, Colors.Transparent.ToRGBA());
			#endif
		}

		protected override void Initialize()
		{
			base.Initialize();
#if GTK2
			Control.ExposeEvent += Connector.HandleExpose;
#else
			Control.Drawn += Connector.HandleDrawn;
#endif
		}

		protected new ImageViewConnector Connector { get { return (ImageViewConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ImageViewConnector();
		}

		protected class ImageViewConnector : GtkControlConnector
		{
			public new ImageViewHandler Handler { get { return (ImageViewHandler)base.Handler; } }

#if GTK2
			public void HandleExpose(object o, Gtk.ExposeEventArgs args)
			{
				Gdk.EventExpose ev = args.Event;
				var h = Handler;
				var handler = new GraphicsHandler(h.Control, ev.Window);
#else
			public void HandleDrawn(object o, Gtk.DrawnArgs args)
			{
				var h = Handler;
				var handler = new GraphicsHandler(args.Cr, h.Control.CreatePangoContext(), false);
#endif
				if (h.image == null)
					return;
				using (var graphics = new Graphics(handler))
				{

					var widgetSize = new Size(h.Control.Allocation.Width, h.Control.Allocation.Height);
					var imageSize = (SizeF)h.image.Size;
					var scaleWidth = widgetSize.Width / imageSize.Width;
					var scaleHeight = widgetSize.Height / imageSize.Height;
					imageSize *= Math.Min(scaleWidth, scaleHeight);
					var location = new PointF((widgetSize.Width - imageSize.Width) / 2, (widgetSize.Height - imageSize.Height) / 2);

					var destRect = new Rectangle(Point.Round(location), Size.Truncate(imageSize));
					graphics.DrawImage(h.image, destRect);
				}
			}
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				widthSet = value.Width >= 0;
				heightSet = value.Height >= 0;
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				if (!widthSet || !heightSet)
				{
					var imageSize = image?.Size ?? Size.Empty;
					var size = Size;
					Control.SetSizeRequest(widthSet ? size.Width : imageSize.Width, heightSet ? size.Height : imageSize.Height);
				}
				if (Control.Visible)
					Control.QueueDraw();
			}
		}
	}
}

