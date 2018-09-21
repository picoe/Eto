using System;
using Eto.Forms;
using Eto.Drawing;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class SpinnerHandler : MacView<NSProgressIndicator, Spinner, Spinner.ICallback>, Spinner.IHandler
	{
		bool enabled;
		readonly NSView view;

		public override NSView ContainerControl { get { return view; } }

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return new SizeF(16, 16);
		}

		public class EtoProgressIndicator : NSProgressIndicator, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public EtoProgressIndicator()
			{
				Style = NSProgressIndicatorStyle.Spinning;
				ControlSize = NSControlSize.Regular;
			}
		}

		protected override NSProgressIndicator CreateControl()
		{
			return new EtoProgressIndicator();
		}

		public SpinnerHandler()
		{
			view = new MacEventView { Handler = this };
			view.AddSubview(Control);
		}

		protected override void Initialize()
		{
			base.Initialize();
			HandleEvent(Eto.Forms.Control.SizeChangedEvent);
		}

		public override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			var size = Math.Min(Size.Width, Size.Height);
			if (size <= 8)
				Control.ControlSize = NSControlSize.Mini;
			else if (size <= 20)
				Control.ControlSize = NSControlSize.Small;
			else //if (size <= 30)
				Control.ControlSize = NSControlSize.Regular;
			Control.SizeToFit();
			Control.CenterInParent();
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (enabled)
				Control.StartAnimation(Control);
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (enabled)
				Control.StopAnimation(Control);
		}

		protected override bool ControlEnabled
		{
			get => enabled;
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (Widget.Loaded)
					{
						if (enabled)
							Control.StartAnimation(Control);
						else
							Control.StopAnimation(Control);
					}
				}
			}
		}
	}
}

