using sw = Windows.UI.Xaml;
using swc = Windows.UI.Xaml.Controls;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Progress bar handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ProgressBarHandler : WpfControl<swc.ProgressBar, ProgressBar>, IProgressBar
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
