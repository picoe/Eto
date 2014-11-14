using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class ProgressBarHandler : WpfControl<swc.ProgressBar, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		protected override Size DefaultSize { get { return new Size(-1, 22); } }

		public ProgressBarHandler()
		{
			Control = new swc.ProgressBar {
				Minimum = 0,
				Maximum = 100,
			};
		}

		public int MaxValue
		{
			get { return (int)Control.Maximum; }
			set { Control.Maximum = value; }
		}

		public int MinValue
		{
			get { return (int)Control.Minimum; }
			set { Control.Minimum = value; }
		}

		public int Value
		{
			get { return (int)Control.Value; }
			set { Control.Value = value; }
		}

		public bool Indeterminate
		{
			get { return Control.IsIndeterminate; }
			set { Control.IsIndeterminate = value; }
		}

	}
}
