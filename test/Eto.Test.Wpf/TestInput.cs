using Eto.Test.UnitTests;
using Eto.Wpf;
using System.Threading.Tasks;
using swi = System.Windows.Input;

namespace Eto.Test.Wpf
{
	public class TestInput : ITestInput
	{
		static sw.Window GetWindow(Window window) => (sw.Window)window.ControlObject;

		public Task SendKeyDownAsync(Window window, Keys key) => SendKeyAsync(window, key, swi.Keyboard.PreviewKeyDownEvent);

		public Task SendKeyUpAsync(Window window, Keys key) => SendKeyAsync(window, key, swi.Keyboard.PreviewKeyUpEvent);

		static Task SendKeyAsync(Window window, Keys key, sw.RoutedEvent previewEvent)
		{
			var control = GetWindow(window);
			var args = new swi.KeyEventArgs(swi.Keyboard.PrimaryDevice, sw.PresentationSource.FromVisual(control), 0, key.ToWpfKey())
			{
				// the input manager promotes this to the non-preview event after the preview
				RoutedEvent = previewEvent
			};
			swi.InputManager.Current.ProcessInput(args);

			// WPF only types text from WM_CHAR, which faked key events never produce
			var text = previewEvent == swi.Keyboard.PreviewKeyDownEvent ? GetText(key) : null;
			if (text != null && swi.Keyboard.FocusedElement is sw.IInputElement focused)
				swi.TextCompositionManager.StartComposition(new swi.TextComposition(swi.InputManager.Current, focused, text));
			return Task.CompletedTask;
		}

		static string GetText(Keys key)
		{
			var code = key & Keys.KeyMask;
			var shift = key.HasFlag(Keys.Shift);
			if (code >= Keys.A && code <= Keys.Z)
				return ((char)((shift ? 'A' : 'a') + (code - Keys.A))).ToString();
			if (code >= Keys.D0 && code <= Keys.D9 && !shift)
				return ((char)('0' + (code - Keys.D0))).ToString();
			if (code >= Keys.Keypad0 && code <= Keys.Keypad9)
				return ((char)('0' + (code - Keys.Keypad0))).ToString();
			if (code == Keys.Space)
				return " ";
			return null;
		}

		public bool IsWindowFocusedItself(Window window)
		{
			var control = GetWindow(window);
			return ReferenceEquals(swi.Keyboard.FocusedElement, control);
		}

		public bool IsFocusWithinWindow(Window window) => GetWindow(window).IsKeyboardFocusWithin;
	}
}
