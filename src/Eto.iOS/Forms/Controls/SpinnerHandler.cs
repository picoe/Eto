using System;
using UIKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class SpinnerHandler : IosView<UIActivityIndicatorView, Spinner, Spinner.ICallback>, Spinner.IHandler
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

		protected override void Initialize()
		{
			base.Initialize();
			Widget.SizeChanged += HandleSizeChanged;
		}

		void HandleSizeChanged (object sender, EventArgs e)
		{
			var size = Math.Min(Size.Width, Size.Height);
			if (size < 32)
				Control.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
			else
			{
				Control.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
				Control.Color = UIColor.Gray;
			}
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

