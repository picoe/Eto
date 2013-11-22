using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SpinnerHandler : IosControl<UIActivityIndicatorView, Spinner>, ISpinner
	{
		bool enabled;

		public SpinnerHandler()
		{
			Control = new UIActivityIndicatorView
			{ 
				HidesWhenStopped = false,
				ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray
			};
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return new SizeF(16, 16);
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			if (enabled)
				Control.StartAnimating();
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (enabled)
				Control.StopAnimating();
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
							Control.StartAnimating();
						else
							Control.StopAnimating();
					}
				}
			}
		}
	}
}

