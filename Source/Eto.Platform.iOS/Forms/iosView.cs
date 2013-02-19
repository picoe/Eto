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
		Size GetPreferredSize (Size availableSize);
		Size? MinimumSize { get; }
		bool AutoSize { get; }
		UIView ContainerControl { get; }
	}
	
	public interface IiosViewController
	{
		UIViewController Controller { get; }
	}

	public static class ViewExtensions
	{
		public static void AddSubView(this Container parent, Control control)
		{

			var parentViewController = parent.Handler as IiosViewController;
			if (parentViewController != null) {
				var viewController = control.Handler as IiosViewController;
				if (viewController != null) {
					parentViewController.Controller.AddChildViewController(viewController.Controller);
					return;
				}
			}
			var viewHandler = control.Handler as IiosView;
			var view = viewHandler != null ? viewHandler.ContainerControl : control.ControlObject as UIView;

			var parentView = parent.Handler as IiosContainer;
			if (parentView != null && view != null) {
				parentView.ContentControl.AddSubview (view);
			}
		}
	}


	public abstract class iosView<T, W> : iosObject<T, W>, IControl, IiosView
		where T: UIView
		where W: Control
	{
		Size? naturalSize;

		public virtual UIViewController Controller { get { return null; } }

		public virtual UIView ContainerControl
		{
			get { return (UIView)Control; }
		}

		public virtual bool AutoSize { get; protected set; }

		public Size? PreferredSize { get; set; }

		protected virtual Size GetNaturalSize ()
		{
			if (naturalSize != null) 
				return naturalSize.Value;
			var control = Control as UIView;
			if (control != null) {
				naturalSize = control.SizeThatFits(UIView.UILayoutFittingCompressedSize).ToEtoSize ();
				return naturalSize.Value;
			}
			return Size.Empty;
		}
		
		public virtual Size GetPreferredSize (Size availableSize)
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


		public virtual Size? MinimumSize { get; set; }
		public virtual Size? MaximumSize { get; set; }

		public virtual Size Size {
			get { return Control.Frame.Size.ToEtoSize (); }
			set { 
				if (value != this.Size) {
					PreferredSize = value;
					Control.SetFrameSize (value.ToSDSizeF ());
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
			switch (handler) {
			case Eto.Forms.Control.MouseDownEvent:
			case Eto.Forms.Control.MouseUpEvent:
			case Eto.Forms.Control.MouseDoubleClickEvent:
			case Eto.Forms.Control.MouseEnterEvent:
			case Eto.Forms.Control.MouseLeaveEvent:
			case Eto.Forms.Control.KeyDownEvent:
			case Eto.Forms.Control.GotFocusEvent:
			case Eto.Forms.Control.LostFocusEvent:
				break;
			case Eto.Forms.Control.MouseMoveEvent:
				//mouseMove = true;
				CreateTracking ();
				break;
			case Eto.Forms.Control.SizeChangedEvent:
				this.AddControlObserver(new NSString("frame"), delegate {
					Widget.OnSizeChanged (EventArgs.Empty);
				});
				/*UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
				this.AddObserver(null, UIDevice.OrientationDidChangeNotification, delegate {
					Widget.OnSizeChanged (EventArgs.Empty);
				});*/
				/*Control.Window.PostsFrameChangedNotifications = true;
				this.AddObserver (UIView.UIViewFrameDidChangeNotification, delegate {
					Widget.OnSizeChanged (EventArgs.Empty); 
				});*/
				break;
			default:
				base.AttachEvent (handler);
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
			Control.SetNeedsDisplayInRect (rect.ToSDRectangleF ());
		}

		public Graphics CreateGraphics ()
		{
			throw new NotSupportedException ();
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
			get { return Control.BackgroundColor.ToEto (); }
			set { Control.BackgroundColor = value.ToUI (); }
		}

		public virtual bool Enabled {
			get { return Control.UserInteractionEnabled; }
			set { Control.UserInteractionEnabled = value; }
		}

		public bool HasFocus {
			get { return Control.IsFirstResponder; }
		}

		public bool Visible {
			get { return !Control.Hidden; }
			set { Control.Hidden = !value; }
		}

		public virtual Font Font { 
			get; set;
		}
		
		public virtual void OnPreLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoad (EventArgs e)
		{
		}
		
		public virtual void OnLoadComplete (EventArgs e)
		{
		}

		#endregion

		public void MapPlatformAction (string systemAction, BaseAction action)
		{
		}
	}
}

