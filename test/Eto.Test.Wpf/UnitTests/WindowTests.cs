using Eto.Test.UnitTests;
using Eto.Wpf.Forms;
using NUnit.Framework;
using swi = System.Windows.Input;

namespace Eto.Test.Wpf.UnitTests;

[TestFixture]
public class WindowTests : TestBase
{
	[Test]
	public void WindowShouldActuallyBeVisibleWhenShown() => Async(async () => {

		var form = new Form { Content = new Panel { Content = "Hello", Size = new Size(200, 200) } };

		// This resets the Visibility of the WPF window so it can cause the form not to show
		var size = form.GetPreferredSize();

		form.Show();
		
		await Task.Delay(100);

		var handler = form.Handler as FormHandler;

		Assert.That(handler.Control.Visibility, Is.EqualTo(sw.Visibility.Visible), "#1.1 Visibility should be visible");
		Assert.That(handler.Control.IsActive, Is.EqualTo(true), "#1.2 Form should be active");

		form.Close();
	});

	/// <summary>
	/// A window shown right after a key press (e.g. Enter to run a command) focuses its first control the
	/// same way as tabbing to it, which should not pop up that control's tooltip.
	/// </summary>
	[Test]
	public void FirstControlToolTipShouldNotShowWhenWindowIsShownAfterKeyPress() => Async(async () =>
	{
		var button = new Button { Text = "Button", ToolTip = "Tooltip" };
		var form = new Form { Content = new TableLayout(button, new TextBox()) };
		var native = (sw.FrameworkElement)button.ControlObject;

		var toolTipOpened = false;
		native.ToolTipOpening += (sender, e) =>
		{
			toolTipOpened = true;
			e.Handled = true;
		};

		// WPF only shows tooltips on focus when the keyboard was the last input device
		typeof(swi.InputManager).GetProperty(nameof(swi.InputManager.MostRecentInputDevice))
			.SetValue(swi.InputManager.Current, swi.Keyboard.PrimaryDevice);
		Assert.That(swi.InputManager.Current.MostRecentInputDevice, Is.SameAs(swi.Keyboard.PrimaryDevice), "#1 keyboard should be the most recent input device");

		form.Show();
		try
		{
			await Task.Delay(sw.SystemParameters.MouseHoverTime + TimeSpan.FromSeconds(1));

			Assert.That(native.IsKeyboardFocused, Is.True, "#2 first control should have focus");
			Assert.That(toolTipOpened, Is.False, "#3 tooltip should not have been shown");
			Assert.That(swc.ToolTipService.GetIsEnabled(native), Is.True, "#4 tooltips should be enabled again");
		}
		finally
		{
			form.Close();
		}
	});
}
