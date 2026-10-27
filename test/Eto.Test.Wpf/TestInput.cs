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
			return Task.CompletedTask;
		}

		public bool IsWindowFocusedItself(Window window)
		{
			var control = GetWindow(window);
			return ReferenceEquals(swi.Keyboard.FocusedElement, control);
		}

		public bool IsFocusWithinWindow(Window window) => GetWindow(window).IsKeyboardFocusWithin;
	}
}
