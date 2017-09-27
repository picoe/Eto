using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.iOS.Drawing;
using SD = System.Drawing;
using Foundation;
using UIKit;
using Eto.Mac.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms
{
	public interface IIosViewControllerSource 
	{
		UIViewController Controller { get; set; }
	}

	public interface IIosView : IMacControlHandler, IIosViewControllerSource
	{
	}

	public abstract class IosView<TControl, TWidget, TCallback> : MacView<TControl, TWidget, TCallback>
		where TControl: UIView
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		public override UIView ContainerControl { get { return Control; } }
	}

	public abstract class MacView<TControl, TWidget, TCallback> : MacObject<TControl, TWidget, TCallback>, Control.IHandler, IIosView
		where TControl: NSObject
		where TWidget: Control
		where TCallback: Control.ICallback
	{
		SizeF? naturalSize;
		UIViewController controller;

		public bool IsResizing { get; set; }

		public UIViewController Controller
		{ 
			get { return controller ?? (controller = CreateController()); } 
			set { controller = value; }
		}

		protected virtual UIViewController CreateController()
		{
			return null;
		}

		public virtual UIView ContentControl { get { return ContainerControl; } }

		public virtual UIView EventControl { get { return ContainerControl; } }

		public abstract UIView ContainerControl { get; }

		public bool AutoSize { get; protected set; }

		public SizeF? PreferredSize { get; set; }

		public virtual Size MinimumSize { get; set; }

		public virtual Size? MaximumSize { get; set; }

		public virtual Size Size
		{
			get { return ContainerControl.Frame.Size.ToEtoSize(); }
			set
			{ 
				var oldSize = GetPreferredSize(SizeF.MaxValue);
				PreferredSize = value;

				var newSize = ContainerControl.Frame.Size;
				if (value.Width >= 0)
					newSize.Width = value.Width;
				if (value.Height >= 0)
					newSize.Height = value.Height;
				ContainerControl.SetFrameSize(newSize);

				AutoSize = value.Width == -1 && value.Height == -1;
				CreateTracking();
				LayoutIfNeeded(oldSize);
			}
		}

		protected virtual bool LayoutIfNeeded(SizeF? oldPreferredSize = null, bool force = false)
		{
			naturalSize = null;
			if (Widget.Loaded)
			{
				var oldSize = oldPreferredSize ?? ContainerControl.Frame.Size.ToEto();
				var newSize = GetPreferredSize(SizeF.MaxValue);
				if (newSize != oldSize || force)
				{
					var container = Widget.Parent.GetMacContainer();
					if (container != null)
						container.LayoutParent(true);
					return true;
				}
			}
			return false;
		}

		protected virtual void LayoutIfNeeded(Action action)
		{
			if (Widget.Loaded)
			{
				var size = GetPreferredSize(SizeF.MaxValue);
				action();
				LayoutIfNeeded(size);
			}
			else
				action();
		}

		protected virtual SizeF GetNaturalSize(SizeF availableSize)
		{
			if (naturalSize != null)
				return naturalSize.Value;
			var control = Control as UIView;
			if (control != null)
			{
				CoreGraphics.CGSize? size = (Widget.Loaded) ? (CoreGraphics.CGSize?)control.Frame.Size : null;
				IsResizing = true;
				control.SizeToFit();
				naturalSize = control.Frame.Size.ToEto();
				if (size != null)
					control.SetFrameSize(size.Value);
				IsResizing = false;
				return naturalSize.Value;
			}
			return Size.Empty;
		}

		public virtual SizeF GetPreferredSize(SizeF availableSize)
		{
			var size = GetNaturalSize(availableSize);
			if (!AutoSize && PreferredSize != null)
			{
				var preferredSize = PreferredSize.Value;
				if (preferredSize.Width >= 0)
					size.Width = preferredSize.Width;
				if (preferredSize.Height >= 0)
					size.Height = preferredSize.Height;
			}
			if (MinimumSize != Size.Empty)
				size = SizeF.Max(size, MinimumSize);
			if (MaximumSize != null)
				size = SizeF.Min(size, MaximumSize.Value);
			return size;
		}

		protected MacView()
		{
			this.AutoSize = true;
		}

		public virtual Size PositionOffset { get { return Size.Empty; } }

		void CreateTracking()
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

		public virtual void SetParent(Container parent)
		{
		}

		static readonly NSString frameKey = new NSString("frame");

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
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
					CreateTracking();
					break;
				case Eto.Forms.Control.SizeChangedEvent:
					AddControlObserver(frameKey, e =>
					{
						var h = e.Handler as MacView<TControl,TWidget,TCallback>;
						if (!h.IsResizing)
							h.Callback.OnSizeChanged(h.Widget, EventArgs.Empty);
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
					base.AttachEvent(handler);
					break;
			}
		}

		public virtual void Invalidate(bool invalidateChildren)
		{
			EventControl.SetNeedsDisplay();
		}

		public virtual void Invalidate(Rectangle rect, bool invalidateChildren)
		{
			EventControl.SetNeedsDisplayInRect(rect.ToNS());
		}

		public Graphics CreateGraphics()
		{
			throw new NotSupportedException();
		}

		public virtual void SuspendLayout()
		{
		}

		public virtual void ResumeLayout()
		{
		}

		public void Focus()
		{
			EventControl.BecomeFirstResponder();
		}

		public virtual Color BackgroundColor
		{
			get { return ContainerControl.BackgroundColor.ToEto(); }
			set { ContainerControl.BackgroundColor = value.ToNSUI(); }
		}

		public virtual bool Enabled
		{
			get { return EventControl.UserInteractionEnabled; }
			set { EventControl.UserInteractionEnabled = value; }
		}

		public bool HasFocus
		{
			get { return EventControl.IsFirstResponder; }
		}

		public bool Visible
		{
			get { return !ContainerControl.Hidden; }
			set
			{ 
				ContainerControl.Hidden = !value;
				LayoutIfNeeded();
			}
		}

		public virtual Font Font
		{ 
			get;
			set;
		}

		public virtual void OnPreLoad(EventArgs e)
		{
		}

		public virtual void OnLoad(EventArgs e)
		{
		}

		public virtual void OnLoadComplete(EventArgs e)
		{
		}

		public virtual void OnUnLoad(EventArgs e)
		{
		}

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { return Enumerable.Empty<string>(); }
		}

		public void MapPlatformCommand(string systemAction, Command command)
		{
		}

		public PointF PointFromScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			sdpoint = ContainerControl.ConvertPointFromView(sdpoint, null);
			sdpoint.Y = ContainerControl.Frame.Height - sdpoint.Y;
			return sdpoint.ToEto();
		}

		public PointF PointToScreen(PointF point)
		{
			var sdpoint = point.ToNS();
			sdpoint.Y = ContainerControl.Frame.Height - sdpoint.Y;
			sdpoint = ContainerControl.ConvertPointToView(sdpoint, null);
			return sdpoint.ToEto();
		}

		public Point Location
		{
			get { return ContainerControl.Frame.Location.ToEtoPoint(); }
		}


		public string ToolTip
		{
			get { return null; }
			set { }
		}

		public Cursor Cursor
		{
			get { return null; }
			set { }
		}
	}
}

