using Eto.Mac;
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
		using var keyEvent = CreatePhysicalKeyEvent(keyCode, numericPad: true);

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(expectedKey));
	}

	[Test]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldUseKeyCodeForControlCharacters()
	{
		using var keyEvent = CreatePhysicalKeyEvent(51); // Backspace.

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(Keys.Backspace));
	}

	[Test]
	[InvokeOnUI]
	public void ToEtoKeyEventArgsShouldFallBackToKeyCodeWhenCharacterHasNoMapping()
	{
		using var keyEvent = CreatePhysicalKeyEvent(105); // F13.

		var args = keyEvent.ToEtoKeyEventArgs();

		Assert.That(args.Key, Is.EqualTo(Keys.F13));
	}

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

	static NSEvent CreatePhysicalKeyEvent(int keyCode, bool numericPad = false)
	{
#if MONOMAC
		var cgEvent = CGEventCreateKeyboardEvent(IntPtr.Zero, checked((ushort)keyCode), true);
		if (cgEvent == IntPtr.Zero)
			throw new InvalidOperationException("Could not create keyboard event");
		try
		{
			if (numericPad)
				CGEventSetFlags(cgEvent, (ulong)NSEventModifierFlags.NumericPad);
			return NSEvent.EventWithCGEvent(cgEvent);
		}
		finally
		{
			CFRelease(cgEvent);
		}
#else
		using var cgEvent = new CGEvent(null, checked((ushort)keyCode), true)
		{
			Flags = numericPad ? CGEventFlags.NumericPad : 0
		};
		return NSEvent.Create(cgEvent);
#endif
	}

#if MONOMAC
	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern IntPtr CGEventCreateKeyboardEvent(
		IntPtr source,
		ushort virtualKey,
		[MarshalAs(UnmanagedType.I1)] bool keyDown);

	[DllImport(Constants.CoreGraphicsLibrary)]
	static extern void CGEventSetFlags(IntPtr cgEvent, ulong flags);

	[DllImport(Constants.CoreFoundationLibrary)]
	static extern void CFRelease(IntPtr value);
#endif
}
