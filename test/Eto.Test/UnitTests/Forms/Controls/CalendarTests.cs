using System;
using NUnit.Framework;
using Eto.Forms;

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
				Assert.AreEqual(CalendarMode.Single, calendar.Mode, "Calendar should default to single mode");
				Assert.AreEqual(DateTime.Today, calendar.SelectedDate, "Initial SelectedDate should be Today");
				Assert.AreEqual(new Range<DateTime>(DateTime.Today), calendar.SelectedRange, "Initial SelectedRange should be Today");
				Assert.AreEqual(DateTime.MinValue, calendar.MinDate, "Initial MinDate should be DateTime.MinValue");
				Assert.AreEqual(DateTime.MaxValue, calendar.MaxDate, "Initial MaxDate should be DateTime.MaxValue");
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
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not fire when set to the initial value");
				Assert.AreEqual(0, rangeCount, "SelectedRangeChanged should not fire when date is set to the initial value");
				Assert.AreEqual(date, calendar.SelectedDate, "SelectedDate should retain its value");

				date = DateTime.Today.AddDays(10);
				calendar.SelectedDate = date;
				Assert.AreEqual(1, dateCount, "SelectedDateChanged should fire when set to a value");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should be fired when changing SelectedDate");
				Assert.AreEqual(date, calendar.SelectedDate, "SelectedDate should retain its value");

				dateCount = rangeCount = 0;
				calendar.SelectedDate = date;
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not be fired when set to the same date");
				Assert.AreEqual(0, rangeCount, "SelectedRangeChanged should not be fired when set to the same date");
				Assert.AreEqual(date, calendar.SelectedDate, "SelectedDate should retain its value");

				dateCount = rangeCount = 0;
				calendar.SelectedDate = date = DateTime.Today.AddDays(20);
				Assert.AreEqual(1, dateCount, "SelectedDateChanged should fire when set to a specific date");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should be fired when changing SelectedDate");
				Assert.AreEqual(date, calendar.SelectedDate, "SelectedDate should retain its value");
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
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not fire when set to initial value of null");
				Assert.AreEqual(0, rangeCount, "SelectedRangeChanged should fire when set to initial value of null");
				Assert.AreEqual(range, calendar.SelectedRange, "SelectedRange should retain its value");
				Assert.AreEqual(range.Start, calendar.SelectedDate, "SelectedDate should be null when range is set to null");

				rangeCount = dateCount = 0;
				range = new Range<DateTime>(DateTime.Today.AddDays(1), DateTime.Today.AddDays(10));
				calendar.SelectedRange = range;
				Assert.AreEqual(1, dateCount, "SelectedDateChanged should fire when set to a specific date");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should fire when set");
				Assert.AreEqual(range, calendar.SelectedRange, "SelectedDate should retain its value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range;
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not be fired when set to the same date");
				Assert.AreEqual(0, rangeCount, "SelectedRangeChanged should not be fired when set to the same date");
				Assert.AreEqual(range, calendar.SelectedRange, "SelectedRange should retain its value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range;
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not fire when set to the same value");
				Assert.AreEqual(0, rangeCount, "SelectedRangeChanged should not fire when set to the same value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range = new Range<DateTime>(DateTime.Today.AddDays(1), DateTime.Today.AddDays(11));
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not fire when range's start date hasn't changed");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should fire when set to a different date");
				Assert.AreEqual(range, calendar.SelectedRange, "SelectedRange should retain its value");

				rangeCount = dateCount = 0;
				calendar.SelectedRange = range = new Range<DateTime>(DateTime.Today.AddDays(2), DateTime.Today.AddDays(10));
				Assert.AreEqual(1, dateCount, "SelectedDateChanged should fire when range's start date hasn't changed");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should fire when set to a different range");
				Assert.AreEqual(range, calendar.SelectedRange, "SelectedRange should retain its value");
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
				Assert.AreEqual(1, dateCount, "SelectedDateChanged should be fired when changing the min date");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should be fired when changing the min date");
				Assert.AreEqual(date, calendar.SelectedDate, "SelectedDate should be changed to the MinDate");
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
				Assert.AreEqual(1, dateCount, "SelectedDateChanged should be fired when changing the min date");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should be fired when changing the min date");
				Assert.AreEqual(date, calendar.SelectedDate, "SelectedDate should be changed to the MaxDate");
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

				Assert.AreEqual(initialRange, calendar.SelectedRange, "SelectedRange is not set to the initial value");

				calendar.Mode = CalendarMode.Single;
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not be fired when changing the mode");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should be fired when changing the mode when the range changes");
				Assert.AreEqual(initialRange.Start, calendar.SelectedDate, "SelectedDate should be the start of the original range");
				Assert.AreEqual(initialRange.Start, calendar.SelectedRange.Start, "SelectedRange.End should be the same date");
				Assert.AreEqual(initialRange.Start, calendar.SelectedRange.End, "SelectedRange.End should be the same date");

				dateCount = rangeCount = 0;
				calendar.Mode = CalendarMode.Range;
				Assert.AreEqual(initialRange.End, calendar.SelectedRange.End, "SelectedRange.End should be the original end date when changing back to range mode");
				Assert.AreEqual(0, dateCount, "SelectedDateChanged should not be fired when changing the mode");
				Assert.AreEqual(1, rangeCount, "SelectedRangeChanged should be fired when changing the mode when the range changes");
			});
		}

	}
}

