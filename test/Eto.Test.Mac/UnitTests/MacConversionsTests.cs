using Eto.Mac;
using Eto.Mac.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Mac.UnitTests;

[TestFixture]
public class MacConversionsTests : TestBase
{
	[Test]
	[InvokeOnUI]
	public void KeyMapShouldRoundTripAllLayoutDependentKeys()
	{
		var keys = Enumerable.Range((int)Keys.A, (int)Keys.Z - (int)Keys.A + 1)
			.Concat(Enumerable.Range((int)Keys.D0, (int)Keys.D9 - (int)Keys.D0 + 1))
			.Select(value => (Keys)value)
			.Concat(new[]
			{
				Keys.Minus,
				Keys.Grave,
				Keys.Space,
				Keys.Backslash,
				Keys.Equal,
				Keys.Semicolon,
				Keys.Quote,
				Keys.Comma,
				Keys.Period,
				Keys.Slash,
				Keys.LeftBracket,
				Keys.RightBracket
			});

		Assert.Multiple(() =>
		{
			foreach (var key in keys)
			{
				var keyEquivalent = KeyMap.KeyEquivalent(key);
				Assert.That(keyEquivalent, Is.Not.Empty, $"{key} should have a key equivalent");
				Assert.That(KeyMap.Convert(keyEquivalent, 0), Is.EqualTo(key), $"{key} should round trip");
			}
		});
	}

	[Test]
	[InvokeOnUI]
	public void ConvertShouldIgnoreCaseOfKeyEquivalent()
	{
		Assert.Multiple(() =>
		{
			Assert.That(KeyMap.Convert("Q", 0), Is.EqualTo(Keys.Q));
			Assert.That(KeyMap.Convert("q", 0), Is.EqualTo(Keys.Q));
			Assert.That(KeyMap.Convert("Q", NSEventModifierMask.ShiftKeyMask), Is.EqualTo(Keys.Q | Keys.Shift));
			Assert.That(KeyMap.Convert(null, 0), Is.EqualTo(Keys.None));
			Assert.That(KeyMap.Convert(string.Empty, 0), Is.EqualTo(Keys.None));
			Assert.That(KeyMap.Convert("\u0001", 0), Is.EqualTo(Keys.None));
		});
	}

	[Test]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldUseCharactersIgnoringModifiersForKey()
	{
		using var keyEvent = CreateKeyEvent(
			characters: "Q",
			charactersIgnoringModifiers: "q",
			modifierFlags: NSEventModifierMask.ShiftKeyMask,
			keyCode: 0); // A on a US keyboard layout.

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(Keys.Q));
		Assert.That(args.Modifiers, Is.EqualTo(Keys.Shift));
		Assert.That(args.KeyChar, Is.EqualTo('Q'));
		Assert.That(args.KeyEventType, Is.EqualTo(KeyEventType.KeyDown));
	}

	[Test]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldMapQuoteOnAlternativeKeyboardLayout()
	{
		using var keyEvent = CreateKeyEvent(
			characters: "'",
			charactersIgnoringModifiers: "'",
			keyCode: 12); // Q on a US keyboard layout.

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(Keys.Quote));
		Assert.That(args.KeyChar, Is.EqualTo('\''));
	}

	[TestCase("[", 27, Keys.LeftBracket)] // Minus on a US keyboard layout.
	[TestCase("]", 24, Keys.RightBracket)] // Equal on a US keyboard layout.
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldMapBracketsOnAlternativeKeyboardLayout(
		string character,
		int keyCode,
		Keys expectedKey)
	{
		using var keyEvent = CreateKeyEvent(
			characters: character,
			charactersIgnoringModifiers: character,
			keyCode: keyCode);

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(expectedKey));
		Assert.That(args.KeyChar, Is.EqualTo(character[0]));
	}

	[TestCase(83, Keys.Keypad1)]
	[TestCase(65, Keys.Decimal)]
	[TestCase(75, Keys.Divide)]
	[TestCase(78, Keys.Subtract)]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldPreserveNumericKeypadKeys(
		int keyCode,
		Keys expectedKey)
	{
		using var keyEvent = KeyEvents.CreatePhysicalKeyEvent(keyCode, modifiers: NSEventModifierMask.NumericPadKeyMask);

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(expectedKey));
	}

	[Test]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldUseKeyCodeForControlCharacters()
	{
		using var keyEvent = KeyEvents.CreatePhysicalKeyEvent(51); // Backspace.

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(Keys.Backspace));
	}

	[Test]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldFallBackToKeyCodeWhenCharacterHasNoMapping()
	{
		using var keyEvent = KeyEvents.CreatePhysicalKeyEvent(105); // F13.

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(Keys.F13));
	}

	/// <summary>
	/// Issue: RH-97914 - MouseEventArgs.IsDirectionInverted tells you whether the user has natural scrolling
	/// turned on for the device that generated the event, which is what AppKit reports per scroll wheel event.
	/// </summary>
	[TestCase(false)]
	[TestCase(true)]
	public void GetMouseEventShouldReportInvertedScrollDirection(bool isDirectionInverted)
	{
		Shown(form =>
		{
			var panel = new Panel { Size = new Size(100, 100) };
			form.Content = panel;
			return panel;
		},
		panel =>
		{
			using var scrollEvent = ScrollEvents.CreateScrollWheelEvent(3, 5, isDirectionInverted);
			// synthesizing an inverted scroll relies on a CGEventField that isn't public API, so treat it as a
			// precondition (inconclusive) rather than a failure if macOS ever stops honouring it.
			Assume.That(scrollEvent.IsDirectionInvertedFromDevice, Is.EqualTo(isDirectionInverted), "Could not synthesize a scroll wheel event with an inverted direction");

			var args = MacConversions.GetMouseEvent(GetViewHandler(panel), scrollEvent, true);

			Assert.Multiple(() =>
			{
				Assert.That(args.IsDirectionInverted, Is.EqualTo(isDirectionInverted));
				Assert.That(args.Delta, Is.EqualTo(new SizeF(3, 5)));
			});
		});
	}

	/// <summary>
	/// -[NSEvent isDirectionInvertedFromDevice] is only defined for scroll wheel and flick events, so the
	/// conversion must not go near it unless it is reading the wheel.
	/// </summary>
	[Test]
	public void GetMouseEventShouldNotReportInvertedScrollDirectionForOtherEvents()
	{
		Shown(form =>
		{
			var panel = new Panel { Size = new Size(100, 100) };
			form.Content = panel;
			return panel;
		},
		panel =>
		{
			using var mouseEvent = NSEvent.MouseEvent(NSEventType.LeftMouseDown, CGPoint.Empty, 0, 0, 0, null, 0, 1, 0);

			var args = MacConversions.GetMouseEvent(GetViewHandler(panel), mouseEvent, false);

			Assert.Multiple(() =>
			{
				Assert.That(args.IsDirectionInverted, Is.False);
				Assert.That(args.Delta, Is.EqualTo(SizeF.Empty));
			});
		});
	}

	static IMacViewHandler GetViewHandler(Control control) => (IMacViewHandler)((IHandlerSource)control).Handler;

	static NSEvent CreateKeyEvent(
		string characters,
		string charactersIgnoringModifiers,
		NSEventModifierMask modifierFlags = 0,
		int keyCode = 0)
	{
		return NSEvent.KeyEvent(
			NSEventType.KeyDown,
			CGPoint.Empty,
			modifierFlags,
			0,
			0,
			null,
			characters,
			charactersIgnoringModifiers,
			false,
			checked((ushort)keyCode));
	}
}
