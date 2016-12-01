using System;
using SD = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Globalization;

namespace Eto.WinForms.Forms.Controls
{
	public class NumericStepperHandler : WindowsControl<swf.NumericUpDown, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		public class EtoNumericUpDown : swf.NumericUpDown
		{
			public override void UpButton()
			{
				if (ReadOnly)
					return;
				base.UpButton();
			}

			public override void DownButton()
			{
				if (ReadOnly)
					return;
				base.DownButton();
			}
		}

		public NumericStepperHandler()
		{
			Control = new EtoNumericUpDown
			{
				Maximum = decimal.MaxValue,
				Minimum = decimal.MinValue,
				Width = 80
			};
			Control.ValueChanged += (sender, e) =>
			{
				UpdateRequiredDigits();
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			};
			Control.LostFocus += (sender, e) =>
			{
				// ensure value is always shown
				if (string.IsNullOrEmpty(Control.Text))
				{
					Control.Text = Math.Round(Math.Max(MinValue, Math.Min(MaxValue, 0)), DecimalPlaces).ToString();
				}
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
			get { return Math.Round((double)Control.Value, MaximumDecimalPlaces); }
			set
			{
				var val = Math.Max((double)Control.Minimum, Math.Min((double)Control.Maximum, value));
				Control.Value = (decimal)val;
			}
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

		public double Increment
		{
			get { return (double)Control.Increment; }
			set { Control.Increment = (decimal)value; }
		}

		static readonly object MaximumDecimalPlaces_Key = new object();

		public int MaximumDecimalPlaces
		{
			get { return Widget.Properties.Get<int>(MaximumDecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(MaximumDecimalPlaces_Key, value, () =>
				{
					DecimalPlaces = Math.Min(DecimalPlaces, value);
					UpdateRequiredDigits();
				});
			}
		}

		static readonly object DecimalPlaces_Key = new object();

		public int DecimalPlaces
		{
			get { return Widget.Properties.Get<int>(DecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(DecimalPlaces_Key, value, () =>
				{
					MaximumDecimalPlaces = Math.Max(value, MaximumDecimalPlaces);
					UpdateRequiredDigits();
				});
			}
		}

		int GetNumberOfDigits()
		{
			var str = ((double)Control.Value).ToString(CultureInfo.InvariantCulture);
			var idx = str.IndexOf(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
			return idx > 0 ? str.Length - idx - 1 : 0;
		}

		void UpdateRequiredDigits()
		{
			Control.DecimalPlaces = Math.Max(Math.Min(GetNumberOfDigits(), MaximumDecimalPlaces), DecimalPlaces);
		}
	}
}