using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Forms;
using Eto.Drawing;
using System;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoProgressBar : swc.ProgressBar, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}

		bool _isIndeterminate;
		public new bool IsIndeterminate
		{
			get { return _isIndeterminate; }
			set
			{
				if (_isIndeterminate != value)
				{
					if (IsLoaded)
						base.IsIndeterminate = value;
					_isIndeterminate = value;
				}
			}
		}

		public EtoProgressBar()
		{
			// WPF will keep posting to the message loop after a dialog is closed with IsIndeterminate = true
			Loaded += (sender, e) => base.IsIndeterminate = IsIndeterminate;
			Unloaded += (sender, e) => base.IsIndeterminate = false;
		}
	}

	public class ProgressBarHandler : WpfControl<EtoProgressBar, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		protected override sw.Size DefaultSize => new sw.Size(100, 22);

		public ProgressBarHandler()
		{
			Control = new EtoProgressBar
			{
				Handler = this,
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
