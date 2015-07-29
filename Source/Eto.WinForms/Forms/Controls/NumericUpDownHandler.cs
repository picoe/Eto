using System;
using SD = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Controls
{
	public class NumericUpDownHandler : WindowsControl<swf.NumericUpDown, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		public NumericUpDownHandler()
		{
			Control = new swf.NumericUpDown
			{
				Maximum = decimal.MaxValue,
				Minimum = decimal.MinValue,
				Width = 80
			};
			Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
			Control.LostFocus += (sender, e) =>
			{
				// ensure value is always shown
				if (string.IsNullOrEmpty(Control.Text))
					Control.Text = Math.Round(Math.Max(MinValue, Math.Min(MaxValue, 0)), DecimalPlaces).ToString();
			};
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			LeakHelper.UnhookObject(Control);
		}

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public double Value
		{
			get { return Math.Round((double)Control.Value, DecimalPlaces); }
			set { Control.Value = Math.Max(Control.Minimum, Math.Min(Control.Maximum, (decimal)value)); }
		}

		public double MinValue
		{
			get { return Control.Minimum == decimal.MinValue ? double.NegativeInfinity : (double)Control.Minimum; }
			set { Control.Minimum = double.IsNegativeInfinity(value) ? decimal.MinValue : (decimal)value; }
		}

		public double MaxValue
		{
			get { return Control.Maximum == decimal.MaxValue ? double.PositiveInfinity : (double)Control.Maximum; }
			set { Control.Maximum = double.IsPositiveInfinity(value) ? decimal.MaxValue : (decimal)value; }
		}

		public int DecimalPlaces
		{
			get { return Control.DecimalPlaces; }
			set { Control.DecimalPlaces = value; }
		}

		public double Increment
		{
			get { return (double)Control.Increment; }
			set { Control.Increment = (decimal)value; }
		}
	}
}