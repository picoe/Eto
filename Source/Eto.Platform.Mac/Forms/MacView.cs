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
			Widget.OnMouseMove (Generator.GetMouseEvent (View, theEvent));
		}
		
		[Export("mouseEntered:")]
		public void MouseEntered (NSEvent theEvent)
		{
			Widget.OnMouseEnter (Generator.GetMouseEvent (View, theEvent));
		}

		[Export("cursorUpdate::")]
		public void CursorUpdate (NSEvent theEvent)
		{
		}
	
		[Export("mouseExited:")]
		public void MouseExited (NSEvent theEvent)
		{
			Widget.OnMouseLeave (Generator.GetMouseEvent (View, theEvent));
		}
	}
	
	public interface IMacAutoSizing
	{
		bool AutoSize { get; }
		
		Size GetPreferredSize ();
		
	}

	public interface IMacView : IMacAutoSizing
	{
		Size PositionOffset { get; }
		
		Size? PreferredSize { get; }
		
		Size? MinimumSize { get; set; }
		
		Control Widget { get; }
		
		Cursor Cursor { get; set; }

	}
	
	public abstract class MacView<T, W> : MacObject<T, W>, IControl, IMacView
		where T: NSView
		where W: Control
	{
		bool focus;
		NSTrackingArea tracking;
		bool mouseMove;
		NSTrackingAreaOptions mouseOptions;
		MouseDelegate mouseDelegate;
		Cursor cursor;
		Size? oldFrameSize;
		Size? naturalSize;
		
		public virtual bool AutoSize { get; protected set; }

		public virtual Size Size {
			get { return Generator.ConvertF (Control.Frame.Size); }
			set { 
				var oldSize = GetPreferredSize ();
				this.PreferredSize = value;
				Generator.SetSizeWithAuto (Control, value);
				this.AutoSize = false;
				CreateTracking ();
				LayoutIfNeeded (oldSize);
			}
		}
		
		protected virtual bool LayoutIfNeeded (Size? oldPreferredSize = null)
		{
			naturalSize = null;
			if (Widget.Loaded) {
				var oldSize = oldPreferredSize ?? Generator.ConvertF (Control.Frame.Size);
				var newSize = GetPreferredSize ();
				if (newSize != oldSize) {
					var layout = Widget.ParentLayout.Handler as IMacLayout;
					if (layout != null)
						layout.UpdateParentLayout (true);
					return true;
				}
			}
			return false;
		}

		public virtual Size? MinimumSize {
			get;
			set;
		}
		
		public virtual Size? MaximumSize {
			get;
			set;
		}
		
		public Size? PreferredSize {
			get;
			set;
		}

		public MacView ()
		{
			this.AutoSize = true;
		}

		protected virtual Size GetNaturalSize ()
		{
			if (naturalSize != null) 
				return naturalSize.Value;
			var control = Control as NSControl;
			if (control != null) {
				SD.SizeF? size = (Widget.Loaded) ? (SD.SizeF?)control.Frame.Size : null;
				control.SizeToFit ();
				naturalSize = Generator.ConvertF (control.Frame.Size);
				if (size != null) control.SetFrameSize (size.Value);
				return naturalSize.Value;
			}
			return Size.Empty;
		}
		
		public virtual Size GetPreferredSize ()
		{
			var size = GetNaturalSize ();
			if (!AutoSize && PreferredSize != null) {
				var preferredSize = PreferredSize.Value;
				if (preferredSize.Width >= 0)
					size.Width = preferredSize.Width;
				if (preferredSize.Height >= 0)
					size.Height = preferredSize.Height;
			}
			if (MinimumSize != null)
				size = Size.Max (size, MinimumSize.Value);
			if (MaximumSize != null)
				size = Size.Min (size, MaximumSize.Value);
			return size;
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
				NSTrackingAreaOptions.ActiveAlways | mouseOptions | NSTrackingAreaOptions.EnabledDuringMouseDrag | NSTrackingAreaOptions.InVisibleRect, 
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
			case Eto.Forms.Control.MouseEnterEvent:
			case Eto.Forms.Control.MouseLeaveEvent:
				HandleEvent (Eto.Forms.Control.SizeChangedEvent);
				mouseOptions |= NSTrackingAreaOptions.MouseEnteredAndExited;
				mouseMove = true;
				HandleEvent (Eto.Forms.Control.SizeChangedEvent);
				CreateTracking ();
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				mouseOptions |= NSTrackingAreaOptions.MouseMoved;
				mouseMove = true;
				HandleEvent (Eto.Forms.Control.SizeChangedEvent);
				CreateTracking ();
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				Control.PostsFrameChangedNotifications = true;
				this.AddObserver (NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) {
					var h = ((MacView<T, W>)(e.Widget.Handler));
					var oldFrameSize = h.oldFrameSize;
					h.OnSizeChanged (EventArgs.Empty);
					var newSize = e.Widget.Size;
					if (oldFrameSize == null || oldFrameSize.Value != newSize) {
						e.Widget.OnSizeChanged (EventArgs.Empty);
						h.oldFrameSize = newSize;
					}
				});
				break;
			case Eto.Forms.Control.MouseDoubleClickEvent:
			case Eto.Forms.Control.MouseDownEvent:
			case Eto.Forms.Control.MouseUpEvent:
			case Eto.Forms.Control.ShownEvent:
			case Eto.Forms.Control.HiddenEvent:
			case Eto.Forms.Control.KeyDownEvent:
			case Eto.Forms.Control.LostFocusEvent:
			case Eto.Forms.Control.GotFocusEvent:
				// TODO
				break;
			default:
				base.AttachEvent (handler);
				break;

			}
		}
		
		protected virtual void OnSizeChanged (EventArgs e)
		{
			CreateTracking ();
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

		public abstract bool Enabled { get; set; }

		public virtual bool HasFocus {
			get {
				return Control.Window != null && Control.Window.FirstResponder == ControlObject;
			}
		}

		public bool Visible {
			get { return !Control.Hidden; }
			set { Control.Hidden = !value; }
		}
		
		public Cursor Cursor {
			get { return cursor; }
			set { cursor = value; }
		}
		
		public string ToolTip {
			get { return Control.ToolTip; }
			set { Control.ToolTip = value; }
		}
		
		public virtual void OnPreLoad (EventArgs e)
		{
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

