using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Eto.WinForms.Forms.Controls
{
	public class NumericStepperHandler : WindowsControl<NumericStepperHandler.EtoNumericUpDown, NumericStepper, NumericStepper.ICallback>, NumericStepper.IHandler
	{
		public class EtoNumericUpDown : swf.NumericUpDown
		{
			public NumericStepperHandler Handler { get; set; }

			public override void UpButton()
			{
				if (ReadOnly)
					return;
				if (Handler?.NeedsFormat == true)
				{
					UserEdit = false;
				}

				base.UpButton();
			}

			public override void DownButton()
			{
				if (ReadOnly)
					return;
				if (Handler?.NeedsFormat == true)
				{
					UserEdit = false;
				}
				base.DownButton();
			}

			public void UpdateValue() => UpdateValue(true);
			public void UpdateText() => UpdateValue(false);

			string TrimNumericString(string text) => Regex.Replace(text, $"[ ]|({Regex.Escape(Handler.CultureInfo.NumberFormat.NumberGroupSeparator)})", "");

			bool NumberStringsMatch(string num1, string num2) => string.Compare(TrimNumericString(num1), TrimNumericString(num2), Handler.CultureInfo, CompareOptions.IgnoreCase) == 0;

			void UpdateValue(bool userEdit, bool changing = false)
			{
				var h = Handler;
				if (h?.NeedsFormat == true)
				{
					if (userEdit)
					{
						// string to number
						var text = Text;
						if (h.HasFormatString)
							text = Regex.Replace(text, $@"(?!\d|{Regex.Escape(h.CultureInfo.NumberFormat.NumberDecimalSeparator)}|{Regex.Escape(h.CultureInfo.NumberFormat.NegativeSign)}).", ""); // strip any non-numeric value
						double result;
						if (double.TryParse(text, NumberStyles.Any, h.CultureInfo, out result))
						{
							if (h.HasFormatString && result > 0 && NumberStringsMatch((-result).ToString(h.ComputedFormatString, h.CultureInfo), Text))
								result = -result;

							Value = Math.Min(Maximum, Math.Max(Minimum, (decimal)result));

							if (changing)
								ChangingText = true;
							Text = result.ToString(h.ComputedFormatString, h.CultureInfo);
							return;
						}

						// test to see if it matches the zero format which could be blank or some other text
						if (h.HasFormatString && NumberStringsMatch(0.0.ToString(h.ComputedFormatString, h.CultureInfo), Text))
						{
							result = 0;
							Value = Math.Min(Maximum, Math.Max(Minimum, (decimal)result));
							if (changing)
								ChangingText = true;
							Text = result.ToString(h.ComputedFormatString, h.CultureInfo);
							return;
						}
					}
					else
					{
						// number to string
						var val = (double)Value;
						var format = h.ComputedFormatString;
						if (changing)
							ChangingText = true;
						Text = val.ToString(format, h.CultureInfo);
						return;
					}
				}
				if (!userEdit && string.IsNullOrEmpty(Text))
				{
					if (changing)
						ChangingText = true;
					Text = Value.ToString();
					return;
				}
				base.UpdateEditText();
			}

			protected override void UpdateEditText()
			{
				UpdateValue(UserEdit);
			}

			protected override void ValidateEditText()
			{
				if (Handler?.NeedsFormat == true)
					return;
				base.ValidateEditText();
			}

			protected override void OnTextBoxKeyPress(object source, swf.KeyPressEventArgs e)
			{
				if (Handler?.NeedsFormat == true)
					return;
				base.OnTextBoxKeyPress(source, e);
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var size = Handler?.UserPreferredSize ?? new Eto.Drawing.Size(-1, -1);
				var basePreferredSize = base.GetPreferredSize(proposedSize);
				if (size.Width < 0)
					size.Width = basePreferredSize.Width;

				if (size.Height < 0)
					size.Height = basePreferredSize.Height;
				return size.ToSD();
			}
		}

		public NumericStepperHandler()
		{
			Control = new EtoNumericUpDown
			{
				Handler = this,
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
					Control.Value = (decimal)Math.Round(Math.Max(MinValue, Math.Min(MaxValue, 0)), DecimalPlaces);
					Control.UpdateText();
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
			get { return HasFormatString ? (double)Control.Value : Math.Round((double)Control.Value, MaximumDecimalPlaces); }
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

		static readonly object FormatString_Key = new object();

		public string FormatString
		{
			get { return Widget.Properties.Get<string>(FormatString_Key); }
			set
			{
				if (!string.IsNullOrEmpty(value))
					0.0.ToString(value, CultureInfo);
				Widget.Properties.Set(FormatString_Key, value, UpdateFormat);
			}
		}

		static readonly object ComputedFormatString_Key = new object();

		public string ComputedFormatString
		{
			get
			{
				var format = FormatString;
				if (!string.IsNullOrEmpty(format))
					return format;
				format = Widget.Properties.Get<string>(ComputedFormatString_Key);
				if (format == null)
				{
					format = "0.";
					if (DecimalPlaces > 0)
						format += new string('0', DecimalPlaces);
					if (MaximumDecimalPlaces > DecimalPlaces)
						format += new string('#', MaximumDecimalPlaces - DecimalPlaces);
					Widget.Properties.Set(ComputedFormatString_Key, format);
				}
				return format;
			}
		}

		bool NeedsFormat => HasFormatString || CultureInfo != CultureInfo.CurrentCulture;

		bool HasFormatString => !string.IsNullOrEmpty(FormatString);

		static readonly object CultureInfo_Key = new object();

		public CultureInfo CultureInfo
		{
			get { return Widget.Properties.Get<CultureInfo>(CultureInfo_Key, CultureInfo.CurrentCulture); }
			set
			{
				Widget.Properties.Set(CultureInfo_Key, value, UpdateFormat, CultureInfo.CurrentCulture);
			}
		}


		void UpdateFormat()
		{
			Widget.Properties.Remove(ComputedFormatString_Key);
			Control.UpdateText();
		}

		int GetNumberOfDigits()
		{
			var str = ((double)Control.Value).ToString(CultureInfo.InvariantCulture);
			var idx = str.IndexOf(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
			return idx > 0 ? str.Length - idx - 1 : 0;
		}

		void UpdateRequiredDigits()
		{
			Widget.Properties.Remove(ComputedFormatString_Key);
			Control.DecimalPlaces = Math.Max(Math.Min(GetNumberOfDigits(), MaximumDecimalPlaces), DecimalPlaces);
			Control.UpdateText();
		}
	}
}