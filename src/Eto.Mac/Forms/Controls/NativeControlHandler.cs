using System;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;


namespace Eto.Mac.Forms.Controls
{
	public class NativeControlHandler : MacView<NSView, Control, Control.ICallback>
	{
		NSViewController controller;

		public NativeControlHandler(NSView nativeControl)
		{
			Control = nativeControl;
		}

		public override SizeF GetPreferredSize(SizeF availableSize)
		{
			return Control.FittingSize.ToEto();
		}

		public NativeControlHandler(NSViewController nativeControl)
		{
			controller = nativeControl;
			Control = controller.View;
		}

		public override NSView ContainerControl { get { return Control; } }
	}
}

