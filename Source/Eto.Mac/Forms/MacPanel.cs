using System;
using Eto.Forms;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

#if IOS
using NSResponder = UIKit.UIResponder;
using NSView = UIKit.UIView;
using Eto.iOS.Forms;
using UIKit;
using Foundation;
using CoreGraphics;
#elif OSX
using Eto.Mac.Forms.Menu;
#endif

namespace Eto.Mac.Forms
{

	public abstract class MacPanel<TControl, TWidget, TCallback> : MacContainer<TControl, TWidget, TCallback>, Panel.IHandler
		where TControl: NSObject
		where TWidget: Panel
		where TCallback: Panel.ICallback
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
					control.Frame = new CGRect(ContentControl.Bounds.X, ContentControl.Bounds.Y, ContentControl.Bounds.Width, ContentControl.Bounds.Height);
					container.AddSubview(control); // default
#elif IOS
					control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
					control.Frame = new CGRect(0, 0, ContentControl.Bounds.Width, ContentControl.Bounds.Height);
					this.AddChild(value);
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
				EventControl.Menu = contextMenu != null ? ((ContextMenuHandler)contextMenu.Handler).Control : null;
			}
		}
#else
		public virtual ContextMenu ContextMenu
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

		protected virtual CGRect GetContentBounds()
		{
			return ContentControl.Bounds;
		}

		protected virtual CGRect AdjustContent(CGRect rect)
		{
			return rect;
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
				frame.X += padding.Left;
				frame.Width -= padding.Horizontal;
				frame.Y += padding.Bottom;
				frame.Height -= padding.Vertical;
			}
			else
			{
				frame.X = 0;
				frame.Y = 0;
			}
			frame = AdjustContent(frame);

			if (childControl.Frame != frame)
				childControl.Frame = frame;
		}

		public override void SetContentSize(CGSize contentSize)
		{
			base.SetContentSize(contentSize);
			if (MinimumSize != Size.Empty)
			{
				contentSize.Width = (nfloat)Math.Max(contentSize.Width, MinimumSize.Width);
				contentSize.Height = (nfloat)Math.Max(contentSize.Height, MinimumSize.Height);
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

		void HandleSizeChanged(object sender, EventArgs e)
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

