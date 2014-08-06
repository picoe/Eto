using System;
using Eto.Forms;
using MonoMac.AppKit;
using SD = System.Drawing;
using MonoMac.Foundation;
#if Mac64
using CGFloat = System.Double;
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
#else
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
using CGFloat = System.Single;
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
#endif

namespace Eto.Mac.Forms
{
	public class FormHandler : MacWindow<MyWindow, Form, Form.ICallback>, Form.IHandler
	{
		protected override bool DisposeControl { get { return false; } }

		public FormHandler()
		{
			Control = new MyWindow(new NSRect(0, 0, 200, 200), 
			                       NSWindowStyle.Resizable | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Titled, 
			                       NSBackingStore.Buffered, false);
			ConfigureWindow();
		}

		public void Show()
		{
			if (WindowState == WindowState.Minimized)
				Control.MakeKeyWindow();
			else
				Control.MakeKeyAndOrderFront(ApplicationHandler.Instance.AppDelegate);
			if (!Control.IsVisible)
				Callback.OnShown(Widget, EventArgs.Empty);
		}
	}
}
