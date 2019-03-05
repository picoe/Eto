using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wf = System.Windows;
using swc = System.Windows.Controls;
using swcp = System.Windows.Controls.Primitives;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoToggleButton : swcp.ToggleButton, IEtoWpfControl
	{
		public IWpfFrameworkElement Handler { get; set; }

		protected override wf.Size MeasureOverride(wf.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}
	}

	public class ToggleButtonHandler : ButtonHandler<EtoToggleButton, ToggleButton, ToggleButton.ICallback>, ToggleButton.IHandler
	{
		protected override EtoToggleButton CreateControl() => new EtoToggleButton { Handler = this };

		protected override Size GetDefaultMinimumSize() => new Size(23, 23);

		public bool Checked
		{
			get => Control.IsChecked ?? false;
			set => Control.IsChecked = value;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ToggleButton.CheckedChangedEvent:
					AttachPropertyChanged(EtoToggleButton.IsCheckedProperty, OnCheckedChanged);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void OnCheckedChanged(object sender, EventArgs e)
		{
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);
		}
	}
}
