using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class ScrollableHandler : GtkContainer<Gtk.ScrolledWindow, Scrollable>, IScrollable
	{
		Gtk.Viewport vp;
		BorderType border;
		
		bool autoSize = true;
		
		public override object ContainerObject {
			get {
				return Control;
			}
		}
		
		public BorderType Border {
			get {
				return border;
			}
			set {
				border = value;
				switch (border) {
				case BorderType.Bezel:
					vp.ShadowType = Gtk.ShadowType.In;
					//vp.BorderWidth = 2;
					break;
				case BorderType.Line:
					vp.ShadowType = Gtk.ShadowType.In;
					//vp.BorderWidth = 2;
					break;
				case BorderType.None:
					vp.ShadowType = Gtk.ShadowType.None;
					//vp.BorderWidth = 0;
					break;
				default:
					throw new NotSupportedException();
				}
			}
		}

		public ScrollableHandler()
		{
			Control = new Gtk.ScrolledWindow();
			vp = new Gtk.Viewport();
			vp.Shown += delegate {
				if (autoSize) {
					var size = vp.SizeRequest ();
					Control.SetSizeRequest (size.Width, size.Height);
				}
			};
			Border = BorderType.Bezel;
			Control.VScrollbar.VisibilityNotifyEvent += scrollBar_VisibilityChanged;
			Control.HScrollbar.VisibilityNotifyEvent += scrollBar_VisibilityChanged;
			//vp.ShadowType = Gtk.ShadowType.None;
			Control.Add(vp);
			border = BorderType.Bezel;
		}
		
		public override Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				autoSize = false;
			}
		}
		
		void scrollBar_VisibilityChanged(object sender, EventArgs e)
		{
			Widget.OnSizeChanged(EventArgs.Empty);
		}
		
		public override void SetLayout(Layout inner)
		{
			if (vp.Children.Length > 0)
				foreach (Gtk.Widget child in vp.Children)
					vp.Remove(child);
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			vp.Add((Gtk.Widget)gtklayout.ContainerObject);
			//control.Add((Gtk.Widget)layout.ControlObject);

		}

		public override Color BackgroundColor
		{
			get { return Generator.Convert(vp.Style.Background(Gtk.StateType.Normal)); }
			set { vp.ModifyBg(Gtk.StateType.Normal, Generator.Convert(value)); }
		}

		public override Size ClientSize
		{
			get
			{
				Gdk.Rectangle rect = vp.Allocation;
				int spacing = Convert.ToInt32(Control.StyleGetProperty("scrollbar-spacing")) + 1;
				return new Size(rect.Width - spacing, rect.Height - spacing);
			}
			set
			{
				int spacing = Convert.ToInt32(Control.StyleGetProperty("scrollbar-spacing")) + 1;
				vp.SetSizeRequest(value.Width + spacing, value.Height + spacing);
			}
		}

		public void UpdateScrollSizes()
		{
			Control.CheckResize();
			vp.CheckResize();
		}
		
		public Point ScrollPosition
		{
			get { return new Point((int)vp.Hadjustment.Value, (int)vp.Vadjustment.Value); }
			set
			{
				Size clientSize = ClientSize;
				Size scrollSize = ScrollSize;
				vp.Hadjustment.Value = Math.Min(value.X, scrollSize.Width - clientSize.Width);
				vp.Vadjustment.Value = Math.Min(value.Y, scrollSize.Height - clientSize.Height);
			}
		}

		public Size ScrollSize
		{
			get {
				//return scrollSize;
				return new Size((int)(vp.Hadjustment.Upper), (int)(vp.Vadjustment.Upper));
			}
			set {
				//scrollSize = value;
				vp.Hadjustment.Upper = value.Width; vp.Vadjustment.Upper = value.Height;
			}
		}

	}
}
