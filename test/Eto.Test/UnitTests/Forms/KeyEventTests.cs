using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms;

/// <summary>
/// Tests that keys actually reach a window's KeyDown event, driven through each platform's real input
/// routing via <see cref="ITestInput"/>.
/// </summary>
[TestFixture]
public class KeyEventTests : TestBase
{
	/// <summary>
	/// A window with nothing focusable in its content keeps the keyboard focus on the window itself.
	/// Key events should still be raised in that state.
	/// </summary>
	[Test]
	public void KeyDownShouldFireWhenWindowItselfHasFocus() => Async(async () =>
	{
		TestInput.EnsureSupported();

		var keys = new List<Keys>();
		// no focusable controls, so keyboard focus has nowhere to go but the window
		var form = new Form { Content = new Label { Text = "Hello" } };
		form.KeyDown += (sender, e) => RecordKey(keys, e);

		await ShownAsync(form, async () =>
		{
			Assert.That(TestInput.IsWindowFocusedItself(form), Is.True, "#1 Window itself should have keyboard focus");

			await TestInput.SendKeyDownAsync(form, Keys.Escape);

			Assert.That(keys, Is.EqualTo(new[] { Keys.Escape }), "#2 KeyDown should be raised once for the key");
		});
	});

	/// <summary>
	/// Enter is tested apart from <see cref="KeyDownShouldFireWhenWindowItselfHasFocus"/> because a
	/// window is the one place a platform is likely to claim it for itself, and a modeless Form that
	/// wants Enter to mean "finish" has to be able to see it.
	/// </summary>
	/// <remarks>
	/// On Gtk it never can: GtkWindow's activate-default key binding returns true from
	/// key-press-event, and that signal is RUN_LAST with a stop-on-true accumulator, so Eto's
	/// connect-after handler is not reached at all - Form.KeyDown cannot see Return whether the focus
	/// is on the window or on a control. KeyUp is unaffected, key-release-event has no such binding.
	/// Window.PreviewKeyDown does see it, as GtkWindow.HandleWindowPreviewKeyPressEvent connects
	/// before. TODO: link the Gtk issue for this once it is filed.
	/// </remarks>
	[Test]
	public void KeyDownShouldFireForEnterWhenWindowItselfHasFocus() => Async(async () =>
	{
		TestInput.EnsureSupported();
		if (Platform.Instance.IsGtk)
			Assert.Ignore($"{Platform.Instance.ID} consumes Return in the window's activate-default key binding");

		var keys = new List<Keys>();
		// no focusable controls, so keyboard focus has nowhere to go but the window
		var form = new Form { Content = new Label { Text = "Hello" } };
		form.KeyDown += (sender, e) => RecordKey(keys, e);

		await ShownAsync(form, async () =>
		{
			Assert.That(TestInput.IsWindowFocusedItself(form), Is.True, "#1 Window itself should have keyboard focus");

			await TestInput.SendKeyDownAsync(form, Keys.Enter);

			Assert.That(keys, Is.EqualTo(new[] { Keys.Enter }), "#2 KeyDown should be raised once for Enter");
		});
	});

	/// <summary>
	/// A platform may need to hook the key events in more than one place to cover the case above, so
	/// make sure only a single KeyDown is raised when focus is inside the content.
	/// </summary>
	/// <remarks>
	/// The focused control has to be one that does not consume the key: macOS delivers keys to the
	/// focused control first and only passes them on up the responder chain when it does not handle
	/// them, so a TextBox (whose field editor consumes everything) would get no event at all there.
	/// </remarks>
	[Test]
	public void KeyDownShouldFireOnceWhenControlHasFocus() => Async(async () =>
	{
		TestInput.EnsureSupported();

		var keys = new List<Keys>();
		var button = new Button { Text = "Click Me" };
		var form = new Form { Content = button };
		form.KeyDown += (sender, e) => RecordKey(keys, e);

		await ShownAsync(form, async () =>
		{
			button.Focus();
			await Task.Delay(100);
			Assert.That(button.HasFocus, Is.True, "#1 Control should have focus");

			await TestInput.SendKeyDownAsync(form, Keys.Escape);

			Assert.That(keys, Is.EqualTo(new[] { Keys.Escape }), "#2 KeyDown should only be raised once");
		});
	});

