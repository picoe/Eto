﻿using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(NumericStepper))]
	public class NumericStepperSection : Panel
	{
		public NumericStepperSection()
		{
			var numeric = new NumericStepper { Width = 200 };

			LogEvents(numeric);

			var enabled = new CheckBox { Text = "Enabled" };
			enabled.CheckedBinding.Bind(numeric, n => n.Enabled);

			var readOnly = new CheckBox { Text = "ReadOnly" };
			readOnly.CheckedBinding.Bind(numeric, n => n.ReadOnly);

			var minValue = new NumericStepper { Enabled = false, Value = -1000 };
			var minBinding = minValue.ValueBinding.Bind(numeric, n => n.MinValue, DualBindingMode.Manual);

			var chkMinValue = new CheckBox { Text = "MinValue" };
			chkMinValue.CheckedBinding.Convert(r => r == true ? DualBindingMode.OneWayToSource : DualBindingMode.Manual).Bind(minBinding, m => m.Mode);
			chkMinValue.CheckedBinding.Bind(minValue, m => m.Enabled);
			chkMinValue.CheckedBinding.Convert(r => r == false ? double.NegativeInfinity : minValue.Value).Bind(numeric, m => m.MinValue);

			var maxValue = new NumericStepper { Enabled = false, Value = 1000 };
			var maxBinding = maxValue.ValueBinding.Bind(numeric, (n) => n.MaxValue, DualBindingMode.Manual);

			var chkMaxValue = new CheckBox { Text = "MaxValue" };
			chkMaxValue.CheckedBinding.Convert(r => r == true ? DualBindingMode.OneWayToSource : DualBindingMode.Manual).Bind(maxBinding, m => m.Mode);
			chkMaxValue.CheckedBinding.Bind(maxValue, m => m.Enabled);
			chkMaxValue.CheckedBinding.Convert(r => r == false ? double.PositiveInfinity : maxValue.Value).Bind(numeric, m => m.MaxValue);

			var decimalPlaces = new NumericStepper { MaxValue = 15, MinValue = 0 };
			var decimalBinding = decimalPlaces.ValueBinding.Convert(r => (int)r, r => r).Bind(numeric, n => n.DecimalPlaces);

			var maxDecimalPlaces = new NumericStepper { MaxValue = 15, MinValue = 0 };
			var maxDecimalBinding = maxDecimalPlaces.ValueBinding.Convert(r => (int)r, r => r).Bind(numeric, n => n.MaximumDecimalPlaces);

			maxDecimalBinding.Changed += (sender, e) => decimalBinding.Update(BindingUpdateMode.Destination);
			decimalBinding.Changed += (sender, e) => maxDecimalBinding.Update(BindingUpdateMode.Destination);

			var formatString = new TextBox();
			Func<Exception, bool> valueChanged = ex => {
				formatString.BackgroundColor = ex == null ? SystemColors.ControlBackground : Colors.Red;
				return true; // we handle all exceptions
			};
			formatString.TextBinding.Bind(Binding.Property(numeric, n => n.FormatString).CatchException(valueChanged));
			formatString.TextBinding.Convert(c => string.IsNullOrEmpty(c)).Bind(decimalPlaces, d => d.Enabled);
			formatString.TextBinding.Convert(c => string.IsNullOrEmpty(c)).Bind(maxDecimalPlaces, d => d.Enabled);

			var cultureDropDown = new CultureDropDown();
			cultureDropDown.SelectedValueBinding.Bind(numeric, c => c.CultureInfo);

			var increment = new NumericStepper { MaximumDecimalPlaces = 15 };
			increment.ValueBinding.Bind(numeric, n => n.Increment);

			var options1 = new StackLayout
			{
				Spacing = 5,
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Items =
				{
					enabled,
					readOnly
				}
			};
			var options2 = new StackLayout
			{
				Spacing = 5,
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Items =
				{
					chkMinValue, minValue,
					chkMaxValue, maxValue,
					"Increment", increment
				}
			};
			var options3 = new StackLayout
			{
				Spacing = 5,
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Items =
				{
					"DecimalPlaces", decimalPlaces,
					"MaximumDecimalPlaces", maxDecimalPlaces
				}
			};

			Content = new StackLayout
			{
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Items = {
					options1,
					options2,
					options3,
					TableLayout.Horizontal(5, "FormatString", formatString, "CultureInfo", cultureDropDown),
					"Result:", numeric
				}
			};
		}

		void LogEvents(NumericStepper control)
		{
			control.ValueChanged += delegate
			{
				Log.Write(control, "ValueChanged, Value: {0}", control.Value);
			};
		}
	}
}

