using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class DateTimeMaskedTextStepperTests : TestBase
	{
		class TestStepper : DateTimeMaskedTextStepper
		{
			public new bool SelectEditableRangeAt(int index, bool forward, bool fallbackToFirstLast = true)
				=> base.SelectEditableRangeAt(index, forward, fallbackToFirstLast);
		}

		[Test]
		public void DesignatorDirectlyBeforeHourShouldBeSelectedOnItsOwn()
		{
			// e.g. zh-Hant-HK's "tth:mm", which has no literal between the designator and the hour
			var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
			culture.DateTimeFormat.ShortTimePattern = "tth:mm";
			culture.DateTimeFormat.AMDesignator = "上午";
			culture.DateTimeFormat.PMDesignator = "下午";
			Shown(form =>
			{
				var stepper = new TestStepper { Mode = DateTimePickerMode.Time, Culture = culture, Value = DateTime.Today + new TimeSpan(7, 12, 0) };
				form.Content = stepper;
				return stepper;
			},
			stepper =>
			{
				stepper.Focus();
				stepper.SelectEditableRangeAt(0, forward: true);
				Assert.That(stepper.Selection, Is.EqualTo(new Range<int>(0, 1)), "#1 - designator");

				stepper.SelectEditableRangeAt(2, forward: true);
				Assert.That(stepper.Selection, Is.EqualTo(new Range<int>(2, 3)), "#2 - hour");
			});
		}
	}
}
