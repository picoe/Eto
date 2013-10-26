using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ProgressBarHandler : WindowsControl<SWF.ProgressBar, ProgressBar>, IProgressBar
	{
		public ProgressBarHandler ()
		{
			this.Control = new SWF.ProgressBar {
				Maximum = 100,
				Style = SWF.ProgressBarStyle.Blocks
			};
		}

		static SWF.ProgressBarStyle IndeterminateStyle
		{
			get { return (SWF.Application.RenderWithVisualStyles) ? SWF.ProgressBarStyle.Marquee : SWF.ProgressBarStyle.Continuous; }
		}

		public bool Indeterminate {
			get { return Control.Style == SWF.ProgressBarStyle.Continuous || Control.Style == SWF.ProgressBarStyle.Marquee; }
			set { 
				Control.Style = value ? IndeterminateStyle : SWF.ProgressBarStyle.Blocks;
			}
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

