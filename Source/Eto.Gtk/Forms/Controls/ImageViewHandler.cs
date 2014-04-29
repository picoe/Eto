using System;
using Eto.Forms;
using Eto.GtkSharp.Drawing;
using Eto.Drawing;

namespace Eto.GtkSharp
{
	public class ImageViewHandler : GtkControl<Gtk.DrawingArea, ImageView>, IImageView
	{
		Image image;
		bool widthSet;
		bool heightSet;

		public override Gtk.DrawingArea CreateControl()
		{
			var control = new Gtk.DrawingArea
			{
				CanFocus = false,
				CanDefault = true
			};
#if GTK2
			control.ExposeEvent += Connector.HandleExpose;
#else
			control.Drawn += Connector.HandleDrawn;
#endif
			control.Events |= Gdk.EventMask.ExposureMask;
			return control;
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
				using (var graphics = new Graphics(h.Widget.Platform, handler))
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
				if (image != null && !widthSet || !heightSet)
				{
					Control.SetSizeRequest(widthSet ? Size.Width : image.Size.Width, heightSet ? Size.Height : image.Size.Height);
				}
				if (Control.Visible)
					Control.QueueDraw();
			}
		}
	}
}

