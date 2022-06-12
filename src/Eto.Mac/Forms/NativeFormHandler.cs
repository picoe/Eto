using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Mac.Forms
{
	public class NativeFormHandler : FormHandler
	{
		public NativeFormHandler(NSWindow window)
			: base(window)
		{
		}
		public NativeFormHandler(NSWindowController windowController)
			: base(windowController)
		{
		}

		public override void AttachEvent(string id)
		{
			// native window, so attach observers instead of using the delegate so we don't clobber existing functionality
			switch (id)
			{
				case Window.ClosedEvent:
					AddObserver(NSWindow.WillCloseNotification, n => Callback.OnClosed(Widget, EventArgs.Empty));
					break;
				case Window.SizeChangedEvent:
					AddObserver(NSWindow.DidResizeNotification, n => Callback.OnSizeChanged(Widget, EventArgs.Empty));
					break;
				case Window.GotFocusEvent:
					AddObserver(NSWindow.DidBecomeKeyNotification, n => Callback.OnGotFocus(Widget, EventArgs.Empty));
					break;
				case Window.LostFocusEvent:
					AddObserver(NSWindow.DidResignKeyNotification, n => Callback.OnLostFocus(Widget, EventArgs.Empty));
					break;
			}
			return;
		}

		protected override void ConfigureWindow()
		{
		}

		public override Size Size
		{
			get => Control.Frame.Size.ToEtoSize();
			set => base.Size = value;
		}
	}
}

