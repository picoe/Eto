using System;
using UIKit;
using Eto.Forms;
using System.Linq;
using Eto.iOS.Forms.Controls;
using sd = System.Drawing;
using Eto.iOS.Forms.Toolbar;

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

		protected override CoreGraphics.CGRect AdjustContent(CoreGraphics.CGRect rect)
		{
			rect = base.AdjustContent(rect);
			// only navigation can take full height of a form, otherwise adjust for status bar
			if (!(Content is Navigation) || UseTopToolBar)
			{
				// adjust content outside of status bar
				var sbheight = ApplicationHandler.Instance.StatusBarAdjustment;
				rect.Y += sbheight;
				rect.Height -= sbheight;
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

		public bool ShowActivated { get; set; }

		public bool CanFocus { get; set; }
	}
}

