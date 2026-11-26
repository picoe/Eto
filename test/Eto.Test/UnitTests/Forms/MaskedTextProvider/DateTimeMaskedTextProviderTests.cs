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
		public void ValueShouldRoundTripWithBidiMarksAndEraInDatePattern()
		{
			// ICU's Arabic cultures put right-to-left marks around the separators and may end with the era,
			// e.g. ar-SA's "d‏/M‏/yyyy g". A mask can't hold the marks, so they're left out.
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.DateSeparator = "‏/";
			culture.DateTimeFormat.ShortDatePattern = "d‏/M‏/yyyy g";
			var era = culture.DateTimeFormat.GetEraName(1);
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Date, culture);
			provider.Value = new DateTime(2026, 6, 21);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("21/06/2026 " + era), "#1");
			Assert.That(provider.Value, Is.EqualTo(new DateTime(2026, 6, 21)), "#2");

			provider.Text = "04/07/2025 " + era;
			Assert.That(provider.Value, Is.EqualTo(new DateTime(2025, 7, 4)), "#3");
		}

		[TestCase(3, "a. m.  03:30")]
		[TestCase(15, "p.m. x 03:30")]
		public void ValueShouldRoundTripWithPunctuatedDesignatorsOfDifferentLengths(int hour, string expectedText)
		{
			// e.g. es-US's "a. m.", with the designator first so a shorter one would shift the time after it
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.ShortTimePattern = "tt h:mm";
			culture.DateTimeFormat.AMDesignator = "a. m.";
			culture.DateTimeFormat.PMDesignator = "p.m. x";
			var value = DateTime.Today + new TimeSpan(hour, 30, 0);
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);
			provider.Value = value;
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo(expectedText), "#1");
			Assert.That(provider.Value?.TimeOfDay, Is.EqualTo(value.TimeOfDay), "#2");
		}

		[Test]
		public void EditableRangesShouldSeparateDesignatorFromAdjacentHour()
		{
			// e.g. zh-Hant-HK's "tth:mm", where the edit positions of the designator and hour run together
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.ShortTimePattern = "tth:mm";
			culture.DateTimeFormat.AMDesignator = "上午";
			culture.DateTimeFormat.PMDesignator = "下午";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);

			Assert.That(provider.EditableRanges, Is.EqualTo(new[] { new Range<int>(0, 1), new Range<int>(2, 3), new Range<int>(5, 6) }));
		}

		[Test]
		public void StepAtStartOfHourShouldNotStepAdjacentDesignator()
		{
			// with "tth:mm" the caret at the start of the hour is also just after the designator
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.ShortTimePattern = "tth:mm";
			culture.DateTimeFormat.AMDesignator = "上午";
			culture.DateTimeFormat.PMDesignator = "下午";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);
			provider.Value = DateTime.Today + new TimeSpan(7, 12, 0);

			var stepped = provider.GetStepValue(2, 1, out var segmentStart);

			Assert.That(stepped.TimeOfDay, Is.EqualTo(new TimeSpan(8, 12, 0)), "#1");
			Assert.That(segmentStart, Is.EqualTo(2), "#2");
		}

		[Test]
		public void PmShouldRoundTripWhenAmDesignatorIsItsPrefix()
		{
			// e.g. ak on macOS, where AM is "AN" and PM is "ANW"
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.AMDesignator = "AN";
			culture.DateTimeFormat.PMDesignator = "ANW";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);
			provider.Value = DateTime.Today + new TimeSpan(15, 30, 0);
			provider.CommitText();

			Assert.That(provider.Value?.TimeOfDay, Is.EqualTo(new TimeSpan(15, 30, 0)));
		}

		[TestCase("h:mm tt", "", "")] // e.g. mi with some ICU versions
		[TestCase("h:mm tt", "X", "X")]
		[TestCase("h:mm", "AM", "PM")]
		public void TimeShouldUse24HoursWhenTwelveHourTimeCannotShowAmOrPm(string pattern, string am, string pm)
		{
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.ShortTimePattern = pattern;
			culture.DateTimeFormat.AMDesignator = am;
			culture.DateTimeFormat.PMDesignator = pm;
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);
			provider.Value = DateTime.Today + new TimeSpan(15, 30, 0);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("15:30"), "#1");
			Assert.That(provider.Value?.TimeOfDay, Is.EqualTo(new TimeSpan(15, 30, 0)), "#2");
		}

		[Test]
		public void QuotedHourLiteralShouldNotCountAsTwelveHourTime()
		{
			// fr-CA's "HH 'h' mm" is a 24-hour time with a literal "h"
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.ShortTimePattern = "HH 'h' mm";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);
			provider.Value = DateTime.Today + new TimeSpan(15, 30, 0);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("15 h 30"));
		}

		[Test]
		public void TimeShouldUse24HoursWhenDesignatorsCannotBeMasked()
		{
			// a mask can't hold combining marks, such as the Devanagari vowel signs in some cultures' designators
			var culture = (CultureInfo)EnUS.Clone();
			culture.DateTimeFormat.AMDesignator = "पूर्व";
			culture.DateTimeFormat.PMDesignator = "अपर";
			var provider = new DateTimeMaskedTextProvider(DateTimePickerMode.Time, culture);
			provider.Value = DateTime.Today + new TimeSpan(15, 30, 0);
			provider.CommitText();

			Assert.That(provider.Text, Is.EqualTo("15:30"), "#1");
			Assert.That(provider.Value?.TimeOfDay, Is.EqualTo(new TimeSpan(15, 30, 0)), "#2");
		}

		[TestCase(DateTimePickerMode.Date)]
		[TestCase(DateTimePickerMode.Time)]
		[TestCase(DateTimePickerMode.DateTime)]
		public void ValueShouldRoundTripInEveryCulture(DateTimePickerMode mode)
		{
			var failures = new List<string>();
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				// both AM and PM, as they can differ in length
				foreach (var value in new[] { new DateTime(2026, 11, 25, 3, 30, 0), new DateTime(2026, 11, 25, 15, 30, 0) })
				{
					try
					{
						var provider = new DateTimeMaskedTextProvider(mode, culture);
						provider.Value = value;
						provider.CommitText();

						// typing the text back in should give the same value
						var typed = new DateTimeMaskedTextProvider(mode, culture) { Value = new DateTime(2000, 1, 1, 12, 0, 0) };
						typed.Text = provider.Text;

						if (!IsSameForMode(provider.Value, value, mode) || !IsSameForMode(typed.Value, value, mode))
							failures.Add($"{culture.Name}: '{provider.Text}' -> {provider.Value}, typed -> {typed.Value}");
					}
					catch (Exception ex)
					{
						failures.Add($"{culture.Name}: {ex.Message}");
					}
				}
			}
			Assert.That(failures, Is.Empty);
		}

		static bool IsSameForMode(DateTime? actual, DateTime expected, DateTimePickerMode mode) => mode switch
		{
			DateTimePickerMode.Date => actual?.Date == expected.Date,
			DateTimePickerMode.Time => actual?.TimeOfDay == expected.TimeOfDay,
			_ => actual == expected
		};

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
		[TestCase("ar-SA")]
		[TestCase("ar-EG")]
		[TestCase("ps-AF")]
		[TestCase("ar")] // neutral cultures, which the mask swaps for a specific culture with different separators
		[TestCase("it")]
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
