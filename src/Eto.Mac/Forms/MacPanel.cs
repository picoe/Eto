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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
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
using Eto.Mac.Forms.Controls;
#endif

namespace Eto.Mac.Forms
{
	public interface IMacPanel
	{
		void PerformContentLayout();
	}

	public class MacPanelView : MacEventView
	{
		new IMacPanel Handler => base.Handler as IMacPanel;

		public MacPanelView(IntPtr handle) : base(handle)
		{
		}

		public MacPanelView()
		{
			AutoresizesSubviews = false;
		}

		public override void Layout()
		{
			if (MacView.NewLayout)
				base.Layout();
			Handler?.PerformContentLayout();
			if (!MacView.NewLayout)
				base.Layout();
		}
	}

	static class MacPanel
	{
		public static readonly object ContextMenu_Key = new object();
	}

	public abstract class MacPanel<TControl, TWidget, TCallback> : MacContainer<TControl, TWidget, TCallback>, Panel.IHandler, IMacPanel
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
				InvalidateMeasure();
			}
		}

		#if OSX
		protected virtual NSViewResizingMask ContentResizingMask() =>
						NSViewResizingMask.MaxYMargin
						| NSViewResizingMask.MaxXMargin
						| NSViewResizingMask.WidthSizable
						| NSViewResizingMask.HeightSizable;
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
					SetContent(control);
				}

				InvalidateMeasure();
			}
		}

		void SetContent(NSView control)
		{
#if OSX
			control.AutoresizingMask = ContentResizingMask();
			ContentControl.AddSubview(control); // default
#elif IOS
			control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			control.Frame = new CGRect(0, 0, ContentControl.Bounds.Width, ContentControl.Bounds.Height);
			this.AddChild(value);
#endif
		}

#if OSX

		public ContextMenu ContextMenu
		{
			get => Widget.Properties.Get<ContextMenu>(MacPanel.ContextMenu_Key);
			set
			{
				Widget.Properties.Set(MacPanel.ContextMenu_Key, value);
				EventControl.Menu = (value?.Handler as ContextMenuHandler)?.Control;
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
			var contentControl = content.GetMacControl();
			if (contentControl != null && content.Visible)
				return contentControl.GetPreferredSize(SizeF.Max(SizeF.Empty, availableSize - Padding.Size)) + Padding.Size;
			
			return Padding.Size;
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			ContentControl.NeedsLayout = true;
		}

		/// <summary>
		/// Gets the frame where the content should be placed in the ContentControl.
		/// </summary>
		/// <value>The content frame.</value>
		protected virtual CGRect ContentFrame => ContentControl.Bounds.WithPadding(Padding);

		/// <summary>
		/// Performs the content layout, should be called from NSView.Layout() only, such as with the MacPanelView.
		/// </summary>
		public virtual void PerformContentLayout()
		{
			var viewHandler = Content.GetMacViewHandler();
			if (viewHandler != null)
			{
				viewHandler.SetAlignmentFrame(ContentFrame);
			}
		}
	}
}

