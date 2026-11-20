using System.Globalization;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.MaskedTextProvider
{
	[TestFixture]
	public class DateTimeMaskedTextProviderTests
	{
		static readonly CultureInfo EnUS = CultureInfo.GetCultureInfo("en-US");

		/// <summary>
		/// A mask can only use regular spaces, but ICU-based cultures (macOS/Linux) use a narrow no-break space
		/// in front of the AM/PM designator, so the format has to be normalized to match the mask it feeds.
		/// This uses an explicit culture rather than en-US so it is covered no matter which ICU version, if any,
		/// the platform running the tests uses.
		/// </summary>
		static CultureInfo NarrowSpaceCulture()
		{
			var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
			culture.DateTimeFormat.ShortTimePattern = "h:mm\u202Ftt";
			return culture;
		}

		[Test]
		public void ValueShouldRoundTripWithNarrowSpaceInTimePattern()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, NarrowSpaceCulture());
			provider.Value = DateTime.Today + new TimeSpan(14, 30, 0);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("02:30 PM"), "#1 - the narrow no-break space should become a regular space");
			Assert.That(provider.Value?.TimeOfDay, Is.EqualTo(new TimeSpan(14, 30, 0)), "#2");
		}

		[Test]
		public void ValueShouldRoundTripInDateMode()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS);
			provider.Value = new DateTime(2020, 1, 15);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("01/15/2020"));
			Assert.That(provider.Value, Is.EqualTo(new DateTime(2020, 1, 15)));
		}

		[Test]
		public void EmptyTextShouldHaveNullValue()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS);
			Assert.That(provider.IsEmpty, Is.True);
			Assert.That(provider.Value, Is.Null);
		}

		[Test]
		public void ValueShouldClampToMaxDate()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS)
			{
				MaxDate = new DateTime(2020, 12, 31)
			};
			provider.Value = new DateTime(2021, 6, 1);

			Assert.That(provider.Value, Is.EqualTo(new DateTime(2020, 12, 31)));
		}

		[Test]
		public void ValueShouldClampToMinDate()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS)
			{
				MinDate = new DateTime(2020, 1, 1)
			};
			provider.Value = new DateTime(2019, 6, 1);

			Assert.That(provider.Value, Is.EqualTo(new DateTime(2020, 1, 1)));
		}

		[Test]
		public void ChangingModeShouldPreserveValue()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS);
			provider.Value = new DateTime(2020, 1, 15, 14, 30, 0);

			provider.Mode = DateTimePickerMode.DateTime;
			provider.CommitText();

			Assert.That(provider.Value?.Date, Is.EqualTo(new DateTime(2020, 1, 15)));
		}

		[Test]
		public void TimeModeShouldRoundTripTimeOfDay()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, EnUS);
			provider.Value = DateTime.Today + new TimeSpan(14, 30, 0);
			provider.CommitText();

			Assert.That(provider.Value?.TimeOfDay, Is.EqualTo(new TimeSpan(14, 30, 0)));
		}

		[Test]
		public void StepShouldIncrementSegmentAtCaret()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS);
			provider.Value = new DateTime(2020, 1, 15);
			provider.CommitText();

			// caret at index 3 is within the day segment for the en-US "MM/dd/yyyy" mask
			var stepped = provider.GetStepValue(3, 1, out var segmentStart);

			Assert.That(stepped, Is.EqualTo(new DateTime(2020, 1, 16)));
			Assert.That(segmentStart, Is.EqualTo(3));
		}

		[Test]
		public void StepShouldRespectClamping()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS)
			{
				MaxDate = new DateTime(2020, 1, 15)
			};
			provider.Value = new DateTime(2020, 1, 15);
			provider.CommitText();

			// stepping the day up would exceed MaxDate, so it should clamp
			var stepped = provider.GetStepValue(3, 1, out _);

			Assert.That(stepped, Is.EqualTo(new DateTime(2020, 1, 15)));
		}

		[Test]
		public void ChangingMonthShouldClampDayToEndOfMonth()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS);
			provider.Value = new DateTime(2026, 1, 31);
			provider.Text = "04/31/2026";

			Assert.That(provider.Value, Is.EqualTo(new DateTime(2026, 4, 30)));
		}

		[Test]
		public void ChangingYearOfLeapDayShouldClampToNonLeapYear()
		{
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, EnUS);
			provider.Value = new DateTime(2024, 2, 29);
			provider.Text = "02/29/2025";

			Assert.That(provider.Value, Is.EqualTo(new DateTime(2025, 2, 28)));
		}

		[Test]
		public void TypedTextShouldParseWithEscapedLiteralsInPattern()
		{
			// '.' must be escaped in the mask, which should not shift where each part is read from
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, CultureInfo.GetCultureInfo("de-DE"));
			provider.Value = new DateTime(2026, 11, 19);
			provider.Text = "01.02.2025";

			Assert.That(provider.Value, Is.EqualTo(new DateTime(2025, 2, 1)));
		}

		[Test]
		public void TypedTextShouldParseWithMultiCharacterDateSeparator()
		{
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.DateSeparator = " - ";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, culture);
			provider.Value = new DateTime(2026, 6, 21);
			provider.Text = "07 - 04 - 2025";

			Assert.That(provider.Value, Is.EqualTo(new DateTime(2025, 7, 4)));
		}

		[Test]
		public void ValueShouldRoundTripWithTwoDigitYear()
		{
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.ShortDatePattern = "M/d/yy";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, culture);
			provider.Value = new DateTime(2026, 6, 21);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("06/21/26"));
			Assert.That(provider.Value, Is.EqualTo(new DateTime(2026, 6, 21)));
		}

		[Test]
		public void ValueOutsideCalendarRangeShouldClampToCalendar()
		{
			var culture = CultureInfo.GetCultureInfo("fa-IR");
			var calendar = culture.DateTimeFormat.Calendar;
			Assume.That(calendar, Is.InstanceOf<PersianCalendar>(), "fa-IR doesn't use the Persian calendar on this platform");

			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, culture);
			provider.Value = DateTime.MinValue;
			provider.CommitText();

			Assert.That(provider.Value, Is.EqualTo(calendar.MinSupportedDateTime));
		}

		[TestCase("th-TH")]
		[TestCase("fa-IR")]
		[TestCase("en-US")]
		public void ValueShouldRoundTripInCultureCalendar(string cultureName)
		{
			// the text is shown in the culture's calendar, so it has to be read back in it, every day of a leap cycle
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, CultureInfo.GetCultureInfo(cultureName));
			for (var date = new DateTime(2024, 1, 1); date < new DateTime(2028, 1, 1); date = date.AddDays(1))
			{
				provider.Value = date;
				provider.CommitText();
				provider.CommitText();

				Assert.That(provider.Value, Is.EqualTo(date), $"Text: {provider.Text}");
			}
		}
	}
}
