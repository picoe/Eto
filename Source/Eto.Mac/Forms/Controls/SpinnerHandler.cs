using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

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

		public SpinnerHandler()
		{
			Control = new NSProgressIndicator
			{
				Style = NSProgressIndicatorStyle.Spinning,
				ControlSize = NSControlSize.Regular
			};
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
			var size = Math.Max(Size.Width, Size.Height);
			if (size <= 8)
				Control.ControlSize = NSControlSize.Mini;
			else if (size <= 20)
				Control.ControlSize = NSControlSize.Small;
			else if (size <= 30)
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

		public override bool Enabled
		{
			get { return enabled; }
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

