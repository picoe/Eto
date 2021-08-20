using System;
using Eto.Drawing;
using swc = System.Windows.Controls;
using sw = System.Windows;
using Eto.Forms;
using mwc = Xceed.Wpf.Toolkit;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace Eto.Wpf.Forms.Controls
{
	public class EtoDoubleUpDown : mwc.DoubleUpDown, IEtoWpfControl
	{
		public new swc.TextBox TextBox { get { return base.TextBox; } }
		public new mwc.Spinner Spinner { get { return base.Spinner; } }

		public IWpfFrameworkElement Handler { get; set; }

		NumericStepperHandler StepperHandler => Handler as NumericStepperHandler;

		protected override sw.Size MeasureOverride(sw.Size constraint)
		{
			return Handler?.MeasureOverride(constraint, base.MeasureOverride) ?? base.MeasureOverride(constraint);
		}

		string TrimNumericString(string text) => Regex.Replace(text, $"[ ]|({Regex.Escape(StepperHandler.CultureInfo.NumberFormat.NumberGroupSeparator)})", "");

		bool NumberStringsMatch(string num1, string num2) => string.Compare(TrimNumericString(num1), TrimNumericString(num2), StepperHandler.CultureInfo, CompareOptions.IgnoreCase) == 0;

		protected override double? ConvertTextToValue(string text)
		{
			var h = StepperHandler;
			var trimmedText = text;
			if (h?.HasFormatString == true && trimmedText != null)
				trimmedText = Regex.Replace(trimmedText, $@"(?!\d|{Regex.Escape(h.CultureInfo.NumberFormat.NumberDecimalSeparator)}|{Regex.Escape(h.CultureInfo.NumberFormat.NegativeSign)}).", ""); // strip any non-numeric value
			try
			{
				var result = base.ConvertTextToValue(trimmedText);

				// test if the text matches the negative format
				if (h.HasFormatString && result > 0 && NumberStringsMatch((-result.Value).ToString(FormatString, CultureInfo), text))
					result = -result;

				return result;
			}
			catch
			{
				return null;
			}
		}

		protected override string ConvertValueToText()
		{
			return base.ConvertValueToText();
		}
	}

	public class NumericStepperHandler : WpfControl<EtoDoubleUpDown, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		double lastValue;

		protected override sw.Size DefaultSize => new sw.Size(80, double.NaN);

		protected override bool PreventUserResize { get { return true; } }

		public NumericStepperHandler()
		{
			Control = new EtoDoubleUpDown
			{
				Handler = this,
				FormatString = "0",
				Value = 0
			};
			Control.ValueChanged += Control_ValueChanged;
			Control.Loaded += Control_Loaded;
		}

		static double GetPreciseValue(double value)
		{
			// prevent spinner from accumulating an inprecise value, which would eventually 
			// show values like 1.0000000000001 or 1.999999999998
			var str = value.ToString("G15");
			if (double.TryParse(str, out var val))
				return val;
			else
				return value;
		}

		int valueChanging;
		private void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (valueChanging > 0)
				return;
			var val = GetPreciseValue(Control.Value ?? 0);

			if (Wrap)
			{
				if (val < MinValue)
					val = MaxValue;
				else if (val > MaxValue)
					val = MinValue;
			}
			val = Math.Max(MinValue, Math.Min(MaxValue, val));
			valueChanging++;
			Control.Value = val;
			valueChanging--;
			TriggerValueChanged();
			var textBox = Control.TextBox;
			if (val != Control.Value && textBox != null)
			{
				// callback set a different value, but it still shows just whatever the user typed in.
				// possibly due to some async call in Xceed.Wpf.Toolkit.
				// so, we set the text specifically.
				valueChanging++;
				textBox.Text = GetPreciseValue(Control.Value.Value).ToString(Control.FormatString, Control.CultureInfo);
				textBox.SelectionStart = textBox.Text.Length;
				textBox.SelectionLength = 0;
				valueChanging--;
			}
		}

		void Control_Loaded(object sender, sw.RoutedEventArgs e)
		{
			// ensure changed event fires when changing the text, not just when focus is lost
			if (Control.TextBox != null)
			{
				Control.TextBox.TextChanged += TextBox_TextChanged;
				if (Control.Spinner != null)
					Control.Spinner.GotKeyboardFocus += (sender2, e2) => Control.TextBox?.Focus();
				Control.Loaded -= Control_Loaded;
			}
		}

		private void TextBox_TextChanged(object sender, swc.TextChangedEventArgs e)
		{
			if (valueChanging > 0)
				return;
				
			var val = Value;
			var newval = GetPreciseValue(val);
			if (newval != val)
			{
				valueChanging++;
				Control.Value = newval;
				valueChanging--;
			}
			TriggerValueChanged();
		}

		void TriggerValueChanged()
		{
			var val = Value;
			if (lastValue != val)
			{
				lastValue = val;
				Callback.OnValueChanged(Widget, EventArgs.Empty);
			}
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set { Control.IsReadOnly = value; }
		}

		public double Value
		{
			get { return HasFormatString ? Control.Value ?? 0 : Math.Round(Control.Value ?? 0, MaximumDecimalPlaces); }
			set { Control.Value = Math.Max(MinValue, Math.Min(MaxValue, value)); }
		}

		static readonly object MinValue_Key = new object();

		public double MinValue
		{
			get => Widget.Properties.Get(MinValue_Key, double.MinValue);
			set
			{
				if (Widget.Properties.TrySet(MinValue_Key, value, double.MinValue))
				{
					SetMinMaxValues();
					Control.Value = Math.Max(value, Value);
				}
			}
		}

		static readonly object MaxValue_Key = new object();

		public double MaxValue
		{
			get => Widget.Properties.Get(MaxValue_Key, double.MaxValue);
			set
			{
				if (Widget.Properties.TrySet(MaxValue_Key, value, double.MaxValue))
				{
					SetMinMaxValues();
					Control.Value = Math.Min(value, Value);
				}
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
					if (MaximumDecimalPlaces < value)
						MaximumDecimalPlaces = value;
					UpdateRequiredDigits();
				});
			}
		}

		public double Increment
		{
			get => Control.Increment ?? 1;
			set
			{
				Control.Increment = value;
				SetMinMaxValues();
			}
		}

		public override void Focus()
		{
			// focus the inner text box
			if (Control.IsLoaded)
				Control.TextBox?.Focus();
			else
				Control.Loaded += HandleFocus;
		}

		void HandleFocus(object sender, sw.RoutedEventArgs e)
		{
			Control.TextBox?.Focus();
			Control.Loaded -= HandleFocus;
		}

		static readonly object MaxiumumDecimalPlaces_Key = new object();

		public int MaximumDecimalPlaces
		{
			get { return Widget.Properties.Get<int>(MaxiumumDecimalPlaces_Key); }
			set
			{
				Widget.Properties.Set(MaxiumumDecimalPlaces_Key, value, () =>
				{
					if (DecimalPlaces > value)
						DecimalPlaces = value;
					UpdateRequiredDigits();
				});
			}
		}

		internal bool HasFormatString => !string.IsNullOrEmpty(FormatString);

		static readonly object FormatString_Key = new object();

		public string FormatString
		{
			get { return Widget.Properties.Get<string>(FormatString_Key); }
			set
			{
				Widget.Properties.Set(FormatString_Key, value, UpdateRequiredDigits);
			}
		}

		public CultureInfo CultureInfo
		{
			get { return Control.CultureInfo; }
			set { Control.CultureInfo = value; }
		}

		static readonly object Wrap_Key = new object();

		public bool Wrap
		{
			get => Widget.Properties.Get<bool>(Wrap_Key);
			set
			{
				if (Widget.Properties.TrySet(Wrap_Key, value))
					SetMinMaxValues();
			}
		}

		void SetMinMaxValues()
		{
			var min = MinValue;
			var max = MaxValue;
			if (Wrap)
			{
				if (!double.IsNaN(min))
					min -= Increment;
				if (!double.IsNaN(max))
					max += Increment;
			}
			Control.Minimum = min;
			Control.Maximum = max;
		}

		void UpdateRequiredDigits()
		{
			if (HasFormatString)
				Control.FormatString = FormatString;
			else if (MaximumDecimalPlaces > 0 || DecimalPlaces > 0)
			{
				var format = new StringBuilder();
				format.Append("0.");
				if (DecimalPlaces > 0)
					format.Append(new string('0', DecimalPlaces));
				if (MaximumDecimalPlaces > DecimalPlaces)
					format.Append(new string('#', MaximumDecimalPlaces - DecimalPlaces));
				Control.FormatString = format.ToString();
			}
			else
				Control.FormatString = "0";
		}
	}
}
