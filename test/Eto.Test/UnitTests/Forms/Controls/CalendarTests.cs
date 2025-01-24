using NUnit.Framework;
namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class CalendarTests
	{
		[Test]
		public void InitialValuesShouldBeCorrect()
		{
			TestBase.Invoke(() =>
			{
				var calendar = new Calendar();
				Assert.That(calendar.Mode, Is.EqualTo(CalendarMode.Single), "Calendar should default to single mode");
				Assert.That(calendar.SelectedDate, Is.EqualTo(DateTime.Today), "Initial SelectedDate should be Today");
				Assert.That(calendar.SelectedRange, Is.EqualTo(new Range<DateTime>(DateTime.Today)), "Initial SelectedRange should be Today");
				Assert.That(calendar.MinDate, Is.EqualTo(DateTime.MinValue), "Initial MinDate should be DateTime.MinValue");
				Assert.That(calendar.MaxDate, Is.EqualTo(DateTime.MaxValue), "Initial MaxDate should be DateTime.MaxValue");
			});
		}

		[Test]
		public void SelectedDateShouldTriggerChange()
		{
			TestBase.Invoke(() =>
			{
				var calendar = new Calendar();
				var dateCount = 0;
				var rangeCount = 0;
				calendar.SelectedDateChanged += (sender, e) => dateCount++;
				calendar.SelectedRangeChanged += (sender, e) => rangeCount++;

				DateTime date = DateTime.Today;
				calendar.SelectedDate = date;
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not fire when set to the initial value");
				Assert.That(rangeCount, Is.EqualTo(0), "SelectedRangeChanged should not fire when date is set to the initial value");
				Assert.That(calendar.SelectedDate, Is.EqualTo(date), "SelectedDate should retain its value");

				date = DateTime.Today.AddDays(10);
				calendar.SelectedDate = date;
				Assert.That(dateCount, Is.EqualTo(1), "SelectedDateChanged should fire when set to a value");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should be fired when changing SelectedDate");
				Assert.That(calendar.SelectedDate, Is.EqualTo(date), "SelectedDate should retain its value");

				dateCount = rangeCount = 0;
				calendar.SelectedDate = date;
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not be fired when set to the same date");
				Assert.That(rangeCount, Is.EqualTo(0), "SelectedRangeChanged should not be fired when set to the same date");
				Assert.That(calendar.SelectedDate, Is.EqualTo(date), "SelectedDate should retain its value");

				dateCount = rangeCount = 0;
				calendar.SelectedDate = date = DateTime.Today.AddDays(20);
				Assert.That(dateCount, Is.EqualTo(1), "SelectedDateChanged should fire when set to a specific date");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should be fired when changing SelectedDate");
				Assert.That(calendar.SelectedDate, Is.EqualTo(date), "SelectedDate should retain its value");
			});
		}

		[Test]
		public void SelectedRangeShouldTriggerChange()
		{
			TestBase.Invoke(() =>
			{
				var calendar = new Calendar { Mode = CalendarMode.Range };
				var dateCount = 0;
				var rangeCount = 0;
				calendar.SelectedDateChanged += (sender, e) => dateCount++;
				calendar.SelectedRangeChanged += (sender, e) => rangeCount++;

				rangeCount = dateCount = 0;
				var range = new Range<DateTime>(DateTime.Today);
				calendar.SelectedRange = range;
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not fire when set to initial value of null");
				Assert.That(rangeCount, Is.EqualTo(0), "SelectedRangeChanged should fire when set to initial value of null");
				Assert.That(calendar.SelectedRange, Is.EqualTo(range), "SelectedRange should retain its value");
				Assert.That(calendar.SelectedDate, Is.EqualTo(range.Start), "SelectedDate should be null when range is set to null");

				rangeCount = dateCount = 0;
				range = new Range<DateTime>(DateTime.Today.AddDays(1), DateTime.Today.AddDays(10));
				calendar.SelectedRange = range;
				Assert.That(dateCount, Is.EqualTo(1), "SelectedDateChanged should fire when set to a specific date");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should fire when set");
				Assert.That(calendar.SelectedRange, Is.EqualTo(range), "SelectedDate should retain its value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range;
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not be fired when set to the same date");
				Assert.That(rangeCount, Is.EqualTo(0), "SelectedRangeChanged should not be fired when set to the same date");
				Assert.That(calendar.SelectedRange, Is.EqualTo(range), "SelectedRange should retain its value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range;
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not fire when set to the same value");
				Assert.That(rangeCount, Is.EqualTo(0), "SelectedRangeChanged should not fire when set to the same value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range = new Range<DateTime>(DateTime.Today.AddDays(1), DateTime.Today.AddDays(11));
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not fire when range's start date hasn't changed");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should fire when set to a different date");
				Assert.That(calendar.SelectedRange, Is.EqualTo(range), "SelectedRange should retain its value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range = new Range<DateTime>(DateTime.Today.AddDays(2), DateTime.Today.AddDays(10));
				Assert.That(dateCount, Is.EqualTo(1), "SelectedDateChanged should fire when range's start date hasn't changed");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should fire when set to a different range");
				Assert.That(calendar.SelectedRange, Is.EqualTo(range), "SelectedRange should retain its value");
			});
		}

		[Test]
		public void MinDateShouldChangeSelectedDate()
		{
			TestBase.Invoke(() =>
			{
				var calendar = new Calendar();
				var dateCount = 0;
				var rangeCount = 0;
				calendar.SelectedDateChanged += (sender, e) => dateCount++;
				calendar.SelectedRangeChanged += (sender, e) => rangeCount++;

				calendar.SelectedDate = DateTime.Today;
				dateCount = rangeCount = 0;
				var date = DateTime.Today.AddDays(10);
				calendar.MinDate = date;
				Assert.That(dateCount, Is.EqualTo(1), "SelectedDateChanged should be fired when changing the min date");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should be fired when changing the min date");
				Assert.That(calendar.SelectedDate, Is.EqualTo(date), "SelectedDate should be changed to the MinDate");
			});
		}

		[Test]
		public void MaxDateShouldChangeSelectedDate()
		{
			TestBase.Invoke(() =>
			{
				var calendar = new Calendar();
				var dateCount = 0;
				var rangeCount = 0;
				calendar.SelectedDateChanged += (sender, e) => dateCount++;
				calendar.SelectedRangeChanged += (sender, e) => rangeCount++;

				calendar.SelectedDate = DateTime.Today;
				dateCount = rangeCount = 0;
				var date = DateTime.Today.AddDays(-10);
				calendar.MaxDate = date;
				Assert.That(dateCount, Is.EqualTo(1), "SelectedDateChanged should be fired when changing the min date");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should be fired when changing the min date");
				Assert.That(calendar.SelectedDate, Is.EqualTo(date), "SelectedDate should be changed to the MaxDate");
			});
		}

		[Test]
		public void ModeShouldUpdateDateWhenChangingFromRangeToSingle()
		{
			TestBase.Invoke(() =>
			{
				var initialRange = new Range<DateTime>(DateTime.Today, DateTime.Today.AddDays(10));
				var calendar = new Calendar { Mode = CalendarMode.Range, SelectedRange = initialRange };
				var dateCount = 0;
				var rangeCount = 0;
				calendar.SelectedDateChanged += (sender, e) => dateCount++;
				calendar.SelectedRangeChanged += (sender, e) => rangeCount++;

				Assert.That(calendar.SelectedRange, Is.EqualTo(initialRange), "SelectedRange is not set to the initial value");

				calendar.Mode = CalendarMode.Single;
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not be fired when changing the mode");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should be fired when changing the mode when the range changes");
				Assert.That(calendar.SelectedDate, Is.EqualTo(initialRange.Start), "SelectedDate should be the start of the original range");
				Assert.That(calendar.SelectedRange.Start, Is.EqualTo(initialRange.Start), "SelectedRange.End should be the same date");
				Assert.That(calendar.SelectedRange.End, Is.EqualTo(initialRange.Start), "SelectedRange.End should be the same date");

				dateCount = rangeCount = 0;
				calendar.Mode = CalendarMode.Range;
				Assert.That(calendar.SelectedRange.End, Is.EqualTo(initialRange.End), "SelectedRange.End should be the original end date when changing back to range mode");
				Assert.That(dateCount, Is.EqualTo(0), "SelectedDateChanged should not be fired when changing the mode");
				Assert.That(rangeCount, Is.EqualTo(1), "SelectedRangeChanged should be fired when changing the mode when the range changes");
			});
		}

	}
}

