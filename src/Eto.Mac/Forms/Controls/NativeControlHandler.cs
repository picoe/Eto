using System;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;

using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;

namespace Eto.Mac.Forms.Controls
{
	public class NativeControlHandler : MacView<NSView, Control, Control.ICallback>
	{
		NSViewController controller;

		public NativeControlHandler(NSView nativeControl)
		{
			Control = nativeControl;
		}

		protected override void Initialize()
		{
			base.Initialize();
			AutoSize = false;
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

