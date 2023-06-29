namespace Eto.WinForms.Forms.Controls
{
	public class ProgressBarHandler : WindowsControl<swf.ProgressBar, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		public ProgressBarHandler ()
		{
			this.Control = new swf.ProgressBar {
				Maximum = 100,
				Style = swf.ProgressBarStyle.Blocks
			};
		}

		static swf.ProgressBarStyle IndeterminateStyle
		{
			get { return (swf.Application.RenderWithVisualStyles) ? swf.ProgressBarStyle.Marquee : swf.ProgressBarStyle.Continuous; }
		}

		public bool Indeterminate {
			get { return Control.Style == swf.ProgressBarStyle.Continuous || Control.Style == swf.ProgressBarStyle.Marquee; }
			set { 
				Control.Style = value ? IndeterminateStyle : swf.ProgressBarStyle.Blocks;
			}
		}

		public int MaxValue {
			get { return Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public int MinValue {
			get { return Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public int Value {
			get { return Control.Value; }
			set { Control.Value = value; }
		}
	}
}

