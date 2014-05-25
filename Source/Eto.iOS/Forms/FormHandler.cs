using System;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Linq;
using Eto.iOS.Forms.Controls;
using sd = System.Drawing;

namespace Eto.iOS.Forms
{
	public class FormHandler : IosWindow<UIWindow, Form, Form.ICallback>, Form.IHandler
	{
		public FormHandler()
		{
			Control = new UIWindow(UIScreen.MainScreen.Bounds);
			Control.AutosizesSubviews = true;
			Control.RootViewController = new RotatableViewController();
			Control.RootViewController.View.AutosizesSubviews = true;
			Control.RootViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
		}

		protected override UIViewController CreateController()
		{
			return Control.RootViewController;
		}

		public override UIView ContentControl
		{
			get { return Control.RootViewController.View; }
		}

		public override string Title
		{
			get { return null; }
			set { }
		}

		protected override sd.RectangleF AdjustContent(sd.RectangleF rect)
		{
			if (Platform.IsIpad)
			{
				// need to adjust content outside of status bar for ipad, unlike iphone
				var sbheight = ApplicationHandler.Instance.StatusBarAdjustment;
				rect.Y += sbheight;
				rect.Height -= sbheight;
			}
			if (ToolBar != null)
			{
				var tbheight = ToolBarHandler.GetControl(ToolBar).Frame.Height;
				rect.Height -= tbheight;
				if (ToolBar.Dock == ToolBarDock.Top)
					rect.Y += tbheight;
			}
			return rect;
		}

		public override void Close()
		{
			// cannot close main form
			if (Widget != Application.Instance.MainForm)
			{
				Control.RemoveFromSuperview();
				Control.ResignKeyWindow();
				Control.Hidden = true;
				Callback.OnClosed(Widget, EventArgs.Empty);
			}
		}

		public void Show()
		{
			Control.MakeKeyAndVisible();
			Control.BecomeFirstResponder();
		}

		public override void SendToBack()
		{
			Control.ResignKeyWindow();
			Control.ResignFirstResponder();
			Control.Hidden = true;
		}

		public override void BringToFront()
		{
			Control.MakeKeyAndVisible();
		}
	}
}

