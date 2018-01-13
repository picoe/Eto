
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Eto.Forms;
using System.Drawing;

namespace EmbedEtoInMonoMac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController() : base("MainWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window
		{
			get { return (MainWindow)base.Window; }
		}


		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			// Get native view for the panel
			// passing true so that we can embed, otherwise we just get a reference to the control
			var nativeView = new MyEtoPanel().ToNative(true);

			nativeView.AutoresizingMask = NSViewResizingMask.MinYMargin; // anchor to top left

			// position control, keeping auto size of control
			var contentFrame = Window.ContentView.Frame;
			nativeView.SetFrameOrigin(new PointF(100, contentFrame.Height - nativeView.Frame.Height - 100));

			Window.ContentView.AddSubview(nativeView);
		}
	}
}