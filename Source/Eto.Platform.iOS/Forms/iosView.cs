using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.iOS.Drawing;
using SD = System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms
{
	public interface IiosView
	{
		Size PositionOffset { get; }
		Size? PreferredSize { get; }
		Size? MinimumSize { get; }
		bool AutoSize { get; }
	}
	
	public interface IiosViewController
	{
		UIViewController Controller { get; }
	}

	public abstract class iosView<T, W> : iosObject<T, W>, IControl, IiosView
		where T: UIView
		where W: Control
	{

		public virtual bool AutoSize { get; protected set; }
		
		public virtual Size? PreferredSize { get; set; }
		public virtual Size? MinimumSize { get; set; }

		public virtual Size Size {
			get { return Generator.ConvertF (Control.Frame.Size); }
			set { 
				if (value != this.Size) {
					Control.SetFrameSize (Generator.ConvertF (value));
					Widget.OnSizeChanged (EventArgs.Empty);
					this.AutoSize = false;
					CreateTracking ();
				}
			}
		}

		public iosView ()
		{
			this.AutoSize = true;
		}

		public virtual Size PositionOffset { get { return Size.Empty; } } 

		void CreateTracking ()
		{
			/*
			 * use TOUCHES
			if (!mouseMove)
				return;
			 if (tracking != null)
				Control.RemoveTrackingArea (tracking);
			
			mouseDelegate = new MouseDelegate{ Widget = this.Widget, View = Control };
			tracking = new NSTrackingArea (new SD.RectangleF (new SD.PointF (0, 0), Control.Frame.Size), 
				NSTrackingAreaOptions.ActiveAlways | NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.EnabledDuringMouseDrag, 
			    mouseDelegate, 
				new NSDictionary ());
			Control.AddTrackingArea (tracking);
			*/
		}

		public virtual void SetParentLayout (Layout layout)
		{
		}

		public virtual void SetParent (Control parent)
		{
		}

		public override void AttachEvent (string handler)
		{
			base.AttachEvent (handler);
			switch (handler) {
			case Eto.Forms.Control.MouseMoveEvent:
				//mouseMove = true;
				CreateTracking ();
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
				this.AddObserver(null, UIDevice.OrientationDidChangeNotification, delegate { 
					Widget.OnSizeChanged (EventArgs.Empty);
				});
				/*Control.Window.PostsFrameChangedNotifications = true;
				this.AddObserver (UIView.UIViewFrameDidChangeNotification, delegate {
					Widget.OnSizeChanged (EventArgs.Empty); 
				});*/
				break;
			}
		}

		#region IControl implementation
		
		public virtual void Invalidate ()
		{
			Control.SetNeedsDisplay();
		}

		public virtual void Invalidate (Rectangle rect)
		{
			var region = Generator.ConvertF (rect);
			Control.SetNeedsDisplayInRect (region);
		}

		public Graphics CreateGraphics ()
		{
			return new Graphics (Widget.Generator, new GraphicsHandler (Control));
		}

		public virtual void SuspendLayout ()
		{
		}

		public virtual void ResumeLayout ()
		{
		}

		public void Focus ()
		{
			Control.BecomeFirstResponder();
		}

		public virtual Color BackgroundColor {
			get { 
				return Generator.Convert(Control.BackgroundColor);
			}
			set {
				Control.BackgroundColor = Generator.ConvertUI (value);
			}
		}

		public string Id { get; set; }

		public virtual bool Enabled { get; set; }

		public bool HasFocus {
			get {
				return Control.IsFirstResponder;
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
		}

		#endregion

	}
}