	/// <inheritdoc cref="KeyDownShouldFireWhenWindowItselfHasFocus"/>
	[Test]
	public void KeyUpShouldFireWhenWindowItselfHasFocus() => Async(async () =>
	{
		TestInput.EnsureSupported();

		var keys = new List<Keys>();
		// no focusable controls, so keyboard focus has nowhere to go but the window
		var form = new Form { Content = new Label { Text = "Hello" } };
		form.KeyUp += (sender, e) => RecordKey(keys, e);

		await ShownAsync(form, async () =>
		{
			Assert.That(TestInput.IsWindowFocusedItself(form), Is.True, "#1 Window itself should have keyboard focus");

			await TestInput.SendKeyUpAsync(form, Keys.Enter);
			await TestInput.SendKeyUpAsync(form, Keys.Escape);

			Assert.That(keys, Is.EqualTo(new[] { Keys.Enter, Keys.Escape }), "#2 KeyUp should be raised once per key");
		});
	});

	/// <inheritdoc cref="KeyDownShouldFireOnceWhenControlHasFocus"/>
	[Test]
	public void KeyUpShouldFireOnceWhenControlHasFocus() => Async(async () =>
	{
		TestInput.EnsureSupported();

		var keys = new List<Keys>();
		var button = new Button { Text = "Click Me" };
		var form = new Form { Content = button };
		form.KeyUp += (sender, e) => RecordKey(keys, e);

		await ShownAsync(form, async () =>
		{
			button.Focus();
			await Task.Delay(100);
			Assert.That(button.HasFocus, Is.True, "#1 Control should have focus");

			await TestInput.SendKeyUpAsync(form, Keys.Escape);

			Assert.That(keys, Is.EqualTo(new[] { Keys.Escape }), "#2 KeyUp should only be raised once");
		});
	});

	/// <summary>
	/// Disabling a window can take the keyboard focus away from whatever had it. When the window is
	/// enabled again the focus has to end up somewhere in it, otherwise the window is left getting no
	/// keyboard input at all.
	/// </summary>
	/// <remarks>
	/// Which control ends up with the focus is deliberately not part of this - no backend goes out of
	/// its way to preserve it. Uses buttons rather than text controls, and Escape rather than Enter, so
	/// the last assertion is about the focus and nothing else (see
	/// <see cref="KeyDownShouldFireOnceWhenControlHasFocus"/> and
	/// <see cref="KeyDownShouldFireForEnterWhenWindowItselfHasFocus"/>).
	/// </remarks>
	[Test]
	public void FocusShouldBeRestoredAfterWindowIsDisabledAndEnabled() => Async(async () =>
	{
		TestInput.EnsureSupported();

		var keys = new List<Keys>();
		var first = new Button { Text = "First" };
		var second = new Button { Text = "Second" };
		var form = new Form { Content = new StackLayout { Items = { first, second } } };
		form.KeyDown += (sender, e) => RecordKey(keys, e);

		await ShownAsync(form, async () =>
		{
			second.Focus();
			await Task.Delay(100);
			Assert.That(second.HasFocus, Is.True, "#1 Second control should have focus");

			form.Enabled = false;
			await Task.Delay(100);
			form.Enabled = true;
			await Task.Delay(100);

			Assert.That(TestInput.IsFocusWithinWindow(form), Is.True, "#2 Focus should be restored to the window");

			await TestInput.SendKeyDownAsync(form, Keys.Escape);

			Assert.That(keys, Is.EqualTo(new[] { Keys.Escape }), "#3 KeyDown should be raised once");
		});
	});

	/// <summary>
	/// Records the key of a KeyDown event, ignoring the ones raised for text input.
	/// </summary>
	/// <remarks>
	/// A key press can also produce a character, which platforms report as an extra KeyDown with no
	/// key code but a <see cref="KeyEventArgs.KeyChar"/>. How many of those a single key press
	/// produces is up to the platform, so these tests only look at the actual keys.
	/// </remarks>
	static void RecordKey(List<Keys> keys, KeyEventArgs e)
	{
		if (e.KeyData != Keys.None)
			keys.Add(e.KeyData);
	}

	/// <summary>
	/// Shows the form, waits for the platform to settle its initial focus, runs the test against it,
	/// then closes it again.
	/// </summary>
	/// <remarks>
	/// The form has to be closed even when the test fails - one left open can keep the next test's
	/// window from becoming active, making it fail for the wrong reason.
	/// </remarks>
	static async Task ShownAsync(Form form, Func<Task> test)
	{
		try
		{
			form.Show();
			await Task.Delay(100);
			await test();
		}
		finally
		{
			form.Close();
		}
	}
}
