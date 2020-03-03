using System;

using AppKit;
using CoreGraphics;
using Eto.Forms;
using Foundation;

namespace EmbedEtoInXamarinMac
{
	public partial class ViewController : NSViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			// Get native view for the panel
			// passing true so that we can embed, otherwise we just get a reference to the control
			var nativeView = new MyEtoPanel().ToNative(true);

			nativeView.AutoresizingMask = NSViewResizingMask.MinYMargin; // anchor to top left

			// position control, keeping auto size of control
			var contentFrame = View.Frame;
			nativeView.SetFrameOrigin(new CGPoint(100, contentFrame.Height - nativeView.Frame.Height - 100));

			this.View.AddSubview(nativeView);

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Do any additional setup after loading the view.
		}

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
