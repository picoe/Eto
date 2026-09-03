using Eto.Test.UnitTests;
using NUnit.Framework;
using swi = System.Windows.Input;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class KeyEventTests : TestBase
{
	static void SendKeyDown(Window window, swi.Key key)
	{
		var control = (sw.Window)window.ControlObject;
		var source = sw.PresentationSource.FromVisual(control);
		var args = new swi.KeyEventArgs(swi.Keyboard.PrimaryDevice, source, 0, key)
		{
			RoutedEvent = swi.Keyboard.PreviewKeyDownEvent
		};
		swi.InputManager.Current.ProcessInput(args);
	}

	/// <summary>
	/// Key events are hooked up on an inner element of the window, which is only in the routing path
	/// when something inside the content has keyboard focus. When the window itself has focus (e.g.
	/// when there is nothing focusable in its content) the events should still be raised.
	/// </summary>
	[Test]
	public void KeyDownShouldFireWhenWindowItselfHasFocus() => Async(async () =>
	{
		var keys = new List<Keys>();
		// no focusable controls, so keyboard focus stays on the window itself
		var form = new Form { Content = new Label { Text = "Hello" } };
		form.KeyDown += (sender, e) => keys.Add(e.KeyData);

		form.Show();
		await Task.Delay(100);

		var control = (sw.Window)form.ControlObject;
		Assert.That(swi.Keyboard.FocusedElement, Is.SameAs(control), "#1 Window itself should have keyboard focus");

		SendKeyDown(form, swi.Key.Enter);
		SendKeyDown(form, swi.Key.Escape);

		Assert.That(keys, Is.EqualTo(new[] { Keys.Enter, Keys.Escape }), "#2 KeyDown should be raised once per key");

		form.Close();
	});

	/// <summary>
	/// The window is hooked up for key events twice (see <see cref="KeyDownShouldFireWhenWindowItselfHasFocus"/>),
	/// so make sure only a single KeyDown is raised when focus is inside the content.
	/// </summary>
	[Test]
	public void KeyDownShouldFireOnceWhenControlHasFocus() => Async(async () =>
	{
		var keys = new List<Keys>();
		var textBox = new TextBox();
		var form = new Form { Content = textBox };
		form.KeyDown += (sender, e) => keys.Add(e.KeyData);

		form.Show();
		await Task.Delay(100);

		textBox.Focus();
		await Task.Delay(100);

		var control = (sw.Window)form.ControlObject;
		Assert.That(swi.Keyboard.FocusedElement, Is.Not.SameAs(control), "#1 Control should have keyboard focus");

		SendKeyDown(form, swi.Key.Escape);

		Assert.That(keys, Is.EqualTo(new[] { Keys.Escape }), "#2 KeyDown should only be raised once");

		form.Close();
	});
}
