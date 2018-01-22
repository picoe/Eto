using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class NumericStepperTests : TestBase
	{
		[Test]
		public void DefaultValuesShouldBeCorrect()
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				Assert.AreEqual(double.NegativeInfinity, numeric.MinValue, "MinValue should be double.NegativeInfinity");
				Assert.AreEqual(double.PositiveInfinity, numeric.MaxValue, "MaxValue should be double.PositiveInfinity");
				Assert.AreEqual(0, numeric.Value, "initial value should be 0");

				Assert.AreEqual(0, valueChanged, "ValueChanged event should not fire when setting to default values");
			});
		}

		[Test]
		public void MinMaxShouldRetainValue()
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				int currentValueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.MinValue = 100;
				numeric.MaxValue = 1000;
				Assert.AreEqual(100, numeric.MinValue, "MinValue should return the same value as set");
				Assert.AreEqual(1000, numeric.MaxValue, "MaxValue should return the same value as set");
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event should fire when changing the MinValue");

				numeric.MinValue = double.NegativeInfinity;
				numeric.MaxValue = double.PositiveInfinity;
				numeric.Value = 0;

				Assert.AreEqual(double.NegativeInfinity, numeric.MinValue, "MinValue should be double.NegativeInfinity");
				Assert.AreEqual(double.PositiveInfinity, numeric.MaxValue, "MaxValue should be double.PositiveInfinity");
				Assert.AreEqual(0, numeric.Value, "Value should be back to 0");

				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event should fire when changing the MinValue");
			});
		}

		[Test]
		public void ValueShouldBeLimitedToMinMax()
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				int currentValueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.MinValue = 0;
				numeric.MaxValue = 2000;
				Assert.AreEqual(0, numeric.MinValue, "Could not correctly set MinValue");
				Assert.AreEqual(2000, numeric.MaxValue, "Could not correctly set MaxValue");

				numeric.MinValue = 100;
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event was not fired the correct number of times");
				Assert.AreEqual(100, numeric.Value, "Value should be set after MinValue is set");

				numeric.Value = 1000;
				Assert.AreEqual(1000, numeric.Value, "Could not correctly set Value after Min/Max is set");
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event was not fired the correct number of times");

				numeric.Value = 2001;
				Assert.AreEqual(2000, numeric.Value, "Value should be limited to Min/Max value");
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event was not fired the correct number of times");

				numeric.Value = -1000;
				Assert.AreEqual(100, numeric.Value, "Value should be limited to Min/Max value");
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event was not fired the correct number of times");

				numeric.MinValue = 1000;
				Assert.AreEqual(1000, numeric.Value, "Value should be changed to match new MinValue");
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event was not fired the correct number of times");

				numeric.MinValue = 0;
				numeric.MaxValue = 500;
				Assert.AreEqual(500, numeric.Value, "Value should be changed to match new MaxValue");
				Assert.AreEqual(++currentValueChanged, valueChanged, "ValueChanged event was not fired the correct number of times");
			});
		}

		[TestCase(100, 32.767, 33, 0)]
		[TestCase(100, 32.167, 32, 0)]
		[TestCase(100, 32.767, 32.767, 3)]
		[TestCase(100, 32.767, 32.77, 2)]
		public void FractionalMaxValueShouldSetValueCorrectly(double value, double maxValue, double newValue, int decimalPlaces)
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.DecimalPlaces = decimalPlaces;

				numeric.Value = value;
				Assert.AreEqual(1, valueChanged, "ValueChanged event was not fired the correct number of times");
				numeric.MaxValue = maxValue;
				Assert.AreEqual(newValue, numeric.Value, "Value should be changed to match new MaxValue");
				Assert.AreEqual(2, valueChanged, "ValueChanged event was not fired the correct number of times");
			});
		}

		[TestCase(10, 32.767, 32.767, 3)]
		[TestCase(10, 32.767, 32.77, 2)]
		[TestCase(10, 32.767, 33, 0)]
		[TestCase(10, 32.167, 32, 0)]
		public void FractionalMinValueShouldSetValueCorrectly(double value, double minValue, double newValue, int decimalPlaces)
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.DecimalPlaces = decimalPlaces;

				numeric.Value = value;
				numeric.MinValue = minValue;
				Assert.AreEqual(newValue, numeric.Value, "Value should be changed to match new MaxValue");
				Assert.AreEqual(2, valueChanged, "ValueChanged event was not fired the correct number of times");
			});
		}

		[TestCase(10.126, 10, 0)]
		[TestCase(10.126, 10.1, 1)]
		[TestCase(10.126, 10.13, 2)]
		public void ValueShouldBeRoundedToDecimalPlaces(double value, double newValue, int decimalPlaces)
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.DecimalPlaces = decimalPlaces;

				numeric.Value = value;
				Assert.AreEqual(newValue, numeric.Value, "Value should be be rounded to the number of decimal places");
				Assert.AreEqual(1, valueChanged, "ValueChanged event was not fired the correct number of times");
			});
		}

		[TestCase(10.126, 10, 0)]
		[TestCase(10.126, 10.1, 1)]
		[TestCase(10.126, 10.13, 2)]
		public void ValueShouldBeRoundedToDecimalPlacesWhenSetAfter(double value, double newValue, int decimalPlaces)
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.Value = value;
				Assert.AreEqual(Math.Round(value, 0), numeric.Value, "Value should be set to the initial value");

				numeric.DecimalPlaces = decimalPlaces;

				Assert.AreEqual(newValue, numeric.Value, "Value should be be rounded to the number of decimal places");
				Assert.AreEqual(1, valueChanged, "ValueChanged event was not fired the correct number of times");
			});
		}

		[TestCase(12.3456789, 12, 0, 0)]
		[TestCase(12.3456789, 12.3, 0, 1)]
		[TestCase(12.3456789, 12.35, 0, 2)]
		[TestCase(12.3456789, 12.346, 0, 3)]
		[TestCase(12.3456789, 12.3, 1, 1)]
		[TestCase(12.3456789, 12.35, 1, 2)]
		[TestCase(12.3456789, 12.346, 1, 3)]
		[TestCase(12.3456789, 12.346, 3, 3)]
		[TestCase(12.3456789, 12.3456789, 3, 8)]
		[TestCase(12.34567891234, 12.34567891, 3, 8)]
		[TestCase(12.34567891234, 12.34567891234, 3, 15)]
		[TestCase(12.345678912345, 12.345678912345, 3, 15)]
		public void MaximumDecimalPlacesShouldAllowMorePreciseNumbers(double value, double newValue, int decimalPlaces, int maxDecimalPlaces)
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();
				int valueChanged = 0;
				//numeric.MinimumDecimalPlaces = maxDecimalPlaces;
				numeric.MaximumDecimalPlaces = maxDecimalPlaces;
				numeric.DecimalPlaces = decimalPlaces;
				numeric.ValueChanged += (sender, e) => valueChanged++;

				numeric.Value = value;
				Assert.AreEqual(Math.Round(value, maxDecimalPlaces), numeric.Value, "Value should be set to the initial value");

				numeric.DecimalPlaces = decimalPlaces;

				Assert.AreEqual(newValue, numeric.Value, "Value should be rounded to the maximum number of decimal places");
				Assert.AreEqual(1, valueChanged, "ValueChanged event was not fired the correct number of times");
			});
		}

		[Test]
		public void MaximumDecimalPlacesShouldUpdateWhenDecimalPlacesIsChanged()
		{
			Invoke(() =>
			{
				var numeric = new NumericStepper();

				numeric.DecimalPlaces = 3;
				Assert.AreEqual(3, numeric.DecimalPlaces, "DecimalPlaces isn't roundtripping set values");
				Assert.AreEqual(3, numeric.MaximumDecimalPlaces, "MaximumDecimalPlaces should be changed to at minimum DecimalPlaces");

				numeric.DecimalPlaces = 2;
				Assert.AreEqual(2, numeric.DecimalPlaces, "DecimalPlaces isn't roundtripping set values");
				Assert.AreEqual(3, numeric.MaximumDecimalPlaces, "MaximumDecimalPlaces should only be changed when DecimalPlaces is greater than its current value");


				numeric.MaximumDecimalPlaces = 2;
				Assert.AreEqual(2, numeric.DecimalPlaces, "DecimalPlaces should keep its original value");
				Assert.AreEqual(2, numeric.MaximumDecimalPlaces, "MaximumDecimalPlaces wasn't updated to the new value");

				numeric.MaximumDecimalPlaces = 1;
				Assert.AreEqual(1, numeric.DecimalPlaces, "DecimalPlaces should be updated to the new value of MaximumDecimalPlaces when its current value is greater");
				Assert.AreEqual(1, numeric.MaximumDecimalPlaces, "MaximumDecimalPlaces wasn't updated to the new value");

				numeric.MaximumDecimalPlaces = 0;
				Assert.AreEqual(0, numeric.DecimalPlaces, "DecimalPlaces should be updated to the new value of MaximumDecimalPlaces when its current value is greater");
				Assert.AreEqual(0, numeric.MaximumDecimalPlaces, "MaximumDecimalPlaces wasn't updated to the new value");
			});
		}

		[Test]
		public void SettingValueInChangedHandlerShouldStick()
		{
			NumericStepper numericStepper = null;
			Shown(form =>
			{
				numericStepper = new NumericStepper();
				numericStepper.ValueChanged += (sender, e) =>
				{
					numericStepper.Value = 10;
				};
				form.Content = numericStepper;
			},
			() =>
			{
				Assert.AreEqual(0, numericStepper.Value);
				numericStepper.Value = 2;
				Assert.AreEqual(10, numericStepper.Value);
			});
		}
		[Test, ManualTest]
		public void SettingValueInChangedHandlerShouldStickWhenTyped()
		{
			Exception exception = null;
			ManualForm("Type '2' in the numeric spinner.  It should be set to 10 and ValueChanged called exactly 2 times.",
				form =>
			{
				var label = new Label();
				var numericStepper = new NumericStepper();
				Assert.AreEqual(0, numericStepper.Value, "#1");
				var changedCount = 0;
				numericStepper.ValueChanged += (sender, e) =>
				{
					if (exception != null)
						return;
					try
					{
						changedCount++;
						if (changedCount == 1)
							Assert.AreEqual(2, numericStepper.Value, "#2");
						else if (changedCount == 2)
							Assert.AreEqual(10, numericStepper.Value, "#3");
						else if (changedCount > 2)
							Assert.Fail($"#4. ValueChanged should only fire twice. New value is '{numericStepper.Value}' but should stay at 10.");
						numericStepper.Value = 10;
						Assert.AreEqual(10, numericStepper.Value, "#5");
						Application.Instance.AsyncInvoke(() => label.Text = $"Value is {numericStepper.Value}. ValueChanged called {changedCount} times.");
					}
					catch (Exception ex)
					{
						exception = ex;
						form.Close();
					}
				};
				numericStepper.Focus();
				return new StackLayout
				{
					Items =
					{
						numericStepper,
						label
					}
				};
			});
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}
}
