using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using System.ComponentModel;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoDoubleUpDown : mwc.DoubleUpDown
	{
		public new swc.TextBox TextBox { get { return base.TextBox; } }
	}

	public class NumericUpDownHandler : WpfControl<EtoDoubleUpDown, NumericUpDown, NumericUpDown.ICallback>, NumericUpDown.IHandler
	{
		double lastValue;

		protected override Size DefaultSize { get { return new Size(80, -1); } }

		protected override bool PreventUserResize { get { return true; } }

		public NumericUpDownHandler()
		{
			Control = new EtoDoubleUpDown
			{
				FormatString = "0",
				Value = 0
			};
			Control.ValueChanged += (sender, e) =>
			{
				if (Control.Value == null)
					Control.Value = Math.Max(MinValue, Math.Min(MaxValue, 0));
				else
					TriggerValueChanged();
			};
			Control.Loaded += Control_Loaded;
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// ensure changed event fires when changing the text, not just when focus is lost
			if (Control.TextBox != null)
			{
				Control.TextBox.TextChanged += (sender2, e2) => TriggerValueChanged();
				Control.Loaded -= Control_Loaded;
			}
		}

		void TriggerValueChanged()
		{
			var val = Value;
			if (lastValue != val)
			{
				Callback.OnValueChanged(Widget, EventArgs.Empty);
				lastValue = val;
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool ReadOnly
		{
			get { return !Control.IsEnabled; }
			set { Control.IsEnabled = !value; }
		}

		public double Value
		{
			get { return Math.Round(Control.Value ?? 0, DecimalPlaces); }
			set { Control.Value = Math.Max(MinValue, Math.Min(MaxValue, value)); }
		}

		public double MinValue
		{
			get { return Control.Minimum ?? double.MinValue; }
			set
			{
				Control.Minimum = value;
				Control.Value = Math.Max(value, Value);
			}
		}

		public double MaxValue
		{
			get { return Control.Maximum ?? double.MaxValue; }
			set
			{
				Control.Maximum = value;
				Control.Value = Math.Min(value, Value);
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
					var decimalPlaces = DecimalPlaces;
					Control.FormatString = decimalPlaces > 0 ? "0." + new string('0', decimalPlaces) : "0";
				});
			}
		}

		public double Increment
		{
			get { return Control.Increment ?? 1; }
			set { Control.Increment = value; }
		}

		public override void Focus()
		{
			// focus the inner text box
			if (Control.IsLoaded)
				Control.TextBox.Focus();
			else
				Control.Loaded += HandleFocus;
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
			Control.TextBox.Focus();
			Control.Loaded -= HandleFocus;
		}

	}
}
