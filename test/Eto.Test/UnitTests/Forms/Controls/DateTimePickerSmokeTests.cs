using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class DateTimePickerSmokeTests : TestBase
	{
		[TestCase(DateTimePickerMode.Date)]
		[TestCase(DateTimePickerMode.Time)]
		[TestCase(DateTimePickerMode.DateTime)]
		public void ValueShouldRoundTripPerMode(DateTimePickerMode mode)
		{
			var value = new DateTime(2020, 3, 4, 14, 30, 0);
			Shown(form =>
			{
				var picker = new DateTimePicker { Mode = mode, Value = value };
				form.Content = picker;
				return picker;
			},
			picker =>
			{
				Assert.That(picker.Value, Is.Not.Null);
				if (mode.HasFlag(DateTimePickerMode.Date))
					Assert.That(picker.Value.Value.Date, Is.EqualTo(value.Date), "Date component");
				if (mode.HasFlag(DateTimePickerMode.Time))
					Assert.That(picker.Value.Value.TimeOfDay.Hours, Is.EqualTo(14), "Hour component");
			});
		}

		[Test]
		public void SwitchingModesShouldPreserveValue()
		{
			var value = new DateTime(2021, 7, 8, 9, 15, 0);
			Shown(form =>
			{
				var picker = new DateTimePicker { Mode = DateTimePickerMode.Date, Value = value };
				form.Content = picker;
				return picker;
			},
			picker =>
			{
				picker.Mode = DateTimePickerMode.DateTime;
				Assert.That(picker.Value.Value.Date, Is.EqualTo(value.Date), "after switch to DateTime");

				picker.Mode = DateTimePickerMode.Time;
				Assert.That(picker.Value, Is.Not.Null, "after switch to Time");

				picker.Mode = DateTimePickerMode.Date;
				Assert.That(picker.Value.Value.Date, Is.EqualTo(value.Date), "after switch back to Date");
			});
		}

		[TestCase("ar-SA", DateTimePickerMode.Date)]
		[TestCase("ar-SA", DateTimePickerMode.DateTime)]
		[TestCase("ar-EG", DateTimePickerMode.Date)]
		[TestCase("de-DE", DateTimePickerMode.DateTime)]
		[TestCase("th-TH", DateTimePickerMode.Date)]
		[TestCase("it", DateTimePickerMode.Date)]
		[TestCase("ar", DateTimePickerMode.Date)]
		public void ChangingCultureShouldPreserveValue(string cultureName, DateTimePickerMode mode)
		{
			var value = new DateTime(2021, 7, 8, 9, 15, 0);
			var culture = CultureInfo.GetCultureInfo(cultureName);
			Shown(form =>
			{
				var picker = new DateTimePicker { Mode = mode, Value = value };
				form.Content = picker;
				return picker;
			},
			picker =>
			{
				picker.Culture = culture;
				Assert.That(picker.Culture, Is.EqualTo(culture), "#1");
				Assert.That(picker.Value?.Date, Is.EqualTo(value.Date), "#2");

				picker.Culture = null;
				Assert.That(picker.Culture, Is.EqualTo(CultureInfo.CurrentCulture), "#3");
				Assert.That(picker.Value?.Date, Is.EqualTo(value.Date), "#4");
			});
		}

		[Test]
		public void ValueShouldClampToMinMax()
		{
			// programmatic clamping of an out-of-range value is currently specific to the WPF handler
			if (!Platform.Instance.IsWpf)
				Assert.Ignore("Programmatic min/max clamping is only guaranteed by the WPF handler.");

			Shown(form =>
			{
				var picker = new DateTimePicker
				{
					Mode = DateTimePickerMode.Date,
					MinDate = new DateTime(2020, 1, 1),
					MaxDate = new DateTime(2020, 12, 31),
					Value = new DateTime(2025, 6, 1)
				};
				form.Content = picker;
				return picker;
			},
			picker =>
			{
				Assert.That(picker.Value.Value.Date, Is.EqualTo(new DateTime(2020, 12, 31)));
			});
		}
	}
}
