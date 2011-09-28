using System;
using MonoMac.AppKit;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using Eto.Platform.Mac.Drawing;
using MonoMac.ObjCRuntime;
using SD = System.Drawing;

namespace Eto.Platform.Mac
{
	class MouseDelegate : NSObject
	{
		public NSView View { get; set; }
	
		public Control Widget { get; set; }
	
		[Export("mouseMoved:")]
		public void MouseMoved (NSEvent theEvent)
		{
			var pt = Generator.GetLocation (View, theEvent);
			Key modifiers = KeyMap.GetModifiers (theEvent);
			MouseButtons buttons = KeyMap.GetMouseButtons (theEvent);
			Widget.OnMouseMove (new MouseEventArgs (buttons, modifiers, pt));
		}
		
		[Export("mouseEntered:")]
		public void MouseEntered (NSEvent theEvent)
		{
		}

		[Export("cursorUpdate::")]
		public void CursorUpdate (NSEvent theEvent)
		{
		}
	
		[Export("mouseExited:")]
		public void MouseExited (NSEvent theEvent)
		{
		}
	}

	public interface IMacView
	{
		Size PositionOffset { get; }
		
		Size? PreferredSize { get; }
		
		Size? MinimumSize { get; set; }
		
		Control Widget { get; }

		bool AutoSize { get; }
	}
	
	public abstract class MacView<T, W> : MacObject<T, W>, IControl, IMacView
		where T: NSView
		where W: Control
	{
		bool focus;
		NSTrackingArea tracking;
		bool mouseMove;
		MouseDelegate mouseDelegate;

		public virtual bool AutoSize { get; protected set; }

		public Size Size {
			get { return Generator.ConvertF (Control.Frame.Size); }
			set { 
				this.PreferredSize = value;
				Control.SetFrameSize (Generator.ConvertF (value));
				this.AutoSize = false;
				CreateTracking ();
			}
		}
		
		public virtual Size? MinimumSize
		{
			get { return null; }
			set { }
		}
		
		public Size? PreferredSize
		{
			get; set;
		}
		
		public MacView ()
		{
			this.AutoSize = true;
		}
		
		public virtual Size PositionOffset { get { return Size.Empty; } }
		
		void CreateTracking ()
		{
			if (!mouseMove)
				return;
			if (tracking != null)
				Control.RemoveTrackingArea (tracking);
			//Console.WriteLine ("Adding mouse tracking {0} for area {1}", this.Widget.GetType ().FullName, Control.Frame.Size);
			mouseDelegate = new MouseDelegate{ Widget = this.Widget, View = Control };
			tracking = new NSTrackingArea (new SD.RectangleF (new SD.PointF (0, 0), Control.Frame.Size), 
				NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.EnabledDuringMouseDrag | NSTrackingAreaOptions.InVisibleRect, 
			    mouseDelegate, 
				new NSDictionary ());
			Control.AddTrackingArea (tracking);
		}

		public virtual void SetParentLayout (Layout layout)
		{
		}
		
		public virtual void SetParent (Control parent)
		{
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Eto.Forms.Control.MouseMoveEvent:
				mouseMove = true;
				CreateTracking ();
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				Control.PostsFrameChangedNotifications = true;
				this.AddObserver (NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) {
					((MacView<T, W>)(e.Widget.Handler)). OnSizeChanged (EventArgs.Empty);
					e.Widget.OnSizeChanged (EventArgs.Empty);
				});
				break;
			default:
				base.AttachEvent (handler);
				break;

			}
		}
		
		protected virtual void OnSizeChanged (EventArgs e)
		{
		}
		
		public void Invalidate ()
		{
			Control.NeedsDisplay = true;
		}

		public void Invalidate (Rectangle rect)
		{
			var region = Generator.ConvertF (rect);
			region.Y = Control.Frame.Height - region.Y - region.Height;
			Control.SetNeedsDisplayInRect (region);
		}

		public Graphics CreateGraphics ()
		{
			return new Graphics (Widget.Generator, new GraphicsHandler (Control));
		}

		public void SuspendLayout ()
		{
		}

		public void ResumeLayout ()
		{
		}

		public void Focus ()
		{
			if (Control.Window != null)
				Control.Window.MakeFirstResponder (Control);
			else
				focus = true;
		}

		public virtual Color BackgroundColor {
			get { 
				if (!Control.WantsLayer) {
					Control.WantsLayer = true;
				}
				return Generator.Convert (Control.Layer.BackgroundColor);
			}
			set {
				if (!Control.WantsLayer) {
					Control.WantsLayer = true;
				}
				Control.Layer.BackgroundColor = Generator.Convert (value);
			}
		}

		public string Id { get; set; }

		public virtual bool Enabled { get; set; }

		public virtual bool HasFocus {
			get {
				return Control.Window.FirstResponder == ControlObject;
			}
		}

		public bool Visible {
			get { return !Control.Hidden; }
			set { Control.Hidden = !value; }
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoadComplete (EventArgs e)
		{
			if (focus)
				Control.Window.MakeFirstResponder (Control);
		}
		
		#region IMacView implementation

		Control IMacView.Widget {
			get {
				return this.Widget;
			}
		}
		
		#endregion
	}
}

