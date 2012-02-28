using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ProgressBarHandler : WindowsControl<SWF.ProgressBar, ProgressBar>, IProgressBar
	{

		public ProgressBarHandler()
		{
            this.Control = new SWF.ProgressBar {
                Maximum = 100
            };
		}

        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

		public int MaxValue {
			get { return this.Control.Maximum; }
			set { this.Control.Maximum = value; }
		}

		public int MinValue {
			get { return this.Control.Minimum; }
			set { this.Control.Minimum = value; }
		}

		public int Value {
			get { return this.Control.Value; }
			set { this.Control.Value = value; }
		}
	}
}

