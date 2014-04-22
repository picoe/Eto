using System;
using Eto.Forms;
using Eto.Drawing;
using SD = System.Drawing;
using MonoTouch.UIKit;

#if IOS
using NSResponder = MonoTouch.UIKit.UIResponder;
using NSView = MonoTouch.UIKit.UIView;
#elif OSX
using MonoMac.AppKit;
using Eto.Platform.Mac.Forms.Menu;
#endif

namespace Eto.Platform.Mac.Forms
{
#if IOS
	public static class UIViewExtensions
	{
		/// <summary>
		/// See http://stackoverflow.com/a/2596519/90291
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public static UIViewController GetViewController(this UIView view)
		{
			return view.NextResponder as UIViewController;
		}

		/// <summary>
		/// An extension method that adds a subview to a parent view.
		/// Also adds the subview's view controller to the parent's view
		/// controller, creating it if needed, provided the parent view controller
		/// exists.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		public static void ContainerAddSubView(this UIView parent, UIView child)
		{
			var parentViewController = parent.GetViewController();
			var childViewController = child.GetViewController();

			if (parentViewController != null)
			{
				if (childViewController == null)
					childViewController = new Eto.Platform.iOS.Forms.RotatableViewController { View = child };
				parentViewController.AddChildViewController(childViewController);
			}
			// Note: pass through to AddSubView below.
			// Adding a child view controller still requires adding the subview.
			// see http://stackoverflow.com/questions/10143903/do-i-have-to-call-addsubview-after-calling-addchildviewcontroller
			parent.AddSubview(child);
		}
	}
#endif

	public abstract class MacPanel<TControl, TWidget> : MacContainer<TControl, TWidget>, IPanel
		where TControl: NSResponder
		where TWidget: Panel
	{
		Control content;
		Padding padding;

		public Padding Padding
		{
			get { return padding; }
			set
			{
				padding = value;
				LayoutParent();
			}
		}

		#if OSX
		protected virtual NSViewResizingMask ContentResizingMask()
		{
			return NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
		}
		#endif

		public Control Content
		{
			get { return content; }
			set
			{
				if (content != null)
				{ 
					var oldContent = content.GetContainerView();
					oldContent.RemoveFromSuperview();
				}

				content = value;
				var control = value.GetContainerView();
				if (control != null)
				{
					var container = ContentControl;
#if OSX
					control.AutoresizingMask = ContentResizingMask();
					container.AddSubview(control); // default
#elif IOS
					control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
					container.ContainerAddSubView(control);
#endif
				}

				if (Widget.Loaded)
				{
					LayoutParent();
				}
			}
		}

#if OSX
		ContextMenu contextMenu;
		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set
			{
				contextMenu = value;
				Control.Menu = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
			}
		}
#else
		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
#endif
		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (content == null || !content.Visible)
				return Padding.Size;

			var contentControl = content.GetMacControl();
			if (contentControl != null)
				return contentControl.GetPreferredSize(availableSize) + Padding.Size;
			return Padding.Size;
		}

		protected virtual SD.RectangleF GetContentBounds()
		{
			return ContentControl.Bounds;
		}

		public override void LayoutChildren()
		{
			base.LayoutChildren();

			if (content == null)
				return;

			NSView childControl = content.GetContainerView();
			var frame = GetContentBounds();

			if (frame.Width > padding.Horizontal && frame.Height > padding.Vertical)
			{
				frame.X = padding.Left;
				frame.Width -= padding.Horizontal;
				frame.Y = padding.Bottom;
				frame.Height -= padding.Vertical;
			}
			else
			{
				frame.X = 0;
				frame.Y = 0;
			}

			if (childControl.Frame != frame)
				childControl.Frame = frame;
		}

		public override void SetContentSize(SD.SizeF contentSize)
		{
			base.SetContentSize(contentSize);
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = Math.Max(contentSize.Height, MinimumSize.Height);
			}
			if (Widget.Content != null)
			{
				var child = Widget.Content.Handler as IMacContainer;
				if (child != null)
				{
					child.SetContentSize(contentSize);
				}
			}
		}

		bool isResizing;
		void HandleSizeChanged (object sender, EventArgs e)
		{
			if (!isResizing)
			{
				isResizing = true;
				LayoutChildren();
				isResizing = false;
			}
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			LayoutChildren();
			Widget.SizeChanged += HandleSizeChanged;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			Widget.SizeChanged -= HandleSizeChanged;
		}
	}
}

