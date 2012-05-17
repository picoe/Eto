using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class ProgressBarHandler : MacView<NSProgressIndicator, ProgressBar>, IProgressBar
	{
		public class EtoSlider : NSProgressIndicator, IMacControl
		{
			public object Handler { get; set; }

		}

		public ProgressBarHandler ()
		{
			Control = new EtoSlider { Handler = this, Indeterminate = false };

			MinValue = 0;
			MaxValue = 100;
		}

		protected override Size GetNaturalSize ()
		{
			return new Size (80, 30);
		}

		public override bool Enabled {
			get { return true; }
			set {  }
		}

		public bool Indeterminate {
			get { return Control.Indeterminate; }
			set { 
				Control.Indeterminate = value;
				if (value)
					Control.StartAnimation (Control);
				else
					Control.StopAnimation (Control);
			}
		}

		public int MaxValue {
			get { return (int)Control.MaxValue; }
			set { 
				Control.MaxValue = value;
			}
		}
		
		public int MinValue {
			get { return (int)Control.MinValue; }
			set {
				Control.MinValue = value;
			}
		}

		public int Value {
			get { return (int)Control.DoubleValue; }
			set { Control.DoubleValue = value; }
		}
	}
}

