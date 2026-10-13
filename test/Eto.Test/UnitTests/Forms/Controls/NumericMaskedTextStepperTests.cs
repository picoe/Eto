using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture(typeof(byte))]
	[TestFixture(typeof(sbyte))]
	[TestFixture(typeof(short))]
	[TestFixture(typeof(ushort))]
	[TestFixture(typeof(int))]
	[TestFixture(typeof(uint))]
	[TestFixture(typeof(long))]
	[TestFixture(typeof(ulong))]
	[TestFixture(typeof(float))]
	[TestFixture(typeof(double))]
	[TestFixture(typeof(decimal))]
	public class NumericMaskedTextStepperTests<T> : TestBase
		where T : struct, IComparable<T>
#if NET7_0_OR_GREATER
		, System.Numerics.INumber<T>
#endif
	{
		// exposes OnStep so tests can step without clicking the native stepper
		class TestStepper : NumericMaskedTextStepper<T>
		{
			public void PerformStep(StepperDirection direction) => OnStep(new StepperEventArgs(direction));
		}

		static T V(object value) => (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);

		static bool IsFloatingPoint => typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal);

		static bool IsSigned => Convert.ToDouble(typeof(T).GetField("MinValue").GetValue(null), CultureInfo.InvariantCulture) < 0;

		[Test]
		public void DefaultsShouldMatchType()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper();
				Assert.That(stepper.MinValue, Is.EqualTo((T)typeof(T).GetField("MinValue").GetValue(null)), "MinValue");
				Assert.That(stepper.MaxValue, Is.EqualTo((T)typeof(T).GetField("MaxValue").GetValue(null)), "MaxValue");
				Assert.That(stepper.Increment, Is.EqualTo(V(1)), "Increment");
				Assert.That(stepper.Value, Is.EqualTo(default(T)), "Value");
				Assert.That(stepper.AllowSign, Is.EqualTo(IsSigned), "AllowSign");
				Assert.That(stepper.AllowDecimal, Is.EqualTo(IsFloatingPoint), "AllowDecimal");
			});
		}

		[Test]
		public void SettingValueShouldRoundTrip()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper();
				var valueChanged = 0;
				stepper.ValueChanged += (sender, e) => valueChanged++;

				stepper.Value = V(42);
				Assert.That(stepper.Value, Is.EqualTo(V(42)), "Value");
				Assert.That(stepper.Text, Is.EqualTo("42"), "Text");
				Assert.That(valueChanged, Is.EqualTo(1), "ValueChanged should fire once");

				stepper.Value = V(42);
				Assert.That(valueChanged, Is.EqualTo(1), "ValueChanged should not fire when setting the same value");
			});
		}

		[Test]
		public void SettingTextShouldUpdateValue()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper();
				var valueChanged = 0;
				stepper.ValueChanged += (sender, e) => valueChanged++;

				stepper.Text = "12";
				Assert.That(stepper.Value, Is.EqualTo(V(12)), "Value");
				Assert.That(valueChanged, Is.EqualTo(1), "ValueChanged");
			});
		}

		[Test]
		public void NegativeValuesShouldWorkForSignedTypes()
		{
			if (!IsSigned)
				Assert.Ignore("Type is unsigned");
			Invoke(() =>
			{
				var stepper = new TestStepper();
				stepper.Value = V(-5);
				Assert.That(stepper.Value, Is.EqualTo(V(-5)), "Value");
				Assert.That(stepper.Text, Is.EqualTo("-5"), "Text");
			});
		}

		[Test]
		public void ValueShouldBeClampedToMinMax()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper { MinValue = V(10), MaxValue = V(20) };
				Assert.That(stepper.Value, Is.EqualTo(V(10)), "Value should be raised to the new MinValue");

				stepper.Value = V(100);
				Assert.That(stepper.Value, Is.EqualTo(V(20)), "Value should be limited to MaxValue");

				stepper.Value = V(5);
				Assert.That(stepper.Value, Is.EqualTo(V(10)), "Value should be limited to MinValue");

				stepper.Value = V(15);
				stepper.MaxValue = V(12);
				Assert.That(stepper.Value, Is.EqualTo(V(12)), "Value should be lowered to the new MaxValue");
			});
		}

		[Test]
		public void StepShouldChangeValueByIncrement()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper { Increment = V(5) };
				stepper.Value = V(10);
				var valueChanged = 0;
				stepper.ValueChanged += (sender, e) => valueChanged++;

				stepper.PerformStep(StepperDirection.Up);
				Assert.That(stepper.Value, Is.EqualTo(V(15)), "Value after stepping up");

				stepper.PerformStep(StepperDirection.Down);
				stepper.PerformStep(StepperDirection.Down);
				Assert.That(stepper.Value, Is.EqualTo(V(5)), "Value after stepping down");
				Assert.That(valueChanged, Is.EqualTo(3), "ValueChanged should fire for each step");
			});
		}

		[Test]
		public void StepShouldStopAtMinMax()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper { MinValue = V(0), MaxValue = V(10), Increment = V(3) };
				stepper.Value = V(9);
				Assert.That(stepper.ValidDirection, Is.EqualTo(StepperValidDirections.Both), "ValidDirection below max");

				stepper.PerformStep(StepperDirection.Up);
				Assert.That(stepper.Value, Is.EqualTo(V(10)), "Value should stop at MaxValue");
				Assert.That(stepper.ValidDirection, Is.EqualTo(StepperValidDirections.Down), "ValidDirection at max");

				stepper.Value = V(1);
				stepper.PerformStep(StepperDirection.Down);
				Assert.That(stepper.Value, Is.EqualTo(V(0)), "Value should stop at MinValue");
				Assert.That(stepper.ValidDirection, Is.EqualTo(StepperValidDirections.Up), "ValidDirection at min");
			});
		}

		[Test]
		public void StepShouldWrapAtMinMax()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper { MinValue = V(0), MaxValue = V(10), Wrap = true };
				stepper.Value = V(10);
				Assert.That(stepper.ValidDirection, Is.EqualTo(StepperValidDirections.Both), "ValidDirection at max with Wrap");

				stepper.PerformStep(StepperDirection.Up);
				Assert.That(stepper.Value, Is.EqualTo(V(0)), "Value should wrap to MinValue");

				stepper.PerformStep(StepperDirection.Down);
				Assert.That(stepper.Value, Is.EqualTo(V(10)), "Value should wrap to MaxValue");
			});
		}

		[Test]
		public void StepShouldDoNothingWhenReadOnly()
		{
			Invoke(() =>
			{
				var stepper = new TestStepper { ReadOnly = true };
				stepper.Value = V(5);
				Assert.That(stepper.ValidDirection, Is.EqualTo(StepperValidDirections.None), "ValidDirection");

				stepper.PerformStep(StepperDirection.Up);
				Assert.That(stepper.Value, Is.EqualTo(V(5)), "Value");
			});
		}

		[Test]
		public void FractionalValueShouldRoundToMaximumDecimalPlaces()
		{
			if (!IsFloatingPoint)
				Assert.Ignore("Type is not floating point");
			Invoke(() =>
			{
				var stepper = new TestStepper();
				stepper.Value = V(1.5);
				Assert.That(stepper.Value, Is.EqualTo(V(1.5)), "Value");
				Assert.That(stepper.Text, Is.EqualTo("1.5"), "Text");

				var valueChanged = 0;
				stepper.ValueChanged += (sender, e) => valueChanged++;
				stepper.MaximumDecimalPlaces = 2;
				stepper.Value = V(1.256);
				Assert.That(stepper.Value, Is.EqualTo(V(1.26)), "Value should be rounded");
				Assert.That(stepper.Text, Is.EqualTo("1.26"), "Text should be rounded");
				Assert.That(valueChanged, Is.EqualTo(1), "ValueChanged");
			});
		}

		[Test]
		public void FractionalIncrementShouldStep()
		{
			if (!IsFloatingPoint)
				Assert.Ignore("Type is not floating point");
			Invoke(() =>
			{
				var stepper = new TestStepper { Increment = V(0.5) };
				stepper.PerformStep(StepperDirection.Up);
				stepper.PerformStep(StepperDirection.Up);
				stepper.PerformStep(StepperDirection.Up);
				Assert.That(stepper.Value, Is.EqualTo(V(1.5)), "Value");
				Assert.That(stepper.Text, Is.EqualTo("1.5"), "Text");
			});
		}

		[Test]
		public void DecimalPlacesShouldPadText()
		{
			if (!IsFloatingPoint)
				Assert.Ignore("Type is not floating point");
			Invoke(() =>
			{
				var stepper = new TestStepper { DecimalPlaces = 2 };
				stepper.Value = V(3);
				Assert.That(stepper.Text, Is.EqualTo("3.00"), "Text");
				Assert.That(stepper.Value, Is.EqualTo(V(3)), "Value");
			});
		}

		[Test]
		public void ChangingFormatShouldNotChangeValue()
		{
			if (!IsFloatingPoint)
				Assert.Ignore("Type is not floating point");
			Invoke(() =>
			{
				var stepper = new TestStepper();
				stepper.Value = V(2.25);
				var valueChanged = 0;
				stepper.ValueChanged += (sender, e) => valueChanged++;

				stepper.FormatString = "0.0";
				Assert.That(stepper.Text, Is.EqualTo("2.3").Or.EqualTo("2.2"), "Text should use the format");
				stepper.FormatString = null;
				Assert.That(stepper.Value, Is.EqualTo(V(2.25)), "Value should keep its precision");
				Assert.That(valueChanged, Is.EqualTo(0), "ValueChanged should not fire when only the format changes");
			});
		}
	}
}
