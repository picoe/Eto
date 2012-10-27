using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.GtkSharp
{
	public class ScrollableHandler : GtkContainer<Gtk.ScrolledWindow, Scrollable>, IScrollable
	{
		Gtk.Viewport vp;
		Gtk.HBox hbox;
		Gtk.VBox vbox;
		BorderType border;
		bool autoSize = true;
		bool expandWidth = true;
		bool expandHeight = true;
		Gtk.Widget layoutObject;
		
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
					Control.ShadowType = Gtk.ShadowType.In;
					break;
				case BorderType.Line:
					Control.ShadowType = Gtk.ShadowType.In;
					break;
				case BorderType.None:
					Control.ShadowType = Gtk.ShadowType.None;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}

		public ScrollableHandler ()
		{
			Control = new Gtk.ScrolledWindow ();
			vp = new Gtk.Viewport ();
			hbox = new Gtk.HBox ();
			vbox = new Gtk.VBox ();
			vp.Add (vbox);
			vbox.PackStart (hbox, true, true, 0);
			
			// autosize the scrolled window to the size of the content
			Control.SizeRequested += delegate(object o, Gtk.SizeRequestedArgs args) {
				if (autoSize) {
					args.Requisition = vp.SizeRequest ();
				}
			};
			vp.SizeRequested += delegate(object o, Gtk.SizeRequestedArgs args) {
				if (autoSize) {
					var size = vp.SizeRequest ();
					//Console.WriteLine ("Autosizing to {0}x{1}", size.Width, size.Height);
					args.Requisition = size;
				}
			};

			Control.VScrollbar.VisibilityNotifyEvent += scrollBar_VisibilityChanged;
			Control.HScrollbar.VisibilityNotifyEvent += scrollBar_VisibilityChanged;
			Control.Add (vp);
			vp.ShadowType = Gtk.ShadowType.None;
			this.Border = BorderType.Bezel;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Scrollable.ScrollEvent:
				Control.Events |= Gdk.EventMask.ScrollMask;
				Control.ScrollEvent += delegate(object o, Gtk.ScrollEventArgs args) {
					var pos = new Point ((int)args.Event.X, (int)args.Event.Y);
					this.Widget.OnScroll (new ScrollEventArgs (pos));
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
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
		
		void scrollBar_VisibilityChanged (object sender, EventArgs e)
		{
			Widget.OnSizeChanged (EventArgs.Empty);
		}
		
		public override void SetLayout (Layout layout)
		{
			foreach (Gtk.Widget child in hbox.Children)
				hbox.Remove (child);
			IGtkLayout gtklayout = (IGtkLayout)layout.Handler;
			layoutObject = gtklayout.ContainerObject;
			hbox.PackStart (layoutObject, false, true, 0);
			SetPacking ();
		}

		void SetPacking ()
		{
			if (layoutObject != null)
				hbox.SetChildPacking(layoutObject, expandWidth, expandWidth, 0, Gtk.PackType.Start);
			vbox.SetChildPacking(hbox, expandHeight, expandHeight, 0, Gtk.PackType.Start);
		}

		public override Color BackgroundColor {
			get { return Generator.Convert (vp.Style.Background (Gtk.StateType.Normal)); }
			set { vp.ModifyBg (Gtk.StateType.Normal, Generator.Convert (value)); }
		}

		public override Size ClientSize {
			get {
				Gdk.Rectangle rect = vp.Allocation;
				int spacing = Convert.ToInt32 (Control.StyleGetProperty ("scrollbar-spacing")) + 1;
				return new Size (rect.Width - spacing, rect.Height - spacing);
			}
			set {
				int spacing = Convert.ToInt32 (Control.StyleGetProperty ("scrollbar-spacing")) + 1;
				vp.SetSizeRequest (value.Width + spacing, value.Height + spacing);
			}
		}

		public void UpdateScrollSizes ()
		{
			Control.CheckResize ();
			vp.CheckResize ();
		}
		
		public Point ScrollPosition {
			get { return new Point ((int)vp.Hadjustment.Value, (int)vp.Vadjustment.Value); }
			set {
				Size clientSize = ClientSize;
				Size scrollSize = ScrollSize;
				vp.Hadjustment.Value = Math.Min (value.X, scrollSize.Width - clientSize.Width);
				vp.Vadjustment.Value = Math.Min (value.Y, scrollSize.Height - clientSize.Height);
			}
		}

		public Size ScrollSize {
			get {
				//return scrollSize;
				return new Size ((int)(vp.Hadjustment.Upper), (int)(vp.Vadjustment.Upper));
			}
			set {
				//scrollSize = value;
				vp.Hadjustment.Upper = value.Width;
				vp.Vadjustment.Upper = value.Height;
			}
		}
		
		public Rectangle VisibleRect {
			get { return new Rectangle (ScrollPosition, Size.Min (ScrollSize, ClientSize)); }
		}

		public bool ExpandContentWidth
		{
			get { return expandWidth; }
			set {
				if (expandWidth != value) {
					expandWidth = value;
					SetPacking ();
				}
			}
		}
		public bool ExpandContentHeight
		{
			get { return expandHeight; }
			set {
				if (expandHeight != value) {
					expandHeight = value;
					SetPacking ();
				}
			}
		}
	}
}
