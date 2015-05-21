using System;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
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

namespace Eto.Mac.Forms
{
	public class FormHandler : MacWindow<NSWindow, Form, Form.ICallback>, Form.IHandler
	{
		#pragma warning disable 414
		// keep reference to controller so it doesn't get disposed
		NSWindowController controller;
		#pragma warning restore 414
		protected override bool DisposeControl { get { return false; } }

		public FormHandler(NSWindow window)
		{
			Control = window;
		}

		public FormHandler(NSWindowController controller)
		{
			this.controller = controller;
			Control = controller.Window;
		}

		public FormHandler()
		{
			Control = new MyWindow(new CGRect(0, 0, 200, 200), 
			                       NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled, 
			                       NSBackingStore.Buffered, false);
			ConfigureWindow();
		}

		public void Show()
		{
			var visible = Control.IsVisible;
			if (WindowState == WindowState.Minimized)
				Control.MakeKeyWindow();
			else
				Control.MakeKeyAndOrderFront(ApplicationHandler.Instance.AppDelegate);
			if (!visible)
				Callback.OnShown(Widget, EventArgs.Empty);
		}
	}
}
