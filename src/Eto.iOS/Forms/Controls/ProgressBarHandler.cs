using System;
using UIKit;
using Eto.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class ProgressBarHandler : IosView<UIProgressView, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		public override UIView ContainerControl { get { return Control; } }

		int minValue;
		int maxValue = 100;

		public ProgressBarHandler ()
		{
			Control = new UIProgressView();
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			this.Indeterminate = false;
		}

		public int MaxValue {
			get { return maxValue; }
			set {
				var progress = Value;
				maxValue = value;
				Value = progress;
			}
		}

		public int MinValue {
			get { return minValue; }
			set { minValue = value; }
		}

		public int Value {
			get { return (int)(Control.Progress * MaxValue); }
			set {
				var val = (float)value / MaxValue;
				if (Widget.Loaded)
					Control.SetProgress (val, true);
				else
					Control.Progress = val;
			}
		}

		// TODO
		public bool Indeterminate {
			get; set;
		}
	}
}

