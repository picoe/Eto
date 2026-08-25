using System.Globalization;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class NumericStepperTests : TestBase
	{
		static NSTextField GetTextField(NumericStepper stepper) => (NSTextField)((NSView)stepper.ControlObject).Subviews[0];

		/// <summary>
		/// NSNumberFormatter formats using its NSLocale, which comes from the OS and has nothing to do with the
		/// managed CultureInfo, so anything left to the native formatter used the OS locale's separators instead of
		/// the culture that was asked for.
		/// </summary>
		[TestCase("")]
		[TestCase("en-US")]
		[TestCase("nl-NL")]
		[TestCase("de-DE")]
		public void ValueShouldBeShownUsingTheSpecifiedCulture(string cultureName)
		{
			Invoke(() =>
			{
				var culture = CultureInfo.GetCultureInfo(cultureName);
				Assume.That(culture.Name, Is.Not.EqualTo(OSCultureName), "the OS culture is formatted by the OS - see FormattingShouldBeLeftToTheOSForItsOwnCulture");

				var stepper = new NumericStepper { DecimalPlaces = 1, CultureInfo = culture };
				stepper.Value = 90.5;

				Assert.That(GetTextField(stepper).StringValue, Is.EqualTo(90.5.ToString("0.0", culture)));
			});
		}

		/// <summary>
		/// A culture that was asked for explicitly has to be applied even when it is also what CurrentCulture happens
		/// to be set to.  The culture used to be compared against CurrentCulture, so an invariant culture stepper in
		/// an app whose CurrentCulture is invariant was formatted by the OS instead - which is what RH-96796 was.
		/// </summary>
		[Test]
		public void ExplicitCultureShouldBeAppliedWhenItMatchesCurrentCulture()
		{
			Invoke(() =>
			{
				var oldCulture = Thread.CurrentThread.CurrentCulture;
				try
				{
					// a culture whose separator differs from the OS's, so that which one formatted the value shows
					var culture = ForeignCulture;
					Thread.CurrentThread.CurrentCulture = culture;

					// assigning CultureInfo.CurrentCulture itself is what hit that path, as the property store
					// compares by reference
					var stepper = new NumericStepper { DecimalPlaces = 1, CultureInfo = CultureInfo.CurrentCulture };
					stepper.Value = 90.5;

					Assert.That(GetTextField(stepper).StringValue, Is.EqualTo(90.5.ToString("0.0", culture)));
				}
				finally
				{
					Thread.CurrentThread.CurrentCulture = oldCulture;
				}
			});
		}

		/// <summary>
		/// The other half of that rule: for the OS's own culture the value is left to the native formatter, so that
		/// whatever the user has customized about their number format is respected.  macOS lets the separators be
		/// changed independently of the region and CultureInfo cannot represent that, so formatting such a stepper
		/// with CultureInfo would ignore the customization.
		/// </summary>
		[Test]
		public void FormattingShouldBeLeftToTheOSForItsOwnCulture()
		{
			Invoke(() =>
			{
				CultureInfo osCulture = null;
				try
				{
					osCulture = CultureInfo.GetCultureInfo(OSCultureName);
				}
				catch (CultureNotFoundException)
				{
					Assume.That(false, $"the OS locale '{OSCultureName}' has no CultureInfo equivalent");
				}

				var stepper = new NumericStepper { DecimalPlaces = 1, CultureInfo = osCulture };
				stepper.Value = 90.5;

				// what the native formatter SetFormatter builds would produce
				var expected = new NSNumberFormatter
				{
					NumberStyle = NSNumberFormatterStyle.Decimal,
					UsesGroupingSeparator = false,
					MinimumFractionDigits = 1,
					MaximumFractionDigits = 1
				}.StringFromNumber(new NSNumber(90.5));

				Assert.That(GetTextField(stepper).StringValue, Is.EqualTo(expected));
			});
		}

		/// <summary>
		/// Setting the culture before the decimal places left the format string computed for the old number of
		/// decimal places cached, showing "90" instead of "90.0".  The invariant culture is never the OS culture - a
		/// locale identifier is never empty - so its own formatting always applies here.
		/// </summary>
		[Test]
		public void DecimalPlacesShouldApplyWhenSetAfterCulture()
		{
			Invoke(() =>
			{
				var stepper = new NumericStepper { CultureInfo = CultureInfo.InvariantCulture, DecimalPlaces = 1 };
				stepper.Value = 90;
				Assert.That(GetTextField(stepper).StringValue, Is.EqualTo("90.0"));
			});
		}

		/// <summary>
		/// The decimal separator this machine's regional settings produce.  The typing tests below are written against
		/// it rather than a hardcoded separator, since it differs per machine - and note the formatter needs an
		/// explicit NumberStyle, as the default NoStyle reports a legacy separator instead.
		/// </summary>
		static string OSDecimalSeparator => new NSNumberFormatter { NumberStyle = NSNumberFormatterStyle.Decimal }.DecimalSeparator;

		/// <summary>
		/// The name of the OS locale's culture, in the form CultureInfo.Name uses.
		/// </summary>
		static string OSCultureName => (NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier ?? string.Empty).Split('@')[0].Replace('_', '-');

		/// <summary>
		/// A culture that is neither the OS culture nor uses its decimal separator, so that it is visible which of the
		/// two formatted a value.  Chosen at runtime from the candidates - nothing here assumes a particular OS
		/// setting, since that is a machine (and moment) specific thing.
		/// </summary>
		static CultureInfo ForeignCulture
		{
			get
			{
				var osSeparator = OSDecimalSeparator;
				var osName = OSCultureName;
				var candidates = new[]
				{
					CultureInfo.InvariantCulture,
					CultureInfo.GetCultureInfo("nl-NL"),
					CultureInfo.GetCultureInfo("de-DE"),
					CultureInfo.GetCultureInfo("en-US")
				};
				foreach (var candidate in candidates)
				{
					if (candidate.NumberFormat.NumberDecimalSeparator != osSeparator
						&& !string.Equals(candidate.Name, osName, StringComparison.OrdinalIgnoreCase))
						return candidate;
				}
				Assert.Fail($"No candidate culture differs from the OS locale '{osName}', which separates decimals with '{osSeparator}'");
				return null;
			}
		}

		/// <summary>
		/// Characters that aren't part of a number in the stepper's culture should not be accepted.  The OS locale's
		/// decimal separator is not special-cased: accepting it can't work when it is also the culture's group
		/// separator, where "1.000" is genuinely ambiguous.
		/// </summary>
		[Test]
		public void TypedCharactersShouldBeFilteredByCulture()
		{
			var culture = ForeignCulture;
			TypeIntoStepper(culture, null, "90x5", (stepper, editor, other) =>
			{
				Assert.That(editor.Value, Is.EqualTo("905"), "#1 the letter should have been rejected");
				Assert.That(stepper.Value, Is.EqualTo(905), "#2 value");
			});
		}

		/// <summary>
		/// The stepper culture's decimal separator is what can be typed, whatever the OS locale uses.
		/// </summary>
		[Test]
		public void CultureDecimalSeparatorShouldBeAcceptedWhenTyped()
		{
			var culture = ForeignCulture;
			var separator = culture.NumberFormat.NumberDecimalSeparator;
			TypeIntoStepper(culture, null, "90" + separator + "5", (stepper, editor, other) =>
			{
				Assert.That(editor.Value, Is.EqualTo("90" + separator + "5"), "#1 typed text");
				Assert.That(stepper.Value, Is.EqualTo(90.5), "#2 value");
			});
		}

		/// <summary>
		/// A separator that isn't the stepper culture's is rejected even when the OS locale uses it - it would
		/// otherwise be parsed as a group separator, turning 90,5 into 905.
		/// </summary>
		[Test]
		public void OSLocaleDecimalSeparatorShouldBeRejectedWhenItIsNotTheCultures()
		{
			TypeIntoStepper(ForeignCulture, null, "90" + OSDecimalSeparator + "5", (stepper, editor, other) =>
			{
				Assert.That(editor.Value, Is.EqualTo("905"), "#1 the separator should have been rejected");
				Assert.That(stepper.Value, Is.EqualTo(905), "#2 value");
			});
		}

		/// <summary>
		/// The OS culture is formatted by the native formatter, so its separator is the one the field displays and
		/// parses with - and so the one that has to be typable.  Filtering on CultureInfo's separator instead left
		/// the OS's own separator rejected and the only typable one unparseable, so the value could not be entered.
		/// </summary>
		[Test]
		public void OSDecimalSeparatorShouldBeTypableForTheOSCulture()
		{
			var osCulture = OSCultureOrSkip();
			TypeIntoStepper(osCulture, null, "123" + OSDecimalSeparator + "123", (stepper, editor, other) =>
			{
				Assert.That(editor.Value, Is.EqualTo("123" + OSDecimalSeparator + "123"), "#1 typed text");
				Assert.That(stepper.Value, Is.EqualTo(123.123), "#2 value");
			});
		}

		/// <summary>
		/// And the value that was typed that way survives a switch to another culture, rather than the stepper being
		/// left holding whatever it managed to parse before the separator.
		/// </summary>
		[Test]
		public void ValueTypedUnderTheOSCultureShouldSurviveACultureSwitch()
		{
			var osCulture = OSCultureOrSkip();
			TypeIntoStepper(osCulture, null, "123" + OSDecimalSeparator + "123", (stepper, editor, other) =>
			{
				var culture = ForeignCulture;
				stepper.CultureInfo = culture;

				Assert.That(stepper.Value, Is.EqualTo(123.123), "#1 value should survive the culture change");
				Assert.That(GetTextField(stepper).StringValue, Is.EqualTo(123.123.ToString("0.#####", culture)), "#2 text should be reformatted with the new culture");
			});
		}

		/// <summary>
		/// The OS culture, skipping the test when its number format is not customized - the OS and CultureInfo then
		/// agree on the separators and there is nothing to tell apart.
		/// </summary>
		static CultureInfo OSCultureOrSkip()
		{
			CultureInfo osCulture = null;
			try
			{
				osCulture = CultureInfo.GetCultureInfo(OSCultureName);
			}
			catch (CultureNotFoundException)
			{
				Assume.That(false, $"the OS locale '{OSCultureName}' has no CultureInfo equivalent");
			}
			Assume.That(OSDecimalSeparator, Is.Not.EqualTo(osCulture.NumberFormat.NumberDecimalSeparator),
				"the OS number format is not customized on this machine, so it cannot differ from the culture's");
			return osCulture;
		}

		/// <summary>
		/// Types <paramref name="text"/> a character at a time through the real field editor, so the TextInput filter
		/// sees each keystroke the way it would from the keyboard.  <paramref name="currentCulture"/> overrides
		/// CultureInfo.CurrentCulture for the duration when given.  The test is passed the stepper, its field editor,
		/// and a sibling control it can move focus to.
		/// </summary>
		static void TypeIntoStepper(CultureInfo culture, CultureInfo currentCulture, string text, Action<NumericStepper, NSTextView, Control> test)
		{
			NumericStepper stepper = null;
			TextBox other = null;
			CultureInfo oldCulture = null;
			Shown(form =>
			{
				oldCulture = Thread.CurrentThread.CurrentCulture;
				if (currentCulture != null)
					Thread.CurrentThread.CurrentCulture = currentCulture;

				stepper = new NumericStepper { MaximumDecimalPlaces = 5, CultureInfo = culture };
				other = new TextBox();
				form.Content = new StackLayout { Items = { stepper, other } };
			},
			() =>
			{
				try
				{
					stepper.Focus();
					var editor = GetTextField(stepper).CurrentEditor as NSTextView;
					Assert.That(editor, Is.Not.Null, "stepper should have a field editor while focused");
					editor.SelectAll(editor);
					foreach (var ch in text)
						editor.InsertText(new NSString(ch.ToString()), new NSRange(NSRange.NotFound, 0));

					test(stepper, editor, other);
				}
				finally
				{
					Thread.CurrentThread.CurrentCulture = oldCulture;
				}
			});
		}
	}
}
