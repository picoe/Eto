using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ProgressBarHandler : WpfControl<swc.ProgressBar, ProgressBar>, IProgressBar
	{
		public ProgressBarHandler()
		{
			Control = new swc.ProgressBar {
				Minimum = 0,
				Maximum = 100,
				Height = 24
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
